#ifndef GORGONIZE_TOON_EFFECTS_INCLUDED
#define GORGONIZE_TOON_EFFECTS_INCLUDED

#include "GorgonizeToonProperties.hlsl"
#include "GorgonizeToonUtilities.hlsl"

// ===== FORCE FIELD EFFECTS =====

float3 ApplyForceFieldEffect(float3 color, float3 normal, float3 viewDir, float time, float3 forceFieldColor, float intensity, float frequency)
{
    #ifdef _FORCE_FIELD
        float fresnel = CalculateFresnelAdvanced(viewDir, normal, 2.0, 0);
        
        // Animated energy rings
        float rings = sin(fresnel * frequency + time * 5.0) * 0.5 + 0.5;
        
        // Pulsing effect
        float pulse = sin(time * 3.0) * 0.3 + 0.7;
        
        // Energy color
        float3 energyColor = forceFieldColor * rings * fresnel * pulse * intensity;
        
        return color + energyColor;
    #endif
    
    return color;
}

// ===== HOLOGRAM EFFECTS =====

float3 ApplyHologramEffect(float3 color, float2 uv, float time, float intensity, float flicker, float scanlines)
{
    #ifdef _HOLOGRAM
        // Scan lines
        float scanlineEffect = sin(uv.y * scanlines + time * 10.0) * 0.04 + 0.96;
        
        // Interference patterns
        float interference = sin(uv.x * 200.0 + time * 15.0) * 0.02 + 0.98;
        
        // Flicker effect
        float flickerEffect = sin(time * flicker * 20.0) * 0.05 + 0.95;
        
        // Apply hologram modulation
        color *= scanlineEffect * interference * flickerEffect;
        
        // Add blue holographic tint
        color *= float3(0.8, 0.9, 1.2) * intensity;
        
        // Add some digital noise
        float noise = frac(sin(dot(uv + time, float2(12.9898, 78.233))) * 43758.5453);
        color += noise * 0.05 * intensity;
    #endif
    
    return color;
}

// ===== DISSOLVE EFFECTS =====

float CalculateDissolveEffect(float noiseValue, float dissolveAmount, float edgeWidth)
{
    #ifdef _DISSOLVE
        // Create dissolve mask
        float dissolve = step(dissolveAmount, noiseValue);
        
        // Create edge glow
        float edge = smoothstep(dissolveAmount, dissolveAmount + edgeWidth, noiseValue) - 
                    smoothstep(dissolveAmount + edgeWidth, dissolveAmount + edgeWidth * 2.0, noiseValue);
        
        return dissolve;
    #endif
    
    return 1.0;
}

// ===== WIREFRAME EFFECTS =====

float3 ApplyWireframe(float3 color, float3 worldPos, float3 wireframeColor, float thickness)
{
    #ifdef _WIREFRAME
        // Simple wireframe based on world position derivatives
        float3 fw = fwidth(worldPos);
        float3 wireframe = step(thickness, fw);
        float wireMask = min(min(wireframe.x, wireframe.y), wireframe.z);
        
        return lerp(wireframeColor, color, wireMask);
    #endif
    
    return color;
}

// ===== DEBUG VISUALIZATION =====

float3 ApplyDebugVisualization(float3 color, ToonSurfaceData surfaceData, ToonLightingData lightingData, float debugMode)
{
    #ifdef _DEBUG_MODE
        if (debugMode > 0.5)
        {
            if (debugMode < 1.5) // Normals
            {
                return surfaceData.normalWS * 0.5 + 0.5;
            }
            else if (debugMode < 2.5) // Lighting
            {
                Light mainLight = GetMainLight(lightingData.shadowCoord);
                float NdotL = saturate(dot(surfaceData.normalWS, mainLight.direction));
                return NdotL.xxx;
            }
            else if (debugMode < 3.5) // Shadows
            {
                Light mainLight = GetMainLight(lightingData.shadowCoord);
                return mainLight.shadowAttenuation.xxx;
            }
            else if (debugMode < 4.5) // Hatching
            {
                Light mainLight = GetMainLight(lightingData.shadowCoord);
                float NdotL = saturate(dot(surfaceData.normalWS, mainLight.direction));
                float lightValue = smoothstep(_ShadowThreshold - _ShadowSmoothness, _ShadowThreshold + _ShadowSmoothness, NdotL);
                return lightValue.xxx;
            }
        }
    #endif
    
    return color;
}

// ===== LOD FADE =====

float CalculateLODFade(float3 worldPos, float fadeDistance)
{
    #ifdef _LOD_FADE
        float distance = length(worldPos - _WorldSpaceCameraPos);
        return 1.0 - saturate(distance / fadeDistance);
    #endif
    
    return 1.0;
}

#endif