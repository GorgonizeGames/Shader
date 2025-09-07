#ifndef GORGONIZE_TOON_LIGHTING_INCLUDED
#define GORGONIZE_TOON_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl"
#include "GorgonizeToonProperties.hlsl"
#include "GorgonizeToonUtilities.hlsl"

// ===== MAIN DIRECTIONAL LIGHT CALCULATION =====

float3 CalculateMainLight(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    Light mainLight = GetMainLight(lightingData.shadowCoord);
    float3 lightColor = mainLight.color;
    float3 lightDirection = mainLight.direction;
    float lightAttenuation = mainLight.shadowAttenuation;
    
    // Enhanced NdotL calculation with light wrapping
    float NdotL = dot(lightingData.normalWS, lightDirection);
    NdotL = ApplyAdvancedLightWrapping(NdotL, _LightWrapping, 1.0);
    
    // Calculate toon ramp
    float toonRamp;
    
    #ifdef _USE_RAMP_TEXTURE
        toonRamp = GORGONIZE_TOON_SAMPLE_2D(_LightRampTex, sampler_LightRampTex, float2(NdotL, 0.5)).r;
    #else
        toonRamp = CalculateAdvancedToonRamp(NdotL, _ShadowThreshold, _ShadowSmoothness, (int)_CelShadingSteps);
    #endif
    
    // Apply shadow attenuation
    toonRamp *= lightAttenuation;
    
    // Enhanced shadow mixing
    float3 shadowedColor = lerp(_ShadowColor.rgb * surfaceData.albedo, surfaceData.albedo, _ShadowIntensity);
    float3 litColor = lerp(shadowedColor, surfaceData.albedo, toonRamp);
    
    // Apply volumetric lighting if enabled
    #ifdef _ENABLE_VOLUMETRIC_LIGHTING
    if (_VolumetricIntensity > 0.001)
    {
        float volumetric = CalculateVolumetricLighting(lightingData.positionWS, lightDirection, lightingData.viewDirectionWS);
        litColor += volumetric * lightColor * _VolumetricIntensity;
    }
    #endif
    
    // Apply light cookies if enabled
    #ifdef _ENABLE_LIGHT_COOKIES
    if (_CookieInfluence > 0.001)
    {
        float cookie = CalculateLightCookie(lightingData.positionWS, mainLight);
        litColor *= lerp(1.0, cookie, _CookieInfluence);
    }
    #endif
    
    return litColor * lightColor;
}

// ===== ADDITIONAL LIGHTS CALCULATION =====

float3 CalculateAdditionalLights(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    float3 additionalLighting = 0;
    
    #ifdef _ADDITIONAL_LIGHTS
        uint pixelLightCount = GetAdditionalLightsCount();
        
        LIGHT_LOOP_BEGIN(pixelLightCount)
            Light light = GetAdditionalLight(lightIndex, lightingData.positionWS);
            
            if (light.distanceAttenuation > 0.001)
            {
                float3 lightColor = light.color;
                float3 lightDirection = light.direction;
                float lightAttenuation = light.shadowAttenuation * light.distanceAttenuation;
                
                float NdotL = dot(lightingData.normalWS, lightDirection);
                NdotL = ApplyAdvancedLightWrapping(NdotL, _LightWrapping, 1.0);
                
                float toonRamp;
                #ifdef _USE_RAMP_TEXTURE
                    toonRamp = GORGONIZE_TOON_SAMPLE_2D(_LightRampTex, sampler_LightRampTex, float2(NdotL, 0.5)).r;
                #else
                    toonRamp = CalculateAdvancedToonRamp(NdotL, _ShadowThreshold, _ShadowSmoothness, _CelShadingSteps);
                #endif
                
                toonRamp *= lightAttenuation;
                
                // Reduced intensity for additional lights to prevent over-lighting
                float intensityMultiplier = 0.5;
                additionalLighting += surfaceData.albedo * lightColor * toonRamp * intensityMultiplier;
            }
        LIGHT_LOOP_END
    #endif
    
    return additionalLighting;
}

