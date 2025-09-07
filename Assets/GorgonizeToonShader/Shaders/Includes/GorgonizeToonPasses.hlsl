#ifndef TOON_PASSES_INCLUDED
#define TOON_PASSES_INCLUDED

#include "ToonInput.hlsl"
#include "ToonProperties.hlsl"
#include "ToonUtilities.hlsl"
#include "ToonLighting.hlsl"
#include "ToonEffects.hlsl"

// ============================================================================
// FORWARD LIT PASS
// ============================================================================

ToonVaryings ToonVertex(ToonAttributes input)
{
    ToonVaryings output = (ToonVaryings)0;
    
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
    
    VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);
    
    output.positionHCS = positionInputs.positionCS;
    output.positionWS = positionInputs.positionWS;
    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
    output.normalWS = normalInputs.normalWS;
    
    #ifdef _NORMALMAP
        real sign = real(input.tangentOS.w) * GetOddNegativeScale();
        output.tangentWS = float4(normalInputs.tangentWS.xyz, sign);
    #endif
    
    output.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
    
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        output.shadowCoord = GetShadowCoord(positionInputs);
    #endif
    
    OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
    #ifdef DYNAMICLIGHTMAP_ON
        output.dynamicLightmapUV = input.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
    
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        output.vertexLighting = VertexLighting(positionInputs.positionWS, normalInputs.normalWS);
    #endif
    
    output.fogCoord = ComputeFogFactor(output.positionHCS.z);
    output.screenPos = ComputeScreenPos(output.positionHCS);
    
    return output;
}

float4 ToonFragment(ToonVaryings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    
    // Sample base textures
    float4 albedoAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
    float3 albedo = albedoAlpha.rgb * _BaseColor.rgb;
    float alpha = albedoAlpha.a * _BaseColor.a;
    
    // Alpha test
    #ifdef _ALPHATEST_ON
        clip(alpha - _Cutoff);
    #endif
    
    // Normal mapping
    float3 normalWS = normalize(input.normalWS);
    #ifdef _NORMALMAP
        float4 normalMap = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv);
        float3 normalTS = UnpackNormalScale(normalMap, _BumpScale);
        
        float sgn = input.tangentWS.w;
        float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
        half3x3 tangentToWorld = half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz);
        normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
        normalWS = NormalizeNormalPerPixel(normalWS);
    #endif
    
    float3 viewDirWS = SafeNormalize(input.viewDirWS);
    
    // Setup surface and lighting data
    ToonSurfaceData surfaceData;
    surfaceData.albedo = albedo;
    surfaceData.normalWS = normalWS;
    surfaceData.emission = float3(0, 0, 0);
    surfaceData.metallic = 0;
    surfaceData.smoothness = 0;
    surfaceData.occlusion = _AmbientOcclusion;
    surfaceData.alpha = alpha;
    
    // Setup lighting data
    ToonLightingData lightingData;
    lightingData.normalWS = normalWS;
    lightingData.viewDirectionWS = viewDirWS;
    lightingData.positionWS = input.positionWS;
    lightingData.screenPos = input.screenPos;
    
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        lightingData.shadowCoord = input.shadowCoord;
    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
        lightingData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
    #else
        lightingData.shadowCoord = float4(0, 0, 0, 0);
    #endif
    
    // Calculate baked GI
    lightingData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, normalWS);
    
    // Add emission
    #ifdef _EMISSION
        float2 emissionUV = input.uv + _EmissionScrollSpeed.xy * _Time.y;
        float3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, emissionUV).rgb;
        surfaceData.emission = emission * _EmissionColor.rgb * _EmissionIntensity;
    #endif
    
    // Calculate final lighting
    float3 finalColor = CalculateEnhancedToonLighting(surfaceData, lightingData, input.uv);
    
    // Apply fog
    finalColor = MixFog(finalColor, input.fogCoord);
    
    return float4(finalColor, alpha);
}

// ============================================================================
// OUTLINE PASS
// ============================================================================

