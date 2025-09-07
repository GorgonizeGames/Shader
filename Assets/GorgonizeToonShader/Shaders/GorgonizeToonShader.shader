Shader "Gorgonize/Toon Shader"
{
    Properties
    {
        [Header(_____ GORGONIZE TOON SHADER v4.0 _____)]
        [Space(10)]
        
        [Header(Base Material)]
        _BaseMap ("Albedo Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _Saturation ("Saturation", Range(0, 3)) = 1
        _Brightness ("Brightness", Range(0, 3)) = 1
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _OcclusionStrength ("Occlusion Strength", Range(0, 1)) = 1
        
        [Header(Toon Lighting Core)]
        [Toggle(_USE_RAMP_TEXTURE)] _UseRampTexture ("Use Light Ramp Texture", Float) = 0
        _LightRampTex ("Light Ramp Texture", 2D) = "white" {}
        _ShadowThreshold ("Shadow Threshold", Range(0, 1)) = 0.5
        _ShadowSmoothness ("Shadow Smoothness", Range(0, 0.5)) = 0.05
        _ShadowColor ("Shadow Color", Color) = (0.7, 0.7, 0.8, 1)
        _ShadowIntensity ("Shadow Intensity", Range(0, 1)) = 0.8
        _IndirectLightingBoost ("Indirect Lighting Boost", Range(0, 2)) = 0.3
        _LightWrapping ("Light Wrapping", Range(0, 1)) = 0
        
        [Header(Advanced Lighting)]
        [Toggle(_ENABLE_VOLUMETRIC_LIGHTING)] _EnableVolumetricLighting ("Volumetric Lighting", Float) = 0
        _VolumetricIntensity ("Volumetric Intensity", Range(0, 2)) = 0.5
        [Toggle(_ENABLE_LIGHT_COOKIES)] _EnableLightCookies ("Light Cookies Support", Float) = 1
        _CookieInfluence ("Cookie Influence", Range(0, 1)) = 0.5
        
        [Header(Surface Effects)]
        [Toggle(_RIM_LIGHTING)] _EnableRimLighting ("Enable Rim Lighting", Float) = 1
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Range(0.1, 10)) = 2
        _RimIntensity ("Rim Intensity", Range(0, 5)) = 1
        _RimThreshold ("Rim Threshold", Range(0, 1)) = 0.1
        [Toggle(_RIM_LIGHTING_DUAL_LAYER)] _RimDualLayer ("Dual Layer Rim", Float) = 0
        _RimColorSecondary ("Secondary Rim Color", Color) = (0.5, 0.8, 1, 1)
        _RimPowerSecondary ("Secondary Rim Power", Range(0.1, 10)) = 4
        
        [Header(Advanced Specular)]
        [Toggle(_SPECULAR)] _EnableSpecular ("Enable Specular", Float) = 1
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularSize ("Specular Size", Range(0.001, 1)) = 0.1
        _SpecularSmoothness ("Specular Smoothness", Range(0.001, 0.5)) = 0.05
        _SpecularIntensity ("Specular Intensity", Range(0, 10)) = 1
        [Toggle(_SPECULAR_ANISOTROPIC)] _AnisotropicSpecular ("Anisotropic Specular", Float) = 0
        _Anisotropy ("Anisotropy", Range(-1, 1)) = 0
        [Toggle(_SPECULAR_STEPPED)] _SteppedSpecular ("Stepped Specular", Float) = 0
        _SpecularSteps ("Specular Steps", Range(1, 8)) = 3
        
        [Header(NPR Hatching System)]
        [Toggle(_HATCHING)] _EnableHatching ("Enable Hatching", Float) = 0
        _HatchingTex ("Primary Hatching", 2D) = "white" {}
        _CrossHatchingTex ("Cross Hatching", 2D) = "white" {}
        _HatchingTex2 ("Secondary Hatching", 2D) = "white" {}
        _HatchingDensity ("Hatching Density", Range(0.1, 10)) = 1
        _HatchingIntensity ("Hatching Intensity", Range(0, 2)) = 1
        _HatchingThreshold ("Primary Threshold", Range(0, 1)) = 0.6
        _CrossHatchingThreshold ("Cross Hatch Threshold", Range(0, 1)) = 0.4
        _SecondaryHatchingThreshold ("Secondary Threshold", Range(0, 1)) = 0.2
        _HatchingRotation ("Hatching Rotation", Range(0, 360)) = 45
        [Toggle(_HATCHING_ANIMATED)] _AnimatedHatching ("Animated Hatching", Float) = 0
        _HatchingAnimSpeed ("Animation Speed", Range(0, 5)) = 1
        [Toggle(_SCREEN_SPACE_HATCHING)] _ScreenSpaceHatching ("Screen Space Hatching", Float) = 0
        _ScreenHatchScale ("Screen Hatch Scale", Range(0.1, 20)) = 2
        _ScreenHatchBias ("Screen Hatch Bias", Range(-1, 1)) = 0
        
        [Header(Matcap and IBL)]
        [Toggle(_MATCAP)] _EnableMatcap ("Enable Matcap", Float) = 0
        _MatcapTex ("Matcap Texture", 2D) = "black" {}
        _MatcapIntensity ("Matcap Intensity", Range(0, 5)) = 1
        [Enum(Add,0,Multiply,1,Screen,2,Overlay,3)] _MatcapBlendMode ("Blend Mode", Float) = 0
        _MatcapRotation ("Matcap Rotation", Range(0, 360)) = 0
        [Toggle(_MATCAP_PERSPECTIVE_CORRECTION)] _MatcapPerspectiveCorrection ("Perspective Correction", Float) = 1
        
        [Header(Normal Mapping and Detail)]
        [Toggle(_NORMALMAP)] _EnableNormalMap ("Enable Normal Map", Float) = 0
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Range(0, 3)) = 1
        [Toggle(_DETAIL)] _EnableDetail ("Enable Detail", Float) = 0
        _DetailMap ("Detail Albedo", 2D) = "gray" {}
        _DetailNormalMap ("Detail Normal", 2D) = "bump" {}
        _DetailScale ("Detail Scale", Range(0, 2)) = 1
        _DetailNormalScale ("Detail Normal Scale", Range(0, 2)) = 1
        
        [Header(Advanced Emission)]
        [Toggle(_EMISSION)] _EnableEmission ("Enable Emission", Float) = 0
        _EmissionMap ("Emission Map", 2D) = "black" {}
        [HDR] _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)
        _EmissionIntensity ("Emission Intensity", Range(0, 20)) = 1
        _EmissionScrollSpeed ("Emission Scroll Speed", Vector) = (0, 0, 0, 0)
        [Toggle(_EMISSION_PULSING)] _EmissionPulsing ("Pulsing Emission", Float) = 0
        _EmissionPulseSpeed ("Pulse Speed", Range(0, 10)) = 2
        _EmissionPulseIntensity ("Pulse Intensity", Range(0, 1)) = 0.5
        [Toggle(_EMISSION_TEMPERATURE_BASED)] _EmissionTemperatureBased ("Temperature Based", Float) = 0
        _EmissionTemperature ("Temperature (K)", Range(1000, 12000)) = 6500
        
        [Header(Fresnel and Subsurface)]
        [Toggle(_FRESNEL)] _EnableFresnel ("Enable Fresnel", Float) = 0
        _FresnelColor ("Fresnel Color", Color) = (1, 1, 1, 1)
        _FresnelPower ("Fresnel Power", Range(0.1, 10)) = 2
        _FresnelIntensity ("Fresnel Intensity", Range(0, 5)) = 1
        [Toggle(_FRESNEL_IRIDESCENCE)] _FresnelIridescence ("Iridescence", Float) = 0
        _IridescenceIntensity ("Iridescence Intensity", Range(0, 2)) = 1
        
        [Toggle(_SUBSURFACE)] _EnableSubsurface ("Enable Subsurface", Float) = 0
        _SubsurfaceColor ("Subsurface Color", Color) = (1, 0.5, 0.5, 1)
        _SubsurfacePower ("Subsurface Power", Range(0.1, 10)) = 3
        _SubsurfaceIntensity ("Subsurface Intensity", Range(0, 5)) = 0.5
        _SubsurfaceDistortion ("Subsurface Distortion", Range(0, 2)) = 1
        _SubsurfaceThickness ("Thickness Map", 2D) = "white" {}
        
        [Header(Outline System)]
        [Toggle(_OUTLINE)] _EnableOutline ("Enable Outline", Float) = 0
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.2)) = 0.01
        [Enum(Normal,0,Position,1,Clip,2)] _OutlineMode ("Outline Mode", Float) = 0
        _OutlineFadeDistance ("Fade Distance", Range(0, 100)) = 10
        [Toggle(_OUTLINE_DISTANCE_FADE)] _OutlineDistanceFade ("Distance Fade", Float) = 1
        [Toggle(_OUTLINE_DEPTH_BIAS)] _OutlineDepthBias ("Depth Bias", Float) = 1
        _OutlineDepthBiasValue ("Depth Bias Value", Range(0, 1)) = 0.1
        
        [Header(Color Grading and Stylization)]
        _Hue ("Hue Shift", Range(-180, 180)) = 0
        _Contrast ("Contrast", Range(0.1, 5)) = 1
        _Gamma ("Gamma", Range(0.1, 5)) = 1
        _ColorTemperature ("Color Temperature", Range(-100, 100)) = 0
        _ColorTint ("Color Tint", Range(-100, 100)) = 0
        _Vibrance ("Vibrance", Range(-2, 2)) = 0
        
        [Header(Quantization Effects)]
        [Toggle(_POSTERIZE)] _EnablePosterize ("Enable Posterize", Float) = 0
        _PosterizeLevels ("Posterize Levels", Range(2, 64)) = 8
        [Toggle(_CEL_SHADING)] _EnableCelShading ("Enable Cel Shading", Float) = 0
        _CelShadingSteps ("Cel Shading Steps", Range(2, 16)) = 3
        [Toggle(_DITHERING)] _EnableDithering ("Enable Dithering", Float) = 0
        _DitheringIntensity ("Dithering Intensity", Range(0, 1)) = 0.1
        
        [Header(Advanced Visual Effects)]
        [Toggle(_FORCE_FIELD)] _EnableForceField ("Force Field Effect", Float) = 0
        _ForceFieldColor ("Force Field Color", Color) = (0.2, 0.8, 1, 1)
        _ForceFieldIntensity ("Force Field Intensity", Range(0, 2)) = 1
        _ForceFieldFrequency ("Force Field Frequency", Range(1, 20)) = 10
        
        [Toggle(_HOLOGRAM)] _EnableHologram ("Hologram Effect", Float) = 0
        _HologramIntensity ("Hologram Intensity", Range(0, 2)) = 1
        _HologramFlicker ("Hologram Flicker", Range(0, 10)) = 2
        _HologramScanlines ("Hologram Scanlines", Range(0, 2000)) = 800
        
        [Toggle(_DISSOLVE)] _EnableDissolve ("Dissolve Effect", Float) = 0
        _DissolveNoise ("Dissolve Noise", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _DissolveEdgeWidth ("Dissolve Edge Width", Range(0, 0.2)) = 0.1
        _DissolveEdgeColor ("Dissolve Edge Color", Color) = (1, 0.5, 0, 1)
        
        [Header(Animation System)]
        [Toggle(_ANIMATED_PROPERTIES)] _EnableAnimatedProperties ("Enable Animations", Float) = 0
        _AnimationSpeed ("Global Animation Speed", Range(0, 5)) = 1
        [Toggle(_VERTEX_ANIMATION)] _EnableVertexAnimation ("Vertex Animation", Float) = 0
        _VertexAnimationIntensity ("Vertex Animation Intensity", Range(0, 1)) = 0.1
        _VertexAnimationFrequency ("Vertex Animation Frequency", Range(0, 10)) = 2
        
        [Header(Performance and Quality)]
        [Enum(Low,0,Medium,1,High,2,Ultra,3)] _QualityLevel ("Quality Level", Float) = 2
        [Toggle(_LOD_FADE)] _EnableLODFade ("Enable LOD Fade", Float) = 1
        _LODFadeDistance ("LOD Fade Distance", Range(1, 100)) = 20
        [Toggle(_INSTANCING_SUPPORT)] _EnableInstancing ("GPU Instancing", Float) = 1
        
        [Header(Advanced Rendering)]
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2
        [Enum(Off,0,On,1)] _ZWrite ("Z Write", Float) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("Z Test", Float) = 4
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
        [Toggle(_ALPHA_PREMULTIPLY)] _AlphaPremultiply ("Alpha Premultiply", Float) = 0
        
        [Header(Debug and Visualization)]
        [Toggle(_DEBUG_MODE)] _DebugMode ("Debug Mode", Float) = 0
        [Enum(None,0,Normals,1,Lighting,2,Shadows,3,Hatching,4)] _DebugView ("Debug View", Float) = 0
        [Toggle(_WIREFRAME)] _ShowWireframe ("Show Wireframe", Float) = 0
        _WireframeColor ("Wireframe Color", Color) = (0, 1, 0, 1)
        _WireframeThickness ("Wireframe Thickness", Range(0, 0.1)) = 0.01
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "Geometry"
            "IgnoreProjector" = "True"
        }
        
        LOD 300
        
        // Outline Pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            
            Cull Front
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex OutlineVertex
            #pragma fragment OutlineFragment
            
            // Shader features
            #pragma shader_feature_local _OUTLINE
            #pragma shader_feature_local _OUTLINE_DISTANCE_FADE
            #pragma shader_feature_local _OUTLINE_DEPTH_BIAS
            #pragma shader_feature_local _INSTANCING_SUPPORT
            
            // Unity keywords
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Includes/GorgonizeToonProperties.hlsl"
            
            struct OutlineAttributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct OutlineVaryings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float fogCoord : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            OutlineVaryings OutlineVertex(OutlineAttributes input)
            {
                OutlineVaryings output;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                #ifdef _OUTLINE
                    VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                    VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                    
                    float3 normalWS = normalInputs.normalWS;
                    float3 positionWS = positionInputs.positionWS;
                    
                    // Calculate outline width with distance fade
                    float outlineWidth = _OutlineWidth;
                    #ifdef _OUTLINE_DISTANCE_FADE
                        float distance = length(positionWS - _WorldSpaceCameraPos);
                        float fadeFactor = saturate(1.0 - (distance / _OutlineFadeDistance));
                        outlineWidth *= fadeFactor;
                    #endif
                    
                    // Different outline methods
                    if (_OutlineMode < 0.5) // Normal-based
                    {
                        positionWS += normalWS * outlineWidth;
                    }
                    else if (_OutlineMode < 1.5) // Position-based
                    {
                        float3 viewDir = normalize(positionWS - _WorldSpaceCameraPos);
                        positionWS += viewDir * outlineWidth;
                    }
                    // Clip-space method handled in fragment
                    
                    output.positionHCS = TransformWorldToHClip(positionWS);
                    output.positionWS = positionWS;
                #else
                    output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                    output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                #endif
                
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.fogCoord = ComputeFogFactor(output.positionHCS.z);
                
                return output;
            }
            
            float4 OutlineFragment(OutlineVaryings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                #ifdef _OUTLINE
                    float4 outlineColor = _OutlineColor;
                    
                    // Apply fog
                    outlineColor.rgb = MixFog(outlineColor.rgb, input.fogCoord);
                    
                    return outlineColor;
                #else
                    discard;
                    return float4(0, 0, 0, 0);
                #endif
            }
            ENDHLSL
        }
        
        // Main Forward Pass
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Blend [_SrcBlend] [_DstBlend]
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex ToonVertex
            #pragma fragment ToonFragment
            
            // Local shader features
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _DETAIL
            #pragma shader_feature_local _SPECULAR
            #pragma shader_feature_local _SPECULAR_ANISOTROPIC
            #pragma shader_feature_local _SPECULAR_STEPPED
            #pragma shader_feature_local _RIM_LIGHTING
            #pragma shader_feature_local _RIM_LIGHTING_DUAL_LAYER
            #pragma shader_feature_local _EMISSION
            #pragma shader_feature_local _EMISSION_PULSING
            #pragma shader_feature_local _EMISSION_TEMPERATURE_BASED
            #pragma shader_feature_local _MATCAP
            #pragma shader_feature_local _MATCAP_PERSPECTIVE_CORRECTION
            #pragma shader_feature_local _FRESNEL
            #pragma shader_feature_local _FRESNEL_IRIDESCENCE
            #pragma shader_feature_local _SUBSURFACE
            #pragma shader_feature_local _POSTERIZE
            #pragma shader_feature_local _CEL_SHADING
            #pragma shader_feature_local _DITHERING
            #pragma shader_feature_local _HATCHING
            #pragma shader_feature_local _HATCHING_ANIMATED
            #pragma shader_feature_local _SCREEN_SPACE_HATCHING
            #pragma shader_feature_local _USE_RAMP_TEXTURE
            #pragma shader_feature_local _FORCE_FIELD
            #pragma shader_feature_local _HOLOGRAM
            #pragma shader_feature_local _DISSOLVE
            #pragma shader_feature_local _ANIMATED_PROPERTIES
            #pragma shader_feature_local _VERTEX_ANIMATION
            #pragma shader_feature_local _LOD_FADE
            #pragma shader_feature_local _INSTANCING_SUPPORT
            #pragma shader_feature_local _DEBUG_MODE
            #pragma shader_feature_local _WIREFRAME
            #pragma shader_feature_local _ALPHA_PREMULTIPLY
            #pragma shader_feature_local _ENABLE_VOLUMETRIC_LIGHTING
            #pragma shader_feature_local _ENABLE_LIGHT_COOKIES
            
            // Unity built-in keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
            
            // Include our custom files AFTER Unity's core files to avoid conflicts
            #include "Includes/GorgonizeToonProperties.hlsl"
            #include "Includes/GorgonizeToonUtilities.hlsl"
            #include "Includes/GorgonizeToonLighting.hlsl"
            #include "Includes/GorgonizeToonEffects.hlsl"
            
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
            
            struct ToonVaryings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                half4 tangentWS : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
                half4 fogFactorAndVertexLight : TEXCOORD5;
                float4 shadowCoord : TEXCOORD6;
                float4 screenPos : TEXCOORD7;
                
                DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 8);
                #ifdef DYNAMICLIGHTMAP_ON
                float2 dynamicLightmapUV : TEXCOORD9;
                #endif
                
                #ifdef _ADDITIONAL_LIGHTS_VERTEX
                half4 fogFactorAndVertexLight : TEXCOORD10;
                #endif
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            ToonVaryings ToonVertex(ToonAttributes input)
            {
                ToonVaryings output = (ToonVaryings)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                
                // Vertex animation
                #ifdef _VERTEX_ANIMATION
                    float3 worldPos = positionInputs.positionWS;
                    float time = _Time.y * _AnimationSpeed * _VertexAnimationFrequency;
                    float3 animOffset = sin(worldPos * 0.1 + time) * _VertexAnimationIntensity;
                    positionInputs.positionWS += animOffset * normalInputs.normalWS;
                    positionInputs.positionCS = TransformWorldToHClip(positionInputs.positionWS);
                #endif
                
                output.positionHCS = positionInputs.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.tangentWS = half4(normalInputs.tangentWS.xyz, input.tangentOS.w);
                output.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
                output.screenPos = ComputeScreenPos(output.positionHCS);
                
                // Lighting data
                half3 vertexLight = VertexLighting(positionInputs.positionWS, normalInputs.normalWS);
                half fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
                
                OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
                #ifdef DYNAMICLIGHTMAP_ON
                output.dynamicLightmapUV = input.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
                
                output.shadowCoord = GetShadowCoord(positionInputs);
                
                return output;
            }
            
            half4 ToonFragment(ToonVaryings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Early alpha test
                half4 albedoAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half alpha = albedoAlpha.a * _BaseColor.a;
                #ifdef _ALPHATEST_ON
                    clip(alpha - _Cutoff);
                #endif
                
                // Surface data setup
                ToonSurfaceData surfaceData = (ToonSurfaceData)0;
                ToonLightingData lightingData = (ToonLightingData)0;
                
                // Initialize surface data
                surfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;
                surfaceData.alpha = alpha;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Smoothness;
                surfaceData.occlusion = 1.0;
                
                // Normal mapping
                surfaceData.normalWS = normalize(input.normalWS);
                #ifdef _NORMALMAP
                    half4 normalMap = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv);
                    half3 normalTS = UnpackNormalScale(normalMap, _BumpScale);
                    
                    half sgn = input.tangentWS.w;
                    half3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
                    half3x3 tangentToWorld = half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz);
                    surfaceData.normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
                    surfaceData.tangentWS = input.tangentWS.xyz;
                    surfaceData.bitangentWS = bitangent;
                #endif
                surfaceData.normalWS = NormalizeNormalPerPixel(surfaceData.normalWS);
                
                // Detail mapping
                #ifdef _DETAIL
                    half4 detailAlbedo = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, input.uv * _DetailMap_ST.xy + _DetailMap_ST.zw);
                    surfaceData.albedo = lerp(surfaceData.albedo, surfaceData.albedo * detailAlbedo.rgb, _DetailScale);
                    
                    half4 detailNormal = SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailNormalMap, input.uv * _DetailNormalMap_ST.xy + _DetailNormalMap_ST.zw);
                    half3 detailNormalTS = UnpackNormalScale(detailNormal, _DetailNormalScale);
                    surfaceData.normalWS = BlendNormalRNM(surfaceData.normalWS, detailNormalTS);
                #endif
                
                // Subsurface thickness
                #ifdef _SUBSURFACE
                    half thickness = SAMPLE_TEXTURE2D(_SubsurfaceThickness, sampler_SubsurfaceThickness, input.uv).r;
                    surfaceData.subsurfaceThickness = thickness;
                #endif
                
                // Emission
                surfaceData.emission = 0;
                #ifdef _EMISSION
                    half2 emissionUV = input.uv + _EmissionScrollSpeed.xy * _Time.y;
                    half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, emissionUV).rgb;
                    
                    #ifdef _EMISSION_TEMPERATURE_BASED
                        emission *= BlackBodyRadiation(_EmissionTemperature);
                    #endif
                    
                    emission *= _EmissionColor.rgb * _EmissionIntensity;
                    
                    #ifdef _EMISSION_PULSING
                        half pulse = PulseAnimation(_Time.y * _EmissionPulseSpeed, 1.0, 2.0);
                        emission *= lerp(1.0, pulse, _EmissionPulseIntensity);
                    #endif
                    
                    surfaceData.emission = emission;
                #endif
                
                // Initialize lighting data
                lightingData.normalWS = surfaceData.normalWS;
                lightingData.viewDirectionWS = SafeNormalize(input.viewDirWS);
                lightingData.shadowCoord = input.shadowCoord;
                lightingData.positionWS = input.positionWS;
                lightingData.screenPos = input.screenPos;
                lightingData.fogFactor = input.fogFactorAndVertexLight.x;
                
                // Sample baked GI
                #ifdef LIGHTMAP_ON
                    lightingData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.dynamicLightmapUV, input.vertexSH, lightingData.normalWS);
                #else
                    lightingData.bakedGI = SAMPLE_GI(input.vertexSH, lightingData.normalWS);
                #endif
                
                // Screen space occlusion
                #ifdef _SCREEN_SPACE_OCCLUSION
                    AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(input.screenPos);
                    surfaceData.occlusion = min(surfaceData.occlusion, aoFactor.indirectAmbientOcclusion);
                    lightingData.bakedGI *= aoFactor.indirectAmbientOcclusion;
                #endif
                
                // Calculate main toon lighting
                half3 finalColor = CalculateQualityAwareLighting(surfaceData, lightingData, input.uv);
                
                // Advanced effects
                #ifdef _FORCE_FIELD
                    finalColor = ApplyForceFieldEffect(finalColor, surfaceData.normalWS, lightingData.viewDirectionWS, _Time.y, _ForceFieldColor.rgb, _ForceFieldIntensity, _ForceFieldFrequency);
                #endif
                
                #ifdef _HOLOGRAM
                    finalColor = ApplyHologramEffect(finalColor, input.uv, _Time.y, _HologramIntensity, _HologramFlicker, _HologramScanlines);
                #endif
                
                #ifdef _DISSOLVE
                    half2 dissolveUV = input.uv + _Time.y * 0.1;
                    half dissolveNoise = SAMPLE_TEXTURE2D(_DissolveNoise, sampler_DissolveNoise, dissolveUV).r;
                    half dissolve = CalculateDissolveEffect(dissolveNoise, _DissolveAmount, _DissolveEdgeWidth);
                    finalColor = lerp(_DissolveEdgeColor.rgb, finalColor, dissolve);
                    alpha *= dissolve;
                #endif
                
                // Debug visualization
                #ifdef _DEBUG_MODE
                    finalColor = ApplyDebugVisualization(finalColor, surfaceData, lightingData, _DebugView);
                #endif
                
                #ifdef _WIREFRAME
                    finalColor = ApplyWireframe(finalColor, input.positionWS, _WireframeColor.rgb, _WireframeThickness);
                #endif
                
                // LOD fade
                #ifdef _LOD_FADE
                    half lodFade = CalculateLODFade(input.positionWS, _LODFadeDistance);
                    alpha *= lodFade;
                #endif
                
                // Apply fog
                finalColor = MixFog(finalColor, lightingData.fogFactor);
                
                // Alpha premultiply
                #ifdef _ALPHA_PREMULTIPLY
                    finalColor *= alpha;
                #endif
                
                return half4(finalColor, alpha);
            }
            ENDHLSL
        }
        
        // Shadow Caster Pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull [_Cull]
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _VERTEX_ANIMATION
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Includes/GorgonizeToonProperties.hlsl"
            
            struct ShadowAttributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct ShadowVaryings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            float4 GetShadowPositionHClip(ShadowAttributes input)
            {
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                
                #ifdef _VERTEX_ANIMATION
                    float time = _Time.y * _AnimationSpeed * _VertexAnimationFrequency;
                    float3 animOffset = sin(positionWS * 0.1 + time) * _VertexAnimationIntensity;
                    positionWS += animOffset * normalWS;
                #endif
                
                #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                    float3 lightDirectionWS = normalize(_LightPosition - positionWS);
                #else
                    float3 lightDirectionWS = _LightDirection;
                #endif
                
                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
                
                #if UNITY_REVERSED_Z
                    positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #else
                    positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #endif
                
                return positionCS;
            }
            
            ShadowVaryings ShadowPassVertex(ShadowAttributes input)
            {
                ShadowVaryings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.positionCS = GetShadowPositionHClip(input);
                return output;
            }
            
            half4 ShadowPassFragment(ShadowVaryings input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                #ifdef _ALPHATEST_ON
                    half alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a * _BaseColor.a;
                    clip(alpha - _Cutoff);
                #endif
                
                return 0;
            }
            ENDHLSL
        }
        
        // Depth Only Pass
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            
            ZWrite On
            ColorMask 0
            Cull [_Cull]
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local _VERTEX_ANIMATION
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Includes/GorgonizeToonProperties.hlsl"
            
            struct DepthAttributes
            {
                float4 position : POSITION;
                float3 normalOS : NORMAL;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct DepthVaryings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            DepthVaryings DepthOnlyVertex(DepthAttributes input)
            {
                DepthVaryings output = (DepthVaryings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.position.xyz);
                
                #ifdef _VERTEX_ANIMATION
                    VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                    float3 worldPos = positionInputs.positionWS;
                    float time = _Time.y * _AnimationSpeed * _VertexAnimationFrequency;
                    float3 animOffset = sin(worldPos * 0.1 + time) * _VertexAnimationIntensity;
                    positionInputs.positionWS += animOffset * normalInputs.normalWS;
                    positionInputs.positionCS = TransformWorldToHClip(positionInputs.positionWS);
                #endif
                
                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.positionCS = positionInputs.positionCS;
                return output;
            }
            
            half4 DepthOnlyFragment(DepthVaryings input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                #ifdef _ALPHATEST_ON
                    half alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a * _BaseColor.a;
                    clip(alpha - _Cutoff);
                #endif
                
                return 0;
            }
            ENDHLSL
        }
        
        // Depth Normals Pass
        Pass
        {
            Name "DepthNormals"
            Tags { "LightMode" = "DepthNormals" }
            
            ZWrite On
            Cull [_Cull]
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment
            
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local _VERTEX_ANIMATION
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Includes/GorgonizeToonProperties.hlsl"
            
            struct DepthNormalsAttributes
            {
                float4 positionOS : POSITION;
                float4 tangentOS : TANGENT;
                float3 normalOS : NORMAL;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct DepthNormalsVaryings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                
                #ifdef _NORMALMAP
                half4 tangentWS : TEXCOORD3;
                #endif
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            DepthNormalsVaryings DepthNormalsVertex(DepthNormalsAttributes input)
            {
                DepthNormalsVaryings output = (DepthNormalsVaryings)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                
                #ifdef _VERTEX_ANIMATION
                    float3 worldPos = positionInputs.positionWS;
                    float time = _Time.y * _AnimationSpeed * _VertexAnimationFrequency;
                    float3 animOffset = sin(worldPos * 0.1 + time) * _VertexAnimationIntensity;
                    positionInputs.positionWS += animOffset * normalInputs.normalWS;
                    positionInputs.positionCS = TransformWorldToHClip(positionInputs.positionWS);
                #endif
                
                output.positionCS = positionInputs.positionCS;
                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.normalWS = normalInputs.normalWS;
                
                #ifdef _NORMALMAP
                    real sign = input.tangentOS.w * GetOddNegativeScale();
                    half4 tangentWS = half4(normalInputs.tangentWS.xyz, sign);
                    output.tangentWS = tangentWS;
                #endif
                
                return output;
            }
            
            half4 DepthNormalsFragment(DepthNormalsVaryings input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                #ifdef _ALPHATEST_ON
                    half alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a * _BaseColor.a;
                    clip(alpha - _Cutoff);
                #endif
                
                float3 normalWS = input.normalWS;
                
                #ifdef _NORMALMAP
                    half4 normalMap = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv);
                    half3 normalTS = UnpackNormalScale(normalMap, _BumpScale);
                    half sgn = input.tangentWS.w;
                    half3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
                    half3x3 tangentToWorld = half3x3(input.tangentWS.xyz, bitangent, input.normalWS.xyz);
                    normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
                #endif
                
                normalWS = NormalizeNormalPerPixel(normalWS);
                return half4(PackNormalOctRectEncode(TransformWorldToViewDir(normalWS, true)), 0.0, 0.0);
            }
            ENDHLSL
        }
        
        // Meta Pass for Lightmapping
        Pass
        {
            Name "Meta"
            Tags { "LightMode" = "Meta" }
            
            Cull Off
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaLit
            
            #pragma shader_feature EDITOR_VISUALIZATION
            #pragma shader_feature_local_fragment _SPECULAR_SETUP
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            
            #pragma shader_feature_local_fragment _SPECGLOSSMAP
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UniversalMetaPass.hlsl"
            #include "Includes/GorgonizeToonProperties.hlsl"
            ENDHLSL
        }
        
        // Universal2D Pass
        Pass
        {
            Name "Universal2D"
            Tags { "LightMode" = "Universal2D" }
            
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            Cull [_Cull]
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHA_PREMULTIPLY_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Includes/GorgonizeToonProperties.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positionInputs.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                half2 uv = input.uv;
                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
                half3 color = texColor.rgb * _BaseColor.rgb;
                half alpha = texColor.a * _BaseColor.a;
                
                #ifdef _ALPHATEST_ON
                    clip(alpha - _Cutoff);
                #endif
                
                #ifdef _ALPHA_PREMULTIPLY_ON
                    color *= alpha;
                #endif
                
                return half4(color, alpha);
            }
            ENDHLSL
        }
    }
    
    // Fallback SubShader for older hardware
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "Geometry"
        }
        
        LOD 150
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            Cull [_Cull]
            
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex SimpleToonVertex
            #pragma fragment SimpleToonFragment
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Includes/GorgonizeToonProperties.hlsl"
            
            struct SimpleAttributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct SimpleVaryings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 shadowCoord : TEXCOORD2;
                float fogCoord : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            SimpleVaryings SimpleToonVertex(SimpleAttributes input)
            {
                SimpleVaryings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                
                output.positionHCS = positionInputs.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.normalWS = normalInputs.normalWS;
                output.shadowCoord = GetShadowCoord(positionInputs);
                output.fogCoord = ComputeFogFactor(output.positionHCS.z);
                
                return output;
            }
            
            half4 SimpleToonFragment(SimpleVaryings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                half4 albedoAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half3 albedo = albedoAlpha.rgb * _BaseColor.rgb;
                half alpha = albedoAlpha.a * _BaseColor.a;
                
                Light mainLight = GetMainLight(input.shadowCoord);
                half3 normalWS = normalize(input.normalWS);
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                
                // Simple toon ramp
                half toonRamp = smoothstep(_ShadowThreshold - _ShadowSmoothness, _ShadowThreshold + _ShadowSmoothness, NdotL);
                toonRamp *= mainLight.shadowAttenuation;
                
                half3 color = lerp(_ShadowColor.rgb * albedo, albedo, toonRamp);
                color *= mainLight.color;
                
                color = MixFog(color, input.fogCoord);
                
                return half4(color, alpha);
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "Gorgonize.ToonShader.Editor.GorgonizeToonShaderGUI"
}