#ifndef TOON_LIGHTING_INCLUDED
#define TOON_LIGHTING_INCLUDED

#include "ToonHelpers.hlsl"

// External properties - these will be defined in the main pass
// But we need to declare externs here for Unity 6
extern TEXTURE2D(_RampMap);
extern SAMPLER(sampler_RampMap);
extern half4 _ShadowColor;
extern half4 _HighlightColor;
extern half _ShadowIntensity;
extern half _LightSmoothness;
extern int _RampSteps;
extern half _ShadowBlendMode;

struct ToonSurfaceData
{
    half3 albedo;
    half3 normalWS;
    half3 viewDirectionWS;
    float2 uv;
};

struct ToonLightingData
{
    half3 diffuse;
    half lightValue;
    half shadowAttenuation;
};

// Simple toon lighting for mobile
half ToonLightingSimple(half NdotL, half smoothness)
{
    half toonValue = smoothstep(-smoothness, smoothness, NdotL);
    return toonValue;
}

// Ramp-based lighting using 1D texture
half3 SampleRampTexture(half lightValue)
{
    #ifdef _USE_RAMP_SHADING
        return SAMPLE_TEXTURE2D(_RampMap, sampler_RampMap, float2(lightValue, 0.5)).rgb;
    #else
        return half3(1, 1, 1);
    #endif
}

// Procedural stepped gradient
half3 ProceduralRampLighting(half lightValue, int steps, half4 shadowColor, half4 highlightColor)
{
    // Quantize light value based on step count
    half steppedValue = floor(lightValue * (float)steps) / (float)(steps - 1);
    steppedValue = saturate(steppedValue);
    
    // Interpolate between shadow and highlight colors
    return lerp(shadowColor.rgb, highlightColor.rgb, steppedValue);
}

// Apply shadow color blending
half3 ApplyShadowBlending(half3 baseColor, half3 shadowColor, half shadowMask, int blendMode)
{
    half3 result = baseColor;
    
    if (blendMode == 0) // Multiply
    {
        result = lerp(baseColor, baseColor * shadowColor, shadowMask);
    }
    else if (blendMode == 1) // Additive
    {
        result = lerp(baseColor, baseColor + shadowColor, shadowMask);
    }
    else if (blendMode == 2) // Replace
    {
        result = lerp(baseColor, shadowColor, shadowMask);
    }
    
    return result;
}

// Main CalculateToonLighting function - THIS WAS MISSING!
ToonLightingData CalculateToonLighting(ToonSurfaceData surfaceData, Light light, float3 positionWS)
{
    ToonLightingData lightingData;
    
    half3 lightDir = SafeNormalize(light.direction);
    half NdotL = dot(surfaceData.normalWS, lightDir);
    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
    
    // Convert NdotL to 0-1 range for ramp sampling
    half lightValue = NdotL * 0.5 + 0.5;
    
    // Apply toon shading based on settings
    #ifdef _USE_RAMP_SHADING
        // Use ramp texture
        half3 rampColor = SampleRampTexture(lightValue);
        half3 lightColor = rampColor;
    #else
        // Use procedural stepped gradient
        half3 lightColor = ProceduralRampLighting(lightValue, _RampSteps, _ShadowColor, _HighlightColor);
    #endif
    
    // Apply shadow blending
    half shadowMask = 1.0 - saturate(NdotL);
    shadowMask = smoothstep(0.5 - _LightSmoothness, 0.5 + _LightSmoothness, shadowMask);
    
    half3 finalColor = ApplyShadowBlending(surfaceData.albedo, _ShadowColor.rgb, shadowMask * _ShadowIntensity, (int)_ShadowBlendMode);
    
    // Combine with light color and attenuation
    half3 diffuseColor = finalColor * lightColor * light.color * lightAttenuation;
    
    // Store results
    lightingData.diffuse = diffuseColor;
    lightingData.lightValue = lightValue;
    lightingData.shadowAttenuation = light.shadowAttenuation;
    
    return lightingData;
}

// Simple version for additional lights
ToonLightingData CalculateToonLightingSimple(ToonSurfaceData surfaceData, Light light, float3 positionWS)
{
    ToonLightingData lightingData;
    
    half3 lightDir = SafeNormalize(light.direction);
    half NdotL = dot(surfaceData.normalWS, lightDir);
    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
    
    // Convert NdotL to 0-1 range for ramp sampling
    half lightValue = NdotL * 0.5 + 0.5;
    
    // Simple toon lighting
    half smoothedLight = smoothstep(0.5 - _LightSmoothness, 0.5 + _LightSmoothness, lightValue);
    
    // Simple color mixing
    half3 lightColor = lerp(_ShadowColor.rgb, _HighlightColor.rgb, smoothedLight);
    
    // Combine with light color and attenuation
    half3 diffuseColor = surfaceData.albedo * lightColor * light.color * lightAttenuation;
    
    // Store results
    lightingData.diffuse = diffuseColor;
    lightingData.lightValue = lightValue;
    lightingData.shadowAttenuation = light.shadowAttenuation;
    
    return lightingData;
}

// Lambert lighting with toon quantization (for additional lights)
half3 CalculateAdditionalToonLight(half3 albedo, Light light, half3 normalWS)
{
    half3 lightDir = SafeNormalize(light.direction);
    half NdotL = dot(normalWS, lightDir);
    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
    
    // Simple toon shading for additional lights
    half toonValue = smoothstep(0.5 - _LightSmoothness, 0.5 + _LightSmoothness, NdotL * 0.5 + 0.5);
    
    return albedo * light.color * lightAttenuation * toonValue;
}

// Wrapped lighting for softer shadows (commonly used in toon shading)
half CalculateWrappedLighting(half NdotL, half wrapValue)
{
    return saturate((NdotL + wrapValue) / (1.0 + wrapValue));
}

// Calculate ambient lighting with toon characteristics
half3 CalculateAmbientToon(half3 albedo, half3 normalWS)
{
    // Sample ambient lighting (SH or lightmaps)
    half3 ambient = SampleSH(normalWS);
    
    // Apply toon characteristics to ambient
    ambient = saturate(ambient);
    
    // Quantize ambient lighting to match toon aesthetic
    ambient = floor(ambient * 3.0) / 3.0; // 3-step quantization
    
    return albedo * ambient * 0.3; // Reduced ambient contribution
}

#endif