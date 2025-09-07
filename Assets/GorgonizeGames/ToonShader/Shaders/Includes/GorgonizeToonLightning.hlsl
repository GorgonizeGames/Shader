#ifndef GORGONIZE_TOON_LIGHTING_INCLUDED
#define GORGONIZE_TOON_LIGHTING_INCLUDED

// Data structures for toon shading
struct ToonSurfaceData
{
    half3 albedo;
    half alpha;
    half3 normalWS;
    half3 tangentWS;
    half3 bitangentWS;
    half3 specular;
    half smoothness;
    half specularSize;
};

struct ToonInputData
{
    float3 positionWS;
    half3 normalWS;
    half3 viewDirectionWS;
    float4 shadowCoord;
    half fogCoord;
    half3 vertexLighting;
    half3 bakedGI;
};

// Calculate toon ramp based on NdotL
half CalculateToonRamp(half NdotL)
{
    #if USE_RAMP_TEXTURE
        // Use 1D ramp texture for lighting
        half rampUV = saturate(NdotL * 0.5 + 0.5);
        return SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, half2(rampUV, 0.5)).r;
    #else
        // Procedural stepped ramp
        half stepped = floor(NdotL * _RampSteps) / (_RampSteps - 1);
        return lerp(stepped, NdotL, _RampSmoothness);
    #endif
}

// Shadow blending modes
half3 BlendShadowColor(half3 baseColor, half3 shadowColor, half shadowAttenuation, float blendMode)
{
    half3 result = baseColor;
    
    if (blendMode < 0.5) // Multiply
    {
        result = lerp(baseColor * shadowColor, baseColor, shadowAttenuation);
    }
    else if (blendMode < 1.5) // Additive
    {
        result = baseColor + (shadowColor * (1 - shadowAttenuation));
    }
    else // Replace
    {
        result = lerp(shadowColor, baseColor, shadowAttenuation);
    }
    
    return result;
}

// Main directional light calculation
half3 CalculateMainLight(Light mainLight, ToonInputData inputData, ToonSurfaceData surfaceData)
{
    half NdotL = dot(surfaceData.normalWS, mainLight.direction);
    
    // Calculate toon ramp
    half ramp = CalculateToonRamp(NdotL);
    
    // Apply shadow attenuation
    half shadowAttenuation = mainLight.shadowAttenuation;
    
    #if STYLIZED_SHADOWS
        // Stylized shadow handling
        shadowAttenuation = smoothstep(0.5 - _ShadowSharpness * 0.5, 
                                     0.5 + _ShadowSharpness * 0.5, shadowAttenuation);
        half3 shadowTint = _ShadowTint.rgb;
        half3 lightColor = lerp(mainLight.color * shadowTint, mainLight.color, shadowAttenuation);
    #else
        half3 lightColor = mainLight.color * shadowAttenuation;
    #endif
    
    // Combine base color with lighting
    half3 diffuse = surfaceData.albedo * lightColor * ramp;
    
    // Blend shadow color
    diffuse = BlendShadowColor(diffuse, _ShadowColor.rgb, shadowAttenuation, _ShadowBlendMode);
    
    return diffuse;
}

// Specular calculation for toon shading
half3 CalculateSpecular(Light light, ToonInputData inputData, ToonSurfaceData surfaceData)
{
    #if !ENABLE_SPECULAR
        return half3(0, 0, 0);
    #endif
    
    half3 halfVector = SafeNormalize(light.direction + inputData.viewDirectionWS);
    half NdotH = saturate(dot(surfaceData.normalWS, halfVector));
    
    #if ENABLE_ANISOTROPIC && !MOBILE_OPTIMIZED
        // Anisotropic specular
        half3 tangent = surfaceData.tangentWS;
        half3 binormal = surfaceData.bitangentWS;
        
        half TdotH = dot(tangent, halfVector);
        half BdotH = dot(binormal, halfVector);
        
        half anisotropy = length(_AnisotropyDirection.xyz);
        half3 anisoDir = normalize(_AnisotropyDirection.xyz);
        
        half spec = sqrt(max(0, 1 - TdotH * TdotH - BdotH * BdotH)) / 
                   max(0.001, sqrt(TdotH * TdotH / (anisotropy * anisotropy) + BdotH * BdotH));
        spec = pow(spec, 1 / surfaceData.specularSize);
    #else
        // Standard specular with toon styling
        half spec = pow(NdotH, 1 / max(0.001, surfaceData.specularSize));
    #endif
    
    // Make specular toon-like with sharp cutoff
    spec = smoothstep(0.5, 0.51, spec);
    
    return surfaceData.specular * light.color * spec * light.shadowAttenuation;
}

