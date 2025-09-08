#ifndef TOON_LIGHTING_INCLUDED
#define TOON_LIGHTING_INCLUDED

#include "ToonHelpers.hlsl"

// External properties and textures are declared in the main pass file
// We'll access them directly here

// Texture declarations (these will be declared in main pass)
// TEXTURE2D(_RampMap);
// SAMPLER(sampler_RampMap);

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

// Enhanced toon lighting calculation with ramps and procedural gradients
ToonLightingData CalculateToonLighting(ToonSurfaceData surfaceData, Light light, float3 positionWS, 
                                      half4 shadowColor, half4 highlightColor, half shadowBlendMode, 
                                      half shadowIntensity, half lightSmoothness, int rampSteps)
{
    ToonLightingData lightingData;
    
    half3 lightDir = normalize(light.direction);
    half NdotL = dot(surfaceData.normalWS, lightDir);
    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
    
    // Convert NdotL to 0-1 range for ramp sampling
    half lightValue = NdotL * 0.5 + 0.5;
    
    // Apply light smoothness (soft transitions)
    half smoothedLight = smoothstep(-lightSmoothness, lightSmoothness, NdotL);
    
    half3 lightColor;
    
    #ifdef _USE_RAMP_SHADING
        // Use ramp texture for lighting
        lightColor = SampleRampTexture(lightValue);
    #else
        // Use procedural gradient based on steps
        lightColor = ProceduralRampLighting(smoothedLight, rampSteps, shadowColor, highlightColor);
    #endif
    
    // Apply shadow blending
    half shadowMask = 1.0 - smoothedLight;
    half3 finalAlbedo = ApplyShadowBlending(surfaceData.albedo, shadowColor.rgb, 
                                           shadowMask * shadowIntensity, (int)shadowBlendMode);
    
    // Combine with light color and attenuation
    half3 diffuseColor = finalAlbedo * lightColor * light.color * lightAttenuation;
    
    // Store results
    lightingData.diffuse = diffuseColor;
    lightingData.lightValue = lightValue;
    lightingData.shadowAttenuation = light.shadowAttenuation;
    
    return lightingData;
}

// Lambert lighting with toon quantization (for additional lights)
half3 CalculateAdditionalToonLight(half3 albedo, Light light, half3 normalWS)
{
    half3 lightDir = normalize(light.direction);
    half NdotL = dot(normalWS, lightDir);
    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
    
    // Simple toon shading for additional lights
    half toonValue = step(0.0, NdotL);
    toonValue = lerp(0.2, 1.0, toonValue); // Maintain some ambient lighting
    
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