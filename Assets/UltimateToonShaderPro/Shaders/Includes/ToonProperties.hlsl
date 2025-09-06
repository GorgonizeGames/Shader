#ifndef TOON_PROPERTIES_INCLUDED
#define TOON_PROPERTIES_INCLUDED

// Texture declarations
TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);
TEXTURE2D(_LightRampTex);
SAMPLER(sampler_LightRampTex);

#ifdef _NORMALMAP
TEXTURE2D(_BumpMap);
SAMPLER(sampler_BumpMap);
#endif

#ifdef _DETAIL
TEXTURE2D(_DetailMap);
SAMPLER(sampler_DetailMap);
TEXTURE2D(_DetailNormalMap);
SAMPLER(sampler_DetailNormalMap);
#endif

#ifdef _EMISSION
TEXTURE2D(_EmissionMap);
SAMPLER(sampler_EmissionMap);
#endif

#ifdef _MATCAP
TEXTURE2D(_MatcapTex);
SAMPLER(sampler_MatcapTex);
#endif

#ifdef _HATCHING
TEXTURE2D(_HatchingTex);
SAMPLER(sampler_HatchingTex);
TEXTURE2D(_CrossHatchingTex);
SAMPLER(sampler_CrossHatchingTex);
#endif

// Material property buffer
CBUFFER_START(UnityPerMaterial)
    // Base Properties
    float4 _BaseColor;
    float4 _BaseMap_ST;
    float _Saturation;
    float _Brightness;
    
    // Lighting Properties
    float _ShadowThreshold;
    float _ShadowSmoothness;
    float4 _ShadowColor;
    float _ShadowIntensity;
    float4 _LightRampTex_ST;
    float _IndirectLightingBoost;
    float _AmbientOcclusion;
    float _LightWrapping;
    
    // Rim Lighting
    float4 _RimColor;
    float _RimPower;
    float _RimIntensity;
    float _RimThreshold;
    
    // Specular
    float4 _SpecularColor;
    float _SpecularSize;
    float _SpecularSmoothness;
    float _SpecularIntensity;
    
    // Hatching Properties
    float4 _HatchingTex_ST;
    float4 _CrossHatchingTex_ST;
    float _HatchingDensity;
    float _HatchingIntensity;
    float _HatchingThreshold;
    float _CrossHatchingThreshold;
    float _HatchingRotation;
    float _ScreenHatchScale;
    float _ScreenHatchBias;
    
    // Matcap
    float _MatcapIntensity;
    float _MatcapBlendMode;
    
    // Normal Mapping
    float _BumpScale;
    
    // Detail
    float4 _DetailMap_ST;
    float _DetailScale;
    float _DetailNormalScale;
    
    // Emission
    float4 _EmissionColor;
    float _EmissionIntensity;
    float4 _EmissionScrollSpeed;
    
    // Fresnel
    float4 _FresnelColor;
    float _FresnelPower;
    float _FresnelIntensity;
    
    // Subsurface
    float4 _SubsurfaceColor;
    float _SubsurfacePower;
    float _SubsurfaceIntensity;
    
    // Outline
    float4 _OutlineColor;
    float _OutlineWidth;
    
    // Color Grading
    float _Hue;
    float _Contrast;
    float _Gamma;
    
    // Stylization
    float _PosterizeLevels;
    float _CelShadingSteps;
    
    // Advanced
    float _Cutoff;
CBUFFER_END

#endif