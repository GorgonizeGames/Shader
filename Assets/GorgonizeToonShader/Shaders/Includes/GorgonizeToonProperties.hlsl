#ifndef GORGONIZE_TOON_PROPERTIES_INCLUDED
#define GORGONIZE_TOON_PROPERTIES_INCLUDED

// ===== TEXTURE DECLARATIONS =====

// Base textures
TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);
TEXTURE2D(_LightRampTex);
SAMPLER(sampler_LightRampTex);

// Normal and detail textures
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

// Emission textures
#ifdef _EMISSION
TEXTURE2D(_EmissionMap);
SAMPLER(sampler_EmissionMap);
#endif

// Matcap textures
#ifdef _MATCAP
TEXTURE2D(_MatcapTex);
SAMPLER(sampler_MatcapTex);
#endif

// Hatching textures
#ifdef _HATCHING
TEXTURE2D(_HatchingTex);
SAMPLER(sampler_HatchingTex);
TEXTURE2D(_CrossHatchingTex);
SAMPLER(sampler_CrossHatchingTex);
TEXTURE2D(_HatchingTex2);
SAMPLER(sampler_HatchingTex2);
#endif

// Subsurface textures
#ifdef _SUBSURFACE
TEXTURE2D(_SubsurfaceThickness);
SAMPLER(sampler_SubsurfaceThickness);
#endif

// Dissolve textures
#ifdef _DISSOLVE
TEXTURE2D(_DissolveNoise);
SAMPLER(sampler_DissolveNoise);
#endif

// ===== MATERIAL PROPERTY BUFFER =====
CBUFFER_START(UnityPerMaterial)
    // Base Properties
    float4 _BaseColor;
    float4 _BaseMap_ST;
    float _Saturation;
    float _Brightness;
    float _Metallic;
    float _Smoothness;
    float _OcclusionStrength;
    
    // Lighting Properties
    float _ShadowThreshold;
    float _ShadowSmoothness;
    float4 _ShadowColor;
    float _ShadowIntensity;
    float4 _LightRampTex_ST;
    float _IndirectLightingBoost;
    float _LightWrapping;
    float _VolumetricIntensity;
    float _CookieInfluence;
    
    // Rim Lighting
    float4 _RimColor;
    float _RimPower;
    float _RimIntensity;
    float _RimThreshold;
    float4 _RimColorSecondary;
    float _RimPowerSecondary;
    
    // Specular Properties
    float4 _SpecularColor;
    float _SpecularSize;
    float _SpecularSmoothness;
    float _SpecularIntensity;
    float _Anisotropy;
    float _SpecularSteps;
    
    // Hatching Properties
    float4 _HatchingTex_ST;
    float4 _CrossHatchingTex_ST;
    float4 _HatchingTex2_ST;
    float _HatchingDensity;
    float _HatchingIntensity;
    float _HatchingThreshold;
    float _CrossHatchingThreshold;
    float _SecondaryHatchingThreshold;
    float _HatchingRotation;
    float _HatchingAnimSpeed;
    float _ScreenHatchScale;
    float _ScreenHatchBias;
    
    // Matcap Properties
    float _MatcapIntensity;
    float _MatcapBlendMode;
    float _MatcapRotation;
    
    // Normal Mapping
    float _BumpScale;
    
    // Detail Properties
    float4 _DetailMap_ST;
    float4 _DetailNormalMap_ST;
    float _DetailScale;
    float _DetailNormalScale;
    
    // Emission Properties
    float4 _EmissionColor;
    float _EmissionIntensity;
    float4 _EmissionScrollSpeed;
    float _EmissionPulseSpeed;
    float _EmissionPulseIntensity;
    float _EmissionTemperature;
    
    // Fresnel Properties
    float4 _FresnelColor;
    float _FresnelPower;
    float _FresnelIntensity;
    float _IridescenceIntensity;
    
    // Subsurface Properties
    float4 _SubsurfaceColor;
    float _SubsurfacePower;
    float _SubsurfaceIntensity;
    float _SubsurfaceDistortion;
    
    // Outline Properties
    float4 _OutlineColor;
    float _OutlineWidth;
    float _OutlineMode;
    float _OutlineFadeDistance;
    float _OutlineDepthBiasValue;
    
    // Color Grading
    float _Hue;
    float _Contrast;
    float _Gamma;
    float _ColorTemperature;
    float _ColorTint;
    float _Vibrance;
    
    // Stylization
    float _PosterizeLevels;
    float _CelShadingSteps;
    float _DitheringIntensity;
    
    // Advanced Effects
    float4 _ForceFieldColor;
    float _ForceFieldIntensity;
    float _ForceFieldFrequency;
    float _HologramIntensity;
    float _HologramFlicker;
    float _HologramScanlines;
    float _DissolveAmount;
    float _DissolveEdgeWidth;
    float4 _DissolveEdgeColor;
    
    // Animation Properties
    float _AnimationSpeed;
    float _VertexAnimationIntensity;
    float _VertexAnimationFrequency;
    
    // Performance and Quality
    float _QualityLevel;
    float _LODFadeDistance;
    
    // Debug Properties
    float _DebugView;
    float4 _WireframeColor;
    float _WireframeThickness;
    
    // Advanced Rendering
    float _Cutoff;
    float _Cull;
    float _ZWrite;
    float _ZTest;
    float _SrcBlend;
    float _DstBlend;
