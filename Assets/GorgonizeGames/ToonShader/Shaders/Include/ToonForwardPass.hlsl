#ifndef TOON_FORWARD_PASS_INCLUDED
#define TOON_FORWARD_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

// Declare textures and samplers before including other files
TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);
TEXTURE2D(_RampMap);
SAMPLER(sampler_RampMap);
TEXTURE2D(_MatCapMap);
SAMPLER(sampler_MatCapMap);
TEXTURE2D(_HatchingMap);
SAMPLER(sampler_HatchingMap);
TEXTURE2D(_EmissionMap);
SAMPLER(sampler_EmissionMap);

// Property declarations in CBUFFER for SRP Batcher compatibility
CBUFFER_START(UnityPerMaterial)
    float4 _BaseMap_ST;
    half4 _BaseColor;
    half4 _ShadowColor;
    half4 _HighlightColor;
    half _ShadowBlendMode;
    half _ShadowIntensity;
    half _LightSmoothness;
    int _RampSteps;
    
    // Specular
    half4 _SpecularColor;
    half _SpecularSize;
    half _SpecularSmoothness;
    half _Anisotropy;
    
    // Rim Lighting
    half4 _RimColor;
    half _RimPower;
    half _RimIntensity;
    half4 _SecondaryRimColor;
    half _SecondaryRimPower;
    half _SecondaryRimIntensity;
    
    // Shadows
    half4 _ShadowTint;
    half _ShadowSharpness;
    half _DitherScale;
    
    // Effects
    half _HatchingScale;
    half _HatchingIntensity;
    half _ColorDitherScale;
    
    // Emission
    half4 _EmissionColor;
    
    // Rendering
    half _Cull;
    half _SrcBlend;
    half _DstBlend;
    half _ZWrite;
    half _ZTest;
CBUFFER_END

// Now include the helper files that use these variables
#include "ToonHelpers.hlsl"
#include "ToonLighting.hlsl"
#include "ToonSpecular.hlsl"
#include "ToonRim.hlsl"
#include "ToonShadows.hlsl"
#include "ToonEffects.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float2 texcoord : TEXCOORD0;
    float2 staticLightmapUV : TEXCOORD1;
    float2 dynamicLightmapUV : TEXCOORD2;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
    float4 tangentWS : TEXCOORD3;
    float3 viewDirWS : TEXCOORD4;
    half4 fogFactorAndVertexLight : TEXCOORD5; // x: fogFactor, yzw: vertex light
    
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    float4 shadowCoord : TEXCOORD6;
    #endif
    
    DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 7);
    
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
    half3 vertexLighting : TEXCOORD8;
    #endif
    
    float4 positionCS : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

// Vertex Shader
Varyings ToonVertex(Attributes input)
{
    Varyings output = (Varyings)0;
    
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
    
    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
    
    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.positionWS = vertexInput.positionWS;
    output.positionCS = vertexInput.positionCS;
    output.normalWS = normalInput.normalWS;
    
    real sign = input.tangentOS.w * GetOddNegativeScale();
    half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
    output.tangentWS = tangentWS;
    
    output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
    
    // Fog and vertex lighting
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
    output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
    
    // Shadow coordinates
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    output.shadowCoord = GetShadowCoord(vertexInput);
    #endif
    
    // Lightmaps
    OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
    #ifdef DYNAMICLIGHTMAP_ON
    output.dynamicLightmapUV = input.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
    
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
    output.vertexLighting = vertexLight;
    #endif
    
    return output;
}

// Fragment Shader
half4 ToonFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    
    // Sample base texture
    half4 albedoAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
    albedoAlpha *= _BaseColor;
    
    half3 albedo = albedoAlpha.rgb;
    half alpha = albedoAlpha.a;
    
    // World space vectors
    float3 positionWS = input.positionWS;
    half3 normalWS = NormalizeNormalPerPixel(input.normalWS);
    half3 viewDirWS = SafeNormalize(input.viewDirWS);
    
    // Shadow coordinates
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    float4 shadowCoord = input.shadowCoord;
    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
    float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
    #else
    float4 shadowCoord = float4(0, 0, 0, 0);
    #endif
    
    // Get main light
    Light mainLight = GetMainLight(shadowCoord);
    
    #if defined(_MOBILE_MODE)
        // Simplified lighting for mobile
        half3 lightDir = SafeNormalize(mainLight.direction);
        half NdotL = dot(normalWS, lightDir);
        half lightAttenuation = mainLight.distanceAttenuation * mainLight.shadowAttenuation;
        
        // Simple toon shading
        half toonLightValue = ToonLightingSimple(NdotL, _LightSmoothness);
        half3 lightColor = lerp(_ShadowColor.rgb, _HighlightColor.rgb, toonLightValue);
        half3 finalColor = albedo * lightColor * mainLight.color * lightAttenuation;
    #else
        // Full featured lighting
        ToonSurfaceData surfaceData;
        surfaceData.albedo = albedo;
        surfaceData.normalWS = normalWS;
        surfaceData.viewDirectionWS = viewDirWS;
        surfaceData.uv = input.uv;
        
        // Calculate toon lighting
        ToonLightingData lightingData = CalculateToonLighting(surfaceData, mainLight, positionWS);
        half3 finalColor = lightingData.diffuse;
        
        #ifdef _ENABLE_SPECULAR
            // Add specular highlights
            half3 specular = CalculateToonSpecular(surfaceData, mainLight);
            finalColor += specular;
        #endif
        
        #ifdef _ENABLE_RIM
            // Add rim lighting
            half3 rimLighting = CalculateRimLighting(surfaceData, mainLight);
            finalColor += rimLighting;
        #endif
        
        #ifdef _ADDITIONAL_LIGHTS
            // Additional lights
            uint pixelLightCount = GetAdditionalLightsCount();
            LIGHT_LOOP_BEGIN(pixelLightCount)
                Light light = GetAdditionalLight(lightIndex, positionWS);
                ToonLightingData additionalLighting = CalculateToonLightingSimple(surfaceData, light, positionWS);
                finalColor += additionalLighting.diffuse * 0.5; // Reduce intensity for additional lights
                
                #ifdef _ENABLE_SPECULAR
                    half3 additionalSpecular = CalculateToonSpecular(surfaceData, light);
                    finalColor += additionalSpecular * 0.5;
                #endif
            LIGHT_LOOP_END
        #endif
        
        // Apply stylized shadows
        #ifdef _STYLIZED_SHADOWS
            finalColor = ApplyStylizedShadows(finalColor, lightingData.shadowAttenuation, positionWS, input.uv);
        #endif
        
        // Apply effects
        #ifdef _ENABLE_HATCHING
            finalColor = ApplyHatching(finalColor, lightingData.lightValue, input.uv);
        #endif
        
        #ifdef _ENABLE_DITHERING
            finalColor = ApplyColorDithering(finalColor, input.positionCS.xy);
        #endif
    #endif
    
    // Apply emission
    #ifdef _EMISSION
        half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb;
        emission *= _EmissionColor.rgb;
        finalColor += emission;
    #endif
    
    // Apply fog
    finalColor = MixFog(finalColor, input.fogFactorAndVertexLight.x);
    
    return half4(finalColor, alpha);
}

#endif