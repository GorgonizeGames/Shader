#ifndef TOON_INPUT_INCLUDED
#define TOON_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// Vertex input structure
struct ToonAttributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float2 uv : TEXCOORD0;
    float2 staticLightmapUV : TEXCOORD1;
    float2 dynamicLightmapUV : TEXCOORD2;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// Fragment input structure
struct ToonVaryings
{
    float4 positionHCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
    
    #ifdef _NORMALMAP
        float4 tangentWS : TEXCOORD3;
    #endif
    
    float3 viewDirWS : TEXCOORD4;
    
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        float4 shadowCoord : TEXCOORD5;
    #endif
    
    float fogCoord : TEXCOORD6;
    
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        float3 vertexLighting : TEXCOORD7;
    #endif
    
    #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
        float3 viewDirTS : TEXCOORD8;
    #endif
    
    #ifdef LIGHTMAP_ON
        float2 staticLightmapUV : TEXCOORD9;
    #endif
    
    float3 vertexSH : TEXCOORD10;
    
    float4 screenPos : TEXCOORD11;
    
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

// Outline pass structures
struct OutlineAttributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct OutlineVaryings
{
    float4 positionHCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    float fogCoord : TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

// Shadow caster structures
struct ShadowAttributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct ShadowVaryings
{
    float4 positionHCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

// Depth only structures
struct DepthOnlyAttributes
{
    float4 positionOS : POSITION;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct DepthOnlyVaryings
{
    float4 positionHCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

// Meta pass structures (for lightmap baking)
struct MetaAttributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 uv0 : TEXCOORD0;
    float2 uv1 : TEXCOORD1;
    float2 uv2 : TEXCOORD2;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct MetaVaryings
{
    float4 positionHCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    #ifdef EDITOR_VISUALIZATION
        float2 vizUV : TEXCOORD1;
        float4 lightCoord : TEXCOORD2;
    #endif
};

#endif