#ifndef TOON_EFFECTS_INCLUDED
#define TOON_EFFECTS_INCLUDED

#include "ToonUtilities.hlsl"

// Advanced rim lighting with multiple layers
float3 CalculateAdvancedRimLighting(float3 viewDir, float3 normal, float3 lightDir, float3 rimColor, float power, float intensity, float threshold)
{
    float rim = CalculateRimLighting(viewDir, normal, power, threshold);
    
    // Add light-dependent rim enhancement
    float lightRim = saturate(dot(normal, lightDir)) * 0.5 + 0.5;
    rim *= lightRim;
    
    return rim * rimColor * intensity;
}

// Multi-layer specular with different sizes
float3 CalculateMultiSpecular(float3 lightDir, float3 viewDir, float3 normal, float3 specColor, float intensity)
{
    float3 specular = 0;
    
    // Primary specular
    float spec1 = CalculateToonSpecular(lightDir, viewDir, normal, 0.1, 0.02);
    specular += spec1 * intensity;
    
    // Secondary smaller specular
    float spec2 = CalculateToonSpecular(lightDir, viewDir, normal, 0.05, 0.01);
    specular += spec2 * intensity * 0.5;
    
    return specular * specColor;
}

// Dynamic hatching with animation support
float CalculateAnimatedHatching(float2 uv, float lightValue, float4 screenPos, float time)
{
    float hatching = 1.0;
    
    #ifdef _HATCHING
        // Animated rotation
        float animatedRotation = _HatchingRotation + sin(time * 0.5) * 10.0;
        
        float2 hatchUV = RotateUV(uv * _HatchingDensity, animatedRotation);
        
        // Add some noise to make it more organic
        hatchUV += Noise(uv * 10.0 + time) * 0.02;
        
        float hatch1 = SAMPLE_TEXTURE2D(_HatchingTex, sampler_HatchingTex, hatchUV).r;
        
        if (lightValue < _HatchingThreshold)
        {
            hatching *= lerp(1.0, hatch1, _HatchingIntensity);
        }
        
        if (lightValue < _CrossHatchingThreshold)
        {
            float2 crossHatchUV = RotateUV(uv * _HatchingDensity, animatedRotation + 90);
            crossHatchUV += Noise(uv * 8.0 + time * 0.7) * 0.02;
            float crossHatch = SAMPLE_TEXTURE2D(_CrossHatchingTex, sampler_CrossHatchingTex, crossHatchUV).r;
            hatching *= lerp(1.0, crossHatch, _HatchingIntensity);
        }
    #endif
    
    return hatching;
}

// Outline calculation for edge detection
float CalculateOutlineFromDepth(float2 uv, float2 texelSize)
{
    #ifdef _OUTLINE
        float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
        
        float depthN = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv + float2(0, texelSize.y));
        float depthS = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv - float2(0, texelSize.y));
        float depthE = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv + float2(texelSize.x, 0));
        float depthW = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv - float2(texelSize.x, 0));
        
        float edge = abs(depthN - depth) + abs(depthS - depth) + abs(depthE - depth) + abs(depthW - depth);
        return saturate(edge * 100);
    #endif
    
    return 0;
}

// Normal-based outline calculation
float CalculateOutlineFromNormal(float3 normalWS, float3 viewDirWS)
{
    #ifdef _OUTLINE
        float normalOutline = 1.0 - abs(dot(normalWS, viewDirWS));
        return smoothstep(0.0, 1.0, normalOutline);
    #endif
    
    return 0;
}

// Advanced emission with scrolling and pulsing
float3 CalculateAdvancedEmission(float2 uv, float time)
{
    float3 emission = 0;
    
    #ifdef _EMISSION
        // Scrolling UV
        float2 emissionUV = uv + _EmissionScrollSpeed.xy * time;
        
        // Sample emission texture
        float3 emissionTex = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, emissionUV).rgb;
        
        // Apply pulsing animation
        float pulse = PulseAnimation(time, 2.0, 2.0);
        
        // Combine with color and intensity
        emission = emissionTex * _EmissionColor.rgb * _EmissionIntensity * pulse;
        
        // Add some noise for organic feel
        float noise = AnimatedNoise(uv * 5.0, time);
        emission *= (0.8 + noise * 0.2);
    #endif
    
    return emission;
}

// Stylized fog effect
float3 ApplyStylizedFog(float3 color, float3 worldPos, float3 cameraPos)
{
    float distance = length(worldPos - cameraPos);
    
    // Stylized fog with steps
    float fogFactor = saturate(distance * 0.01);
    fogFactor = Posterize(fogFactor, 4); // Quantize fog for toon look
    
    float3 fogColor = lerp(float3(0.8, 0.9, 1.0), float3(0.5, 0.6, 0.8), fogFactor);
    
    return lerp(color, fogColor, fogFactor * 0.3);
}

// Wind animation for vegetation
float3 ApplyWindAnimation(float3 worldPos, float3 normal, float time)
{
    float windStrength = 0.1;
    float windSpeed = 2.0;
    
    // Create wind waves
    float wind = sin(worldPos.x * 0.1 + time * windSpeed) * cos(worldPos.z * 0.1 + time * windSpeed * 0.7);
    wind *= windStrength;
    
    // Apply wind based on normal (affects leaves more than trunk)
    float3 windOffset = float3(wind, wind * 0.5, wind * 0.3) * saturate(normal.y);
    
    return worldPos + windOffset;
}