// ===== VOLUMETRIC LIGHTING =====

float CalculateVolumetricLighting(float3 worldPos, float3 lightDir, float3 viewDir)
{
    // Simple volumetric scattering approximation
    float scattering = pow(saturate(dot(-lightDir, viewDir)), 2.0);
    
    // Add some noise for organic feel
    float noise = SimplexNoise(worldPos.xz * 0.01 + _Time.y * 0.1);
    scattering *= (0.8 + noise * 0.2);
    
    return scattering;
}

// ===== LIGHT COOKIE CALCULATION =====

float CalculateLightCookie(float3 worldPos, Light light)
{
    // Simplified light cookie calculation
    // In a full implementation, this would sample from light cookie textures
    float2 cookieUV = worldPos.xz * 0.1;
    return PerlinNoise(cookieUV) * 0.5 + 0.5;
}

// ===== RIM LIGHTING =====

float3 CalculateAdvancedRimLighting(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    float3 rimLighting = 0;
    
    #ifdef _RIM_LIGHTING
        Light mainLight = GetMainLight(lightingData.shadowCoord);
        
        // Primary rim lighting
        float rim1 = CalculateRimLighting(lightingData.viewDirectionWS, surfaceData.normalWS, _RimPower, _RimThreshold);
        
        // Light-dependent rim enhancement
        float lightInfluence = saturate(dot(surfaceData.normalWS, mainLight.direction)) * 0.5 + 0.5;
        rim1 *= lightInfluence;
        
        rimLighting = rim1 * _RimColor.rgb * _RimIntensity;
        
        // Dual layer rim lighting
        #ifdef _RIM_LIGHTING_DUAL_LAYER
            float rim2 = CalculateRimLighting(lightingData.viewDirectionWS, surfaceData.normalWS, _RimPowerSecondary, _RimThreshold * 0.5);
            rimLighting += rim2 * _RimColorSecondary.rgb * _RimIntensity * 0.5;
        #endif
        
        // Apply main light color influence
        rimLighting *= mainLight.color * mainLight.shadowAttenuation;
    #endif
    
    return rimLighting;
}

// ===== SPECULAR HIGHLIGHTS =====

float3 CalculateAdvancedSpecular(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    float3 specular = 0;
    
    #ifdef _SPECULAR
        Light mainLight = GetMainLight(lightingData.shadowCoord);
        float3 lightDirection = mainLight.direction;
        float lightAttenuation = mainLight.shadowAttenuation;
        
        float spec = 0;
        
        #ifdef _SPECULAR_ANISOTROPIC
            // Anisotropic specular
            spec = CalculateAnisotropicSpecular(lightDirection, lightingData.viewDirectionWS, 
                                              surfaceData.normalWS, surfaceData.tangentWS, 
                                              _SpecularSize, _SpecularSize * (1.0 + _Anisotropy));
        #elif defined(_SPECULAR_STEPPED)
            // Stepped specular
            spec = CalculateSteppedSpecular(lightDirection, lightingData.viewDirectionWS, 
                                          surfaceData.normalWS, _SpecularSize, 
                                          _SpecularSmoothness, _SpecularSteps);
        #else
            // Standard toon specular
            spec = CalculateToonSpecular(lightDirection, lightingData.viewDirectionWS, 
                                       surfaceData.normalWS, _SpecularSize, _SpecularSmoothness);
        #endif
        
        // Apply lighting conditions
        float NdotL = saturate(dot(surfaceData.normalWS, lightDirection));
        float toonRamp = CalculateAdvancedToonRamp(NdotL, _ShadowThreshold, _ShadowSmoothness, _CelShadingSteps);
        
        spec *= toonRamp * lightAttenuation * _SpecularIntensity;
        specular = spec * _SpecularColor.rgb * mainLight.color;
        
        // Add specular from additional lights
        #ifdef _ADDITIONAL_LIGHTS
            uint pixelLightCount = GetAdditionalLightsCount();
            
            LIGHT_LOOP_BEGIN(pixelLightCount)
                Light additionalLight = GetAdditionalLight(lightIndex, lightingData.positionWS);
                
                if (additionalLight.distanceAttenuation > 0.001)
                {
                    float additionalSpec = CalculateToonSpecular(additionalLight.direction, lightingData.viewDirectionWS,
                                                               surfaceData.normalWS, _SpecularSize, _SpecularSmoothness);
                    
                    float additionalNdotL = saturate(dot(surfaceData.normalWS, additionalLight.direction));
                    float additionalToonRamp = CalculateAdvancedToonRamp(additionalNdotL, _ShadowThreshold, _ShadowSmoothness, _CelShadingSteps);
                    
                    additionalSpec *= additionalToonRamp * additionalLight.shadowAttenuation * additionalLight.distanceAttenuation * _SpecularIntensity * 0.5;
                    specular += additionalSpec * _SpecularColor.rgb * additionalLight.color;
                }
            LIGHT_LOOP_END
        #endif
    #endif
    
    return specular;
}

