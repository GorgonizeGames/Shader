#ifndef TOON_RIM_INCLUDED
#define TOON_RIM_INCLUDED

#include "ToonHelpers.hlsl"

// External properties are declared in the main pass file
// We'll access them directly here

// Calculate rim lighting effects
half3 CalculateRimLighting(ToonSurfaceData surfaceData, Light light)
{
    #ifdef _ENABLE_RIM
        half3 viewDir = normalize(surfaceData.viewDirectionWS);
        half3 normalWS = surfaceData.normalWS;
        
        // Primary rim light calculation
        half rimDot = 1.0 - saturate(dot(viewDir, normalWS));
        half rimIntensity = pow(rimDot, _RimPower);
        
        #ifdef _LIGHT_BASED_RIM
            // Light-based rim lighting (dynamic with light direction)
            half3 lightDir = normalize(light.direction);
            half lightRimFactor = saturate(dot(normalWS, lightDir));
            rimIntensity *= lightRimFactor;
        #endif
        
        // Apply rim smoothness and intensity
        rimIntensity = smoothstep(0.0, 1.0, rimIntensity);
        half3 primaryRim = _RimColor.rgb * rimIntensity * _RimIntensity;
        
        #ifdef _ENABLE_SECONDARY_RIM
            // Secondary rim light (often used for backlighting effect)
            half secondaryRimDot = 1.0 - saturate(dot(viewDir, normalWS));
            half secondaryRimIntensity = pow(secondaryRimDot, _SecondaryRimPower);
            
            #ifdef _LIGHT_BASED_RIM
                // Apply same light influence to secondary rim
                secondaryRimIntensity *= lightRimFactor;
            #endif
            
            secondaryRimIntensity = smoothstep(0.0, 1.0, secondaryRimIntensity);
            half3 secondaryRim = _SecondaryRimColor.rgb * secondaryRimIntensity * _SecondaryRimIntensity;
            
            // Combine primary and secondary rim
            return primaryRim + secondaryRim;
        #else
            return primaryRim;
        #endif
    #else
        return half3(0, 0, 0);
    #endif
}

// Advanced rim lighting with light position masking
half3 CalculateAdvancedRimLighting(ToonSurfaceData surfaceData, Light light, half3 lightPositionMask)
{
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    half3 normalWS = surfaceData.normalWS;
    half3 lightDir = SafeNormalize(light.direction);
    
    // Basic rim calculation
    half rimDot = 1.0 - saturate(dot(viewDir, normalWS));
    half rimIntensity = pow(rimDot, _RimPower);
    
    // Apply light direction masking (rim only appears where light hits)
    half lightAlignment = saturate(dot(normalWS, lightDir));
    rimIntensity *= lightAlignment;
    
    // Apply positional masking (e.g., only top, bottom, or sides)
    half3 worldUp = half3(0, 1, 0);
    half3 worldRight = half3(1, 0, 0);
    half3 worldForward = half3(0, 0, 1);
    
    half upMask = saturate(dot(normalWS, worldUp)) * lightPositionMask.y;
    half sideMask = abs(dot(normalWS, worldRight)) * lightPositionMask.x;
    half frontMask = saturate(dot(normalWS, worldForward)) * lightPositionMask.z;
    
    half positionMask = max(max(upMask, sideMask), frontMask);
    rimIntensity *= positionMask;
    
    return _RimColor.rgb * rimIntensity * _RimIntensity * light.color;
}

// Fresnel-based rim lighting
half3 CalculateFresnelRim(ToonSurfaceData surfaceData, half fresnelPower, half3 fresnelColor)
{
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    half3 normalWS = surfaceData.normalWS;
    
    // Calculate Fresnel term
    half fresnel = 1.0 - saturate(dot(viewDir, normalWS));
    fresnel = pow(fresnel, fresnelPower);
    
    // Apply toon-style quantization to Fresnel
    fresnel = smoothstep(0.3, 0.8, fresnel);
    
    return fresnelColor * fresnel;
}

// Distance-based rim lighting (for atmospheric effects)
half3 CalculateDistanceRim(ToonSurfaceData surfaceData, float3 positionWS, half maxDistance)
{
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    half3 normalWS = surfaceData.normalWS;
    
    // Calculate distance from camera
    float distanceToCamera = length(_WorldSpaceCameraPos - positionWS);
    half distanceFactor = saturate(distanceToCamera / maxDistance);
    
    // Basic rim calculation
    half rimDot = 1.0 - saturate(dot(viewDir, normalWS));
    half rimIntensity = pow(rimDot, _RimPower);
    
    // Apply distance-based fade
    rimIntensity *= distanceFactor;
    
    return _RimColor.rgb * rimIntensity * _RimIntensity;
}

// Animated rim lighting (for magical or energy effects)
half3 CalculateAnimatedRim(ToonSurfaceData surfaceData, half animationSpeed, half pulseIntensity)
{
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    half3 normalWS = surfaceData.normalWS;
    
    // Time-based animation
    half time = _Time.y * animationSpeed;
    half pulse = sin(time) * 0.5 + 0.5; // 0-1 oscillation
    pulse = pow(pulse, pulseIntensity); // Control pulse sharpness
    
    // Basic rim calculation
    half rimDot = 1.0 - saturate(dot(viewDir, normalWS));
    half rimIntensity = pow(rimDot, _RimPower);
    
    // Apply animation
    rimIntensity *= pulse;
    
    // Animate color as well
    half3 animatedColor = lerp(_RimColor.rgb, _SecondaryRimColor.rgb, pulse);
    
    return animatedColor * rimIntensity * _RimIntensity;
}

