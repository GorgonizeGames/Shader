Shader "GorgonizeGames/Toon Shader"
{
    Properties
    {
        [Header(Base Color)]
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
        
        [Header(Lighting)]
        [Toggle(_USE_RAMP_SHADING)] _UseRampShading("Use Ramp Shading", Float) = 0
        [NoScaleOffset] _RampMap("Ramp Map", 2D) = "white" {}
        [IntRange] _RampSteps("Ramp Steps (Procedural)", Range(2, 5)) = 3
        _ShadowColor("Shadow Color", Color) = (0.5, 0.5, 0.8, 1)
        _HighlightColor("Highlight Color", Color) = (1, 1, 1, 1)
        [Enum(Multiply, 0, Additive, 1, Replace, 2)] _ShadowBlendMode("Shadow Blend Mode", Float) = 0
        _ShadowIntensity("Shadow Intensity", Range(0, 1)) = 0.8
        _LightSmoothness("Light Smoothness", Range(0, 1)) = 0.1
        
        [Header(Specular)]
        [Toggle(_ENABLE_SPECULAR)] _EnableSpecular("Enable Specular", Float) = 0
        _SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularSize("Specular Size", Range(0.001, 1)) = 0.1
        _SpecularSmoothness("Specular Smoothness", Range(0, 1)) = 0.5
        [Toggle(_ANISOTROPIC_SPECULAR)] _AnisotropicSpecular("Anisotropic Specular", Float) = 0
        _Anisotropy("Anisotropy", Range(-1, 1)) = 0
        [Toggle(_USE_MATCAP)] _UseMatCap("Use MatCap", Float) = 0
        [NoScaleOffset] _MatCapMap("MatCap Map", 2D) = "black" {}
        
        [Header(Rim Lighting)]
        [Toggle(_ENABLE_RIM)] _EnableRim("Enable Rim Lighting", Float) = 0
        _RimColor("Primary Rim Color", Color) = (1, 1, 1, 1)
        _RimPower("Primary Rim Power", Range(0.1, 10)) = 2
        _RimIntensity("Primary Rim Intensity", Range(0, 5)) = 1
        [Toggle(_ENABLE_SECONDARY_RIM)] _EnableSecondaryRim("Enable Secondary Rim", Float) = 0
        _SecondaryRimColor("Secondary Rim Color", Color) = (0.5, 0.5, 1, 1)
        _SecondaryRimPower("Secondary Rim Power", Range(0.1, 10)) = 4
        _SecondaryRimIntensity("Secondary Rim Intensity", Range(0, 5)) = 0.5
        [Toggle(_LIGHT_BASED_RIM)] _LightBasedRim("Light Based Rim", Float) = 0
        
        [Header(Outline)]
        [Toggle(_ENABLE_OUTLINE)] _EnableOutline("Enable Outline", Float) = 0
        _OutlineWidth("Outline Width", Range(0, 0.1)) = 0.01
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineDistanceFade("Distance Fade", Range(0, 1)) = 0.5
        [Toggle(_CORNER_ROUNDING)] _CornerRounding("Corner Rounding", Float) = 0
        _CornerRoundness("Corner Roundness", Range(0, 1)) = 0.5
        
        [Header(Shadows)]
        [Toggle(_STYLIZED_SHADOWS)] _StylizedShadows("Stylized Shadows", Float) = 1
        _ShadowTint("Shadow Tint", Color) = (0.7, 0.7, 1, 1)
        _ShadowSharpness("Shadow Sharpness", Range(0, 1)) = 0.5
        [Toggle(_SHADOW_DITHERING)] _ShadowDithering("Shadow Dithering", Float) = 0
        _DitherScale("Dither Scale", Range(1, 100)) = 10
        
        [Header(Effects)]
        [Toggle(_ENABLE_HATCHING)] _EnableHatching("Enable Hatching", Float) = 0
        [NoScaleOffset] _HatchingMap("Hatching Map", 2D) = "white" {}
        _HatchingScale("Hatching Scale", Range(0.1, 10)) = 1
        _HatchingIntensity("Hatching Intensity", Range(0, 1)) = 0.5
        [Toggle(_ENABLE_DITHERING)] _EnableDithering("Enable Color Dithering", Float) = 0
        _ColorDitherScale("Color Dither Scale", Range(1, 100)) = 20
        
        [Header(Emission)]
        [Toggle(_EMISSION)] _EnableEmission("Enable Emission", Float) = 0
        [NoScaleOffset] _EmissionMap("Emission Map", 2D) = "white" {}
        [HDR] _EmissionColor("Emission Color", Color) = (0, 0, 0, 1)
        
        [Header(Mobile Optimizations)]
        [Toggle(_MOBILE_MODE)] _MobileMode("Mobile Mode", Float) = 0
        
        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend", Float) = 0
        [Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque" 
            "Queue" = "Geometry"
            "UniversalMaterialType" = "Lit"
        }
        LOD 300
        
        // Main Toon Pass
        Pass
        {
            Name "ToonForward"
            Tags { "LightMode" = "UniversalForward" }
            
            Cull [_Cull]
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            
            HLSLPROGRAM
            #pragma target 3.0
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            
            // Unity Keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            
            // Custom Shader Features
            #pragma shader_feature_local _USE_RAMP_SHADING
            #pragma shader_feature_local _ENABLE_SPECULAR
            #pragma shader_feature_local _ANISOTROPIC_SPECULAR
            #pragma shader_feature_local _USE_MATCAP
            #pragma shader_feature_local _ENABLE_RIM
            #pragma shader_feature_local _ENABLE_SECONDARY_RIM
            #pragma shader_feature_local _LIGHT_BASED_RIM
            #pragma shader_feature_local _STYLIZED_SHADOWS
            #pragma shader_feature_local _SHADOW_DITHERING
            #pragma shader_feature_local _ENABLE_HATCHING
            #pragma shader_feature_local _ENABLE_DITHERING
            #pragma shader_feature_local _EMISSION
            #pragma shader_feature_local _MOBILE_MODE
            
            #pragma vertex ToonVertex
            #pragma fragment ToonFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Assets/GorgonizeGames/ToonShader/Shaders/Include/ToonForwardPass.hlsl"
            
            ENDHLSL
        }
        
        // Outline Pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            
            Cull Front
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            
            HLSLPROGRAM
            #pragma target 3.0
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            
            #pragma shader_feature_local _ENABLE_OUTLINE
            #pragma shader_feature_local _CORNER_ROUNDING
            #pragma shader_feature_local _MOBILE_MODE
            
            #pragma vertex OutlineVertex
            #pragma fragment OutlineFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/GorgonizeGames/ToonShader/Shaders/Include/OutlinePass.hlsl"
            
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
            #pragma target 3.0
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Assets/GorgonizeGames/ToonShader/Shaders/Include/ShadowCasterPass.hlsl"
            
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
            #pragma target 3.0
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/GorgonizeGames/ToonShader/Shaders/Include/DepthOnlyPass.hlsl"
            
            ENDHLSL
        }
    }
    
    Fallback "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "GorgonizeGames.ToonShader.ToonShaderGUI"
}
