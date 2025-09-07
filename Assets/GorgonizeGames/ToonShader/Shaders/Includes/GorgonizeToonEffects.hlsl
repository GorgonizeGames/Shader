#ifndef GORGONIZE_TOON_EFFECTS_INCLUDED
#define GORGONIZE_TOON_EFFECTS_INCLUDED

#include "Assets/GorgonizeGames/ToonShader/Shaders/Includes/GorgonizeToonHelpers.hlsl"

// Hatching effect implementation
half3 ApplyHatchingEffect(float2 uv, float3 worldPos, half3 albedo, half lightIntensity)
{
    #if !ENABLE_HATCHING
        return half3(0, 0, 0);
    #endif
    
    // Multiple hatching patterns based on light intensity
    float2 hatchUV1 = TRANSFORM_TEX(uv, _HatchingTex) * _HatchingScale;
    float2 hatchUV2 = hatchUV1 * 2.0;
    float2 hatchUV3 = hatchUV1 * 4.0;
    
    // Rotate UV coordinates for different hatching directions
    float angle1 = 0.0;
    float angle2 = 3.14159 * 0.25; // 45 degrees
    float angle3 = 3.14159 * 0.5;  // 90 degrees
    
    // Rotation matrices
    float2x2 rot1 = float2x2(cos(angle1), -sin(angle1), sin(angle1), cos(angle1));
    float2x2 rot2 = float2x2(cos(angle2), -sin(angle2), sin(angle2), cos(angle2));
    float2x2 rot3 = float2x2(cos(angle3), -sin(angle3), sin(angle3), cos(angle3));
    
    // Sample hatching patterns
    half hatch1 = SAMPLE_TEXTURE2D(_HatchingTex, sampler_HatchingTex, mul(rot1, hatchUV1)).r;
    half hatch2 = SAMPLE_TEXTURE2D(_HatchingTex, sampler_HatchingTex, mul(rot2, hatchUV2)).r;
    half hatch3 = SAMPLE_TEXTURE2D(_HatchingTex, sampler_HatchingTex, mul(rot3, hatchUV3)).r;
    
    // Combine hatching patterns based on light intensity
    half hatchMask1 = 1.0 - smoothstep(0.8, 1.0, lightIntensity);
    half hatchMask2 = 1.0 - smoothstep(0.6, 0.8, lightIntensity);
    half hatchMask3 = 1.0 - smoothstep(0.3, 0.6, lightIntensity);
    
    half combinedHatch = 
        hatch1 * hatchMask1 + 
        hatch2 * hatchMask2 + 
        hatch3 * hatchMask3;
    
    combinedHatch = saturate(combinedHatch) * _HatchingIntensity;
    
    // Apply hatching as a darkening effect
    return albedo * (1.0 - combinedHatch * 0.7);
}

// Stylized fog effect
half3 ApplyStylizedFog(half3 color, float3 worldPos, half fogDensity)
{
    float distance = length(worldPos - GetCameraPositionWS());
    
    // Stylized fog calculation
    half fogFactor = exp2(-fogDensity * distance * distance * 1.442695);
    fogFactor = 1.0 - fogFactor;
    
    // Toon-style fog with steps
    fogFactor = floor(fogFactor * 4.0) / 4.0;
    
    // Mix with fog color
    half3 fogColor = unity_FogColor.rgb;
    return lerp(color, fogColor, fogFactor);
}

// Color grading for toon look
half3 ApplyToonColorGrading(half3 color)
{
    // Increase saturation
    half3 hsv = RGBToHSV(color);
    hsv.y = saturate(hsv.y * 1.2); // Boost saturation
    hsv.z = saturate(hsv.z * 1.1); // Slight brightness boost
    
    return HSVToRGB(hsv);
}

// Posterization effect
half3 ApplyPosterization(half3 color, half levels)
{
    return floor(color * levels) / levels;
}

// Cross-hatching effect for shadows
half3 ApplyCrossHatching(float2 uv, half shadowIntensity, half3 baseColor)
{
    #if !ENABLE_HATCHING
        return baseColor;
    #endif
    
    // Base hatching pattern
    float2 hatchUV = uv * _HatchingScale * 10.0;
    
    // Primary hatching lines (horizontal)
    half hatch1 = step(0.5, frac(hatchUV.y));
    
    // Secondary hatching lines (vertical)
    half hatch2 = step(0.5, frac(hatchUV.x));
    
    // Diagonal hatching
    half hatch3 = step(0.5, frac((hatchUV.x + hatchUV.y) * 0.707));
    half hatch4 = step(0.5, frac((hatchUV.x - hatchUV.y) * 0.707));
    
    // Combine hatching based on shadow intensity
    half crossHatch = 0;
    if (shadowIntensity > 0.75)
        crossHatch = hatch1 * hatch2 * hatch3 * hatch4;
    else if (shadowIntensity > 0.5)
        crossHatch = hatch1 * hatch2 * hatch3;
    else if (shadowIntensity > 0.25)
        crossHatch = hatch1 * hatch2;
    else if (shadowIntensity > 0.0)
        crossHatch = hatch1;
    
    // Apply cross-hatching as darkening
    return baseColor * (1.0 - crossHatch * shadowIntensity * 0.3);
}

