#ifndef TOON_SPECULAR_INCLUDED
#define TOON_SPECULAR_INCLUDED

#include "ToonHelpers.hlsl"

// Declare the external variables that will be defined in the main pass
// These need to be properly declared for Unity 6 compatibility

// Function declarations for external textures/samplers
TEXTURE2D(_MatCapMap);
SAMPLER(sampler_MatCapMap);

// Calculate stylized specular highlights
half3 CalculateToonSpecular(ToonSurfaceData surfaceData, Light light)
{
    #ifdef _ENABLE_SPECULAR
        // Access properties from CBUFFER defined in main pass
        // We assume these are available from UnityPerMaterial CBUFFER
        
        half3 lightDir = SafeNormalize(light.direction);
        half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
        half3 halfVector = SafeNormalize(lightDir + viewDir);
        
        half NdotH = saturate(dot(surfaceData.normalWS, halfVector));
        half LdotH = saturate(dot(lightDir, halfVector));
        
        #ifdef _ANISOTROPIC_SPECULAR
            // Anisotropic specular for hair and fabric-like materials
            half3 tangent = half3(1, 0, 0); // Default tangent, should be passed from vertex
            half3 binormal = SafeNormalize(cross(surfaceData.normalWS, tangent));
            half HdotB = dot(halfVector, binormal);
            
            // Anisotropic highlight calculation
            half anisotropicHighlight = sqrt(max(0.001, 1.0 - HdotB * HdotB));
            anisotropicHighlight = pow(anisotropicHighlight, max(0.001, 1.0 / _SpecularSize));
            
            // Apply anisotropy direction
            anisotropicHighlight *= (1.0 + _Anisotropy * HdotB);
            
            half specularTerm = anisotropicHighlight;
        #else
            // Standard specular calculation
            half specularTerm = pow(NdotH, max(0.001, 1.0 / _SpecularSize));
        #endif
        
        // Apply toon-style quantization to specular
        half toonSpecular = smoothstep(1.0 - _SpecularSmoothness, 1.0, specularTerm);
        
        half3 specularColor = _SpecularColor.rgb * toonSpecular;
        specularColor *= light.color * light.distanceAttenuation;
        
        #ifdef _USE_MATCAP
            // MatCap-based specular (fake reflections)
            half3 matCapSpecular = CalculateMatCapSpecular(surfaceData);
            specularColor = lerp(specularColor, matCapSpecular, 0.5);
        #endif
        
        return specularColor;
    #else
        return half3(0, 0, 0);
    #endif
}

// MatCap specular calculation
half3 CalculateMatCapSpecular(ToonSurfaceData surfaceData)
{
    #ifdef _USE_MATCAP
        // Transform normal to view space for MatCap sampling
        float4x4 viewMatrix = UNITY_MATRIX_V;
        half3 normalVS = mul((float3x3)viewMatrix, surfaceData.normalWS);
        normalVS = SafeNormalize(normalVS);
        
        // Convert to UV coordinates for MatCap sampling
        half2 matCapUV = normalVS.xy * 0.5 + 0.5;
        
        // Sample MatCap texture
        half3 matCapColor = SAMPLE_TEXTURE2D(_MatCapMap, sampler_MatCapMap, matCapUV).rgb;
        
        return matCapColor * _SpecularColor.rgb;
    #else
        return half3(0, 0, 0);
    #endif
}

// Stylized Blinn-Phong specular with toon characteristics
half3 CalculateStylizedBlinnPhong(ToonSurfaceData surfaceData, Light light, half roughness)
{
    half3 lightDir = SafeNormalize(light.direction);
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    half3 halfVector = SafeNormalize(lightDir + viewDir);
    
    half NdotH = saturate(dot(surfaceData.normalWS, halfVector));
    half NdotL = saturate(dot(surfaceData.normalWS, lightDir));
    
    // Convert roughness to specular power
    half specularPower = max(0.001, 2.0 / (roughness * roughness) - 2.0);
    half specularTerm = pow(NdotH, specularPower) * NdotL;
    
    // Apply toon-style quantization
    specularTerm = floor(specularTerm * 4.0) / 4.0; // 4-step quantization
    
    return _SpecularColor.rgb * specularTerm * light.color;
}