CBUFFER_END

// ===== SURFACE DATA STRUCTURES =====
struct ToonSurfaceData
{
    float3 albedo;
    float3 normalWS;
    float3 emission;
    float metallic;
    float smoothness;
    float occlusion;
    float alpha;
    float3 bakedGI;
    float subsurfaceThickness;
    float3 tangentWS;
    float3 bitangentWS;
};

struct ToonLightingData
{
    float3 normalWS;
    float3 viewDirectionWS;
    float4 shadowCoord;
    float3 bakedGI;
    float3 positionWS;
    float4 screenPos;
    float3 reflectionProbeData;
    float fogFactor;
};

// ===== SHADER PROPERTY IDS =====
// These match the C# ToonShaderProperties class
static const int PROPERTY_BASE_COLOR = 0;
static const int PROPERTY_BASE_MAP = 1;
static const int PROPERTY_SATURATION = 2;
static const int PROPERTY_BRIGHTNESS = 3;
static const int PROPERTY_SHADOW_THRESHOLD = 4;
static const int PROPERTY_SHADOW_SMOOTHNESS = 5;
static const int PROPERTY_SHADOW_COLOR = 6;
static const int PROPERTY_SHADOW_INTENSITY = 7;
static const int PROPERTY_RIM_COLOR = 8;
static const int PROPERTY_RIM_POWER = 9;
static const int PROPERTY_RIM_INTENSITY = 10;
static const int PROPERTY_RIM_THRESHOLD = 11;
static const int PROPERTY_SPECULAR_COLOR = 12;
static const int PROPERTY_SPECULAR_SIZE = 13;
static const int PROPERTY_SPECULAR_SMOOTHNESS = 14;
static const int PROPERTY_SPECULAR_INTENSITY = 15;

// ===== KEYWORD DEFINITIONS =====
// Local shader features
#define KEYWORD_RIM_LIGHTING "_RIM_LIGHTING"
#define KEYWORD_RIM_LIGHTING_DUAL_LAYER "_RIM_LIGHTING_DUAL_LAYER"
#define KEYWORD_SPECULAR "_SPECULAR"
#define KEYWORD_SPECULAR_ANISOTROPIC "_SPECULAR_ANISOTROPIC"
#define KEYWORD_SPECULAR_STEPPED "_SPECULAR_STEPPED"
#define KEYWORD_HATCHING "_HATCHING"
#define KEYWORD_HATCHING_ANIMATED "_HATCHING_ANIMATED"
#define KEYWORD_SCREEN_SPACE_HATCHING "_SCREEN_SPACE_HATCHING"
#define KEYWORD_MATCAP "_MATCAP"
#define KEYWORD_MATCAP_PERSPECTIVE_CORRECTION "_MATCAP_PERSPECTIVE_CORRECTION"
#define KEYWORD_NORMALMAP "_NORMALMAP"
#define KEYWORD_DETAIL "_DETAIL"
#define KEYWORD_EMISSION "_EMISSION"
#define KEYWORD_EMISSION_PULSING "_EMISSION_PULSING"
#define KEYWORD_EMISSION_TEMPERATURE_BASED "_EMISSION_TEMPERATURE_BASED"
#define KEYWORD_FRESNEL "_FRESNEL"
#define KEYWORD_FRESNEL_IRIDESCENCE "_FRESNEL_IRIDESCENCE"
#define KEYWORD_SUBSURFACE "_SUBSURFACE"
#define KEYWORD_OUTLINE "_OUTLINE"
#define KEYWORD_OUTLINE_DISTANCE_FADE "_OUTLINE_DISTANCE_FADE"
#define KEYWORD_OUTLINE_DEPTH_BIAS "_OUTLINE_DEPTH_BIAS"
#define KEYWORD_USE_RAMP_TEXTURE "_USE_RAMP_TEXTURE"
#define KEYWORD_POSTERIZE "_POSTERIZE"
#define KEYWORD_CEL_SHADING "_CEL_SHADING"
#define KEYWORD_DITHERING "_DITHERING"
#define KEYWORD_FORCE_FIELD "_FORCE_FIELD"
#define KEYWORD_HOLOGRAM "_HOLOGRAM"
#define KEYWORD_DISSOLVE "_DISSOLVE"
#define KEYWORD_ANIMATED_PROPERTIES "_ANIMATED_PROPERTIES"
#define KEYWORD_VERTEX_ANIMATION "_VERTEX_ANIMATION"
#define KEYWORD_LOD_FADE "_LOD_FADE"
#define KEYWORD_INSTANCING_SUPPORT "_INSTANCING_SUPPORT"
#define KEYWORD_DEBUG_MODE "_DEBUG_MODE"
#define KEYWORD_WIREFRAME "_WIREFRAME"
#define KEYWORD_ALPHA_PREMULTIPLY "_ALPHA_PREMULTIPLY"
#define KEYWORD_ENABLE_VOLUMETRIC_LIGHTING "_ENABLE_VOLUMETRIC_LIGHTING"
#define KEYWORD_ENABLE_LIGHT_COOKIES "_ENABLE_LIGHT_COOKIES"