// Multi-layered rim lighting for complex effects
half3 CalculateLayeredRim(ToonSurfaceData surfaceData, Light light)
{
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    half3 normalWS = surfaceData.normalWS;
    
    half rimDot = 1.0 - saturate(dot(viewDir, normalWS));
    
    // Inner rim (sharp, bright)
    half innerRim = pow(rimDot, _RimPower * 2.0);
    innerRim = step(0.8, innerRim); // Sharp cutoff
    
    // Outer rim (soft, subtle)
    half outerRim = pow(rimDot, _RimPower * 0.5);
    outerRim = smoothstep(0.2, 0.6, outerRim);
    
    // Combine layers
    half3 innerColor = _RimColor.rgb * innerRim;
    half3 outerColor = _SecondaryRimColor.rgb * outerRim * 0.5;
    
    return (innerColor + outerColor) * _RimIntensity;
}

// Stylized rim lighting with noise for organic effects
half3 CalculateNoisyRim(ToonSurfaceData surfaceData, half noiseScale, half noiseIntensity)
{
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    half3 normalWS = surfaceData.normalWS;
    
    // Basic rim calculation
    half rimDot = 1.0 - saturate(dot(viewDir, normalWS));
    half rimIntensity = pow(rimDot, _RimPower);
    
    // Add noise variation
    float2 noiseUV = surfaceData.uv * noiseScale + _Time.y * 0.1;
    half noise = SimpleNoise(noiseUV) * 0.5 + 0.5;
    
    // Apply noise to rim intensity
    rimIntensity *= lerp(1.0 - noiseIntensity, 1.0 + noiseIntensity, noise);
    rimIntensity = saturate(rimIntensity);
    
    return _RimColor.rgb * rimIntensity * _RimIntensity;
}

// Temperature-based rim lighting (hot/cold effects)
half3 CalculateTemperatureRim(ToonSurfaceData surfaceData, half temperature)
{
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    half3 normalWS = surfaceData.normalWS;
    
    // Basic rim calculation
    half rimDot = 1.0 - saturate(dot(viewDir, normalWS));
    half rimIntensity = pow(rimDot, _RimPower);
    
    // Temperature-based color mixing
    half3 coldColor = half3(0.3, 0.5, 1.0); // Blue-ish for cold
    half3 hotColor = half3(1.0, 0.3, 0.1);  // Red-ish for hot
    half3 temperatureColor = lerp(coldColor, hotColor, temperature);
    
    // Combine with user-defined rim color
    half3 finalColor = lerp(_RimColor.rgb, temperatureColor, 0.7);
    
    return finalColor * rimIntensity * _RimIntensity;
}

// Directional rim lighting (based on world space directions)
half3 CalculateDirectionalRim(ToonSurfaceData surfaceData, half3 rimDirection, half directionInfluence)
{
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    half3 normalWS = surfaceData.normalWS;
    
    // Basic rim calculation
    half rimDot = 1.0 - saturate(dot(viewDir, normalWS));
    half rimIntensity = pow(rimDot, _RimPower);
    
    // Apply directional influence
    half directionDot = saturate(dot(normalWS, normalize(rimDirection)));
    rimIntensity *= lerp(1.0, directionDot, directionInfluence);
    
    return _RimColor.rgb * rimIntensity * _RimIntensity;
}

// Screen-space rim lighting for consistent edge detection
half3 CalculateScreenSpaceRim(ToonSurfaceData surfaceData, float4 screenPos)
{
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    half3 normalWS = surfaceData.normalWS;
    
    // Convert to screen space for consistent rim width
    float2 screenUV = screenPos.xy / screenPos.w;
    
    // Sample depth or normal buffer for edge detection (if available)
    // This is a simplified version - in practice you'd sample actual buffers
    half rimDot = 1.0 - saturate(dot(viewDir, normalWS));
    half rimIntensity = pow(rimDot, _RimPower);
    
    // Screen space consistent rim width
    half screenSpaceRim = smoothstep(0.4, 0.6, rimIntensity);
    
    return _RimColor.rgb * screenSpaceRim * _RimIntensity;
}

// Vertex color-based rim modulation
half3 CalculateVertexColorRim(ToonSurfaceData surfaceData, half4 vertexColor)
{
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    half3 normalWS = surfaceData.normalWS;
    
    // Basic rim calculation
    half rimDot = 1.0 - saturate(dot(viewDir, normalWS));
    half rimIntensity = pow(rimDot, _RimPower);
    
    // Use vertex color to modulate rim properties
    half intensityMod = vertexColor.r; // Red channel controls intensity
    half powerMod = lerp(0.5, 2.0, vertexColor.g); // Green channel controls power
    half3 colorMod = vertexColor.rgb; // RGB channels control color
    
    rimIntensity = pow(rimDot, _RimPower * powerMod);
    rimIntensity *= intensityMod;
    
    half3 modifiedColor = _RimColor.rgb * colorMod;
    
    return modifiedColor * rimIntensity * _RimIntensity;
}

#endif