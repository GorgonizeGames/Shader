Shader "Hidden/GorgonizeGames/ToonOutlinePostProcess"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width", Float) = 1.0
        _DepthThreshold("Depth Threshold", Float) = 0.1
        _NormalThreshold("Normal Threshold", Float) = 0.4
        _ColorThreshold("Color Threshold", Float) = 0.2
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZTest Always
        ZWrite Off
        Cull Off

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Varyings
        {
            float2 uv : TEXCOORD0;
            float4 positionCS : SV_POSITION;
        };

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineWidth;
            float _DepthThreshold;
            float _NormalThreshold;
            float _ColorThreshold;
        CBUFFER_END

        Varyings vert(Attributes input)
        {
            Varyings output = (Varyings)0;
            output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
            output.uv = TRANSFORM_TEX(input.uv, _MainTex);
            return output;
        }

        // Sobel edge detection
        float SobelDepth(float2 uv)
        {
            float2 texelSize = _MainTex_TexelSize.xy * _OutlineWidth;
            
            // Sample depth values in 3x3 grid
            float d1 = SampleSceneDepth(uv + float2(-texelSize.x, -texelSize.y));
            float d2 = SampleSceneDepth(uv + float2(0, -texelSize.y));
            float d3 = SampleSceneDepth(uv + float2(texelSize.x, -texelSize.y));
            float d4 = SampleSceneDepth(uv + float2(-texelSize.x, 0));
            float d5 = SampleSceneDepth(uv);
            float d6 = SampleSceneDepth(uv + float2(texelSize.x, 0));
            float d7 = SampleSceneDepth(uv + float2(-texelSize.x, texelSize.y));
            float d8 = SampleSceneDepth(uv + float2(0, texelSize.y));
            float d9 = SampleSceneDepth(uv + float2(texelSize.x, texelSize.y));

            // Sobel X
            float sobelX = d1 * -1 + d2 * 0 + d3 * 1 +
                          d4 * -2 + d5 * 0 + d6 * 2 +
                          d7 * -1 + d8 * 0 + d9 * 1;

            // Sobel Y
            float sobelY = d1 * -1 + d2 * -2 + d3 * -1 +
                          d4 * 0  + d5 * 0  + d6 * 0 +
                          d7 * 1  + d8 * 2  + d9 * 1;

            return sqrt(sobelX * sobelX + sobelY * sobelY);
        }

        float3 SobelNormal(float2 uv)
        {
            float2 texelSize = _MainTex_TexelSize.xy * _OutlineWidth;
            
            // Sample normal values in 3x3 grid
            float3 n1 = SampleSceneNormals(uv + float2(-texelSize.x, -texelSize.y));
            float3 n2 = SampleSceneNormals(uv + float2(0, -texelSize.y));
            float3 n3 = SampleSceneNormals(uv + float2(texelSize.x, -texelSize.y));
            float3 n4 = SampleSceneNormals(uv + float2(-texelSize.x, 0));
            float3 n5 = SampleSceneNormals(uv);
            float3 n6 = SampleSceneNormals(uv + float2(texelSize.x, 0));
            float3 n7 = SampleSceneNormals(uv + float2(-texelSize.x, texelSize.y));
            float3 n8 = SampleSceneNormals(uv + float2(0, texelSize.y));
            float3 n9 = SampleSceneNormals(uv + float2(texelSize.x, texelSize.y));

            // Sobel X
            float3 sobelX = n1 * -1 + n2 * 0 + n3 * 1 +
                           n4 * -2 + n5 * 0 + n6 * 2 +
                           n7 * -1 + n8 * 0 + n9 * 1;

            // Sobel Y
            float3 sobelY = n1 * -1 + n2 * -2 + n3 * -1 +
                           n4 * 0  + n5 * 0  + n6 * 0 +
                           n7 * 1  + n8 * 2  + n9 * 1;

            return length(sobelX) + length(sobelY);
        }

        float3 SobelColor(float2 uv)
        {
            float2 texelSize = _MainTex_TexelSize.xy * _OutlineWidth;
            
            // Sample color values in 3x3 grid
            float3 c1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-texelSize.x, -texelSize.y)).rgb;
            float3 c2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, -texelSize.y)).rgb;
            float3 c3 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(texelSize.x, -texelSize.y)).rgb;
            float3 c4 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-texelSize.x, 0)).rgb;
            float3 c5 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).rgb;
            float3 c6 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(texelSize.x, 0)).rgb;
            float3 c7 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-texelSize.x, texelSize.y)).rgb;
            float3 c8 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, texelSize.y)).rgb;
            float3 c9 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(texelSize.x, texelSize.y)).rgb;

            // Sobel X
            float3 sobelX = c1 * -1 + c2 * 0 + c3 * 1 +
                           c4 * -2 + c5 * 0 + c6 * 2 +
                           c7 * -1 + c8 * 0 + c9 * 1;

            // Sobel Y
            float3 sobelY = c1 * -1 + c2 * -2 + c3 * -1 +
                           c4 * 0  + c5 * 0  + c6 * 0 +
                           c7 * 1  + c8 * 2  + c9 * 1;

            return length(sobelX) + length(sobelY);
        }

        ENDHLSL

        // Pass 0: Outline Detection
        Pass
        {
            Name "OutlineDetection"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature_local USE_DEPTH_TEXTURE
            #pragma shader_feature_local USE_NORMAL_TEXTURE
            #pragma shader_feature_local USE_COLOR_TEXTURE

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                half4 originalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                
                float edge = 0;
                
                #if USE_DEPTH_TEXTURE
                    // Depth-based edge detection
                    float depthEdge = SobelDepth(uv);
                    edge += step(_DepthThreshold, depthEdge);
                #endif
                
                #if USE_NORMAL_TEXTURE
                    // Normal-based edge detection
                    float normalEdge = SobelNormal(uv);
                    edge += step(_NormalThreshold, normalEdge);
                #endif
                
                #if USE_COLOR_TEXTURE
                    // Color-based edge detection
                    float colorEdge = SobelColor(uv);
                    edge += step(_ColorThreshold, colorEdge);
                #endif
                
                // Clamp edge value
                edge = saturate(edge);
                
                // Mix original color with outline color
                half3 finalColor = lerp(originalColor.rgb, _OutlineColor.rgb, edge * _OutlineColor.a);
                
                return half4(finalColor, originalColor.a);
            }
            ENDHLSL
        }

        // Pass 1: Upsample (for downsampled outline)
        Pass
        {
            Name "Upsample"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            half4 frag(Varyings input) : SV_Target
            {
                // Simple bilinear upsampling
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
            }
            ENDHLSL
        }
    }

    FallBack Off
}