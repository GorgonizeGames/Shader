using UnityEngine;
using UnityEditor;
using System;

namespace GorgonizeGames.ToonShader
{
    /// <summary>
    /// Optimized Material Editor for Unity 6 URP
    /// </summary>
    public class ToonShaderGUI : ShaderGUI
    {
        // Foldout states (made non-static for Unity 6 compatibility)
        private bool showBaseSettings = true;
        private bool showLightingSettings = true;
        private bool showSpecularSettings = false;
        private bool showRimSettings = false;
        private bool showOutlineSettings = false;
        private bool showShadowSettings = false;
        private bool showEffectsSettings = false;
        private bool showEmissionSettings = false;
        private bool showRenderingSettings = false;

        // Property references
        private MaterialProperty baseMap;
        private MaterialProperty baseColor;
        
        // Lighting
        private MaterialProperty useRampShading;
        private MaterialProperty rampMap;
        private MaterialProperty rampSteps;
        private MaterialProperty shadowColor;
        private MaterialProperty highlightColor;
        private MaterialProperty shadowBlendMode;
        private MaterialProperty shadowIntensity;
        private MaterialProperty lightSmoothness;
        
        // Specular
        private MaterialProperty enableSpecular;
        private MaterialProperty specularColor;
        private MaterialProperty specularSize;
        private MaterialProperty specularSmoothness;
        private MaterialProperty anisotropicSpecular;
        private MaterialProperty anisotropy;
        private MaterialProperty useMatCap;
        private MaterialProperty matCapMap;
        
        // Rim Lighting
        private MaterialProperty enableRim;
        private MaterialProperty rimColor;
        private MaterialProperty rimPower;
        private MaterialProperty rimIntensity;
        private MaterialProperty enableSecondaryRim;
        private MaterialProperty secondaryRimColor;
        private MaterialProperty secondaryRimPower;
        private MaterialProperty secondaryRimIntensity;
        private MaterialProperty lightBasedRim;
        
        // Outline
        private MaterialProperty enableOutline;
        private MaterialProperty outlineWidth;
        private MaterialProperty outlineColor;
        private MaterialProperty outlineDistanceFade;
        private MaterialProperty cornerRounding;
        private MaterialProperty cornerRoundness;
        
        // Shadows
        private MaterialProperty stylizedShadows;
        private MaterialProperty shadowTint;
        private MaterialProperty shadowSharpness;
        private MaterialProperty shadowDithering;
        private MaterialProperty ditherScale;
        
        // Effects
        private MaterialProperty enableHatching;
        private MaterialProperty hatchingMap;
        private MaterialProperty hatchingScale;
        private MaterialProperty hatchingIntensity;
        private MaterialProperty enableDithering;
        private MaterialProperty colorDitherScale;
        
        // Emission
        private MaterialProperty enableEmission;
        private MaterialProperty emissionMap;
        private MaterialProperty emissionColor;
        
        // Mobile
        private MaterialProperty mobileMode;
        
        // Rendering
        private MaterialProperty cull;
        private MaterialProperty srcBlend;
        private MaterialProperty dstBlend;
        private MaterialProperty zWrite;
        private MaterialProperty zTest;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            // Find all properties
            FindProperties(properties);
            
            Material material = materialEditor.target as Material;
            
            // Header
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("GorgonizeGames Toon Shader", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Documentation", GUILayout.Width(100)))
                {
                    Application.OpenURL("https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest");
                }
            }
            GUILayout.Label("Unity 6 URP Optimized Toon Shader", EditorStyles.miniLabel);
            EditorGUILayout.Space();
            
            // Quick presets
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Mobile"))
                    ApplyMobilePreset(material);
                if (GUILayout.Button("Standard"))
                    ApplyStandardPreset(material);
                if (GUILayout.Button("High Quality"))
                    ApplyHighQualityPreset(material);
            }
            
            EditorGUILayout.Space();
            
            // Base Settings
            DrawBaseSettings(materialEditor, material);
            
            // Lighting Settings
            DrawLightingSettings(materialEditor, material);
            
            // Specular Settings
            DrawSpecularSettings(materialEditor, material);
            
            // Rim Lighting Settings
            DrawRimLightingSettings(materialEditor, material);
            
            // Outline Settings
            DrawOutlineSettings(materialEditor, material);
            
            // Shadow Settings
            DrawShadowSettings(materialEditor, material);
            
