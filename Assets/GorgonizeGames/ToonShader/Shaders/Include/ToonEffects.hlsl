#ifndef TOON_EFFECTS_INCLUDED
#define TOON_EFFECTS_INCLUDED

#include "ToonHelpers.hlsl"

// External properties and textures are declared in the main pass file
// We'll access them directly here

// Apply hatching effect based on light intensity
half3 ApplyHatching(half3 baseColor, half lightValue, float2 uv)
{
    #ifdef _ENABLE_HATCHING
        // Sample hatching texture
        half4 hatchingTexture = SAMPLE_TEXTURE2D(_HatchingMap, sampler_HatchingMap, uv * _HatchingScale);
        
        // Use different channels for different light intensities
        half hatch1 = hatchingTexture.r; // Darkest areas
        half hatch2 = hatchingTexture.g; // Medium areas
        half hatch3 = hatchingTexture.b; // Light areas
        
        // Determine which hatching layer to use based on light value
        half hatchingMask = 0.0;
        
        if (lightValue < 0.3)
        {
            // Dark areas - use all hatching layers
            hatchingMask = hatch1 * hatch2 * hatch3;
        }
        else if (lightValue < 0.6)
        {
            // Medium areas - use two hatching layers
            hatchingMask = hatch2 * hatch3;
        }
        else if (lightValue < 0.8)
        {
            // Light areas - use one hatching layer
            hatchingMask = hatch3;
        }
        // Brightest areas have no hatching
        
        // Apply hatching intensity
        hatchingMask *= _HatchingIntensity;
        
        // Blend hatching with base color
        return lerp(baseColor, baseColor * hatchingMask, 0.5);
    #else
        return baseColor;
    #endif
}

// Apply color dithering for smooth gradients
half3 ApplyColorDithering(half3 baseColor, float2 screenPos)
{
    #ifdef _ENABLE_DITHERING
        // Generate dither pattern
        float2 ditherUV = screenPos / _ColorDitherScale;
        half ditherValue = GenerateDitherPattern(ditherUV);
        
        // Apply dithering to color
        half3 ditheredColor = baseColor + (ditherValue - 0.5) * 0.02; // Small dither amount
        
        return ditheredColor;
    #else
        return baseColor;
    #endif
}

// Generate various dither patterns
half GenerateDitherPattern(float2 uv)
{
    // Bayer matrix 4x4 dithering
    float4x4 bayerMatrix = float4x4(
        0.0, 8.0, 2.0, 10.0,
        12.0, 4.0, 14.0, 6.0,
        3.0, 11.0, 1.0, 9.0,
        15.0, 7.0, 13.0, 5.0
    );
    
    int2 matrixPos = int2(uv) % 4;
    return bayerMatrix[matrixPos.x][matrixPos.y] / 16.0;
}

// Cross-hatching effect for artistic rendering
half3 ApplyCrossHatching(half3 baseColor, half lightValue, float2 uv, half3 normalWS)
{
    #ifdef _ENABLE_HATCHING
        // Multiple hatching directions
        float2 hatchUV1 = uv * _HatchingScale;
        float2 hatchUV2 = uv * _HatchingScale;
        
        // Rotate hatching patterns
        half cosAngle = cos(0.785398); // 45 degrees
        half sinAngle = sin(0.785398);
        
        float2 rotatedUV = float2(
            hatchUV2.x * cosAngle - hatchUV2.y * sinAngle,
            hatchUV2.x * sinAngle + hatchUV2.y * cosAngle
        );
        
        // Sample hatching textures
        half hatch1 = SAMPLE_TEXTURE2D(_HatchingMap, sampler_HatchingMap, hatchUV1).r;
        half hatch2 = SAMPLE_TEXTURE2D(_HatchingMap, sampler_HatchingMap, rotatedUV).r;
        
        // Combine hatching based on light value
        half combinedHatch = 1.0;
        
        if (lightValue < 0.7)
        {
            combinedHatch *= hatch1;
        }
        if (lightValue < 0.4)
        {
            combinedHatch *= hatch2;
        }
        
        // Apply normal-based variation
        half normalVariation = abs(dot(normalWS, half3(1, 1, 1))) * 0.5 + 0.5;
        combinedHatch = lerp(combinedHatch, 1.0, normalVariation * 0.3);
        
        return baseColor * lerp(1.0, combinedHatch, _HatchingIntensity);
    #else
        return baseColor;
    #endif
}

