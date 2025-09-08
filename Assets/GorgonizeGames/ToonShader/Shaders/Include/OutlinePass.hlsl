#ifndef OUTLINE_PASS_INCLUDED
#define OUTLINE_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "ToonHelpers.hlsl"

// Outline properties
CBUFFER_START(UnityPerMaterial)
    half _OutlineWidth;
    half4 _OutlineColor;
    half _OutlineDistanceFade;
    half _CornerRoundness;
CBUFFER_END

struct OutlineAttributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float2 texcoord : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct OutlineVaryings
{
    float2 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1;
    float4 positionCS : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

// Calculate outline vertex position
float4 CalculateOutlinePosition(float4 positionOS, float3 normalOS, float4 tangentOS)
{
    #ifdef _ENABLE_OUTLINE
        
        float3 normal = normalOS;
        
        #ifdef _CORNER_ROUNDING
            // Smooth normals for better corner handling
            // Mix between vertex normal and face normal based on corner roundness
            float3 faceNormal = normalize(cross(ddy(positionOS.xyz), ddx(positionOS.xyz)));
            normal = lerp(normal, faceNormal, _CornerRoundness);
        #endif
        
        // Transform normal to view space for consistent outline width
        float3 normalVS = TransformWorldToViewDir(TransformObjectToWorldNormal(normal));
        
        // Calculate outline width with distance fade
        float4 positionCS = TransformObjectToHClip(positionOS.xyz);
        float distance = positionCS.w;
        float distanceFade = lerp(1.0, 0.0, saturate((distance - 5.0) / 20.0));
        distanceFade = lerp(distanceFade, 1.0, 1.0 - _OutlineDistanceFade);
        
        float outlineWidth = _OutlineWidth * distanceFade;
        
        #ifdef _MOBILE_MODE
            // Simplified outline calculation for mobile
            outlineWidth *= 0.5; // Reduce outline width on mobile
        #endif
        
        // Apply outline offset
        float2 normalScreenSpace = normalize(normalVS.xy);
        positionCS.xy += normalScreenSpace * outlineWidth * positionCS.w * 2.0;
        
        return positionCS;
        
    #else
        // If outline is disabled, return original position
        return TransformObjectToHClip(positionOS.xyz);
    #endif
}

// Outline vertex shader
OutlineVaryings OutlineVertex(OutlineAttributes input)
{
    OutlineVaryings output = (OutlineVaryings)0;
    
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
    
    output.uv = input.texcoord;
    
    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    output.positionWS = vertexInput.positionWS;
    
    // Calculate outline position
    output.positionCS = CalculateOutlinePosition(input.positionOS, input.normalOS, input.tangentOS);
    
    return output;
}

// Enhanced outline width calculation with vertex color support
float CalculateEnhancedOutlineWidth(float baseWidth, float3 positionWS, float vertexColorR)
{
    // Distance-based width scaling
    float distanceToCamera = length(_WorldSpaceCameraPos - positionWS);
    float distanceScale = saturate(1.0 - (distanceToCamera - 5.0) / 15.0);
    distanceScale = lerp(distanceScale, 1.0, 1.0 - _OutlineDistanceFade);
    
    // Vertex color can be used to control outline width per vertex
    float vertexScale = lerp(0.5, 1.5, vertexColorR); // Assumes red channel controls width
    
    return baseWidth * distanceScale * vertexScale;
}

// Outline fragment shader
half4 OutlineFragment(OutlineVaryings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    
    #ifdef _ENABLE_OUTLINE
        half4 outlineColor = _OutlineColor;
        
        // Optional: Sample a texture for outline variation
        // half4 outlineTexture = SAMPLE_TEXTURE2D(_OutlineTexture, sampler_OutlineTexture, input.uv);
        // outlineColor *= outlineTexture;
        
        // Optional: Apply depth-based fade for better integration
        float depth = input.positionCS.z / input.positionCS.w;
        half depthFade = saturate(depth * 10.0); // Fade outline at distance
        outlineColor.a *= depthFade;
        
        return outlineColor;
    #else
        // If outline is disabled, discard fragment
        discard;
        return half4(0, 0, 0, 0);
    #endif
}

// Alternative outline vertex calculation for better quality
float4 CalculateAdvancedOutline(float4 positionOS, float3 normalOS, float outlineWidth)
{
    // Transform to world space
    float3 positionWS = TransformObjectToWorld(positionOS.xyz);
    float3 normalWS = TransformObjectToWorldNormal(normalOS);
    
    // Camera-facing outline adjustment
    float3 viewDir = normalize(_WorldSpaceCameraPos - positionWS);
    float3 adjustedNormal = normalWS;
    
    // Reduce outline on surfaces facing away from camera
    float facingRatio = dot(normalWS, viewDir);
    adjustedNormal = lerp(adjustedNormal, viewDir, saturate(1.0 - facingRatio));
    
    // Apply outline offset in world space
    positionWS += adjustedNormal * outlineWidth * 0.01;
    
    // Transform back to clip space
    return TransformWorldToHClip(positionWS);
}

// Outline width modulation based on surface curvature
float CalculateCurvatureBasedWidth(float3 normalOS, float4 tangentOS, float baseWidth)
{
    // Calculate curvature approximation
    float3 bitangent = cross(normalOS, tangentOS.xyz) * tangentOS.w;
    
    // Simple curvature estimation (this is a simplified approach)
    float curvature = length(fwidth(normalOS));
    
    // Modulate outline width based on curvature
    float curvatureScale = lerp(0.5, 1.5, saturate(curvature * 10.0));
    
    return baseWidth * curvatureScale;
}

#endif