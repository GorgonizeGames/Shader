Shader "Hidden/Edge Detection"
{
    Properties
    {
        _OutlineThickness ("Fırça Kalınlığı", Float) = 8.0
        _OutlineColor ("Fırça Rengi", Color) = (0,0,0,1)
        _DepthThreshold ("Derinlik Hassasiyeti", Float) = 0.01
        _NormalsThreshold ("Normal Hassasiyeti", Range(0, 1)) = 0.4
        _AntiAliasingAmount ("Kenar Yumuşatma Miktarı", Range(0, 1)) = 0.5
        _BrushTex ("Fırça Dokusu", 2D) = "white" {}
        _BrushTiling ("Fırça Deseni Boyutu", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"="Opaque"
        }

        ZWrite Off
        Cull Off
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Name "TEXTURED BRUSH OUTLINE"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            // Değişkenler
            float _OutlineThickness;
            float4 _OutlineColor;
            float _DepthThreshold;
            float _NormalsThreshold;
            float _AntiAliasingAmount;
            
            TEXTURE2D(_BrushTex);
            SAMPLER(sampler_BrushTex);

            float _BrushTiling;

            float4 frag(Varyings IN) : SV_TARGET
            {
                float2 uv = IN.texcoord;
                float2 texel_size = _ScreenParams.zw - 1.0;

                // --- SOBEL OPERATÖRÜ LOGIĞI ---
                float2 uv_up = uv + float2(0, texel_size.y * _OutlineThickness);
                float2 uv_down = uv + float2(0, -texel_size.y * _OutlineThickness);
                float2 uv_left = uv + float2(-texel_size.x * _OutlineThickness, 0);
                float2 uv_right = uv + float2(texel_size.x * _OutlineThickness, 0);

                // --- View-Space Derinlik Hesaplaması ---
                float raw_depth_up = SampleSceneDepth(uv_up);
                float raw_depth_down = SampleSceneDepth(uv_down);
                float raw_depth_left = SampleSceneDepth(uv_left);
                float raw_depth_right = SampleSceneDepth(uv_right);

                float depth_up = abs(ComputeViewSpacePosition(uv_up, raw_depth_up, UNITY_MATRIX_I_P).z);
                float depth_down = abs(ComputeViewSpacePosition(uv_down, raw_depth_down, UNITY_MATRIX_I_P).z);
                float depth_left = abs(ComputeViewSpacePosition(uv_left, raw_depth_left, UNITY_MATRIX_I_P).z);
                float depth_right = abs(ComputeViewSpacePosition(uv_right, raw_depth_right, UNITY_MATRIX_I_P).z);

                float3 normal_up = SampleSceneNormals(uv_up);
                float3 normal_down = SampleSceneNormals(uv_down);
                float3 normal_left = SampleSceneNormals(uv_left);
                float3 normal_right = SampleSceneNormals(uv_right);
                
                float depth_h_grad = depth_right - depth_left;
                float depth_v_grad = depth_up - depth_down;
                float depth_gradient = sqrt(depth_h_grad * depth_h_grad + depth_v_grad * depth_v_grad);

                float3 normal_h_grad = normal_right - normal_left;
                float3 normal_v_grad = normal_up - normal_down;
                float normal_gradient = sqrt(dot(normal_h_grad, normal_h_grad) + dot(normal_v_grad, normal_v_grad));

                // --- EN ETKİLİ Anti-Aliasing ---
                // Yumuşatma miktarını doğrudan _AntiAliasingAmount'a bağlayarak slider'ın
                // etkisini iki katına çıkarıyoruz. Bu sayede çok daha güçlü bir yumuşatma elde edilir.
                float softness = _AntiAliasingAmount;

                float depth_edge = smoothstep(_DepthThreshold - softness, _DepthThreshold + softness, depth_gradient);
                float normal_edge = smoothstep(_NormalsThreshold - softness, _NormalsThreshold + softness, normal_gradient);


                float edge = max(depth_edge, normal_edge);
                
                // --- DOKU UYGULAMA MANTIĞI ---
                if (edge > 0.0)
                {
                    float raw_depth = SampleSceneDepth(uv);
                    float3 worldPos = ComputeViewSpacePosition(uv, raw_depth, UNITY_MATRIX_I_VP);
                    
                    float2 brush_uv = worldPos.xy * _BrushTiling;
                    float brush_sample = SAMPLE_TEXTURE2D(_BrushTex, sampler_BrushTex, brush_uv).r;
                    edge *= brush_sample;
                }
                
                float final_alpha = edge * _OutlineColor.a;
                
                return float4(_OutlineColor.rgb, final_alpha);
            }
            ENDHLSL
        }
    }
}