            // Effects Settings
            DrawEffectsSettings(materialEditor, material);
            
            // Emission Settings
            DrawEmissionSettings(materialEditor, material);
            
            // Rendering Settings
            DrawRenderingSettings(materialEditor, material);
            
            // Performance warnings
            DrawPerformanceWarnings(material);
            
            // Set shader keywords based on properties
            SetMaterialKeywords(material);
        }

        private void FindProperties(MaterialProperty[] properties)
        {
            baseMap = FindProperty("_BaseMap", properties, false);
            baseColor = FindProperty("_BaseColor", properties, false);
            
            useRampShading = FindProperty("_UseRampShading", properties, false);
            rampMap = FindProperty("_RampMap", properties, false);
            rampSteps = FindProperty("_RampSteps", properties, false);
            shadowColor = FindProperty("_ShadowColor", properties, false);
            highlightColor = FindProperty("_HighlightColor", properties, false);
            shadowBlendMode = FindProperty("_ShadowBlendMode", properties, false);
            shadowIntensity = FindProperty("_ShadowIntensity", properties, false);
            lightSmoothness = FindProperty("_LightSmoothness", properties, false);
            
            enableSpecular = FindProperty("_EnableSpecular", properties, false);
            specularColor = FindProperty("_SpecularColor", properties, false);
            specularSize = FindProperty("_SpecularSize", properties, false);
            specularSmoothness = FindProperty("_SpecularSmoothness", properties, false);
            anisotropicSpecular = FindProperty("_AnisotropicSpecular", properties, false);
            anisotropy = FindProperty("_Anisotropy", properties, false);
            useMatCap = FindProperty("_UseMatCap", properties, false);
            matCapMap = FindProperty("_MatCapMap", properties, false);
            
            enableRim = FindProperty("_EnableRim", properties, false);
            rimColor = FindProperty("_RimColor", properties, false);
            rimPower = FindProperty("_RimPower", properties, false);
            rimIntensity = FindProperty("_RimIntensity", properties, false);
            enableSecondaryRim = FindProperty("_EnableSecondaryRim", properties, false);
            secondaryRimColor = FindProperty("_SecondaryRimColor", properties, false);
            secondaryRimPower = FindProperty("_SecondaryRimPower", properties, false);
            secondaryRimIntensity = FindProperty("_SecondaryRimIntensity", properties, false);
            lightBasedRim = FindProperty("_LightBasedRim", properties, false);
            
            enableOutline = FindProperty("_EnableOutline", properties, false);
            outlineWidth = FindProperty("_OutlineWidth", properties, false);
            outlineColor = FindProperty("_OutlineColor", properties, false);
            outlineDistanceFade = FindProperty("_OutlineDistanceFade", properties, false);
            cornerRounding = FindProperty("_CornerRounding", properties, false);
            cornerRoundness = FindProperty("_CornerRoundness", properties, false);
            
            stylizedShadows = FindProperty("_StylizedShadows", properties, false);
            shadowTint = FindProperty("_ShadowTint", properties, false);
            shadowSharpness = FindProperty("_ShadowSharpness", properties, false);
            shadowDithering = FindProperty("_ShadowDithering", properties, false);
            ditherScale = FindProperty("_DitherScale", properties, false);
            
            enableHatching = FindProperty("_EnableHatching", properties, false);
            hatchingMap = FindProperty("_HatchingMap", properties, false);
            hatchingScale = FindProperty("_HatchingScale", properties, false);
            hatchingIntensity = FindProperty("_HatchingIntensity", properties, false);
            enableDithering = FindProperty("_EnableDithering", properties, false);
            colorDitherScale = FindProperty("_ColorDitherScale", properties, false);
            
            enableEmission = FindProperty("_EnableEmission", properties, false);
            emissionMap = FindProperty("_EmissionMap", properties, false);
            emissionColor = FindProperty("_EmissionColor", properties, false);
            
            mobileMode = FindProperty("_MobileMode", properties, false);
            
            cull = FindProperty("_Cull", properties, false);
            srcBlend = FindProperty("_SrcBlend", properties, false);
            dstBlend = FindProperty("_DstBlend", properties, false);
            zWrite = FindProperty("_ZWrite", properties, false);
            zTest = FindProperty("_ZTest", properties, false);
        }

        private void DrawBaseSettings(MaterialEditor materialEditor, Material material)
        {
            showBaseSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showBaseSettings, "Base Settings");
            if (showBaseSettings)
            {
                EditorGUI.indentLevel++;
                
                if (baseMap != null && baseColor != null)
                    materialEditor.TexturePropertySingleLine(new GUIContent("Base Map"), baseMap, baseColor);
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawLightingSettings(MaterialEditor materialEditor, Material material)
        {
            showLightingSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showLightingSettings, "Lighting Settings");
            if (showLightingSettings)
            {
                EditorGUI.indentLevel++;
                
                if (useRampShading != null)
                {
                    materialEditor.ShaderProperty(useRampShading, "Use Ramp Shading");
                    
                    if (useRampShading.floatValue > 0.5f)
                    {
                        if (rampMap != null)
                        {
                            materialEditor.TexturePropertySingleLine(new GUIContent("Ramp Map"), rampMap);
                            EditorGUILayout.HelpBox("Use a 1D ramp texture for custom lighting transitions.", MessageType.Info);
                        }
                    }
                    else
                    {
                        if (rampSteps != null)
                        {
                            materialEditor.ShaderProperty(rampSteps, "Ramp Steps (Procedural)");
                            EditorGUILayout.HelpBox("Procedural gradient with 2-5 color steps.", MessageType.Info);
                        }
                    }
                }
                
                if (shadowColor != null) materialEditor.ShaderProperty(shadowColor, "Shadow Color");
                if (highlightColor != null) materialEditor.ShaderProperty(highlightColor, "Highlight Color");
                if (shadowBlendMode != null) materialEditor.ShaderProperty(shadowBlendMode, "Shadow Blend Mode");
                if (shadowIntensity != null) materialEditor.ShaderProperty(shadowIntensity, "Shadow Intensity");
                if (lightSmoothness != null) materialEditor.ShaderProperty(lightSmoothness, "Light Smoothness");
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawSpecularSettings(MaterialEditor materialEditor, Material material)
        {
            showSpecularSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showSpecularSettings, "Specular Settings");
            if (showSpecularSettings)
            {
                EditorGUI.indentLevel++;
                
                if (enableSpecular != null)
                {
                    materialEditor.ShaderProperty(enableSpecular, "Enable Specular");
                    
                    if (enableSpecular.floatValue > 0.5f)
                    {
                        if (specularColor != null) materialEditor.ShaderProperty(specularColor, "Specular Color");
                        if (specularSize != null) materialEditor.ShaderProperty(specularSize, "Specular Size");
                        if (specularSmoothness != null) materialEditor.ShaderProperty(specularSmoothness, "Specular Smoothness");
                        
                        if (anisotropicSpecular != null)
                        {
                            materialEditor.ShaderProperty(anisotropicSpecular, "Anisotropic Specular");
                            if (anisotropicSpecular.floatValue > 0.5f && anisotropy != null)
                            {
                                materialEditor.ShaderProperty(anisotropy, "Anisotropy");
                                EditorGUILayout.HelpBox("Great for hair and fabric materials.", MessageType.Info);
                            }
                        }
                        
                        if (useMatCap != null)
                        {
                            materialEditor.ShaderProperty(useMatCap, "Use MatCap");
                            if (useMatCap.floatValue > 0.5f && matCapMap != null)
                            {
                                materialEditor.TexturePropertySingleLine(new GUIContent("MatCap Map"), matCapMap);
                                EditorGUILayout.HelpBox("MatCap provides fake reflections for better performance.", MessageType.Info);
                            }
                        }
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawRimLightingSettings(MaterialEditor materialEditor, Material material)
        {
            showRimSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showRimSettings, "Rim Lighting Settings");
            if (showRimSettings)
            {
                EditorGUI.indentLevel++;
                
                if (enableRim != null)
                {
                    materialEditor.ShaderProperty(enableRim, "Enable Rim Lighting");
                    
                    if (enableRim.floatValue > 0.5f)
                    {
                        if (rimColor != null) materialEditor.ShaderProperty(rimColor, "Primary Rim Color");
                        if (rimPower != null) materialEditor.ShaderProperty(rimPower, "Primary Rim Power");
                        if (rimIntensity != null) materialEditor.ShaderProperty(rimIntensity, "Primary Rim Intensity");
                        
                        if (enableSecondaryRim != null)
                        {
                            materialEditor.ShaderProperty(enableSecondaryRim, "Enable Secondary Rim");
                            if (enableSecondaryRim.floatValue > 0.5f)
                            {
                                if (secondaryRimColor != null) materialEditor.ShaderProperty(secondaryRimColor, "Secondary Rim Color");
                                if (secondaryRimPower != null) materialEditor.ShaderProperty(secondaryRimPower, "Secondary Rim Power");
                                if (secondaryRimIntensity != null) materialEditor.ShaderProperty(secondaryRimIntensity, "Secondary Rim Intensity");
                            }
                        }
                        
                        if (lightBasedRim != null)
                        {
                            materialEditor.ShaderProperty(lightBasedRim, "Light Based Rim");
                            if (lightBasedRim.floatValue > 0.5f)
                            {
                                EditorGUILayout.HelpBox("Rim lighting will be influenced by light direction.", MessageType.Info);
                            }
                        }
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawOutlineSettings(MaterialEditor materialEditor, Material material)
        {
            showOutlineSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showOutlineSettings, "Outline Settings");
            if (showOutlineSettings)
            {
                EditorGUI.indentLevel++;
                
                if (enableOutline != null)
                {
                    materialEditor.ShaderProperty(enableOutline, "Enable Outline");
                    
                    if (enableOutline.floatValue > 0.5f)
                    {
                        if (outlineWidth != null) materialEditor.ShaderProperty(outlineWidth, "Outline Width");
                        if (outlineColor != null) materialEditor.ShaderProperty(outlineColor, "Outline Color");
                        if (outlineDistanceFade != null) materialEditor.ShaderProperty(outlineDistanceFade, "Distance Fade");
                        
                        if (cornerRounding != null)
                        {
                            materialEditor.ShaderProperty(cornerRounding, "Corner Rounding");
                            if (cornerRounding.floatValue > 0.5f && cornerRoundness != null)
                            {
                                materialEditor.ShaderProperty(cornerRoundness, "Corner Roundness");
                            }
                        }
                        
                        EditorGUILayout.HelpBox("Uses Inverted Hull technique for consistent outline rendering.", MessageType.Info);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawShadowSettings(MaterialEditor materialEditor, Material material)
        {
            showShadowSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showShadowSettings, "Shadow Settings");
            if (showShadowSettings)
            {
                EditorGUI.indentLevel++;
                
                if (stylizedShadows != null)
                {
                    materialEditor.ShaderProperty(stylizedShadows, "Stylized Shadows");
                    
                    if (stylizedShadows.floatValue > 0.5f)
                    {
                        if (shadowTint != null) materialEditor.ShaderProperty(shadowTint, "Shadow Tint");
                        if (shadowSharpness != null) materialEditor.ShaderProperty(shadowSharpness, "Shadow Sharpness");
                        
                        if (shadowDithering != null)
                        {
                            materialEditor.ShaderProperty(shadowDithering, "Shadow Dithering");
                            if (shadowDithering.floatValue > 0.5f && ditherScale != null)
                            {
                                materialEditor.ShaderProperty(ditherScale, "Dither Scale");
                                EditorGUILayout.HelpBox("Dithering creates smoother shadow transitions.", MessageType.Info);
                            }
                        }
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawEffectsSettings(MaterialEditor materialEditor, Material material)
        {
            showEffectsSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showEffectsSettings, "Effects Settings");
            if (showEffectsSettings)
            {
                EditorGUI.indentLevel++;
                
                if (enableHatching != null)
                {
                    materialEditor.ShaderProperty(enableHatching, "Enable Hatching");
                    if (enableHatching.floatValue > 0.5f)
                    {
                        if (hatchingMap != null) materialEditor.TexturePropertySingleLine(new GUIContent("Hatching Map"), hatchingMap);
                        if (hatchingScale != null) materialEditor.ShaderProperty(hatchingScale, "Hatching Scale");
                        if (hatchingIntensity != null) materialEditor.ShaderProperty(hatchingIntensity, "Hatching Intensity");
                        EditorGUILayout.HelpBox("Use RGB channels for different hatching densities.", MessageType.Info);
                    }
                }
                
                if (enableDithering != null)
                {
                    materialEditor.ShaderProperty(enableDithering, "Enable Color Dithering");
                    if (enableDithering.floatValue > 0.5f && colorDitherScale != null)
                    {
                        materialEditor.ShaderProperty(colorDitherScale, "Color Dither Scale");
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawEmissionSettings(MaterialEditor materialEditor, Material material)
        {
            showEmissionSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showEmissionSettings, "Emission Settings");
            if (showEmissionSettings)
            {
                EditorGUI.indentLevel++;
                
                if (enableEmission != null)
                {
                    materialEditor.ShaderProperty(enableEmission, "Enable Emission");
                    
                    if (enableEmission.floatValue > 0.5f)
                    {
                        if (emissionMap != null) materialEditor.TexturePropertySingleLine(new GUIContent("Emission Map"), emissionMap);
                        if (emissionColor != null) materialEditor.ShaderProperty(emissionColor, "Emission Color");
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawRenderingSettings(MaterialEditor materialEditor, Material material)
        {
            showRenderingSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showRenderingSettings, "Rendering Settings");
            if (showRenderingSettings)
            {
                EditorGUI.indentLevel++;
                
                if (mobileMode != null)
                {
                    materialEditor.ShaderProperty(mobileMode, "Mobile Mode");
                    if (mobileMode.floatValue > 0.5f)
                    {
                        EditorGUILayout.HelpBox("Mobile mode reduces shader complexity for better performance.", MessageType.Info);
                    }
                }
                
                EditorGUILayout.Space();
                
                if (cull != null) materialEditor.ShaderProperty(cull, "Cull Mode");
                if (srcBlend != null) materialEditor.ShaderProperty(srcBlend, "Src Blend");
                if (dstBlend != null) materialEditor.ShaderProperty(dstBlend, "Dst Blend");
                if (zWrite != null) materialEditor.ShaderProperty(zWrite, "ZWrite");
                if (zTest != null) materialEditor.ShaderProperty(zTest, "ZTest");
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawPerformanceWarnings(Material material)
        {
            int activeFeatures = 0;
            
            if (enableSpecular != null && enableSpecular.floatValue > 0.5f) activeFeatures++;
            if (enableRim != null && enableRim.floatValue > 0.5f) activeFeatures++;
            if (enableOutline != null && enableOutline.floatValue > 0.5f) activeFeatures++;
            if (enableHatching != null && enableHatching.floatValue > 0.5f) activeFeatures++;
            if (enableDithering != null && enableDithering.floatValue > 0.5f) activeFeatures++;
            if (anisotropicSpecular != null && anisotropicSpecular.floatValue > 0.5f) activeFeatures++;
            if (shadowDithering != null && shadowDithering.floatValue > 0.5f) activeFeatures++;
            
            if (activeFeatures > 4)
            {
                EditorGUILayout.HelpBox($"Warning: {activeFeatures} features are active. Consider disabling some for better performance.", MessageType.Warning);
            }
            
            if (mobileMode != null && mobileMode.floatValue > 0.5f && activeFeatures > 2)
            {
                EditorGUILayout.HelpBox("Mobile mode is enabled but multiple expensive features are active. Consider using a mobile preset.", MessageType.Warning);
            }
        }

        private void ApplyMobilePreset(Material material)
        {
            // Enable mobile mode
            if (mobileMode != null) material.SetFloat("_MobileMode", 1.0f);
            
            // Disable expensive features
            if (enableSpecular != null) material.SetFloat("_EnableSpecular", 0.0f);
            if (enableSecondaryRim != null) material.SetFloat("_EnableSecondaryRim", 0.0f);
            if (shadowDithering != null) material.SetFloat("_ShadowDithering", 0.0f);
            if (enableHatching != null) material.SetFloat("_EnableHatching", 0.0f);
            if (enableDithering != null) material.SetFloat("_EnableDithering", 0.0f);
            if (cornerRounding != null) material.SetFloat("_CornerRounding", 0.0f);
            if (anisotropicSpecular != null) material.SetFloat("_AnisotropicSpecular", 0.0f);
            if (useMatCap != null) material.SetFloat("_UseMatCap", 0.0f);
            
            // Reduce outline width
            if (enableOutline != null && material.GetFloat("_EnableOutline") > 0.5f)
            {
                if (outlineWidth != null) material.SetFloat("_OutlineWidth", Mathf.Min(material.GetFloat("_OutlineWidth"), 0.005f));
            }
            
            // Simplify lighting
            if (useRampShading != null) material.SetFloat("_UseRampShading", 0.0f);
            if (rampSteps != null) material.SetFloat("_RampSteps", 3.0f);
            
            EditorUtility.SetDirty(material);
            Debug.Log("Mobile preset applied to " + material.name);
        }

        private void ApplyStandardPreset(Material material)
        {
            if (mobileMode != null) material.SetFloat("_MobileMode", 0.0f);
            if (enableSpecular != null) material.SetFloat("_EnableSpecular", 1.0f);
            if (enableRim != null) material.SetFloat("_EnableRim", 1.0f);
            if (enableOutline != null) material.SetFloat("_EnableOutline", 1.0f);
            if (stylizedShadows != null) material.SetFloat("_StylizedShadows", 1.0f);
            
            // Disable most expensive features
            if (anisotropicSpecular != null) material.SetFloat("_AnisotropicSpecular", 0.0f);
            if (shadowDithering != null) material.SetFloat("_ShadowDithering", 0.0f);
            if (enableHatching != null) material.SetFloat("_EnableHatching", 0.0f);
            
            EditorUtility.SetDirty(material);
            Debug.Log("Standard preset applied to " + material.name);
        }

        private void ApplyHighQualityPreset(Material material)
        {
            if (mobileMode != null) material.SetFloat("_MobileMode", 0.0f);
            if (enableSpecular != null) material.SetFloat("_EnableSpecular", 1.0f);
            if (enableRim != null) material.SetFloat("_EnableRim", 1.0f);
            if (enableSecondaryRim != null) material.SetFloat("_EnableSecondaryRim", 1.0f);
            if (enableOutline != null) material.SetFloat("_EnableOutline", 1.0f);
            if (stylizedShadows != null) material.SetFloat("_StylizedShadows", 1.0f);
            if (shadowDithering != null) material.SetFloat("_ShadowDithering", 1.0f);
            if (anisotropicSpecular != null) material.SetFloat("_AnisotropicSpecular", 1.0f);
            
            EditorUtility.SetDirty(material);
            Debug.Log("High quality preset applied to " + material.name);
        }

        private void SetMaterialKeywords(Material material)
        {
            // Lighting keywords
            SetKeyword(material, "_USE_RAMP_SHADING", useRampShading != null && material.GetFloat("_UseRampShading") > 0.5f);
            
            // Specular keywords
            SetKeyword(material, "_ENABLE_SPECULAR", enableSpecular != null && material.GetFloat("_EnableSpecular") > 0.5f);
            SetKeyword(material, "_ANISOTROPIC_SPECULAR", anisotropicSpecular != null && material.GetFloat("_AnisotropicSpecular") > 0.5f);
            SetKeyword(material, "_USE_MATCAP", useMatCap != null && material.GetFloat("_UseMatCap") > 0.5f);
            
            // Rim lighting keywords
            SetKeyword(material, "_ENABLE_RIM", enableRim != null && material.GetFloat("_EnableRim") > 0.5f);
            SetKeyword(material, "_ENABLE_SECONDARY_RIM", enableSecondaryRim != null && material.GetFloat("_EnableSecondaryRim") > 0.5f);
            SetKeyword(material, "_LIGHT_BASED_RIM", lightBasedRim != null && material.GetFloat("_LightBasedRim") > 0.5f);
            
            // Outline keywords
            SetKeyword(material, "_ENABLE_OUTLINE", enableOutline != null && material.GetFloat("_EnableOutline") > 0.5f);
            SetKeyword(material, "_CORNER_ROUNDING", cornerRounding != null && material.GetFloat("_CornerRounding") > 0.5f);
            
            // Shadow keywords
            SetKeyword(material, "_STYLIZED_SHADOWS", stylizedShadows != null && material.GetFloat("_StylizedShadows") > 0.5f);
            SetKeyword(material, "_SHADOW_DITHERING", shadowDithering != null && material.GetFloat("_ShadowDithering") > 0.5f);
            
            // Effects keywords
            SetKeyword(material, "_ENABLE_HATCHING", enableHatching != null && material.GetFloat("_EnableHatching") > 0.5f);
            SetKeyword(material, "_ENABLE_DITHERING", enableDithering != null && material.GetFloat("_EnableDithering") > 0.5f);
            
            // Emission keywords
            SetKeyword(material, "_EMISSION", enableEmission != null && material.GetFloat("_EnableEmission") > 0.5f);
            
            // Mobile keywords
            SetKeyword(material, "_MOBILE_MODE", mobileMode != null && material.GetFloat("_MobileMode") > 0.5f);
        }

        private void SetKeyword(Material material, string keyword, bool state)
        {
            if (state)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }
    }
}