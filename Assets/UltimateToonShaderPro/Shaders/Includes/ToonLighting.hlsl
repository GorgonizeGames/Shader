#ifndef TOON_LIGHTING_INCLUDED
#define TOON_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "ToonUtilities.hlsl"

struct ToonSurfaceData
{
    float3 albedo;
    float3 normalWS;
    float3 emission;
    float metallic;
    float smoothness;
    float occlusion;
    float alpha;
    float3 bakedGI;
};

struct ToonLightingData
{
    float3 normalWS;
    float3 viewDirectionWS;
    float4 shadowCoord;
    float3 bakedGI;
    float3 positionWS;
    float4 screenPos;
};

// Calculate main directional light contribution
float3 CalculateMainLight(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    Light mainLight = GetMainLight(lightingData.shadowCoord);
    float3 lightColor = mainLight.color;
    float3 lightDirection = mainLight.direction;
    float lightAttenuation = mainLight.shadowAttenuation;
    
    // Basic NdotL calculation
    float NdotL = dot(lightingData.normalWS, lightDirection);
    
    // Apply light wrapping
    NdotL = ApplyLightWrapping(NdotL, _LightWrapping);
    
    // Calculate toon ramp
    float toonRamp;
    
    #ifdef _USE_RAMP_TEXTURE
        toonRamp = SAMPLE_TEXTURE2D(_LightRampTex, sampler_LightRampTex, float2(NdotL, 0.5)).r;
    #else
        toonRamp = CalculateToonRamp(NdotL, _ShadowThreshold, _ShadowSmoothness);
    #endif
    
    // Apply cel shading quantization
    #ifdef _CEL_SHADING
        toonRamp = QuantizeLighting(toonRamp, _CelShadingSteps);
    #endif
    
    // Apply shadow attenuation
    toonRamp *= lightAttenuation;
    
    // Mix shadow color with albedo
    float3 shadowedColor = lerp(_ShadowColor.rgb * surfaceData.albedo, surfaceData.albedo, _ShadowIntensity);
    float3 litColor = lerp(shadowedColor, surfaceData.albedo, toonRamp);
    
    return litColor * lightColor;
}

// Calculate additional lights contribution
float3 CalculateAdditionalLights(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    float3 additionalLighting = 0;
    
    #ifdef _ADDITIONAL_LIGHTS
        uint pixelLightCount = GetAdditionalLightsCount();
        
        for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
        {
            Light light = GetAdditionalLight(lightIndex, lightingData.positionWS);
            float3 lightColor = light.color;
            float3 lightDirection = light.direction;
            float lightAttenuation = light.shadowAttenuation * light.distanceAttenuation;
            
            float NdotL = dot(lightingData.normalWS, lightDirection);
            NdotL = ApplyLightWrapping(NdotL, _LightWrapping);
            
            float toonRamp;
            #ifdef _USE_RAMP_TEXTURE
                toonRamp = SAMPLE_TEXTURE2D(_LightRampTex, sampler_LightRampTex, float2(NdotL, 0.5)).r;
            #else
                toonRamp = CalculateToonRamp(NdotL, _ShadowThreshold, _ShadowSmoothness);
            #endif
            
            #ifdef _CEL_SHADING
                toonRamp = QuantizeLighting(toonRamp, _CelShadingSteps);
            #endif
            
            toonRamp *= lightAttenuation;
            additionalLighting += surfaceData.albedo * lightColor * toonRamp * 0.5; // Reduced intensity for additional lights
        }
    #endif
    
    return additionalLighting;
}

// Calculate rim lighting
float3 CalculateRimLighting(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    float3 rimLighting = 0;
    
    #ifdef _RIM_LIGHTING
        float rim = CalculateRimLighting(lightingData.viewDirectionWS, lightingData.normalWS, _RimPower, _RimThreshold);
        rimLighting = rim * _RimColor.rgb * _RimIntensity;
    #endif
    
    return rimLighting;
}

// Calculate specular highlights
float3 CalculateSpecular(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    float3 specular = 0;
    
    #ifdef _SPECULAR
        Light mainLight = GetMainLight(lightingData.shadowCoord);
        float3 lightDirection = mainLight.direction;
        float lightAttenuation = mainLight.shadowAttenuation;
        
        float spec = CalculateToonSpecular(lightDirection, lightingData.viewDirectionWS, 
                                         lightingData.normalWS, _SpecularSize, _SpecularSmoothness);
        
        // Apply lighting conditions
        float NdotL = saturate(dot(lightingData.normalWS, lightDirection));
        float toonRamp = CalculateToonRamp(NdotL, _ShadowThreshold, _ShadowSmoothness);
        
        spec *= toonRamp * lightAttenuation * _SpecularIntensity;
        specular = spec * _SpecularColor.rgb * mainLight.color;
    #endif
    
    return specular;
}

// Calculate fresnel effects
float3 CalculateFresnel(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    float3 fresnel = 0;
    
    #ifdef _FRESNEL
        float fresnelTerm = CalculateFresnel(lightingData.viewDirectionWS, lightingData.normalWS, _FresnelPower);
        fresnel = fresnelTerm * _FresnelColor.rgb * _FresnelIntensity;
    #endif
    
    return fresnel;
}

// Calculate subsurface scattering
float3 CalculateSubsurface(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    float3 subsurface = 0;
    
    #ifdef _SUBSURFACE
        Light mainLight = GetMainLight(lightingData.shadowCoord);
        subsurface = CalculateSubsurface(mainLight.direction, lightingData.viewDirectionWS, 
                                       lightingData.normalWS, _SubsurfaceColor.rgb, 
                                       _SubsurfacePower, _SubsurfaceIntensity);
    #endif
    
    return subsurface;
}