// ===== QUALITY LEVEL DEFINITIONS =====
#define QUALITY_LOW 0
#define QUALITY_MEDIUM 1
#define QUALITY_HIGH 2
#define QUALITY_ULTRA 3

// ===== BLEND MODE DEFINITIONS =====
#define BLEND_MODE_ADD 0
#define BLEND_MODE_MULTIPLY 1
#define BLEND_MODE_SCREEN 2
#define BLEND_MODE_OVERLAY 3

// ===== OUTLINE MODE DEFINITIONS =====
#define OUTLINE_MODE_NORMAL 0
#define OUTLINE_MODE_POSITION 1
#define OUTLINE_MODE_CLIP 2

// ===== DEBUG VIEW DEFINITIONS =====
#define DEBUG_VIEW_NONE 0
#define DEBUG_VIEW_NORMALS 1
#define DEBUG_VIEW_LIGHTING 2
#define DEBUG_VIEW_SHADOWS 3
#define DEBUG_VIEW_HATCHING 4

// ===== UTILITY MACROS =====
#define GORGONIZE_TOON_SAMPLE_2D(tex, samp, uv) SAMPLE_TEXTURE2D(tex, samp, uv)
#define GORGONIZE_TOON_SAMPLE_2D_LOD(tex, samp, uv, lod) SAMPLE_TEXTURE2D_LOD(tex, samp, uv, lod)
#define GORGONIZE_TOON_SAMPLE_2D_BIAS(tex, samp, uv, bias) SAMPLE_TEXTURE2D_BIAS(tex, samp, uv, bias)

// ===== PERFORMANCE OPTIMIZATION MACROS =====
#if defined(SHADER_API_MOBILE) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)
    #define GORGONIZE_MOBILE_OPTIMIZATIONS 1
    #define GORGONIZE_MAX_HATCHING_LAYERS 2
    #define GORGONIZE_MAX_ADDITIONAL_LIGHTS 4
#else
    #define GORGONIZE_MOBILE_OPTIMIZATIONS 0
    #define GORGONIZE_MAX_HATCHING_LAYERS 3
    #define GORGONIZE_MAX_ADDITIONAL_LIGHTS 8
#endif

// ===== FEATURE AVAILABILITY CHECKS =====
#define GORGONIZE_HAS_ADVANCED_FEATURES (!GORGONIZE_MOBILE_OPTIMIZATIONS || _QualityLevel >= QUALITY_HIGH)
#define GORGONIZE_HAS_EXPENSIVE_EFFECTS (!GORGONIZE_MOBILE_OPTIMIZATIONS || _QualityLevel >= QUALITY_ULTRA)

// ===== VERSION INFORMATION =====
#define GORGONIZE_TOON_SHADER_VERSION_MAJOR 4
#define GORGONIZE_TOON_SHADER_VERSION_MINOR 0
#define GORGONIZE_TOON_SHADER_VERSION_PATCH 0

#endif