// Stippling effect (dot-based shading)
half3 ApplyStippling(half3 baseColor, half lightValue, float2 uv)
{
    // Generate stippling pattern
    float2 stipplingUV = uv * 50.0; // Higher frequency for dots
    
    // Create dot pattern using noise
    half dotPattern = SimpleNoise(stipplingUV);
    dotPattern = step(lightValue, dotPattern); // Use light value as threshold
    
    // Apply stippling
    return baseColor * lerp(0.3, 1.0, dotPattern);
}

// Halftone effect
half3 ApplyHalftone(half3 baseColor, half lightValue, float2 uv, float2 screenPos)
{
    // Calculate halftone dot size based on light value
    half dotSize = (1.0 - lightValue) * 0.8 + 0.1;
    
    // Generate halftone pattern
    float2 halftoneUV = screenPos * 0.01; // Screen-space pattern
    half2 gridUV = frac(halftoneUV * 20.0) - 0.5; // Grid pattern
    half dotDistance = length(gridUV);
    
    half halftone = step(dotDistance, dotSize * 0.5);
    
    return baseColor * lerp(0.2, 1.0, halftone);
}

// Posterization effect (color quantization)
half3 ApplyPosterization(half3 baseColor, int levels)
{
    // Quantize each color channel
    half3 posterized;
    posterized.r = floor(baseColor.r * (float)levels) / (float)(levels - 1);
    posterized.g = floor(baseColor.g * (float)levels) / (float)(levels - 1);
    posterized.b = floor(baseColor.b * (float)levels) / (float)(levels - 1);
    
    return posterized;
}

// Outline glow effect
half3 ApplyOutlineGlow(half3 baseColor, half3 normalWS, half3 viewDirWS, half glowIntensity)
{
    // Calculate rim-like glow
    half rimDot = 1.0 - saturate(dot(viewDirWS, normalWS));
    half glow = pow(rimDot, 2.0) * glowIntensity;
    
    // Add glow to base color
    return baseColor + half3(glow, glow, glow);
}

// Color gradient mapping
half3 ApplyColorGradient(half3 baseColor, half gradientValue, half3 gradientColorA, half3 gradientColorB)
{
    // Map grayscale value to gradient
    half luminance = Luminance(baseColor);
    half3 gradientColor = lerp(gradientColorA, gradientColorB, gradientValue);
    
    // Blend with original color
    return lerp(baseColor, gradientColor, 0.5);
}

// Sketch effect
half3 ApplySketchEffect(half3 baseColor, float2 uv, half3 normalWS)
{
    // Generate sketch lines based on normal direction
    half3 worldRight = half3(1, 0, 0);
    half3 worldUp = half3(0, 1, 0);
    
    half rightAlignment = abs(dot(normalWS, worldRight));
    half upAlignment = abs(dot(normalWS, worldUp));
    
    // Create sketch pattern
    float2 sketchUV1 = uv * 100.0;
    float2 sketchUV2 = uv * 100.0;
    sketchUV2 = float2(sketchUV2.y, -sketchUV2.x); // Perpendicular
    
    half sketch1 = step(0.7, frac(sketchUV1.x)) * rightAlignment;
    half sketch2 = step(0.7, frac(sketchUV2.x)) * upAlignment;
    
    half sketchMask = max(sketch1, sketch2);
    
    return baseColor * lerp(0.5, 1.0, sketchMask);
}

// Watercolor effect
half3 ApplyWatercolorEffect(half3 baseColor, float2 uv, half wetness)
{
    // Generate watercolor bleeding pattern
    float2 watercolorUV = uv * 5.0 + _Time.y * 0.1;
    half watercolorNoise = SimpleNoise(watercolorUV);
    
    // Apply color bleeding
    half3 bleedColor = baseColor * 1.2; // Slightly brighter
    half bleedMask = step(1.0 - wetness, watercolorNoise);
    
    return lerp(baseColor, bleedColor, bleedMask * 0.3);
}

// Edge enhancement for comic book style
half3 ApplyEdgeEnhancement(half3 baseColor, float2 uv, half edgeStrength)
{
    // Simple edge detection using UV derivatives
    half2 uvDelta = abs(ddx(uv)) + abs(ddy(uv));
    half edgeMask = saturate(length(uvDelta) * 1000.0);
    
    // Enhance edges
    half3 edgeColor = half3(0, 0, 0); // Black edges
    return lerp(baseColor, edgeColor, edgeMask * edgeStrength);
}

#endif