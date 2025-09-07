Shader "Gorgonize/UltimateToonShaderPro"
{
    Properties
    {
        [Header(Base)]
        _BaseMap ("Albedo Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _Saturation ("Saturation", Range(0, 2)) = 1
        _Brightness ("Brightness", Range(0, 2)) = 1
        
        [Header(Toon Lighting)]
        _ShadowThreshold ("Shadow Threshold", Range(0, 1)) = 0.5
        _ShadowSmoothness ("Shadow Smoothness", Range(0, 0.5)) = 0.05
        _ShadowColor ("Shadow Color", Color) = (0.7, 0.7, 0.8, 1)
        _ShadowIntensity ("Shadow Intensity", Range(0, 1)) = 0.8
        
        [Header(Rim Light)]
        [Toggle(_RIM_LIGHTING)] _EnableRimLighting ("Enable Rim Lighting", Float) = 1
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Range(0.1, 10)) = 2
        _RimIntensity ("Rim Intensity", Range(0, 5)) = 1
        _RimThreshold ("Rim Threshold", Range(0, 1)) = 0.1
        
        [Header(Specular)]
        [Toggle(_SPECULAR)] _EnableSpecular ("Enable Specular", Float) = 1
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularSize ("Specular Size", Range(0.001, 1)) = 0.1
        _SpecularSmoothness ("Specular Smoothness", Range(0.001, 0.5)) = 0.05
        _SpecularIntensity ("Specular Intensity", Range(0, 5)) = 1
        
        [Header(Hatching Effects)]
        [Toggle(_HATCHING)] _EnableHatching ("Enable Hatching", Float) = 0
        _HatchingTex ("Hatching Texture", 2D) = "white" {}
        _CrossHatchingTex ("Cross Hatching Texture", 2D) = "white" {}
        _HatchingDensity ("Hatching Density", Range(0.1, 5)) = 1
        _HatchingIntensity ("Hatching Intensity", Range(0, 2)) = 1
        _HatchingThreshold ("Hatching Threshold", Range(0, 1)) = 0.5
        _CrossHatchingThreshold ("Cross Hatching Threshold", Range(0, 1)) = 0.3
        _HatchingRotation ("Hatching Rotation", Range(0, 360)) = 45
        
        [Header(Matcap)]
        [Toggle(_MATCAP)] _EnableMatcap ("Enable Matcap", Float) = 0
        _MatcapTex ("Matcap Texture", 2D) = "black" {}
        _MatcapIntensity ("Matcap Intensity", Range(0, 2)) = 1
        _MatcapBlendMode ("Matcap Blend Mode", Range(0, 2)) = 0
        
        [Header(Normal Map)]
        [Toggle(_NORMALMAP)] _EnableNormalMap ("Enable Normal Map", Float) = 0
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Range(0, 2)) = 1
        
        [Header(Emission)]
        [Toggle(_EMISSION)] _EnableEmission ("Enable Emission", Float) = 0
        _EmissionMap ("Emission Map", 2D) = "black" {}
        [HDR] _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 1
        _EmissionScrollSpeed ("Emission Scroll Speed", Vector) = (0, 0, 0, 0)
        
        [Header(Fresnel Effects)]
        [Toggle(_FRESNEL)] _EnableFresnel ("Enable Fresnel", Float) = 0
        _FresnelColor ("Fresnel Color", Color) = (1, 1, 1, 1)
        _FresnelPower ("Fresnel Power", Range(0.1, 10)) = 2
        _FresnelIntensity ("Fresnel Intensity", Range(0, 3)) = 1
        
        [Header(Subsurface Scattering)]
        [Toggle(_SUBSURFACE)] _EnableSubsurface ("Enable Subsurface", Float) = 0
        _SubsurfaceColor ("Subsurface Color", Color) = (1, 0.5, 0.5, 1)
        _SubsurfacePower ("Subsurface Power", Range(0.1, 10)) = 3
        _SubsurfaceIntensity ("Subsurface Intensity", Range(0, 2)) = 0.5
        
        [Header(Outline)]
        [Toggle(_OUTLINE)] _EnableOutline ("Enable Outline", Float) = 0
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.01
        
        [Header(Color Grading)]
        _Hue ("Hue Shift", Range(-180, 180)) = 0
        _Contrast ("Contrast", Range(0.5, 3)) = 1
        _Gamma ("Gamma", Range(0.5, 3)) = 1
        
        [Header(Stylization)]
        [Toggle(_POSTERIZE)] _EnablePosterize ("Enable Posterize", Float) = 0
        _PosterizeLevels ("Posterize Levels", Range(2, 32)) = 8
        [Toggle(_CEL_SHADING)] _EnableCelShading ("Enable Cel Shading", Float) = 0
        _CelShadingSteps ("Cel Shading Steps", Range(2, 10)) = 3
        
        [Header(Advanced)]
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "Geometry"
        }
        
        // Outline Pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            
            Cull Front
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex OutlineVertex
            #pragma fragment OutlineFragment
            #pragma shader_feature_local _OUTLINE
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct OutlineAttributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct OutlineVaryings
            {
                float4 positionHCS : SV_POSITION;
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineWidth;
            CBUFFER_END
            
            OutlineVaryings OutlineVertex(OutlineAttributes input)
            {
                OutlineVaryings output;
                
                #ifdef _OUTLINE
                    VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                    VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                    
                    float3 normalWS = normalInputs.normalWS;
                    float3 positionWS = positionInputs.positionWS;
                    positionWS += normalWS * _OutlineWidth;
                    output.positionHCS = TransformWorldToHClip(positionWS);
                #else
                    output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                #endif
                
                return output;
            }
            
            float4 OutlineFragment(OutlineVaryings input) : SV_Target
            {
                #ifdef _OUTLINE
                    return _OutlineColor;
                #else
                    discard;
                    return float4(0, 0, 0, 0);
                #endif
            }
            ENDHLSL
        }
        
        // Main Forward Lit Pass
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            Cull [_Cull]
            
            HLSLPROGRAM
            #pragma vertex ToonVertex
            #pragma fragment ToonFragment
            
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _SPECULAR
            #pragma shader_feature_local _RIM_LIGHTING
            #pragma shader_feature_local _EMISSION
            #pragma shader_feature_local _MATCAP
            #pragma shader_feature_local _FRESNEL
            #pragma shader_feature_local _SUBSURFACE
            #pragma shader_feature_local _POSTERIZE
            #pragma shader_feature_local _CEL_SHADING
            #pragma shader_feature_local _HATCHING
            #pragma shader_feature_local _OUTLINE
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);
            TEXTURE2D(_MatcapTex);
            SAMPLER(sampler_MatcapTex);
            TEXTURE2D(_HatchingTex);
            SAMPLER(sampler_HatchingTex);
            TEXTURE2D(_CrossHatchingTex);
            SAMPLER(sampler_CrossHatchingTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
                float _Saturation;
                float _Brightness;
                float _ShadowThreshold;
                float _ShadowSmoothness;
                float4 _ShadowColor;
                float _ShadowIntensity;
                float4 _RimColor;
                float _RimPower;
                float _RimIntensity;
                float _RimThreshold;
                float4 _SpecularColor;
                float _SpecularSize;
                float _SpecularSmoothness;
                float _SpecularIntensity;
                float4 _HatchingTex_ST;
                float4 _CrossHatchingTex_ST;
                float _HatchingDensity;
                float _HatchingIntensity;
                float _HatchingThreshold;
                float _CrossHatchingThreshold;
                float _HatchingRotation;
                float _MatcapIntensity;
                float _MatcapBlendMode;
                float _BumpScale;
                float4 _EmissionColor;
                float _EmissionIntensity;
                float4 _EmissionScrollSpeed;
                float4 _FresnelColor;
                float _FresnelPower;
                float _FresnelIntensity;
                float4 _SubsurfaceColor;
                float _SubsurfacePower;
                float _SubsurfaceIntensity;
                float _Hue;
                float _Contrast;
                float _Gamma;
                float _PosterizeLevels;
                float _CelShadingSteps;
                float _Cutoff;
            CBUFFER_END
            
            // Utility functions
            float3 RGBtoHSV(float3 rgb)
            {
                float4 k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(rgb.bg, k.wz), float4(rgb.gb, k.xy), step(rgb.b, rgb.g));
                float4 q = lerp(float4(p.xyw, rgb.r), float4(rgb.r, p.yzx), step(p.x, rgb.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }
            
            float3 HSVtoRGB(float3 hsv)
            {
                float4 k = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(hsv.xxx + k.xyz) * 6.0 - k.www);
                return hsv.z * lerp(k.xxx, saturate(p - k.xxx), hsv.y);
            }
            
            float3 ApplyColorGrading(float3 color)
            {
                color = pow(color, _Gamma);
                color = ((color - 0.5) * _Contrast) + 0.5;
                
                if (abs(_Hue) > 0.001)
                {
                    float3 hsv = RGBtoHSV(color);
                    hsv.x += _Hue / 360.0;
                    hsv.x = frac(hsv.x);
                    color = HSVtoRGB(hsv);
                }
                
                color = saturate(color * _Brightness);
                float luminance = dot(color, float3(0.299, 0.587, 0.114));
                color = lerp(luminance.xxx, color, _Saturation);
                
                return saturate(color);
            }
            
            float3 Posterize(float3 color, float levels)
            {
                return floor(color * levels) / levels;
            }
            
            float2 RotateUV(float2 uv, float rotation)
            {
                float rad = radians(rotation);
                float cosAngle = cos(rad);
                float sinAngle = sin(rad);
                float2 center = float2(0.5, 0.5);
                uv -= center;
                float2 rotatedUV;
                rotatedUV.x = uv.x * cosAngle - uv.y * sinAngle;
                rotatedUV.y = uv.x * sinAngle + uv.y * cosAngle;
                return rotatedUV + center;
            }
            
            float CalculateHatching(float2 uv, float lightValue)
            {
                float hatching = 1.0;
                
                #ifdef _HATCHING
                    float2 hatchUV = RotateUV(uv * _HatchingDensity, _HatchingRotation);
                    float hatch1 = SAMPLE_TEXTURE2D(_HatchingTex, sampler_HatchingTex, hatchUV).r;
                    
                    if (lightValue < _HatchingThreshold)
                    {
                        hatching *= lerp(1.0, hatch1, _HatchingIntensity);
                    }
                    
                    if (lightValue < _CrossHatchingThreshold)
                    {
                        float2 crossHatchUV = RotateUV(uv * _HatchingDensity, _HatchingRotation + 90);
                        float crossHatch = SAMPLE_TEXTURE2D(_CrossHatchingTex, sampler_CrossHatchingTex, crossHatchUV).r;
                        hatching *= lerp(1.0, crossHatch, _HatchingIntensity);
                    }
                #endif
                
                return hatching;
            }
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
                float4 shadowCoord : TEXCOORD5;
                float fogCoord : TEXCOORD6;
            };
            
            Varyings ToonVertex(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                
                output.positionHCS = positionInputs.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                
                #ifdef _NORMALMAP
                output.tangentWS = float4(normalInputs.tangentWS.xyz, input.tangentOS.w);
                #endif
                
                output.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
                output.shadowCoord = GetShadowCoord(positionInputs);
                output.fogCoord = ComputeFogFactor(output.positionHCS.z);
                
                return output;
            }
            
            float4 ToonFragment(Varyings input) : SV_Target
            {
                // Sample base textures
                float4 albedoAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                float3 albedo = albedoAlpha.rgb * _BaseColor.rgb;
                float alpha = albedoAlpha.a * _BaseColor.a;
                
                // Normal mapping
                float3 normalWS = normalize(input.normalWS);
                #ifdef _NORMALMAP
                    float4 normalMap = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv);
                    float3 normalTS = UnpackNormalScale(normalMap, _BumpScale);
                    
                    float sgn = input.tangentWS.w;
                    float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
                    float3x3 tangentToWorld = float3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz);
                    normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
                #endif
                normalWS = NormalizeNormalPerPixel(normalWS);
                
                float3 viewDirWS = SafeNormalize(input.viewDirWS);
                
                // Get main light
                Light mainLight = GetMainLight(input.shadowCoord);
                float3 lightDirWS = normalize(mainLight.direction);
                float3 lightColor = mainLight.color;
                
                // Calculate NdotL
                float NdotL = saturate(dot(normalWS, lightDirWS));
                
                // Toon ramp calculation
                float toonRamp = smoothstep(_ShadowThreshold - _ShadowSmoothness, _ShadowThreshold + _ShadowSmoothness, NdotL);
                
                // Apply cel shading quantization
                #ifdef _CEL_SHADING
                    toonRamp = floor(toonRamp * _CelShadingSteps) / _CelShadingSteps;
                #endif
                
                // Apply shadow attenuation
                toonRamp *= mainLight.shadowAttenuation;
                
                // Apply hatching
                float hatchingMask = CalculateHatching(input.uv, toonRamp);
                
                // Mix shadow color with albedo
                float3 shadowedColor = lerp(_ShadowColor.rgb * albedo, albedo, _ShadowIntensity);
                float3 litColor = lerp(shadowedColor, albedo, toonRamp);
                litColor *= hatchingMask;
                
                // Apply main light color
                litColor *= lightColor;
                
                // Rim lighting
                #ifdef _RIM_LIGHTING
                    float rim = 1.0 - saturate(dot(viewDirWS, normalWS));
                    rim = smoothstep(_RimThreshold, 1.0, rim);
                    rim = pow(rim, _RimPower) * _RimIntensity;
                    litColor += rim * _RimColor.rgb;
                #endif
                
                // Specular highlights
                #ifdef _SPECULAR
                    float3 halfVector = normalize(lightDirWS + viewDirWS);
                    float NdotH = saturate(dot(normalWS, halfVector));
                    float specular = pow(NdotH, (1.0 - _SpecularSize) * 128.0);
                    specular = smoothstep(0.5 - _SpecularSmoothness, 0.5 + _SpecularSmoothness, specular);
                    specular *= toonRamp * mainLight.shadowAttenuation * _SpecularIntensity;
                    litColor += specular * _SpecularColor.rgb;
                #endif
                
                // Matcap
                #ifdef _MATCAP
                    float3 normalVS = mul((float3x3)UNITY_MATRIX_V, normalWS);
                    float2 matcapUV = normalVS.xy * 0.5 + 0.5;
                    float3 matcap = SAMPLE_TEXTURE2D(_MatcapTex, sampler_MatcapTex, matcapUV).rgb;
                    
                    if (_MatcapBlendMode < 0.5)
                        litColor += matcap * _MatcapIntensity;
                    else if (_MatcapBlendMode < 1.5)
                        litColor *= lerp(1, matcap, _MatcapIntensity);
                    else
                        litColor = 1 - (1 - litColor) * (1 - matcap * _MatcapIntensity);
                #endif
                
                // Fresnel effect
                #ifdef _FRESNEL
                    float fresnel = pow(1.0 - saturate(dot(viewDirWS, normalWS)), _FresnelPower) * _FresnelIntensity;
                    litColor += fresnel * _FresnelColor.rgb;
                #endif
                
                // Subsurface scattering
                #ifdef _SUBSURFACE
                    float3 lightDirViewSpace = mul((float3x3)UNITY_MATRIX_V, -lightDirWS);
                    float3 viewDirViewSpace = mul((float3x3)UNITY_MATRIX_V, -viewDirWS);
                    float subsurface = pow(saturate(dot(lightDirViewSpace, -viewDirViewSpace)), _SubsurfacePower);
                    litColor += subsurface * _SubsurfaceColor.rgb * _SubsurfaceIntensity;
                #endif
                
                // Emission
                #ifdef _EMISSION
                    float2 emissionUV = input.uv + _EmissionScrollSpeed.xy * _Time.y;
                    float3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, emissionUV).rgb;
                    emission *= _EmissionColor.rgb * _EmissionIntensity;
                    litColor += emission;
                #endif
                
                // Apply color grading
                litColor = ApplyColorGrading(litColor);
                
                // Posterize effect
                #ifdef _POSTERIZE
                    litColor = Posterize(litColor, _PosterizeLevels);
                #endif
                
                // Apply fog
                litColor = MixFog(litColor, input.fogCoord);
                
                return float4(litColor, alpha);
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull [_Cull]
            
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
    
    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}