// Rim lighting calculation
half3 CalculateRimLighting(ToonInputData inputData, ToonSurfaceData surfaceData)
{
    #if !ENABLE_RIM
        return half3(0, 0, 0);
    #endif
    
    half NdotV = 1 - saturate(dot(surfaceData.normalWS, inputData.viewDirectionWS));
    
    // Primary rim
    half rimPrimary = pow(NdotV, _RimPower) * _RimIntensity;
    half3 rimColor = _RimColor.rgb * rimPrimary;
    
    #if ENABLE_RIM_SECONDARY
        // Secondary rim
        half rimSecondary = pow(NdotV, _RimPowerSecondary) * _RimIntensitySecondary;
        rimColor += _RimColorSecondary.rgb * rimSecondary;
    #endif
    
    return rimColor;
}

// MatCap calculation
half3 CalculateMatCap(ToonInputData inputData, ToonSurfaceData surfaceData)
{
    #if !ENABLE_MATCAP
        return half3(0, 0, 0);
    #endif
    
    // Calculate MatCap UV from view-space normal
    half3 viewNormal = TransformWorldToViewNormal(surfaceData.normalWS, true);
    half2 matCapUV = viewNormal.xy * 0.5 + 0.5;
    
    half3 matCapColor = SAMPLE_TEXTURE2D(_MatCapTex, sampler_MatCapTex, matCapUV).rgb;
    return matCapColor * _MatCapIntensity;
}

// Additional lights calculation (Point/Spot lights)
half3 CalculateAdditionalLights(ToonInputData inputData, ToonSurfaceData surfaceData)
{
    #if !_ADDITIONAL_LIGHTS || MOBILE_OPTIMIZED
        return half3(0, 0, 0);
    #endif
    
    half3 additionalLighting = half3(0, 0, 0);
    
    #ifdef _ADDITIONAL_LIGHTS
        uint pixelLightCount = GetAdditionalLightsCount();
        
        LIGHT_LOOP_BEGIN(pixelLightCount)
        {
            Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
            
            // Apply toon shading to additional lights
            half NdotL = saturate(dot(surfaceData.normalWS, light.direction));
            half ramp = CalculateToonRamp(NdotL);
            
            half3 lightContribution = surfaceData.albedo * light.color * light.distanceAttenuation * ramp;
            
            // Add specular from additional lights
            lightContribution += CalculateSpecular(light, inputData, surfaceData);
            
            additionalLighting += lightContribution;
        }
        LIGHT_LOOP_END
    #endif
    
    return additionalLighting;
}

// Hatching effect calculation
half3 CalculateHatching(ToonInputData inputData, ToonSurfaceData surfaceData, half2 uv, half lightIntensity)
{
    #if !ENABLE_HATCHING
        return half3(0, 0, 0);
    #endif
    
    // Sample hatching texture with scaled UV
    half2 hatchingUV = TRANSFORM_TEX(uv, _HatchingTex) * _HatchingScale;
    half hatchingPattern = SAMPLE_TEXTURE2D(_HatchingTex, sampler_HatchingTex, hatchingUV).r;
    
    // Apply hatching based on light intensity (darker areas get more hatching)
    half hatchingMask = 1 - lightIntensity;
    hatchingMask = smoothstep(0.3, 0.7, hatchingMask);
    
    return hatchingPattern * hatchingMask * _HatchingIntensity * surfaceData.albedo;
}

// Main toon lighting function
half4 CalculateToonLighting(ToonInputData inputData, ToonSurfaceData surfaceData)
{
    // Get main light
    Light mainLight = GetMainLight(inputData.shadowCoord);
    
    // Calculate main lighting
    half3 color = CalculateMainLight(mainLight, inputData, surfaceData);
    
    // Add specular highlights
    color += CalculateSpecular(mainLight, inputData, surfaceData);
    
    // Add additional lights
    color += CalculateAdditionalLights(inputData, surfaceData);
    
    // Add rim lighting
    color += CalculateRimLighting(inputData, surfaceData);
    
    // Add MatCap
    color += CalculateMatCap(inputData, surfaceData);
    
    // Add global illumination (ambient lighting)
    half3 gi = inputData.bakedGI;
    color += surfaceData.albedo * gi;
    
    // Add vertex lighting (for additional lights in vertex shader)
    color += inputData.vertexLighting * surfaceData.albedo;
    
    // Calculate overall light intensity for hatching
    #if ENABLE_HATCHING
        half NdotL = saturate(dot(surfaceData.normalWS, mainLight.direction));
        half lightIntensity = NdotL * mainLight.shadowAttenuation;
        color -= CalculateHatching(inputData, surfaceData, inputData.positionWS.xz, lightIntensity);
    #endif
    
    return half4(color, surfaceData.alpha);
}

#endif