// Dissolve effect
float CalculateDissolveEffect(float2 uv, float dissolveAmount, float edgeWidth)
{
    float noise = Noise(uv * 10.0);
    
    float dissolve = step(dissolveAmount, noise);
    float edge = smoothstep(dissolveAmount, dissolveAmount + edgeWidth, noise);
    
    return dissolve;
}

// Hologram effect
float3 ApplyHologramEffect(float3 color, float2 uv, float time)
{
    // Scan lines
    float scanlines = sin(uv.y * 800.0 + time * 10.0) * 0.04 + 0.96;
    
    // Interference
    float interference = sin(uv.x * 200.0 + time * 15.0) * 0.02 + 0.98;
    
    // Flicker
    float flicker = sin(time * 20.0) * 0.05 + 0.95;
    
    // Apply hologram modulation
    color *= scanlines * interference * flicker;
    
    // Add blue tint
    color *= float3(0.8, 0.9, 1.2);
    
    return color;
}

// Force field effect
float3 ApplyForceFieldEffect(float3 color, float3 normal, float3 viewDir, float time)
{
    float fresnel = CalculateFresnel(viewDir, normal, 2.0);
    
    // Animated rings
    float rings = sin(fresnel * 10.0 + time * 5.0) * 0.5 + 0.5;
    
    // Energy color
    float3 energyColor = float3(0.2, 0.8, 1.0) * rings * fresnel;
    
    return color + energyColor * 0.5;
}

// Cartoon water effect
float3 CalculateCartoonWater(float2 uv, float time, float3 normal, float3 viewDir)
{
    // Animated water normals
    float2 waterUV1 = uv * 0.1 + time * 0.02;
    float2 waterUV2 = uv * 0.15 + time * -0.015;
    
    float wave1 = sin(waterUV1.x * 20.0 + time * 3.0) * cos(waterUV1.y * 20.0 + time * 2.0);
    float wave2 = sin(waterUV2.x * 15.0 + time * -2.5) * cos(waterUV2.y * 15.0 + time * 3.5);
    
    float waves = (wave1 + wave2) * 0.1;
    
    // Simple water color
    float3 waterColor = float3(0.2, 0.6, 0.8);
    
    // Fresnel for water surface
    float fresnel = CalculateFresnel(viewDir, normal, 3.0);
    
    // Reflection tint
    float3 reflection = float3(0.8, 0.9, 1.0) * fresnel;
    
    return waterColor + reflection + waves;
}

// Toon cloud shadows
float CalculateToonCloudShadows(float3 worldPos, float time)
{
    float2 cloudUV = worldPos.xz * 0.001 + time * 0.001;
    
    float clouds = Noise(cloudUV) * Noise(cloudUV * 2.1) * Noise(cloudUV * 4.3);
    clouds = Posterize(clouds, 3); // Quantize for toon look
    
    return lerp(0.7, 1.0, clouds);
}

// Stylized caustics
float3 CalculateStylizedCaustics(float2 uv, float time)
{
    float2 causticsUV = uv * 2.0 + time * 0.1;
    
    float caustic1 = sin(causticsUV.x * 10.0 + time) * cos(causticsUV.y * 8.0 + time * 0.7);
    float caustic2 = sin(causticsUV.x * 7.0 + time * 1.3) * cos(causticsUV.y * 11.0 + time * 0.9);
    
    float caustics = (caustic1 + caustic2) * 0.5 + 0.5;
    caustics = Posterize(caustics, 4);
    
    return float3(0.8, 0.9, 1.0) * caustics * 0.3;
}

// Magical sparkle effects
float3 CalculateMagicalSparkles(float2 uv, float time, float intensity)
{
    float sparkles = 0;
    
    // Multiple layers of sparkles
    for (int i = 0; i < 3; i++)
    {
        float2 sparkleUV = uv * (5.0 + i * 3.0) + time * (0.5 + i * 0.2);
        float sparkle = Hash(floor(sparkleUV));
        
        // Make sparkles appear and disappear
        float sparkleTime = frac(time * (1.0 + i * 0.3) + sparkle * 10.0);
        sparkle *= step(0.95, sparkle) * smoothstep(0.0, 0.1, sparkleTime) * smoothstep(1.0, 0.9, sparkleTime);
        
        sparkles += sparkle;
    }
    
    return float3(1.0, 0.9, 0.7) * sparkles * intensity;
}

// Cartoon fire effect
float3 CalculateCartoonFire(float2 uv, float time)
{
    float2 fireUV = uv + float2(sin(time * 2.0) * 0.1, time * 0.5);
    
    float fire = Noise(fireUV * 3.0) * Noise(fireUV * 6.0 + time) * Noise(fireUV * 12.0 + time * 2.0);
    fire = smoothstep(0.3, 1.0, fire);
    
    // Fire colors
    float3 fireColor = lerp(float3(1.0, 0.3, 0.0), float3(1.0, 1.0, 0.2), fire);
    
    return fireColor * fire;
}

// Screen distortion effect
float2 ApplyScreenDistortion(float2 uv, float intensity, float time)
{
    float2 distortion = float2(
        sin(uv.y * 10.0 + time * 3.0),
        cos(uv.x * 8.0 + time * 2.0)
    ) * intensity * 0.01;
    
    return uv + distortion;
}

#endif