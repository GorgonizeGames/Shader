#ifndef TOON_SHADOWS_INCLUDED
#define TOON_SHADOWS_INCLUDED

#include "ToonHelpers.hlsl"

// External properties are declared in the main pass file
// We'll access them directly here

// Apply stylized shadows to the final color
half3 ApplyStylizedShadows(half3 baseColor, half shadowAttenuation, float3 positionWS, float2 uv)
{
    #ifdef _STYLIZED_SHADOWS
        // Convert shadow attenuation to shadow mask
        half shadowMask = 1.0 - shadowAttenuation;
        
        // Apply shadow sharpness
        shadowMask = smoothstep(0.5 - _ShadowSharpness * 0.5, 0.5 + _ShadowSharpness * 0.5, shadowMask);
        
        // Apply shadow tint
        half3 shadowColor = baseColor * _ShadowTint.rgb;
        
        #ifdef _SHADOW_DITHERING
            // Apply dithering to shadow edges
            shadowMask = ApplyShadowDithering(shadowMask, positionWS, uv);
        #endif
        
        // Blend shadow color with base color
        return lerp(baseColor, shadowColor, shadowMask);
    #else
        return baseColor;
    #endif
}

// Apply dithering to shadow transitions
half ApplyShadowDithering(half shadowMask, float3 positionWS, float2 uv)
{
    #ifdef _SHADOW_DITHERING
        // Generate dither pattern
        float2 ditherUV = uv * _DitherScale;
        half ditherValue = GenerateDitherPattern(ditherUV);
        
        // Apply dithering only to shadow transition areas
        half transitionMask = smoothstep(0.3, 0.7, shadowMask);
        half ditherAmount = (1.0 - transitionMask) * 0.1; // Reduce dither strength
        
        shadowMask += (ditherValue - 0.5) * ditherAmount;
        return saturate(shadowMask);
    #else
        return shadowMask;
    #endif
}

// Calculate received shadows with toon characteristics
half CalculateReceivedShadows(float4 shadowCoord, float3 positionWS)
{
    // Sample shadow map
    half shadowAttenuation = MainLightRealtimeShadow(shadowCoord);
    
    // Apply toon-style quantization to shadows
    shadowAttenuation = step(0.5, shadowAttenuation);
    
    return shadowAttenuation;
}

// Multi-step shadow calculation for layered shadows
half CalculateMultiStepShadows(float4 shadowCoord, int shadowSteps)
{
    half shadowAttenuation = MainLightRealtimeShadow(shadowCoord);
    
    // Quantize shadow into multiple steps
    shadowAttenuation = floor(shadowAttenuation * (float)shadowSteps) / (float)(shadowSteps - 1);
    
    return shadowAttenuation;
}

// Stylized cascade shadow blending
half CalculateCascadeShadows(float4 shadowCoord, float3 positionWS)
{
    // This would typically involve sampling multiple cascade levels
    // For simplicity, we'll use the main light shadow function
    half shadowAttenuation = MainLightRealtimeShadow(shadowCoord);
    
    // Distance-based shadow softening for far cascades
    float distanceToCamera = length(_WorldSpaceCameraPos - positionWS);
    half distanceFade = saturate((distanceToCamera - 20.0) / 30.0);
    
    // Soften shadows at distance
    shadowAttenuation = lerp(shadowAttenuation, 1.0, distanceFade * 0.3);
    
    // Apply toon quantization
    shadowAttenuation = smoothstep(0.4, 0.6, shadowAttenuation);
    
    return shadowAttenuation;
}

// Contact shadows for small-scale occlusion
half CalculateContactShadows(float3 positionWS, half3 normalWS, half3 lightDir)
{
    // Simple contact shadow approximation
    half contactShadow = 1.0;
    
    // Ray march in light direction for small distance
    float3 rayStart = positionWS + normalWS * 0.01;
    float3 rayDir = lightDir;
    float stepSize = 0.1;
    int steps = 5;
    
    for (int i = 0; i < steps; i++)
    {
        float3 samplePos = rayStart + rayDir * stepSize * (float)i;
        // In a real implementation, you'd sample a depth buffer here
        // For now, we'll just return no contact shadows
    }
    
    return contactShadow;
}

// Ambient occlusion integration with toon shading
half3 ApplyAmbientOcclusion(half3 baseColor, half ambientOcclusion)
{
    // Apply AO with toon characteristics
    half toonAO = smoothstep(0.2, 0.8, ambientOcclusion);
    toonAO = step(0.5, toonAO); // Hard cutoff for stylized look
    
    return baseColor * toonAO;
}

// Volumetric shadow approximation
half CalculateVolumetricShadows(float3 positionWS, half3 lightDir, half volumeDensity)
{
    // Simple volumetric shadow calculation
    float distanceInVolume = length(positionWS - _WorldSpaceCameraPos);
    half volumetricAttenuation = exp(-volumeDensity * distanceInVolume * 0.01);
    
    // Apply toon quantization
    volumetricAttenuation = floor(volumetricAttenuation * 4.0) / 4.0;
    
    return volumetricAttenuation;
}

// Shadow color temperature variation
half3 ApplyShadowTemperature(half3 shadowColor, half temperature)
{
    // Adjust shadow color based on temperature
    half3 coldShadow = shadowColor * half3(0.7, 0.8, 1.2); // Bluish
    half3 warmShadow = shadowColor * half3(1.1, 0.9, 0.7); // Orangish
    
    return lerp(coldShadow, warmShadow, temperature);
}

// Animated shadow effects
half3 ApplyAnimatedShadows(half3 baseColor, half shadowMask, float2 uv)
{
    // Time-based shadow animation
    half time = _Time.y;
    
    // Create moving shadow patterns
    float2 animatedUV = uv + half2(sin(time * 0.5), cos(time * 0.3)) * 0.1;
    half animationMask = SimpleNoise(animatedUV * 10.0);
    
    // Apply animated shadows subtly
    shadowMask *= lerp(0.95, 1.05, animationMask);
    shadowMask = saturate(shadowMask);
    
    half3 animatedShadowColor = baseColor * _ShadowTint.rgb;
    return lerp(baseColor, animatedShadowColor, shadowMask);
}

// Gradient-based shadow falloff
half CalculateGradientShadows(float3 positionWS, half3 gradientDirection, half gradientPower)
{
    // Calculate position along gradient
    half gradientValue = dot(positionWS, normalize(gradientDirection));
    
    // Apply power curve and normalize
    gradientValue = pow(saturate(gradientValue), gradientPower);
    
    // Quantize for toon effect
    gradientValue = floor(gradientValue * 3.0) / 3.0;
    
    return gradientValue;
}

#endif