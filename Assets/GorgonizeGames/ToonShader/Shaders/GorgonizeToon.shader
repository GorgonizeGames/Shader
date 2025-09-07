Shader "GorgonizeGames/Toon Shader"
{
    Properties
    {
        [Header(Base Settings)]
        _MainTex("Base Color (RGB) Alpha (A)", 2D) = "white" {}
        _Color("Base Color", Color) = (1,1,1,1)
        
        [Header(Lighting)]
        [Toggle(USE_RAMP_TEXTURE)] _UseRampTexture("Use Ramp Texture", Float) = 0
        _RampTex("Ramp Texture", 2D) = "white" {}
        _RampSteps("Ramp Steps", Range(2, 5)) = 3
        _RampSmoothness("Ramp Smoothness", Range(0, 1)) = 0.1
        _ShadowColor("Shadow Color", Color) = (0.5, 0.5, 0.5, 1)
        _HighlightColor("Highlight Color", Color) = (1, 1, 1, 1)
        _ShadowBlendMode("Shadow Blend Mode", Float) = 0
        
        [Header(Specular)]
        [Toggle(ENABLE_SPECULAR)] _EnableSpecular("Enable Specular", Float) = 0
        _SpecColor("Specular Color", Color) = (1, 1, 1, 1)
        _Glossiness("Glossiness", Range(0.01, 1)) = 0.5
        _SpecularSize("Specular Size", Range(0.001, 0.5)) = 0.05
        [Toggle(ENABLE_ANISOTROPIC)] _EnableAnisotropic("Enable Anisotropic", Float) = 0
        _AnisotropyDirection("Anisotropy Direction", Vector) = (1, 0, 0, 0)
        
        [Header(MatCap)]
        [Toggle(ENABLE_MATCAP)] _EnableMatCap("Enable MatCap", Float) = 0
        _MatCapTex("MatCap Texture", 2D) = "white" {}
        _MatCapIntensity("MatCap Intensity", Range(0, 2)) = 1
        
        [Header(Rim Lighting)]
        [Toggle(ENABLE_RIM)] _EnableRim("Enable Rim", Float) = 0
        _RimColor("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower("Rim Power", Range(0.1, 10)) = 2
        _RimIntensity("Rim Intensity", Range(0, 2)) = 1
        [Toggle(ENABLE_RIM_SECONDARY)] _EnableRimSecondary("Enable Secondary Rim", Float) = 0
        _RimColorSecondary("Secondary Rim Color", Color) = (0.5, 0.5, 1, 1)
        _RimPowerSecondary("Secondary Rim Power", Range(0.1, 10)) = 4
        _RimIntensitySecondary("Secondary Rim Intensity", Range(0, 2)) = 0.5
        
        [Header(Outline)]
        [Toggle(ENABLE_OUTLINE)] _EnableOutline("Enable Outline", Float) = 0
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width", Range(0, 0.01)) = 0.001
        _OutlineDistanceFade("Distance Fade", Range(0, 1)) = 0.5
        
        [Header(Shadows)]
        [Toggle(STYLIZED_SHADOWS)] _StylizedShadows("Stylized Shadows", Float) = 0
        _ShadowTint("Shadow Tint", Color) = (0.8, 0.8, 1, 1)
        _ShadowSharpness("Shadow Sharpness", Range(0, 1)) = 0.5
        [Toggle(SHADOW_DITHERING)] _ShadowDithering("Shadow Dithering", Float) = 0
        
        [Header(Effects)]
        [Toggle(ENABLE_HATCHING)] _EnableHatching("Enable Hatching", Float) = 0
        _HatchingTex("Hatching Texture", 2D) = "white" {}
        _HatchingScale("Hatching Scale", Range(0.1, 10)) = 1
        _HatchingIntensity("Hatching Intensity", Range(0, 2)) = 1
        
        [Toggle(ENABLE_DITHERING)] _EnableDithering("Enable Dithering", Float) = 0
        _DitheringScale("Dithering Scale", Range(1, 64)) = 8
        
        [Toggle(ENABLE_EMISSION)] _EnableEmission("Enable Emission", Float) = 0
        _EmissionMap("Emission Map", 2D) = "black" {}
        _EmissionColor("Emission Color", Color) = (0, 0, 0, 1)
        
        [Header(Advanced)]
        [Toggle(MOBILE_OPTIMIZED)] _MobileOptimized("Mobile Optimized", Float) = 0
        [Toggle(ADDITIONAL_LIGHTS)] _AdditionalLights("Additional Lights", Float) = 1
        
        // Hidden properties for ShaderGUI
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "Geometry"
        }
        
        // Forward Pass
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 3.0
            
            // Shader Features
            #pragma shader_feature_local USE_RAMP_TEXTURE
            #pragma shader_feature_local ENABLE_SPECULAR
            #pragma shader_feature_local ENABLE_ANISOTROPIC
            #pragma shader_feature_local ENABLE_MATCAP
            #pragma shader_feature_local ENABLE_RIM
            #pragma shader_feature_local ENABLE_RIM_SECONDARY
            #pragma shader_feature_local STYLIZED_SHADOWS
            #pragma shader_feature_local SHADOW_DITHERING
            #pragma shader_feature_local ENABLE_HATCHING
            #pragma shader_feature_local ENABLE_DITHERING
            #pragma shader_feature_local ENABLE_EMISSION
            #pragma shader_feature_local MOBILE_OPTIMIZED
            
            // Multi Compile Keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "GorgonizeToonCore.hlsl"

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
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        // Depth Only Pass
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }

    // Outline SubShader (Inverted Hull Method)
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "Geometry-1"
        }

        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            
            Cull Front
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma target 2.0
            #pragma shader_feature_local ENABLE_OUTLINE

            #pragma vertex OutlineVertex
            #pragma fragment OutlineFragment

            #include "Assets/GorgonizeGames/ToonShader/Shaders/Includes/GorgonizeToonOutline.hlsl"

            ENDHLSL
        }
    }

    CustomEditor "GorgonizeGames.ToonShaderGUI"
    Fallback "Universal Render Pipeline/Lit"
}