// ===== FRESNEL EFFECTS =====

float3 CalculateAdvancedFresnel(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    float3 fresnel = 0;
    
    #ifdef _FRESNEL
        float fresnelTerm = CalculateFresnelAdvanced(lightingData.viewDirectionWS, surfaceData.normalWS, _FresnelPower, 0);
        fresnel = fresnelTerm * _FresnelColor.rgb * _FresnelIntensity;
        
        // Iridescence effect
        #ifdef _FRESNEL_IRIDESCENCE
        if (_IridescenceIntensity > 0.001)
        {
            float3 iridescence = CalculateIridescence(lightingData.viewDirectionWS, surfaceData.normalWS, fresnelTerm);
            fresnel += iridescence * _IridescenceIntensity;
        }
        #endif
    #endif
    
    return fresnel;
}

// Iridescence calculation
float3 CalculateIridescence(float3 viewDir, float3 normal, float fresnel)
{
    float phase = fresnel * TWO_PI * 3.0; // Multiple of wavelength
    
    float3 iridescent;
    iridescent.r = sin(phase) * 0.5 + 0.5;
    iridescent.g = sin(phase + TWO_PI / 3.0) * 0.5 + 0.5;
    iridescent.b = sin(phase + TWO_PI * 2.0 / 3.0) * 0.5 + 0.5;
    
    return iridescent * fresnel;
}

// ===== SUBSURFACE SCATTERING =====

float3 CalculateSubsurfaceScattering(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    float3 subsurface = 0;
    
    #ifdef _SUBSURFACE
        Light mainLight = GetMainLight(lightingData.shadowCoord);
        
        subsurface = CalculateAdvancedSubsurface(mainLight.direction, lightingData.viewDirectionWS, 
                                               surfaceData.normalWS, _SubsurfaceColor.rgb, 
                                               _SubsurfacePower, _SubsurfaceIntensity, 
                                               _SubsurfaceDistortion, surfaceData.subsurfaceThickness);
        
        // Apply main light influence
        subsurface *= mainLight.color * mainLight.shadowAttenuation;
    #endif
    
    return subsurface;
}

// ===== INDIRECT LIGHTING =====

float3 CalculateIndirectLighting(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    // Sample baked GI and apply toon treatment
    float3 indirectLighting = lightingData.bakedGI * surfaceData.albedo;
    
    // Apply indirect lighting boost
    indirectLighting *= _IndirectLightingBoost;
    
    // Apply ambient occlusion
    indirectLighting *= surfaceData.occlusion;
    
    // Apply color grading to indirect lighting
    indirectLighting = ApplyAdvancedColorGrading(indirectLighting, _Hue * 0.5, _Saturation, 
                                               _Brightness * 0.8, _Contrast, _Gamma, 
                                               _ColorTemperature * 0.5, _ColorTint * 0.5, _Vibrance * 0.5);
    
    return indirectLighting;
}

// ===== REFLECTION PROBES =====