OutlineVaryings OutlineVertex(OutlineAttributes input)
{
    OutlineVaryings output = (OutlineVaryings)0;
    
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
    
    #ifdef _OUTLINE
        VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
        VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);
        
        // Expand position along normal for outline
        float3 positionWS = positionInputs.positionWS;
        float3 normalWS = normalInputs.normalWS;
        
        // Scale outline with distance for consistent screen-space width
        float4 positionCS = TransformWorldToHClip(positionWS);
        float outlineWidth = _OutlineWidth;
        
        // Perspective correction
        #ifdef _OUTLINE_SCREEN_SPACE
            outlineWidth *= positionCS.w;
        #endif
        
        positionWS += normalWS * outlineWidth;
        output.positionHCS = TransformWorldToHClip(positionWS);
    #else
        output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
    #endif
    
    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
    output.fogCoord = ComputeFogFactor(output.positionHCS.z);
    
    return output;
}

float4 OutlineFragment(OutlineVaryings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    
    #ifdef _OUTLINE
        float4 color = _OutlineColor;
        
        // Sample base texture for alpha if needed
        #ifdef _OUTLINE_ALPHA_CUTOFF
            float alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a;
            clip(alpha - _Cutoff);
        #endif
        
        // Apply fog to outline
        color.rgb = MixFog(color.rgb, input.fogCoord);
        
        return color;
    #else
        discard;
        return float4(0, 0, 0, 0);
    #endif
}

// ============================================================================
// SHADOW CASTER PASS
// ============================================================================

float4 GetShadowPositionHClip(ShadowAttributes input)
{
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
    
    #if _CASTING_PUNCTUAL_LIGHT_SHADOW
        float3 lightDirectionWS = normalize(_LightPosition - positionWS);
    #else
        float3 lightDirectionWS = _LightDirection;
    #endif
    
    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
    
    #if UNITY_REVERSED_Z
        positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #else
        positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #endif
    
    return positionCS;
}

ShadowVaryings ShadowPassVertex(ShadowAttributes input)
{
    ShadowVaryings output = (ShadowVaryings)0;
    
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
    
    output.positionHCS = GetShadowPositionHClip(input);
    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
    
    return output;
}

half4 ShadowPassFragment(ShadowVaryings input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    
    #ifdef _ALPHATEST_ON
        float alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a * _BaseColor.a;
        clip(alpha - _Cutoff);
    #endif
    
    return 0;
}

// ============================================================================
// DEPTH ONLY PASS
// ============================================================================

DepthOnlyVaryings DepthOnlyVertex(DepthOnlyAttributes input)
{
    DepthOnlyVaryings output = (DepthOnlyVaryings)0;
    
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
    
    output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
    
    return output;
}

half4 DepthOnlyFragment(DepthOnlyVaryings input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    
    #ifdef _ALPHATEST_ON
        float alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a * _BaseColor.a;
        clip(alpha - _Cutoff);
    #endif
    
    return 0;
}

// ============================================================================
// META PASS (for lightmap baking)
// ============================================================================

MetaVaryings MetaVertex(MetaAttributes input)
{
    MetaVaryings output = (MetaVaryings)0;
    
    output.positionHCS = UnityMetaVertexPosition(input.positionOS.xyz, input.uv1, input.uv2);
    output.uv = TRANSFORM_TEX(input.uv0, _BaseMap);
    
    #ifdef EDITOR_VISUALIZATION
        UnityEditorVizData(input.positionOS.xyz, input.uv0, input.uv1, input.uv2, output.vizUV, output.lightCoord);
    #endif
    
    return output;
}

float4 MetaFragment(MetaVaryings input) : SV_Target
{
    float4 albedoAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
    float3 albedo = albedoAlpha.rgb * _BaseColor.rgb;
    
    // Apply color grading for consistent look in lightmaps
    albedo = ApplyColorGrading(albedo, _Hue, _Saturation, _Brightness, _Contrast, _Gamma);
    
    UnityMetaInput metaInput;
    metaInput.Albedo = albedo;
    metaInput.Emission = 0;
    
    #ifdef _EMISSION
        float2 emissionUV = input.uv;
        float3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, emissionUV).rgb;
        metaInput.Emission = emission * _EmissionColor.rgb * _EmissionIntensity;
    #endif
    
    return UnityMetaFragment(metaInput);
}

#endif