// Screen-space dithering for transparency
half ApplyTransparencyDithering(float2 screenPos, half alpha)
{
    #if !ENABLE_DITHERING
        return alpha;
    #endif
    
    // Generate dithering pattern
    float2 ditherCoord = screenPos / 4.0;
    
    // Simple 4x4 Bayer matrix
    int x = int(ditherCoord.x) % 4;
    int y = int(ditherCoord.y) % 4;
    
    float ditherThreshold[16] = {
        0.0625, 0.5625, 0.1875, 0.6875,
        0.8125, 0.3125, 0.9375, 0.4375,
        0.25,   0.75,   0.125,  0.625,
        1.0,    0.5,    0.875,  0.375
    };
    
    half threshold = ditherThreshold[y * 4 + x];
    
    // Apply dithering
    return step(threshold, alpha);
}

// Stylized reflection calculation
half3 CalculateStylizedReflection(half3 viewDir, half3 normal, half3 worldPos)
{
    // Simple fake reflection using world position and normal
    half3 reflectionDir = reflect(-viewDir, normal);
    
    // Use world position as a pseudo-environment map
    half3 envColor = sin(worldPos * 0.1 + _Time.y) * 0.5 + 0.5;
    
    // Apply toon-style quantization
    envColor = floor(envColor * 4.0) / 4.0;
    
    return envColor;
}

// Wind animation for vegetation
float3 ApplyWindAnimation(float3 worldPos, float3 objectPos, half windStrength)
{
    // Simple wind effect using sine waves
    half windPhase = _Time.y * 2.0 + worldPos.x * 0.1 + worldPos.z * 0.1;
    half windOffset = sin(windPhase) * windStrength;
    
    // Apply wind mainly to Y axis
    return objectPos + float3(windOffset * 0.1, windOffset, windOffset * 0.1);
}

// Outline glow effect
half3 ApplyOutlineGlow(half3 outlineColor, float2 screenPos, half glowIntensity)
{
    // Simple glow effect by sampling neighboring pixels
    float2 texelSize = 1.0 / _ScreenParams.xy;
    
    half glow = 0;
    for (int x = -1; x <= 1; x++)
    {
        for (int y = -1; y <= 1; y++)
        {
            if (x == 0 && y == 0) continue;
            
            float2 offset = float2(x, y) * texelSize * 2.0;
            // Note: This would require access to the outline render texture
            // glow += SampleOutlineTexture(screenPos + offset);
        }
    }
    
    glow *= glowIntensity * 0.125; // Average of 8 samples
    
    return outlineColor + glow;
}

// Texture bombing for more natural look
half4 TextureBombing(TEXTURE2D(tex), SAMPLER(samp), float2 uv, half scale, half randomness)
{
    // Split UV into cells
    float2 cellUV = floor(uv * scale);
    float2 localUV = frac(uv * scale);
    
    // Generate random offset for each cell
    float2 randomOffset = float2(
        Random(cellUV + float2(0, 0)),
        Random(cellUV + float2(1, 1))
    ) * randomness;
    
    // Sample texture with offset
    return SAMPLE_TEXTURE2D(tex, samp, localUV + randomOffset);
}

// Edge detection for outline enhancement
half DetectEdges(float2 uv, TEXTURE2D(depthTex), SAMPLER(depthSampler))
{
    float2 texelSize = 1.0 / _ScreenParams.xy;
    
    // Sample depth in cross pattern
    half depthC = SAMPLE_TEXTURE2D(depthTex, depthSampler, uv).r;
    half depthN = SAMPLE_TEXTURE2D(depthTex, depthSampler, uv + float2(0, texelSize.y)).r;
    half depthS = SAMPLE_TEXTURE2D(depthTex, depthSampler, uv + float2(0, -texelSize.y)).r;
    half depthE = SAMPLE_TEXTURE2D(depthTex, depthSampler, uv + float2(texelSize.x, 0)).r;
    half depthW = SAMPLE_TEXTURE2D(depthTex, depthSampler, uv + float2(-texelSize.x, 0)).r;
    
    // Calculate edge strength
    half edgeH = abs(depthE - depthW);
    half edgeV = abs(depthN - depthS);
    
    return saturate(edgeH + edgeV);
}

#endif