float3 CalculateReflectionProbes(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    float3 reflection = 0;
    
    #if defined(_REFLECTION_PROBE_BLENDING) || defined(_REFLECTION_PROBE_BOX_PROJECTION)
        float3 reflectVector = reflect(-lightingData.viewDirectionWS, surfaceData.normalWS);
        
        // Sample reflection probe
        reflection = GlossyEnvironmentReflection(reflectVector, lightingData.positionWS, 
                                               surfaceData.smoothness, surfaceData.occlusion);
        
        // Apply toon treatment to reflections
        reflection = lerp(reflection, PosterizeWithDithering(reflection, 8.0, lightingData.screenPos.xy, 0.1), 0.3);
        
        // Metallic workflow
        reflection *= surfaceData.metallic;
    #endif
    
    return reflection;
}

// ===== MAIN LIGHTING FUNCTION =====

float3 CalculateGorgonizeToonLighting(ToonSurfaceData surfaceData, ToonLightingData lightingData, float2 uv)
{
    // Calculate main directional light
    float3 mainLighting = CalculateMainLight(surfaceData, lightingData);
    
    // Calculate light value for hatching and other effects
    Light mainLight = GetMainLight(lightingData.shadowCoord);
    float NdotL = saturate(dot(lightingData.normalWS, mainLight.direction));
    float lightValue = CalculateAdvancedToonRamp(NdotL, _ShadowThreshold, _ShadowSmoothness, _CelShadingSteps) * mainLight.shadowAttenuation;
    
    // Apply hatching effects
    float hatchingMask = CalculateAdvancedHatching(uv, lightValue, lightingData.screenPos, _Time.y * _AnimationSpeed);
    mainLighting *= hatchingMask;
    
    // Add additional lights
    float3 additionalLighting = CalculateAdditionalLights(surfaceData, lightingData);
    
    // Add indirect lighting
    float3 indirectLighting = CalculateIndirectLighting(surfaceData, lightingData);
    
    // Add reflection probes
    float3 reflectionLighting = CalculateReflectionProbes(surfaceData, lightingData);
    
    // Combine all lighting
    float3 finalColor = mainLighting + additionalLighting + indirectLighting + reflectionLighting;
    
    // Add rim lighting
    finalColor += CalculateAdvancedRimLighting(surfaceData, lightingData);
    
    // Add specular highlights
    finalColor += CalculateAdvancedSpecular(surfaceData, lightingData);
    
    // Apply matcap
    float3 matcap = CalculateAdvancedMatcap(surfaceData.normalWS, lightingData.viewDirectionWS, 
                                           lightingData.positionWS, _MatcapIntensity, _MatcapBlendMode, _MatcapRotation);
    
    // Apply matcap based on blend mode
    if (_MatcapBlendMode < 0.5) // Add
    {
        finalColor += matcap;
    }
    else if (_MatcapBlendMode < 1.5) // Multiply
    {
        finalColor *= matcap;
    }
    else if (_MatcapBlendMode < 2.5) // Screen
    {
        finalColor = 1 - (1 - finalColor) * (1 - matcap);
    }
    else // Overlay
    {
        finalColor = lerp(finalColor, finalColor < 0.5 ? 2.0 * finalColor * matcap : 1.0 - 2.0 * (1.0 - finalColor) * (1.0 - matcap), 1.0);
    }
    
    // Add fresnel effects
    finalColor += CalculateAdvancedFresnel(surfaceData, lightingData);
    
    // Add subsurface scattering
    finalColor += CalculateSubsurfaceScattering(surfaceData, lightingData);
    
    // Add emission
    finalColor += surfaceData.emission;
    
    // Apply global color grading
    finalColor = ApplyAdvancedColorGrading(finalColor, _Hue, _Saturation, _Brightness, 
                                         _Contrast, _Gamma, _ColorTemperature, _ColorTint, _Vibrance);
    
    // Apply posterization if enabled
    #ifdef _POSTERIZE
        finalColor = PosterizeWithDithering(finalColor, _PosterizeLevels, lightingData.screenPos.xy, _DitheringIntensity);
    #endif
    
    return finalColor;
}

// ===== ENHANCED LIGHTING WITH QUALITY SCALING =====

