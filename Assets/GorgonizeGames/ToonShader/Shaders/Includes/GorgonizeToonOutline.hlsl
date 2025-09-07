#ifndef GORGONIZE_TOON_OUTLINE_INCLUDED
#define GORGONIZE_TOON_OUTLINE_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Assets/GorgonizeGames/ToonShader/Shaders/Includes/GorgonizeToonHelpers.hlsl"

// Outline Vertex Input
struct OutlineAttributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// Outline Vertex Output
struct OutlineVaryings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

// Outline Vertex Shader
OutlineVaryings OutlineVertex(OutlineAttributes input)
{
    OutlineVaryings output = (OutlineVaryings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    #if ENABLE_OUTLINE
        // Get vertex position inputs
        VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
        output.positionWS = vertexInput.positionWS;

        // Calculate distance-based outline width
        half dynamicWidth = CalculateOutlineWidth(vertexInput.positionWS, _OutlineWidth, _OutlineDistanceFade);

        // Method 1: Inverted Hull (Object Space)
        // Expand vertices along normal direction in object space
        float3 normalOS = normalize(input.normalOS);
        float3 expandedPositionOS = input.positionOS.xyz + normalOS * dynamicWidth;
        
        // Transform to clip space
        output.positionCS = TransformObjectToHClip(expandedPositionOS);

        // Alternative Method 2: Screen Space Expansion (commented out)
        /*
        // Transform to clip space first
        float4 clipPos = TransformObjectToHClip(input.positionOS.xyz);
        
        // Get normal in view space
        float3 normalVS = TransformWorldToViewNormal(TransformObjectToWorldNormal(input.normalOS), true);
        
        // Expand in screen space
        float2 screenNormal = normalize(normalVS.xy);
        clipPos.xy += screenNormal * dynamicWidth * clipPos.w;
        
        output.positionCS = clipPos;
        */

        // UV coordinates
        output.uv = input.uv;
    #else
        // If outline is disabled, render vertex at far plane
        output.positionCS = float4(0, 0, 0, 0);
        output.positionWS = float3(0, 0, 0);
        output.uv = input.uv;
    #endif

    return output;
}

// Outline Fragment Shader
half4 OutlineFragment(OutlineVaryings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    #if ENABLE_OUTLINE
        // Base outline color
        half4 outlineColor = _OutlineColor;

        // Optional: Sample main texture for outline color variation
        // half4 mainTexColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
        // outlineColor.rgb *= mainTexColor.rgb;

        // Distance-based color fading (optional)
        float distanceToCamera = distance(input.positionWS, GetCameraPositionWS());
        half distanceFade = saturate(1.0 - distanceToCamera * _OutlineDistanceFade * 0.1);
        outlineColor.a *= distanceFade;

        return outlineColor;
    #else
        // If outline is disabled, discard pixel
        discard;
        return half4(0, 0, 0, 0);
    #endif
}

#endif