// Calculate matcap lighting
float3 CalculateMatcap(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    float3 matcap = 0;
    
    #ifdef _MATCAP
        // Convert normal to view space for matcap sampling
        float3 normalVS = mul((float3x3)UNITY_MATRIX_V, lightingData.normalWS);
        float2 matcapUV = normalVS.xy * 0.5 + 0.5;
        float3 matcapColor = SAMPLE_TEXTURE2D(_MatcapTex, sampler_MatcapTex, matcapUV).rgb;
        
        // Apply blend mode
        if (_MatcapBlendMode < 0.5) // Add
        {
            matcap = matcapColor * _MatcapIntensity;
        }
        else if (_MatcapBlendMode < 1.5) // Multiply
        {
            matcap = lerp(float3(1, 1, 1), matcapColor, _MatcapIntensity);
        }
        else // Screen
        {
            matcap = float3(1, 1, 1) - (float3(1, 1, 1) - matcapColor * _MatcapIntensity);
        }
    #endif
    
    return matcap;
}

// Calculate hatching effects
float CalculateHatching(float2 uv, float lightValue, float4 screenPos)
{
    float hatching = 1.0;
    
    #ifdef _HATCHING
        // World space hatching
        float2 hatchUV = RotateUV(uv * _HatchingDensity, _HatchingRotation);
        float hatch1 = SAMPLE_TEXTURE2D(_HatchingTex, sampler_HatchingTex, hatchUV).r;
        
        if (lightValue < _HatchingThreshold)
        {
            hatching *= lerp(1.0, hatch1, _HatchingIntensity);
        }
        
        if (lightValue < _CrossHatchingThreshold)
        {
            float2 crossHatchUV = RotateUV(uv * _HatchingDensity, _HatchingRotation + 90);
            float crossHatch = SAMPLE_TEXTURE2D(_CrossHatchingTex, sampler_CrossHatchingTex, crossHatchUV).r;
            hatching *= lerp(1.0, crossHatch, _HatchingIntensity);
        }
    #endif
    
    #ifdef _SCREEN_SPACE_HATCHING
        // Screen space hatching
        float2 screenUV = screenPos.xy / screenPos.w;
        screenUV *= _ScreenParams.xy / _ScreenHatchScale;
        
        float screenHatch = sin(screenUV.x * 3.14159) * sin(screenUV.y * 3.14159);
        screenHatch = saturate(screenHatch + _ScreenHatchBias);
        
        if (lightValue < _HatchingThreshold)
        {
            hatching *= lerp(1.0, screenHatch, _HatchingIntensity);
        }
    #endif
    
    return hatching;
}

// Calculate indirect lighting contribution
float3 CalculateIndirectLighting(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    // Sample baked GI
    float3 indirectLighting = lightingData.bakedGI * surfaceData.albedo;
    
    // Apply indirect lighting boost
    indirectLighting *= _IndirectLightingBoost;
    
    // Apply ambient occlusion
    indirectLighting *= surfaceData.occlusion * _AmbientOcclusion;
    
    return indirectLighting;
}

// Main toon lighting function
float3 CalculateToonLighting(ToonSurfaceData surfaceData, ToonLightingData lightingData, float2 uv)
{
    // Calculate main directional light
    float3 mainLighting = CalculateMainLight(surfaceData, lightingData);
    
    // Calculate light value for hatching
    Light mainLight = GetMainLight(lightingData.shadowCoord);
    float NdotL = saturate(dot(lightingData.normalWS, mainLight.direction));
    float lightValue = CalculateToonRamp(NdotL, _ShadowThreshold, _ShadowSmoothness) * mainLight.shadowAttenuation;
    
    // Apply hatching
    float hatchingMask = CalculateHatching(uv, lightValue, lightingData.screenPos);
    mainLighting *= hatchingMask;
    
    // Add additional lights
    float3 additionalLighting = CalculateAdditionalLights(surfaceData, lightingData);
    
    // Add indirect lighting
    float3 indirectLighting = CalculateIndirectLighting(surfaceData, lightingData);
    
    // Combine all lighting
    float3 finalColor = mainLighting + additionalLighting + indirectLighting;
    
    // Add rim lighting
    finalColor += CalculateRimLighting(surfaceData, lightingData);
    
    // Add specular
    finalColor += CalculateSpecular(surfaceData, lightingData);
    
    // Apply matcap
    float3 matcap = CalculateMatcap(surfaceData, lightingData);
    if (_MatcapBlendMode < 0.5) // Add
    {
        finalColor += matcap;
    }
    else if (_MatcapBlendMode < 1.5) // Multiply
    {
        finalColor *= matcap;
    }
    else // Screen
    {
        finalColor = 1 - (1 - finalColor) * (1 - matcap);
    }
    
    // Add fresnel
    finalColor += CalculateFresnel(surfaceData, lightingData);
    
    // Add subsurface
    finalColor += CalculateSubsurface(surfaceData, lightingData);
    
    // Add emission
    finalColor += surfaceData.emission;
    
    return finalColor;
}

// Enhanced lighting function with more realistic features
float3 CalculateEnhancedToonLighting(ToonSurfaceData surfaceData, ToonLightingData lightingData, float2 uv)
{
    float3 finalColor = CalculateToonLighting(surfaceData, lightingData, uv);
    
    // Apply color grading
    finalColor = ApplyColorGrading(finalColor, _Hue, _Saturation, _Brightness, _Contrast, _Gamma);
    
    // Apply posterization if enabled
    #ifdef _POSTERIZE
        finalColor = Posterize(finalColor, _PosterizeLevels);
    #endif
    
    return finalColor;
}

#endif