float3 CalculateGorgonizeToonLightingEnhanced(ToonSurfaceData surfaceData, ToonLightingData lightingData, float2 uv)
{
    float3 finalColor = CalculateGorgonizeToonLighting(surfaceData, lightingData, uv);
    
    // Quality-dependent enhancements
    if (ShouldUseHighQualityFeature())
    {
        // Add secondary bounce lighting approximation
        finalColor += CalculateSecondaryBounce(surfaceData, lightingData);
        
        // Enhanced atmospheric scattering
        finalColor += CalculateAtmosphericScattering(surfaceData, lightingData);
    }
    
    if (ShouldUseUltraQualityFeature())
    {
        // Add volumetric fog
        finalColor = ApplyVolumetricFog(finalColor, lightingData.positionWS, lightingData.viewDirectionWS);
        
        // Add advanced caustics
        finalColor += CalculateAdvancedCaustics(surfaceData, lightingData, uv);
    }
    
    return finalColor;
}

// ===== SECONDARY BOUNCE LIGHTING =====

float3 CalculateSecondaryBounce(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    // Simplified secondary bounce calculation
    Light mainLight = GetMainLight(lightingData.shadowCoord);
    
    // Assume ground bounce from a horizontal surface
    float3 bounceDirection = float3(0, 1, 0);
    float bounceAmount = saturate(-dot(surfaceData.normalWS, bounceDirection));
    
    // Approximate bounce color (warmer ground reflection)
    float3 bounceColor = float3(0.8, 0.7, 0.6) * 0.3;
    
    return bounceAmount * bounceColor * surfaceData.albedo * mainLight.color * mainLight.shadowAttenuation;
}

// ===== ATMOSPHERIC SCATTERING =====

float3 CalculateAtmosphericScattering(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    float distance = length(lightingData.positionWS - _WorldSpaceCameraPos);
    float scattering = 1.0 - exp(-distance * 0.001);
    
    // Simple atmospheric color (blue sky scattering)
    float3 atmosphereColor = float3(0.5, 0.7, 1.0) * 0.1;
    
    return scattering * atmosphereColor;
}

// ===== VOLUMETRIC FOG =====

float3 ApplyVolumetricFog(float3 color, float3 worldPos, float3 viewDir)
{
    float distance = length(worldPos - _WorldSpaceCameraPos);
    
    // Height-based fog
    float fogHeight = worldPos.y;
    float fogDensity = exp(-fogHeight * 0.1) * 0.01;
    
    // Distance fog
    float fogAmount = 1.0 - exp(-distance * fogDensity);
    
    // Fog color with atmospheric perspective
    float3 fogColor = float3(0.6, 0.7, 0.8);
    
    // Add some scattering towards the sun
    Light mainLight = GetMainLight();
    float sunScattering = pow(saturate(dot(-viewDir, mainLight.direction)), 4.0);
    fogColor += float3(1.0, 0.8, 0.6) * sunScattering * 0.5;
    
    return lerp(color, fogColor, fogAmount);
}

// ===== ADVANCED CAUSTICS =====

float3 CalculateAdvancedCaustics(ToonSurfaceData surfaceData, ToonLightingData lightingData, float2 uv)
{
    float time = _Time.y * 0.5;
    
    // Multiple layers of caustics
    float2 causticsUV1 = uv * 3.0 + time * 0.1;
    float2 causticsUV2 = uv * 5.0 + time * 0.15;
    float2 causticsUV3 = uv * 7.0 + time * 0.08;
    
    float caustic1 = PerlinNoise(causticsUV1);
    float caustic2 = PerlinNoise(causticsUV2);
    float caustic3 = PerlinNoise(causticsUV3);
    
    float caustics = (caustic1 + caustic2 * 0.5 + caustic3 * 0.25) / 1.75;
    caustics = pow(saturate(caustics), 2.0); // Sharpen the caustics
    
    // Apply only to surfaces facing up (water surface effect)
    float upFacing = saturate(dot(surfaceData.normalWS, float3(0, 1, 0)));
    caustics *= upFacing;
    
    // Caustic color (blue-white)
    float3 causticColor = float3(0.8, 0.9, 1.0) * 0.2;
    
    return caustics * causticColor;
}

