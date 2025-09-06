Shader "Gorgonize/BasicToonTest"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _ShadowThreshold ("Shadow Threshold", Range(0, 1)) = 0.5
        _ShadowColor ("Shadow Color", Color) = (0.7, 0.7, 0.8, 1)
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
        }
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float4 shadowCoord : TEXCOORD1;
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _ShadowThreshold;
                float4 _ShadowColor;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                
                output.positionHCS = positionInputs.positionCS;
                output.normalWS = normalInputs.normalWS;
                output.shadowCoord = GetShadowCoord(positionInputs);
                
                return output;
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                Light mainLight = GetMainLight(input.shadowCoord);
                float3 normalWS = normalize(input.normalWS);
                
                float NdotL = saturate(dot(normalWS, mainLight.direction));
                float toonRamp = step(_ShadowThreshold, NdotL);
                toonRamp *= mainLight.shadowAttenuation;
                
                float3 color = lerp(_ShadowColor.rgb, _BaseColor.rgb, toonRamp);
                color *= mainLight.color;
                
                return float4(color, 1.0);
            }
            ENDHLSL
        }
    }
    
    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}