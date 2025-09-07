#ifndef GORGONIZE_TOON_CORE_INCLUDED
#define GORGONIZE_TOON_CORE_INCLUDED

// Includes
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
#include "Assets/GorgonizeGames/ToonShader/Shaders/Includes/GorgonizeToonHelpers.hlsl"
#include "Assets/GorgonizeGames/ToonShader/Shaders/Includes/GorgonizeToonLighting.hlsl"
#include "Assets/GorgonizeGames/ToonShader/Shaders/Includes/GorgonizeToonEffects.hlsl"

// Texture and Sampler declarations
TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

#if USE_RAMP_TEXTURE
    TEXTURE2D(_RampTex);
    SAMPLER(sampler_RampTex);
#endif

#if ENABLE_MATCAP
    TEXTURE2D(_MatCapTex);
    SAMPLER(sampler_MatCapTex);
#endif

#if ENABLE_HATCHING
    TEXTURE2D(_HatchingTex);
    SAMPLER(sampler_HatchingTex);
#endif

#if ENABLE_EMISSION
    TEXTURE2D(_EmissionMap);
    SAMPLER(sampler_EmissionMap);
#endif

// Constant Buffer for material properties
CBUFFER_START(UnityPerMaterial)
    // Base Properties
    float4 _MainTex_ST;
    float4 _Color;
    
    // Lighting Properties
    float _RampSteps;
    float _RampSmoothness;
    float4 _ShadowColor;
    float4 _HighlightColor;
    float _ShadowBlendMode;
    
    // Specular Properties
    float4 _SpecColor;
    float _Glossiness;
    float _SpecularSize;
    float4 _AnisotropyDirection;
    
    // MatCap Properties
    float _MatCapIntensity;
    
    // Rim Properties
    float4 _RimColor;
    float _RimPower;
    float _RimIntensity;
    float4 _RimColorSecondary;
    float _RimPowerSecondary;
    float _RimIntensitySecondary;
    
    // Shadow Properties
    float4 _ShadowTint;
    float _ShadowSharpness;
    
    // Effects Properties
    float4 _HatchingTex_ST;
    float _HatchingScale;
    float _HatchingIntensity;
    float _DitheringScale;
    
    // Emission Properties
    float4 _EmissionMap_ST;
    float4 _EmissionColor;
    
    // Outline Properties (used in outline pass)
    float4 _OutlineColor;
    float _OutlineWidth;
    float _OutlineDistanceFade;
CBUFFER_END

// Vertex Input Structure
struct Attributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float2 uv : TEXCOORD0;
    float2 lightmapUV : TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// Vertex Output Structure
struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    
    float3 positionWS : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
    float3 viewDirWS : TEXCOORD3;
    
    #if defined(ENABLE_RIM) || defined(ENABLE_SPECULAR) || defined(ENABLE_MATCAP)
        float3 tangentWS : TEXCOORD4;
        float3 bitangentWS : TEXCOORD5;
    #endif
    
    #ifdef _MAIN_LIGHT_SHADOWS_SCREEN
        float4 shadowCoord : TEXCOORD6;
    #endif
    
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 7);
    
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        half4 fogFactorAndVertexLight : TEXCOORD8;
    #else
        half fogFactor : TEXCOORD8;
    #endif
    
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

// Vertex Shader
Varyings vert(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    // Position transformations
    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    output.positionCS = vertexInput.positionCS;
    output.positionWS = vertexInput.positionWS;

    // Normal and tangent transformations
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
    output.normalWS = normalInput.normalWS;
    output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);

    #if defined(ENABLE_RIM) || defined(ENABLE_SPECULAR) || defined(ENABLE_MATCAP)
        output.tangentWS = normalInput.tangentWS;
        output.bitangentWS = normalInput.bitangentWS;
    #endif

    // UV calculations
    output.uv = TRANSFORM_TEX(input.uv, _MainTex);

    // Shadow coordinates
    #ifdef _MAIN_LIGHT_SHADOWS_SCREEN
        output.shadowCoord = ComputeScreenPos(output.positionCS);
    #endif

    // Lightmap UV or SH
    OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

    // Fog and vertex lighting
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
        half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
        output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
    #else
        output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
    #endif

    return output;
}

// Fragment Shader
half4 frag(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    // Surface data preparation
    ToonSurfaceData surfaceData = (ToonSurfaceData)0;
    InitializeToonSurfaceData(input, surfaceData);

    // Input data preparation
    ToonInputData inputData = (ToonInputData)0;
    InitializeToonInputData(input, surfaceData.normalWS, inputData);

    // Main lighting calculation
    half4 color = CalculateToonLighting(inputData, surfaceData);

    // Apply additional effects
    #if ENABLE_EMISSION
        half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, 
                                        TRANSFORM_TEX(input.uv, _EmissionMap)).rgb;
        color.rgb += emission * _EmissionColor.rgb;
    #endif

    // Apply fog
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        color.rgb = MixFog(color.rgb, input.fogFactorAndVertexLight.x);
    #else
        color.rgb = MixFog(color.rgb, input.fogFactor);
    #endif

    // Apply dithering if enabled
    #if ENABLE_DITHERING
        color.rgb = ApplyDithering(color.rgb, input.positionCS.xy, _DitheringScale);
    #endif

    return color;
}

// Surface data initialization
void InitializeToonSurfaceData(Varyings input, out ToonSurfaceData surfaceData)
{
    // Base color sampling
    half4 albedoAlpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
    surfaceData.albedo = albedoAlpha.rgb * _Color.rgb;
    surfaceData.alpha = albedoAlpha.a * _Color.a;
    
    // Normal vector
    surfaceData.normalWS = normalize(input.normalWS);
    
    #if defined(ENABLE_RIM) || defined(ENABLE_SPECULAR) || defined(ENABLE_MATCAP)
        surfaceData.tangentWS = normalize(input.tangentWS);
        surfaceData.bitangentWS = normalize(input.bitangentWS);
    #endif
    
    // Specular properties
    #if ENABLE_SPECULAR
        surfaceData.specular = _SpecColor.rgb;
        surfaceData.smoothness = _Glossiness;
        surfaceData.specularSize = _SpecularSize;
    #else
        surfaceData.specular = 0;
        surfaceData.smoothness = 0;
        surfaceData.specularSize = 0;
    #endif
}

// Input data initialization
void InitializeToonInputData(Varyings input, half3 normalWS, out ToonInputData inputData)
{
    inputData.positionWS = input.positionWS;
    inputData.normalWS = normalWS;
    inputData.viewDirectionWS = normalize(input.viewDirWS);
    
    #ifdef _MAIN_LIGHT_SHADOWS_SCREEN
        inputData.shadowCoord = input.shadowCoord;
    #else
        inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
    #endif
    
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        inputData.fogCoord = input.fogFactorAndVertexLight.x;
        inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
    #else
        inputData.fogCoord = input.fogFactor;
        inputData.vertexLighting = half3(0, 0, 0);
    #endif
    
    inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
}

#endif