// ===== LIGHT LAYERS SUPPORT =====

float3 CalculateLightLayers(ToonSurfaceData surfaceData, ToonLightingData lightingData, uint lightLayers)
{
    float3 layeredLighting = 0;
    
    #ifdef _LIGHT_LAYERS
        // This would require proper light layer setup in URP
        // For now, this is a placeholder for future light layer support
        uint surfaceLayers = asuint(surfaceData.metallic * 255); // Use metallic as layer mask for demo
        
        if ((lightLayers & surfaceLayers) != 0)
        {
            // Apply special lighting for matching layers
            layeredLighting = surfaceData.albedo * 0.1;
        }
    #endif
    
    return layeredLighting;
}

// ===== TEMPORAL EFFECTS =====

float3 ApplyTemporalEffects(float3 color, ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    #ifdef _ANIMATED_PROPERTIES
    if (_AnimationSpeed > 0.001)
    {
        float time = _Time.y * _AnimationSpeed;
        
        // Temporal color shifting
        float colorShift = sin(time + dot(lightingData.positionWS, float3(0.1, 0.1, 0.1))) * 0.05;
        color.rgb += colorShift;
        
        // Temporal rim lighting pulse
        #ifdef _RIM_LIGHTING
            float rimPulse = sin(time * 2.0) * 0.1 + 1.0;
            // This would need to be applied in the rim calculation above
        #endif
        
        // Temporal emission flicker
        #ifdef _EMISSION
            float emissionFlicker = sin(time * 5.0 + SimplexNoise(lightingData.positionWS.xz * 0.1)) * 0.1 + 1.0;
            // This would need to be applied in the emission calculation
        #endif
    }
    #endif
    
    return color;
}

// ===== MOBILE OPTIMIZED LIGHTING =====

float3 CalculateMobileOptimizedLighting(ToonSurfaceData surfaceData, ToonLightingData lightingData, float2 uv)
{
    // Simplified lighting path for mobile devices
    Light mainLight = GetMainLight(lightingData.shadowCoord);
    float NdotL = saturate(dot(lightingData.normalWS, mainLight.direction));
    
    // Simple toon ramp without advanced features
    float toonRamp = smoothstep(_ShadowThreshold - _ShadowSmoothness, _ShadowThreshold + _ShadowSmoothness, NdotL);
    toonRamp *= mainLight.shadowAttenuation;
    
    // Basic lighting calculation
    float3 shadowedColor = lerp(_ShadowColor.rgb * surfaceData.albedo, surfaceData.albedo, _ShadowIntensity);
    float3 finalColor = lerp(shadowedColor, surfaceData.albedo, toonRamp) * mainLight.color;
    
    // Add basic rim lighting if enabled
    #ifdef _RIM_LIGHTING
        float rim = 1.0 - saturate(dot(lightingData.viewDirectionWS, surfaceData.normalWS));
        rim = smoothstep(_RimThreshold, 1.0, rim);
        rim = pow(rim, _RimPower) * _RimIntensity;
        finalColor += rim * _RimColor.rgb * mainLight.color;
    #endif
    
    // Add emission
    finalColor += surfaceData.emission;
    
    // Add indirect lighting (simplified)
    finalColor += lightingData.bakedGI * surfaceData.albedo * _IndirectLightingBoost;
    
    return finalColor;
}

// ===== QUALITY-AWARE LIGHTING DISPATCHER =====

float3 CalculateQualityAwareLighting(ToonSurfaceData surfaceData, ToonLightingData lightingData, float2 uv)
{
    #if GORGONIZE_MOBILE_OPTIMIZATIONS
        return CalculateMobileOptimizedLighting(surfaceData, lightingData, uv);
    #else
        if (_QualityLevel >= QUALITY_ULTRA)
        {
            return CalculateGorgonizeToonLightingEnhanced(surfaceData, lightingData, uv);
        }
        else
        {
            return CalculateGorgonizeToonLighting(surfaceData, lightingData, uv);
        }
    #endif
}

#endif