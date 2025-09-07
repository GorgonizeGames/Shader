using UnityEngine;
using UnityEditor;
using System.IO;
using Gorgonize.ToonShader.Presets;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Main shader inspector GUI for Ultimate Toon Shader Pro
    /// Provides a comprehensive interface for all shader properties
    /// </summary>
    public class ToonShaderGUI : ShaderGUI
    {
        private static Texture2D logoTexture;
        private static bool stylesInitialized = false;
        
        // Material Properties - organized by category
        private MaterialProperty[] allProperties;
        
        // Base Properties
        private MaterialProperty baseMap;
        private MaterialProperty baseColor;
        private MaterialProperty saturation;
        private MaterialProperty brightness;
        
        // Lighting Properties
        private MaterialProperty shadowThreshold;
        private MaterialProperty shadowSmoothness;
        private MaterialProperty shadowColor;
        private MaterialProperty shadowIntensity;
        private MaterialProperty lightRampTex;
        private MaterialProperty useRampTexture;
        private MaterialProperty indirectLightingBoost;
        private MaterialProperty ambientOcclusion;
        private MaterialProperty lightWrapping;
        
        // Visual Effects Properties
        private MaterialProperty enableRimLighting;
        private MaterialProperty rimColor;
        private MaterialProperty rimPower;
        private MaterialProperty rimIntensity;
        private MaterialProperty rimThreshold;
        
        private MaterialProperty enableSpecular;
        private MaterialProperty specularColor;
        private MaterialProperty specularSize;
        private MaterialProperty specularSmoothness;
        private MaterialProperty specularIntensity;
        
        // Hatching Properties
        private MaterialProperty enableHatching;
        private MaterialProperty hatchingTex;
        private MaterialProperty crossHatchingTex;
        private MaterialProperty hatchingDensity;
        private MaterialProperty hatchingIntensity;
        private MaterialProperty hatchingThreshold;
        private MaterialProperty crossHatchingThreshold;
        private MaterialProperty hatchingRotation;
        private MaterialProperty enableScreenSpaceHatching;
        private MaterialProperty screenHatchScale;
        private MaterialProperty screenHatchBias;
        
        // Other Effects
        private MaterialProperty enableMatcap;
        private MaterialProperty matcapTex;
        private MaterialProperty matcapIntensity;
        private MaterialProperty matcapBlendMode;
        
        private MaterialProperty enableNormalMap;
        private MaterialProperty bumpMap;
        private MaterialProperty bumpScale;
        
        private MaterialProperty enableEmission;
        private MaterialProperty emissionMap;
        private MaterialProperty emissionColor;
        private MaterialProperty emissionIntensity;
        private MaterialProperty emissionScrollSpeed;
        
        private MaterialProperty enableFresnel;
        private MaterialProperty fresnelColor;
        private MaterialProperty fresnelPower;
        private MaterialProperty fresnelIntensity;
        
        private MaterialProperty enableSubsurface;
        private MaterialProperty subsurfaceColor;
        private MaterialProperty subsurfacePower;
        private MaterialProperty subsurfaceIntensity;
        
        private MaterialProperty enableOutline;
        private MaterialProperty outlineColor;
        private MaterialProperty outlineWidth;
        
        // Color Grading
        private MaterialProperty hue;
        private MaterialProperty contrast;
        private MaterialProperty gamma;
        
        // Stylization
        private MaterialProperty enablePosterize;
        private MaterialProperty posterizeLevels;
        private MaterialProperty enableCelShading;
        private MaterialProperty celShadingSteps;
        
        // Advanced
        private MaterialProperty cutoff;
        private MaterialProperty cull;
        private MaterialProperty zwrite;
        private MaterialProperty ztest;
        
        // Foldout states - persistent across inspector redraws
        private static bool showBase = true;
        private static bool showLighting = true;
        private static bool showAdvancedLighting = false;
        private static bool showRimLighting = false;
        private static bool showSpecular = false;
        private static bool showHatching = false;
        private static bool showMatcap = false;
        private static bool showNormalMap = false;
        private static bool showEmission = false;
        private static bool showFresnel = false;
        private static bool showSubsurface = false;
        private static bool showOutline = false;
        private static bool showColorGrading = false;
        private static bool showStylization = false;
        private static bool showAdvanced = false;
        private static bool showPerformanceInfo = false;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            allProperties = properties;
            FindProperties();
            
            Material material = materialEditor.target as Material;
            
            if (!stylesInitialized)
            {
                ToonGUIStyles.RefreshStyles();
                stylesInitialized = true;
            }
            
            EditorGUILayout.Space();
            DrawHeader();
            EditorGUILayout.Space();
            
            // Preset System
            DrawPresetSystem(materialEditor, material);
            ToonGUIStyles.DrawSeparator();
            
            // Performance Info
            DrawPerformanceSection(material);
            ToonGUIStyles.DrawSeparator();
            
            // Main Properties Sections
            DrawBaseProperties(materialEditor);
            DrawLightingSection(materialEditor, material);
            DrawVisualEffectsSection(materialEditor, material);
            DrawStylizationSection(materialEditor, material);
            DrawAdvancedSection(materialEditor, material);
            
            EditorGUILayout.Space();
            DrawFooter();
        }
        
        private void DrawHatchingSection(MaterialEditor materialEditor, Material material)
        {
            if (enableHatching == null) return;
            
            showHatching = ToonGUIStyles.DrawStyledFoldout(showHatching, "Hatching Effects", "🖊️");
            
            if (showHatching)
            {
                ToonGUIStyles.BeginStyledBox();
                
                materialEditor.ShaderProperty(enableHatching, "Enable Hatching");
                SetKeywordSafe(material, "_HATCHING", enableHatching.floatValue > 0);
                
                if (enableHatching.floatValue > 0)
                {
                    EditorGUI.indentLevel++;
                    
                    // Textures
                    if (hatchingTex != null)
                        materialEditor.TexturePropertySingleLine(new GUIContent("Hatching Texture", "Main hatching pattern"), hatchingTex);
                    if (crossHatchingTex != null)
                        materialEditor.TexturePropertySingleLine(new GUIContent("Cross Hatching Texture", "Cross hatching for deeper shadows"), crossHatchingTex);
                    
                    ToonGUIStyles.AddVerticalSpace(3f);
                    
                    // Parameters
                    if (hatchingDensity != null)
                        materialEditor.RangeProperty(hatchingDensity, "Hatching Density");
                    if (hatchingIntensity != null)
                        materialEditor.RangeProperty(hatchingIntensity, "Hatching Intensity");
                    if (hatchingThreshold != null)
                        materialEditor.RangeProperty(hatchingThreshold, "Hatching Threshold");
                    if (crossHatchingThreshold != null)
                        materialEditor.RangeProperty(crossHatchingThreshold, "Cross Hatching Threshold");
                    if (hatchingRotation != null)
                        materialEditor.RangeProperty(hatchingRotation, "Hatching Rotation");
                    
                    ToonGUIStyles.AddVerticalSpace(5f);
                    
                    // Screen space hatching
                    if (enableScreenSpaceHatching != null)
                    {
                        materialEditor.ShaderProperty(enableScreenSpaceHatching, "Enable Screen Space Hatching");
                        SetKeywordSafe(material, "_SCREEN_SPACE_HATCHING", enableScreenSpaceHatching.floatValue > 0);
                        
                        if (enableScreenSpaceHatching.floatValue > 0)
                        {
                            EditorGUI.indentLevel++;
                            if (screenHatchScale != null)
                                materialEditor.RangeProperty(screenHatchScale, "Screen Hatch Scale");
                            if (screenHatchBias != null)
                                materialEditor.RangeProperty(screenHatchBias, "Screen Hatch Bias");
                            EditorGUI.indentLevel--;
                        }
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                ToonGUIStyles.DrawInfoBox("Hatching adds sketch-like crosshatch patterns based on lighting. Lower thresholds = more hatching in dark areas.", MessageType.Info);
                ToonGUIStyles.EndStyledBox();
            }
        }
        
        private void DrawMatcapSection(MaterialEditor materialEditor, Material material)
        {
            if (enableMatcap == null) return;
            
            showMatcap = ToonGUIStyles.DrawStyledFoldout(showMatcap, "Matcap", "🎭");
            
            if (showMatcap)
            {
                ToonGUIStyles.BeginStyledBox();
                
                materialEditor.ShaderProperty(enableMatcap, "Enable Matcap");
                SetKeywordSafe(material, "_MATCAP", enableMatcap.floatValue > 0);
                
                if (enableMatcap.floatValue > 0)
                {
                    EditorGUI.indentLevel++;
                    if (matcapTex != null)
                        materialEditor.TexturePropertySingleLine(new GUIContent("Matcap Texture"), matcapTex);
                    if (matcapIntensity != null)
                        materialEditor.RangeProperty(matcapIntensity, "Matcap Intensity");
                    
                    if (matcapBlendMode != null)
                    {
                        string[] blendModes = {"Add", "Multiply", "Screen"};
                        int blendMode = (int)matcapBlendMode.floatValue;
                        blendMode = EditorGUILayout.Popup("Blend Mode", blendMode, blendModes);
                        matcapBlendMode.floatValue = blendMode;
                    }
                    EditorGUI.indentLevel--;
                }
                
                ToonGUIStyles.EndStyledBox();
            }
        }
        
        private void DrawFresnelSection(MaterialEditor materialEditor, Material material)
        {
            if (enableFresnel == null) return;
            
            showFresnel = ToonGUIStyles.DrawStyledFoldout(showFresnel, "Fresnel Effect", "🌀");
            
            if (showFresnel)
            {
                ToonGUIStyles.BeginStyledBox();
                
                materialEditor.ShaderProperty(enableFresnel, "Enable Fresnel");
                SetKeywordSafe(material, "_FRESNEL", enableFresnel.floatValue > 0);
                
                if (enableFresnel.floatValue > 0)
                {
                    EditorGUI.indentLevel++;
                    if (fresnelColor != null)
                        materialEditor.ColorProperty(fresnelColor, "Fresnel Color");
                    if (fresnelPower != null)
                        materialEditor.RangeProperty(fresnelPower, "Fresnel Power");
                    if (fresnelIntensity != null)
                        materialEditor.RangeProperty(fresnelIntensity, "Fresnel Intensity");
                    EditorGUI.indentLevel--;
                }
                
                ToonGUIStyles.EndStyledBox();
            }
        }
        
        private void DrawSubsurfaceSection(MaterialEditor materialEditor, Material material)
        {
            if (enableSubsurface == null) return;
            
            showSubsurface = ToonGUIStyles.DrawStyledFoldout(showSubsurface, "Subsurface Scattering", "🌸");
            
            if (showSubsurface)
            {
                ToonGUIStyles.BeginStyledBox();
                
                materialEditor.ShaderProperty(enableSubsurface, "Enable Subsurface");
                SetKeywordSafe(material, "_SUBSURFACE", enableSubsurface.floatValue > 0);
                
                if (enableSubsurface.floatValue > 0)
                {
                    EditorGUI.indentLevel++;
                    if (subsurfaceColor != null)
                        materialEditor.ColorProperty(subsurfaceColor, "Subsurface Color");
                    if (subsurfacePower != null)
                        materialEditor.RangeProperty(subsurfacePower, "Subsurface Power");
                    if (subsurfaceIntensity != null)
                        materialEditor.RangeProperty(subsurfaceIntensity, "Subsurface Intensity");
                    EditorGUI.indentLevel--;
                }
                
                ToonGUIStyles.EndStyledBox();
            }
        }
        
        private void DrawEmissionSection(MaterialEditor materialEditor, Material material)
        {
            if (enableEmission == null) return;
            
            showEmission = ToonGUIStyles.DrawStyledFoldout(showEmission, "Emission", "🔥");
            
            if (showEmission)
            {
                ToonGUIStyles.BeginStyledBox();
                
                materialEditor.ShaderProperty(enableEmission, "Enable Emission");
                SetKeywordSafe(material, "_EMISSION", enableEmission.floatValue > 0);
                
                if (enableEmission.floatValue > 0)
                {
                    EditorGUI.indentLevel++;
                    if (emissionMap != null && emissionColor != null)
                        materialEditor.TexturePropertyWithHDRColor(new GUIContent("Emission"), emissionMap, emissionColor, false);
                    else if (emissionColor != null)
                        materialEditor.ColorProperty(emissionColor, "Emission Color");
                    
                    if (emissionIntensity != null)
                        materialEditor.RangeProperty(emissionIntensity, "Emission Intensity");
                    if (emissionScrollSpeed != null)
                        materialEditor.VectorProperty(emissionScrollSpeed, "Scroll Speed (X,Y)");
                    EditorGUI.indentLevel--;
                }
                
                ToonGUIStyles.EndStyledBox();
            }
        }
        
        private void DrawOutlineSection(MaterialEditor materialEditor, Material material)
        {
            if (enableOutline == null) return;
            
            showOutline = ToonGUIStyles.DrawStyledFoldout(showOutline, "Outline", "🖼️");
            
            if (showOutline)
            {
                ToonGUIStyles.BeginStyledBox();
                
                materialEditor.ShaderProperty(enableOutline, "Enable Outline");
                SetKeywordSafe(material, "_OUTLINE", enableOutline.floatValue > 0);
                
                if (enableOutline.floatValue > 0)
                {
                    EditorGUI.indentLevel++;
                    if (outlineColor != null)
                        materialEditor.ColorProperty(outlineColor, "Outline Color");
                    if (outlineWidth != null)
                        materialEditor.RangeProperty(outlineWidth, "Outline Width");
                    EditorGUI.indentLevel--;
                }
                
                ToonGUIStyles.EndStyledBox();
            }
        }
        
        private void DrawNormalMapSection(MaterialEditor materialEditor, Material material)
        {
            if (enableNormalMap == null) return;
            
            showNormalMap = ToonGUIStyles.DrawStyledFoldout(showNormalMap, "Normal Mapping", "🗺️");
            
            if (showNormalMap)
            {
                ToonGUIStyles.BeginStyledBox();
                
                materialEditor.ShaderProperty(enableNormalMap, "Enable Normal Map");
                SetKeywordSafe(material, "_NORMALMAP", enableNormalMap.floatValue > 0);
                
                if (enableNormalMap.floatValue > 0)
                {
                    EditorGUI.indentLevel++;
                    if (bumpMap != null)
                        materialEditor.TexturePropertySingleLine(new GUIContent("Normal Map"), bumpMap);
                    if (bumpScale != null && bumpMap?.textureValue != null)
                        materialEditor.RangeProperty(bumpScale, "Normal Scale");
                    EditorGUI.indentLevel--;
                }
                
                ToonGUIStyles.EndStyledBox();
            }
        }
        
        private void DrawStylizationSection(MaterialEditor materialEditor, Material material)
        {
            showColorGrading = ToonGUIStyles.DrawStyledFoldout(showColorGrading, "Color Grading", "🎨");
            
            if (showColorGrading)
            {
                ToonGUIStyles.BeginStyledBox();
                
                if (hue != null)
                    materialEditor.RangeProperty(hue, "Hue Shift");
                if (contrast != null)
                    materialEditor.RangeProperty(contrast, "Contrast");
                if (gamma != null)
                    materialEditor.RangeProperty(gamma, "Gamma");
                
                ToonGUIStyles.EndStyledBox();
            }
            
            showStylization = ToonGUIStyles.DrawStyledFoldout(showStylization, "Stylization", "🎭");
            
            if (showStylization)
            {
                ToonGUIStyles.BeginStyledBox();
                
                // Posterization
                if (enablePosterize != null)
                {
                    materialEditor.ShaderProperty(enablePosterize, "Enable Posterize");
                    SetKeywordSafe(material, "_POSTERIZE", enablePosterize.floatValue > 0);
                    
                    if (enablePosterize.floatValue > 0 && posterizeLevels != null)
                    {
                        EditorGUI.indentLevel++;
                        materialEditor.RangeProperty(posterizeLevels, "Posterize Levels");
                        EditorGUI.indentLevel--;
                    }
                }
                
                // Cel Shading
                if (enableCelShading != null)
                {
                    materialEditor.ShaderProperty(enableCelShading, "Enable Cel Shading");
                    SetKeywordSafe(material, "_CEL_SHADING", enableCelShading.floatValue > 0);
                    
                    if (enableCelShading.floatValue > 0 && celShadingSteps != null)
                    {
                        EditorGUI.indentLevel++;
                        materialEditor.RangeProperty(celShadingSteps, "Cel Shading Steps");
                        EditorGUI.indentLevel--;
                    }
                }
                
                ToonGUIStyles.EndStyledBox();
            }
        }
        
        private void DrawAdvancedSection(MaterialEditor materialEditor, Material material)
        {
            showAdvanced = ToonGUIStyles.DrawStyledFoldout(showAdvanced, "Advanced Settings", "⚙️");
            
            if (showAdvanced)
            {
                ToonGUIStyles.BeginStyledBox();
                
                if (cutoff != null)
                    materialEditor.RangeProperty(cutoff, "Alpha Cutoff");
                if (cull != null)
                    materialEditor.ShaderProperty(cull, "Cull Mode");
                if (zwrite != null)
                    materialEditor.ShaderProperty(zwrite, "Z Write");
                if (ztest != null)
                    materialEditor.ShaderProperty(ztest, "Z Test");
                
                ToonGUIStyles.EndStyledBox();
            }
        }
        
        private void DrawFooter()
        {
            ToonGUIStyles.BeginStyledBox();
            EditorGUILayout.LabelField("Shader Information", ToonGUIStyles.BoldLabelStyle);
            
            ToonGUIStyles.BeginHorizontalLayout();
            EditorGUILayout.LabelField("Unity URP Compatible", ToonGUIStyles.SmallLabelStyle);
            EditorGUILayout.LabelField("Mobile Optimized", ToonGUIStyles.SmallLabelStyle);
            ToonGUIStyles.EndHorizontalLayout();
            
            ToonGUIStyles.BeginHorizontalLayout();
            EditorGUILayout.LabelField("Advanced Toon Features", ToonGUIStyles.SmallLabelStyle);
            EditorGUILayout.LabelField("Professional Edition", ToonGUIStyles.SmallLabelStyle);
            ToonGUIStyles.EndHorizontalLayout();
            
            ToonGUIStyles.AddVerticalSpace(5f);
            ToonGUIStyles.DrawInfoBox("Use the preset buttons for quick setup, then fine-tune individual properties below!", MessageType.Info);
            ToonGUIStyles.EndStyledBox();
        }
        
        #region Utility Methods
        
        private void SetKeywordSafe(Material material, string keyword, bool enabled)
        {
            if (material == null) return;
            
            if (enabled)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }
        
        private void ApplyPresetByIndex(Material material, MaterialEditor materialEditor, int index)
        {
            var preset = ToonPresetManager.GetPresetByIndex(index);
            if (preset == null || material == null) return;
            
            // Apply base color
            if (baseColor != null)
                baseColor.colorValue = preset.baseColor;
            
            // Apply lighting settings
            if (preset.lightingSettings != null)
            {
                if (shadowThreshold != null)
                    shadowThreshold.floatValue = preset.lightingSettings.shadowThreshold;
                if (shadowSmoothness != null)
                    shadowSmoothness.floatValue = preset.lightingSettings.shadowSmoothness;
                if (shadowColor != null)
                    shadowColor.colorValue = preset.lightingSettings.shadowColor;
                if (shadowIntensity != null)
                    shadowIntensity.floatValue = preset.lightingSettings.shadowIntensity;
                if (useRampTexture != null)
                    useRampTexture.floatValue = preset.lightingSettings.useRampTexture ? 1f : 0f;
                if (indirectLightingBoost != null)
                    indirectLightingBoost.floatValue = preset.lightingSettings.indirectLightingBoost;
                if (ambientOcclusion != null)
                    ambientOcclusion.floatValue = preset.lightingSettings.ambientOcclusion;
                if (lightWrapping != null)
                    lightWrapping.floatValue = preset.lightingSettings.lightWrapping;
            }
            
            // Apply visual effects
            if (preset.visualEffects != null)
            {
                // Rim lighting
                if (enableRimLighting != null)
                    enableRimLighting.floatValue = preset.visualEffects.rimLighting.enableRimLighting ? 1f : 0f;
                if (rimColor != null)
                    rimColor.colorValue = preset.visualEffects.rimLighting.rimColor;
                if (rimPower != null)
                    rimPower.floatValue = preset.visualEffects.rimLighting.rimPower;
                if (rimIntensity != null)
                    rimIntensity.floatValue = preset.visualEffects.rimLighting.rimIntensity;
                if (rimThreshold != null)
                    rimThreshold.floatValue = preset.visualEffects.rimLighting.rimThreshold;
                
                // Specular
                if (enableSpecular != null)
                    enableSpecular.floatValue = preset.visualEffects.specular.enableSpecular ? 1f : 0f;
                if (specularColor != null)
                    specularColor.colorValue = preset.visualEffects.specular.specularColor;
                if (specularSize != null)
                    specularSize.floatValue = preset.visualEffects.specular.specularSize;
                if (specularSmoothness != null)
                    specularSmoothness.floatValue = preset.visualEffects.specular.specularSmoothness;
                if (specularIntensity != null)
                    specularIntensity.floatValue = preset.visualEffects.specular.specularIntensity;
                
                // Outline
                if (enableOutline != null)
                    enableOutline.floatValue = preset.visualEffects.outline.enableOutline ? 1f : 0f;
                if (outlineColor != null)
                    outlineColor.colorValue = preset.visualEffects.outline.outlineColor;
                if (outlineWidth != null)
                    outlineWidth.floatValue = preset.visualEffects.outline.outlineWidth;
            }
            
            // Apply stylization
            if (preset.stylization != null)
            {
                // Hatching
                if (enableHatching != null)
                    enableHatching.floatValue = preset.stylization.hatching.enableHatching ? 1f : 0f;
                if (hatchingDensity != null)
                    hatchingDensity.floatValue = preset.stylization.hatching.hatchingDensity;
                if (hatchingIntensity != null)
                    hatchingIntensity.floatValue = preset.stylization.hatching.hatchingIntensity;
                if (hatchingThreshold != null)
                    hatchingThreshold.floatValue = preset.stylization.hatching.hatchingThreshold;
                if (crossHatchingThreshold != null)
                    crossHatchingThreshold.floatValue = preset.stylization.hatching.crossHatchingThreshold;
                if (hatchingRotation != null)
                    hatchingRotation.floatValue = preset.stylization.hatching.hatchingRotation;
                if (enableScreenSpaceHatching != null)
                    enableScreenSpaceHatching.floatValue = preset.stylization.hatching.enableScreenSpaceHatching ? 1f : 0f;
                if (screenHatchScale != null)
                    screenHatchScale.floatValue = preset.stylization.hatching.screenHatchScale;
                if (screenHatchBias != null)
                    screenHatchBias.floatValue = preset.stylization.hatching.screenHatchBias;
                
                // Color grading
                if (saturation != null)
                    saturation.floatValue = preset.stylization.colorGrading.saturation;
                if (brightness != null)
                    brightness.floatValue = preset.stylization.colorGrading.brightness;
                if (hue != null)
                    hue.floatValue = preset.stylization.colorGrading.hueShift;
                if (contrast != null)
                    contrast.floatValue = preset.stylization.colorGrading.contrast;
                if (gamma != null)
                    gamma.floatValue = preset.stylization.colorGrading.gamma;
                
                // Quantization
                if (enablePosterize != null)
                    enablePosterize.floatValue = preset.stylization.quantization.enablePosterize ? 1f : 0f;
                if (posterizeLevels != null)
                    posterizeLevels.floatValue = preset.stylization.quantization.posterizeLevels;
                if (enableCelShading != null)
                    enableCelShading.floatValue = preset.stylization.quantization.enableCelShading ? 1f : 0f;
                if (celShadingSteps != null)
                    celShadingSteps.floatValue = preset.stylization.quantization.celShadingSteps;
            }
            
            // Update keywords based on new values
            UpdateAllKeywords(material);
            
            materialEditor.PropertiesChanged();
        }
        
        private void UpdateAllKeywords(Material material)
        {
            if (material == null) return;
            
            SetKeywordSafe(material, "_USE_RAMP_TEXTURE", useRampTexture?.floatValue > 0);
            SetKeywordSafe(material, "_RIM_LIGHTING", enableRimLighting?.floatValue > 0);
            SetKeywordSafe(material, "_SPECULAR", enableSpecular?.floatValue > 0);
            SetKeywordSafe(material, "_HATCHING", enableHatching?.floatValue > 0);
            SetKeywordSafe(material, "_SCREEN_SPACE_HATCHING", enableScreenSpaceHatching?.floatValue > 0);
            SetKeywordSafe(material, "_OUTLINE", enableOutline?.floatValue > 0);
            SetKeywordSafe(material, "_MATCAP", enableMatcap?.floatValue > 0);
            SetKeywordSafe(material, "_NORMALMAP", enableNormalMap?.floatValue > 0);
            SetKeywordSafe(material, "_EMISSION", enableEmission?.floatValue > 0);
            SetKeywordSafe(material, "_FRESNEL", enableFresnel?.floatValue > 0);
            SetKeywordSafe(material, "_SUBSURFACE", enableSubsurface?.floatValue > 0);
            SetKeywordSafe(material, "_POSTERIZE", enablePosterize?.floatValue > 0);
            SetKeywordSafe(material, "_CEL_SHADING", enableCelShading?.floatValue > 0);
        }
        
        private int CountActiveFeatures(Material material)
        {
            if (material == null) return 0;
            
            int count = 0;
            if (material.IsKeywordEnabled("_RIM_LIGHTING")) count++;
            if (material.IsKeywordEnabled("_SPECULAR")) count++;
            if (material.IsKeywordEnabled("_HATCHING")) count++;
            if (material.IsKeywordEnabled("_SCREEN_SPACE_HATCHING")) count++;
            if (material.IsKeywordEnabled("_OUTLINE")) count++;
            if (material.IsKeywordEnabled("_MATCAP")) count++;
            if (material.IsKeywordEnabled("_NORMALMAP")) count++;
            if (material.IsKeywordEnabled("_EMISSION")) count++;
            if (material.IsKeywordEnabled("_FRESNEL")) count++;
            if (material.IsKeywordEnabled("_SUBSURFACE")) count++;
            if (material.IsKeywordEnabled("_POSTERIZE")) count++;
            if (material.IsKeywordEnabled("_CEL_SHADING")) count++;
            
            return count;
        }
        
        private float EstimatePerformanceCost(Material material)
        {
            if (material == null) return 0f;
            
            float cost = 0.1f; // Base toon shader cost
            
            if (material.IsKeywordEnabled("_RIM_LIGHTING")) cost += 0.1f;
            if (material.IsKeywordEnabled("_SPECULAR")) cost += 0.15f;
            if (material.IsKeywordEnabled("_HATCHING")) cost += 0.2f;
            if (material.IsKeywordEnabled("_SCREEN_SPACE_HATCHING")) cost += 0.15f;
            if (material.IsKeywordEnabled("_OUTLINE")) cost += 0.3f;
            if (material.IsKeywordEnabled("_MATCAP")) cost += 0.2f;
            if (material.IsKeywordEnabled("_NORMALMAP")) cost += 0.05f;
            if (material.IsKeywordEnabled("_EMISSION")) cost += 0.05f;
            if (material.IsKeywordEnabled("_FRESNEL")) cost += 0.1f;
            if (material.IsKeywordEnabled("_SUBSURFACE")) cost += 0.25f;
            if (material.IsKeywordEnabled("_POSTERIZE")) cost += 0.1f;
            if (material.IsKeywordEnabled("_CEL_SHADING")) cost += 0.05f;
            
            return Mathf.Clamp01(cost);
        }
        
        private string GetQualityRecommendation(float performanceCost)
        {
            if (performanceCost > 0.8f)
                return "High performance cost detected. Consider reducing active features for mobile platforms.";
            if (performanceCost > 0.6f)
                return "Medium performance cost. Should work well on most platforms.";
            if (performanceCost < 0.3f)
                return "Low performance cost. Excellent for mobile and VR applications.";
            
            return "";
        }
        
        #endregion
    }d FindProperties()
        {
            // Base properties
            baseMap = FindProperty("_BaseMap", allProperties, false);
            baseColor = FindProperty("_BaseColor", allProperties);
            saturation = FindProperty("_Saturation", allProperties);
            brightness = FindProperty("_Brightness", allProperties);
            
            // Lighting properties
            shadowThreshold = FindProperty("_ShadowThreshold", allProperties);
            shadowSmoothness = FindProperty("_ShadowSmoothness", allProperties);
            shadowColor = FindProperty("_ShadowColor", allProperties);
            shadowIntensity = FindProperty("_ShadowIntensity", allProperties);
            lightRampTex = FindProperty("_LightRampTex", allProperties, false);
            useRampTexture = FindProperty("_UseRampTexture", allProperties, false);
            indirectLightingBoost = FindProperty("_IndirectLightingBoost", allProperties, false);
            ambientOcclusion = FindProperty("_AmbientOcclusion", allProperties, false);
            lightWrapping = FindProperty("_LightWrapping", allProperties, false);
            
            // Rim lighting
            enableRimLighting = FindProperty("_EnableRimLighting", allProperties, false);
            rimColor = FindProperty("_RimColor", allProperties, false);
            rimPower = FindProperty("_RimPower", allProperties, false);
            rimIntensity = FindProperty("_RimIntensity", allProperties, false);
            rimThreshold = FindProperty("_RimThreshold", allProperties, false);
            
            // Specular
            enableSpecular = FindProperty("_EnableSpecular", allProperties, false);
            specularColor = FindProperty("_SpecularColor", allProperties, false);
            specularSize = FindProperty("_SpecularSize", allProperties, false);
            specularSmoothness = FindProperty("_SpecularSmoothness", allProperties, false);
            specularIntensity = FindProperty("_SpecularIntensity", allProperties, false);
            
            // Hatching
            enableHatching = FindProperty("_EnableHatching", allProperties, false);
            hatchingTex = FindProperty("_HatchingTex", allProperties, false);
            crossHatchingTex = FindProperty("_CrossHatchingTex", allProperties, false);
            hatchingDensity = FindProperty("_HatchingDensity", allProperties, false);
            hatchingIntensity = FindProperty("_HatchingIntensity", allProperties, false);
            hatchingThreshold = FindProperty("_HatchingThreshold", allProperties, false);
            crossHatchingThreshold = FindProperty("_CrossHatchingThreshold", allProperties, false);
            hatchingRotation = FindProperty("_HatchingRotation", allProperties, false);
            enableScreenSpaceHatching = FindProperty("_EnableScreenSpaceHatching", allProperties, false);
            screenHatchScale = FindProperty("_ScreenHatchScale", allProperties, false);
            screenHatchBias = FindProperty("_ScreenHatchBias", allProperties, false);
            
            // Other properties... (continuing with the pattern)
            enableMatcap = FindProperty("_EnableMatcap", allProperties, false);
            matcapTex = FindProperty("_MatcapTex", allProperties, false);
            matcapIntensity = FindProperty("_MatcapIntensity", allProperties, false);
            matcapBlendMode = FindProperty("_MatcapBlendMode", allProperties, false);
            
            enableNormalMap = FindProperty("_EnableNormalMap", allProperties, false);
            bumpMap = FindProperty("_BumpMap", allProperties, false);
            bumpScale = FindProperty("_BumpScale", allProperties, false);
            
            enableEmission = FindProperty("_EnableEmission", allProperties, false);
            emissionMap = FindProperty("_EmissionMap", allProperties, false);
            emissionColor = FindProperty("_EmissionColor", allProperties, false);
            emissionIntensity = FindProperty("_EmissionIntensity", allProperties, false);
            emissionScrollSpeed = FindProperty("_EmissionScrollSpeed", allProperties, false);
            
            enableFresnel = FindProperty("_EnableFresnel", allProperties, false);
            fresnelColor = FindProperty("_FresnelColor", allProperties, false);
            fresnelPower = FindProperty("_FresnelPower", allProperties, false);
            fresnelIntensity = FindProperty("_FresnelIntensity", allProperties, false);
            
            enableSubsurface = FindProperty("_EnableSubsurface", allProperties, false);
            subsurfaceColor = FindProperty("_SubsurfaceColor", allProperties, false);
            subsurfacePower = FindProperty("_SubsurfacePower", allProperties, false);
            subsurfaceIntensity = FindProperty("_SubsurfaceIntensity", allProperties, false);
            
            enableOutline = FindProperty("_EnableOutline", allProperties, false);
            outlineColor = FindProperty("_OutlineColor", allProperties, false);
            outlineWidth = FindProperty("_OutlineWidth", allProperties, false);
            
            hue = FindProperty("_Hue", allProperties, false);
            contrast = FindProperty("_Contrast", allProperties, false);
            gamma = FindProperty("_Gamma", allProperties, false);
            
            enablePosterize = FindProperty("_EnablePosterize", allProperties, false);
            posterizeLevels = FindProperty("_PosterizeLevels", allProperties, false);
            enableCelShading = FindProperty("_EnableCelShading", allProperties, false);
            celShadingSteps = FindProperty("_CelShadingSteps", allProperties, false);
            
            cutoff = FindProperty("_Cutoff", allProperties, false);
            cull = FindProperty("_Cull", allProperties, false);
            zwrite = FindProperty("_ZWrite", allProperties, false);
            ztest = FindProperty("_ZTest", allProperties, false);
        }
        
        private void DrawHeader()
        {
            LoadLogo();
            
            if (logoTexture != null)
            {
                var rect = GUILayoutUtility.GetRect(0, 80, GUILayout.ExpandWidth(true));
                GUI.DrawTexture(rect, logoTexture, ScaleMode.ScaleToFit);
            }
            
            EditorGUILayout.LabelField("ULTIMATE TOON SHADER PRO", ToonGUIStyles.HeaderStyle);
            EditorGUILayout.LabelField("Unity URP - Professional Edition v3.0", ToonGUIStyles.VersionLabelStyle);
        }
        
        private void LoadLogo()
        {
            if (logoTexture != null) return;
            
            // Try to find logo in the same directory as this script
            string[] guids = AssetDatabase.FindAssets("t:Texture2D toon_logo");
            if (guids.Length > 0)
            {
                string logoPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                logoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(logoPath);
            }
        }
        
        private void DrawPresetSystem(MaterialEditor materialEditor, Material material)
        {
            ToonGUIStyles.BeginStyledBox();
            EditorGUILayout.LabelField("Quick Style Presets", ToonGUIStyles.BoldLabelStyle);
            
            var presetNames = ToonPresetManager.GetPresetNames();
            
            // First row
            ToonGUIStyles.BeginHorizontalLayout();
            for (int i = 0; i < 3 && i < presetNames.Length; i++)
            {
                if (ToonGUIStyles.DrawPresetButton(presetNames[i]))
                {
                    ApplyPresetByIndex(material, materialEditor, i);
                }
            }
            ToonGUIStyles.EndHorizontalLayout();
            
            // Second row
            if (presetNames.Length > 3)
            {
                ToonGUIStyles.BeginHorizontalLayout();
                for (int i = 3; i < 6 && i < presetNames.Length; i++)
                {
                    if (ToonGUIStyles.DrawPresetButton(presetNames[i]))
                    {
                        ApplyPresetByIndex(material, materialEditor, i);
                    }
                }
                ToonGUIStyles.EndHorizontalLayout();
            }
            
            // Third row
            if (presetNames.Length > 6)
            {
                ToonGUIStyles.BeginHorizontalLayout();
                for (int i = 6; i < presetNames.Length; i++)
                {
                    if (ToonGUIStyles.DrawPresetButton(presetNames[i]))
                    {
                        ApplyPresetByIndex(material, materialEditor, i);
                    }
                }
                ToonGUIStyles.EndHorizontalLayout();
            }
            
            ToonGUIStyles.DrawInfoBox("Click any preset to instantly apply predefined settings optimized for different art styles.", MessageType.Info);
            ToonGUIStyles.EndStyledBox();
        }
        
        private void DrawPerformanceSection(Material material)
        {
            showPerformanceInfo = ToonGUIStyles.DrawStyledFoldout(showPerformanceInfo, "Performance Monitor", "⚡");
            
            if (showPerformanceInfo)
            {
                ToonGUIStyles.BeginStyledBox();
                
                // Calculate active features
                int activeFeatures = CountActiveFeatures(material);
                int totalFeatures = 10; // Total available features
                
                ToonGUIStyles.DrawFeatureCount(activeFeatures, totalFeatures);
                
                // Performance cost estimation
                float performanceCost = EstimatePerformanceCost(material);
                ToonGUIStyles.DrawPerformanceBar(performanceCost, "Estimated Cost");
                
                // Quality recommendation
                string recommendation = GetQualityRecommendation(performanceCost);
                if (!string.IsNullOrEmpty(recommendation))
                {
                    ToonGUIStyles.DrawInfoBox(recommendation, MessageType.Info);
                }
                
                ToonGUIStyles.EndStyledBox();
            }
        }
        
        private void DrawBaseProperties(MaterialEditor materialEditor)
        {
            showBase = ToonGUIStyles.DrawStyledFoldout(showBase, "Base Properties", "🎨");
            
            if (showBase)
            {
                ToonGUIStyles.BeginStyledBox();
                
                if (baseMap != null)
                    materialEditor.TexturePropertySingleLine(new GUIContent("Albedo", "Main color texture"), baseMap, baseColor);
                else
                    materialEditor.ColorProperty(baseColor, "Base Color");
                
                if (saturation != null)
                    materialEditor.RangeProperty(saturation, "Saturation");
                
                if (brightness != null)
                    materialEditor.RangeProperty(brightness, "Brightness");
                
                ToonGUIStyles.EndStyledBox();
            }
        }
        
        private void DrawLightingSection(MaterialEditor materialEditor, Material material)
        {
            showLighting = ToonGUIStyles.DrawStyledFoldout(showLighting, "Toon Lighting", "💡");
            
            if (showLighting)
            {
                ToonGUIStyles.BeginStyledBox();
                
                // Ramp texture toggle
                if (useRampTexture != null)
                {
                    materialEditor.ShaderProperty(useRampTexture, "Use Ramp Texture");
                    SetKeywordSafe(material, "_USE_RAMP_TEXTURE", useRampTexture.floatValue > 0);
                    
                    if (useRampTexture.floatValue > 0 && lightRampTex != null)
                    {
                        EditorGUI.indentLevel++;
                        materialEditor.TexturePropertySingleLine(new GUIContent("Light Ramp", "Custom lighting ramp texture"), lightRampTex);
                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        // Standard shadow controls
                        if (shadowThreshold != null)
                            materialEditor.RangeProperty(shadowThreshold, "Shadow Threshold");
                        if (shadowSmoothness != null)
                            materialEditor.RangeProperty(shadowSmoothness, "Shadow Smoothness");
                    }
                }
                
                if (shadowColor != null)
                    materialEditor.ColorProperty(shadowColor, "Shadow Color");
                if (shadowIntensity != null)
                    materialEditor.RangeProperty(shadowIntensity, "Shadow Intensity");
                
                // Advanced lighting in sub-foldout
                showAdvancedLighting = ToonGUIStyles.DrawStyledFoldout(showAdvancedLighting, "Advanced Lighting", "⚡");
                if (showAdvancedLighting)
                {
                    EditorGUI.indentLevel++;
                    if (indirectLightingBoost != null)
                        materialEditor.RangeProperty(indirectLightingBoost, "Indirect Lighting Boost");
                    if (ambientOcclusion != null)
                        materialEditor.RangeProperty(ambientOcclusion, "Ambient Occlusion");
                    if (lightWrapping != null)
                        materialEditor.RangeProperty(lightWrapping, "Light Wrapping");
                    EditorGUI.indentLevel--;
                }
                
                ToonGUIStyles.EndStyledBox();
            }
        }
        
        private void DrawVisualEffectsSection(MaterialEditor materialEditor, Material material)
        {
            // Rim Lighting
            DrawRimLightingSection(materialEditor, material);
            
            // Specular
            DrawSpecularSection(materialEditor, material);
            
            // Hatching
            DrawHatchingSection(materialEditor, material);
            
            // Other effects
            DrawMatcapSection(materialEditor, material);
            DrawFresnelSection(materialEditor, material);
            DrawSubsurfaceSection(materialEditor, material);
            DrawEmissionSection(materialEditor, material);
            DrawOutlineSection(materialEditor, material);
            DrawNormalMapSection(materialEditor, material);
        }
        
        private void DrawRimLightingSection(MaterialEditor materialEditor, Material material)
        {
            if (enableRimLighting == null) return;
            
            showRimLighting = ToonGUIStyles.DrawStyledFoldout(showRimLighting, "Rim Lighting", "🌟");
            
            if (showRimLighting)
            {
                ToonGUIStyles.BeginStyledBox();
                
                materialEditor.ShaderProperty(enableRimLighting, "Enable Rim Lighting");
                SetKeywordSafe(material, "_RIM_LIGHTING", enableRimLighting.floatValue > 0);
                
                if (enableRimLighting.floatValue > 0)
                {
                    EditorGUI.indentLevel++;
                    if (rimColor != null)
                        materialEditor.ColorProperty(rimColor, "Rim Color");
                    if (rimPower != null)
                        materialEditor.RangeProperty(rimPower, "Rim Power");
                    if (rimIntensity != null)
                        materialEditor.RangeProperty(rimIntensity, "Rim Intensity");
                    if (rimThreshold != null)
                        materialEditor.RangeProperty(rimThreshold, "Rim Threshold");
                    EditorGUI.indentLevel--;
                }
                
                ToonGUIStyles.EndStyledBox();
            }
        }
        
        private void DrawSpecularSection(MaterialEditor materialEditor, Material material)
        {
            if (enableSpecular == null) return;
            
            showSpecular = ToonGUIStyles.DrawStyledFoldout(showSpecular, "Specular Highlights", "✨");
            
            if (showSpecular)
            {
                ToonGUIStyles.BeginStyledBox();
                
                materialEditor.ShaderProperty(enableSpecular, "Enable Specular");
                SetKeywordSafe(material, "_SPECULAR", enableSpecular.floatValue > 0);
                
                if (enableSpecular.floatValue > 0)
                {
                    EditorGUI.indentLevel++;
                    if (specularColor != null)
                        materialEditor.ColorProperty(specularColor, "Specular Color");
                    if (specularSize != null)
                        materialEditor.RangeProperty(specularSize, "Specular Size");
                    if (specularSmoothness != null)
                        materialEditor.RangeProperty(specularSmoothness, "Specular Smoothness");
                    if (specularIntensity != null)
                        materialEditor.RangeProperty(specularIntensity, "Specular Intensity");
                    EditorGUI.indentLevel--;
                }
                
                ToonGUIStyles.EndStyledBox();
            }
        }
        
        private voi