// Fake environment reflections for toon shading
half3 CalculateFakeReflections(ToonSurfaceData surfaceData, half reflectionStrength)
{
    #ifdef _USE_MATCAP
        // Use MatCap as fake environment reflection
        half3 reflectionColor = CalculateMatCapSpecular(surfaceData);
        
        // Apply Fresnel-like falloff
        half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
        half fresnel = 1.0 - saturate(dot(surfaceData.normalWS, viewDir));
        fresnel = pow(fresnel, 3.0);
        
        return reflectionColor * fresnel * reflectionStrength;
    #else
        return half3(0, 0, 0);
    #endif
}

// Multi-layer specular for complex materials
half3 CalculateLayeredSpecular(ToonSurfaceData surfaceData, Light light, half layer1Size, half layer2Size)
{
    half3 lightDir = SafeNormalize(light.direction);
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    half3 halfVector = SafeNormalize(lightDir + viewDir);
    half NdotH = saturate(dot(surfaceData.normalWS, halfVector));
    
    // First specular layer (sharp highlight)
    half specular1 = pow(NdotH, max(0.001, 1.0 / layer1Size));
    specular1 = step(0.8, specular1); // Sharp cutoff
    
    // Second specular layer (broader highlight)
    half specular2 = pow(NdotH, max(0.001, 1.0 / layer2Size));
    specular2 = smoothstep(0.3, 0.7, specular2);
    
    half3 combinedSpecular = _SpecularColor.rgb * (specular1 + specular2 * 0.3);
    return combinedSpecular * light.color * light.distanceAttenuation;
}

// Cloth-style specular with fabric characteristics
half3 CalculateClothSpecular(ToonSurfaceData surfaceData, Light light, half fabricRoughness)
{
    half3 lightDir = SafeNormalize(light.direction);
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    
    // Cloth uses inverted fresnel for velvet-like appearance
    half NdotV = saturate(dot(surfaceData.normalWS, viewDir));
    half NdotL = saturate(dot(surfaceData.normalWS, lightDir));
    
    // Velvet-like specular calculation
    half velvetTerm = pow(max(0.001, 1.0 - NdotV), fabricRoughness) * NdotL;
    
    // Apply toon quantization
    velvetTerm = smoothstep(0.3, 0.7, velvetTerm);
    
    return _SpecularColor.rgb * velvetTerm * light.color;
}

// Skin-style specular with subsurface characteristics
half3 CalculateSkinSpecular(ToonSurfaceData surfaceData, Light light, half skinSmoothness)
{
    half3 lightDir = SafeNormalize(light.direction);
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    half3 halfVector = SafeNormalize(lightDir + viewDir);
    
    half NdotH = saturate(dot(surfaceData.normalWS, halfVector));
    half NdotL = saturate(dot(surfaceData.normalWS, lightDir));
    
    // Soft specular for skin
    half skinSpecular = pow(NdotH, max(0.001, 1.0 / _SpecularSize)) * NdotL;
    
    // Apply skin-appropriate smoothing
    skinSpecular = smoothstep(0.1, 0.9, skinSpecular * skinSmoothness);
    
    // Tint specular with warm skin tones
    half3 skinSpecularColor = lerp(_SpecularColor.rgb, half3(1.0, 0.8, 0.6), 0.3);
    
    return skinSpecularColor * skinSpecular * light.color;
}

// Metal-style specular with sharp reflections
half3 CalculateMetallicSpecular(ToonSurfaceData surfaceData, Light light, half metallic)
{
    half3 lightDir = SafeNormalize(light.direction);
    half3 viewDir = SafeNormalize(surfaceData.viewDirectionWS);
    half3 halfVector = SafeNormalize(lightDir + viewDir);
    
    half NdotH = saturate(dot(surfaceData.normalWS, halfVector));
    half NdotV = saturate(dot(surfaceData.normalWS, viewDir));
    
    // Sharp metallic highlight
    half metallicSpecular = pow(NdotH, max(0.001, 1.0 / (_SpecularSize * 0.1))); // Much sharper
    metallicSpecular = step(0.95, metallicSpecular); // Very sharp cutoff
    
    // Fresnel for metallic surfaces
    half fresnel = pow(max(0.001, 1.0 - NdotV), 5.0);
    
    // Tint specular with surface color for metals
    half3 metallicColor = lerp(_SpecularColor.rgb, surfaceData.albedo, metallic);
    
    return metallicColor * metallicSpecular * (1.0 + fresnel) * light.color;
}

#endif