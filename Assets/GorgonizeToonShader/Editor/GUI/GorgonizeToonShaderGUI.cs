using UnityEngine;
using UnityEditor;
using System.IO;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Main shader inspector GUI for Gorgonize Toon Shader
    /// Provides a comprehensive interface for all shader properties
    /// </summary>
    public class GorgonizeToonShaderGUI : ShaderGUI
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
        private MaterialProperty metallic;
        private MaterialProperty smoothness;
        private MaterialProperty occlusionStrength;
        
        // Lighting Properties
        private MaterialProperty shadowThreshold;
        private MaterialProperty shadowSmoothness;
        private MaterialProperty shadowColor;
        private MaterialProperty shadowIntensity;
        private MaterialProperty lightRampTex;
        private MaterialProperty useRampTexture;
        private MaterialProperty indirectLightingBoost;
        private MaterialProperty lightWrapping;
        private MaterialProperty volumetricIntensity;
        private MaterialProperty cookieInfluence;
        
        // Visual Effects Properties
        private MaterialProperty enableRimLighting;
        private MaterialProperty rimColor;
        private MaterialProperty rimPower;
        private MaterialProperty rimIntensity;
        private MaterialProperty rimThreshold;
        private MaterialProperty rimColorSecondary;
        private MaterialProperty rimPowerSecondary;
        
        private MaterialProperty enableSpecular;
        private MaterialProperty specularColor;
        private MaterialProperty specularSize;
        private MaterialProperty specularSmoothness;
        private MaterialProperty specularIntensity;
        private MaterialProperty anisotropy;
        private MaterialProperty specularSteps;
        
        // Hatching Properties
        private MaterialProperty enableHatching;
        private MaterialProperty hatchingTex;
        private MaterialProperty crossHatchingTex;
        private MaterialProperty hatchingTex2;
        private MaterialProperty hatchingDensity;
        private MaterialProperty hatchingIntensity;
        private MaterialProperty hatchingThreshold;
        private MaterialProperty crossHatchingThreshold;
        private MaterialProperty secondaryHatchingThreshold;
        private MaterialProperty hatchingRotation;
        private MaterialProperty hatchingAnimSpeed;
        private MaterialProperty enableScreenSpaceHatching;
        private MaterialProperty screenHatchScale;
        private MaterialProperty screenHatchBias;
        
        // Other Effects
        private MaterialProperty enableMatcap;
        private MaterialProperty matcapTex;
        private MaterialProperty matcapIntensity;
        private MaterialProperty matcapBlendMode;
        private MaterialProperty matcapRotation;
        
        private MaterialProperty enableNormalMap;
        private MaterialProperty bumpMap;
        private MaterialProperty bumpScale;
        
        private MaterialProperty enableDetail;
        private MaterialProperty detailMap;
        private MaterialProperty detailNormalMap;
        private MaterialProperty detailScale;
        private MaterialProperty detailNormalScale;
        
        private MaterialProperty enableEmission;
        private MaterialProperty emissionMap;
        private MaterialProperty emissionColor;
        private MaterialProperty emissionIntensity;
        private MaterialProperty emissionScrollSpeed;
        private MaterialProperty emissionPulseSpeed;
        private MaterialProperty emissionPulseIntensity;
        private MaterialProperty emissionTemperature;
        
        private MaterialProperty enableFresnel;
        private MaterialProperty fresnelColor;
        private MaterialProperty fresnelPower;
        private MaterialProperty fresnelIntensity;
        private MaterialProperty iridescenceIntensity;
        
        private MaterialProperty enableSubsurface;
        private MaterialProperty subsurfaceColor;
        private MaterialProperty subsurfacePower;
        private MaterialProperty subsurfaceIntensity;
        private MaterialProperty subsurfaceDistortion;
        private MaterialProperty subsurfaceThickness;
        
        private MaterialProperty enableOutline;
        private MaterialProperty outlineColor;
        private MaterialProperty outlineWidth;
        private MaterialProperty outlineMode;
        private MaterialProperty outlineFadeDistance;
        private MaterialProperty outlineDepthBiasValue;
        
        // Color Grading
        private MaterialProperty hue;
        private MaterialProperty contrast;
        private MaterialProperty gamma;
        private MaterialProperty colorTemperature;
        private MaterialProperty colorTint;
        private MaterialProperty vibrance;
        
        // Stylization
        private MaterialProperty enablePosterize;
        private MaterialProperty posterizeLevels;
        private MaterialProperty enableCelShading;
        private MaterialProperty celShadingSteps;
        private MaterialProperty enableDithering;
        private MaterialProperty ditheringIntensity;
        
        // Advanced Effects
        private MaterialProperty enableForceField;
        private MaterialProperty forceFieldColor;
        private MaterialProperty forceFieldIntensity;
        private MaterialProperty forceFieldFrequency;
        
        private MaterialProperty enableHologram;
        private MaterialProperty hologramIntensity;
        private MaterialProperty hologramFlicker;
        private MaterialProperty hologramScanlines;
        
        private MaterialProperty enableDissolve;
        private MaterialProperty dissolveNoise;
        private MaterialProperty dissolveAmount;
        private MaterialProperty dissolveEdgeWidth;
        private MaterialProperty dissolveEdgeColor;
        
        // Animation
        private MaterialProperty enableAnimatedProperties;
        private MaterialProperty animationSpeed;
        private MaterialProperty enableVertexAnimation;
        private MaterialProperty vertexAnimationIntensity;
        private MaterialProperty vertexAnimationFrequency;
        
        // Performance
        private MaterialProperty qualityLevel;
        private MaterialProperty enableLODFade;
        private MaterialProperty lodFadeDistance;
        private MaterialProperty enableInstancing;
        
        // Advanced Rendering
        private MaterialProperty cutoff;
        private MaterialProperty cull;
        private MaterialProperty zwrite;
        private MaterialProperty ztest;
        private MaterialProperty srcBlend;
        private MaterialProperty dstBlend;
        
        // Debug
        private MaterialProperty debugMode;
        private MaterialProperty debugView;
        private MaterialProperty showWireframe;
        private MaterialProperty wireframeColor;
        private MaterialProperty wireframeThickness;
        
        // Foldout states - persistent across inspector redraws
        private static bool showBase = true;
        private static bool showLighting = true;
        private static bool showAdvancedLighting = false;
        private static bool showRimLighting = false;
        private static bool showSpecular = false;
        private static bool showHatching = false;
        private static bool showMatcap = false;
        private static bool showNormalMap = false;
        private static bool showDetail = false;
        private static bool showEmission = false;
        private static bool showFresnel = false;
        private static bool showSubsurface = false;
        private static bool showOutline = false;
        private static bool showColorGrading = false;
        private static bool showStylization = false;
        private static bool showAdvancedEffects = false;
        private static bool showAnimation = false;
        private static bool showPerformance = false;
        private static bool showAdvanced = false;
        private static bool showDebug = false;
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
            DrawSeparator();
            
            // Performance Info
            DrawPerformanceSection(material);
            DrawSeparator();
            
            // Main Properties Sections
            DrawBaseProperties(materialEditor);
            DrawLightingSection(materialEditor, material);
            DrawVisualEffectsSection(materialEditor, material);
            DrawStylizationSection(materialEditor, material);
            DrawAdvancedEffectsSection(materialEditor, material);
            DrawAnimationSection(materialEditor, material);
            DrawPerformancePropertiesSection(materialEditor, material);
            DrawAdvancedSection(materialEditor, material);
            DrawDebugSection(materialEditor, material);
            
            EditorGUILayout.Space();
            DrawFooter();
        }
        
        private void FindProperties()
        {
            // Base properties
            baseMap = FindProperty("_BaseMap", allProperties, false);
            baseColor = FindProperty("_BaseColor", allProperties);
            saturation = FindProperty("_Saturation", allProperties, false);
            brightness = FindProperty("_Brightness", allProperties, false);
            metallic = FindProperty("_Metallic", allProperties, false);
            smoothness = FindProperty("_Smoothness", allProperties, false);
            occlusionStrength = FindProperty("_OcclusionStrength", allProperties, false);
            
            // Lighting properties
            shadowThreshold = FindProperty("_ShadowThreshold", allProperties, false);
            shadowSmoothness = FindProperty("_ShadowSmoothness", allProperties, false);
            shadowColor = FindProperty("_ShadowColor", allProperties, false);
            shadowIntensity = FindProperty("_ShadowIntensity", allProperties, false);
            lightRampTex = FindProperty("_LightRampTex", allProperties, false);
            useRampTexture = FindProperty("_UseRampTexture", allProperties, false);
            indirectLightingBoost = FindProperty("_IndirectLightingBoost", allProperties, false);
            lightWrapping = FindProperty("_LightWrapping", allProperties, false);
            volumetricIntensity = FindProperty("_VolumetricIntensity", allProperties, false);
            cookieInfluence = FindProperty("_CookieInfluence", allProperties, false);
            
            // Rim lighting
            enableRimLighting = FindProperty("_EnableRimLighting", allProperties, false);
            rimColor = FindProperty("_RimColor", allProperties, false);
            rimPower = FindProperty("_RimPower", allProperties, false);
            rimIntensity = FindProperty("_RimIntensity", allProperties, false);
            rimThreshold = FindProperty("_RimThreshold", allProperties, false);
            rimColorSecondary = FindProperty("_RimColorSecondary", allProperties, false);
            rimPowerSecondary = FindProperty("_RimPowerSecondary", allProperties, false);
            
            // Specular
            enableSpecular = FindProperty("_EnableSpecular", allProperties, false);
            specularColor = FindProperty("_SpecularColor", allProperties, false);
            specularSize = FindProperty("_SpecularSize", allProperties, false);
            specularSmoothness = FindProperty("_SpecularSmoothness", allProperties, false);
            specularIntensity = FindProperty("_SpecularIntensity", allProperties, false);
            anisotropy = FindProperty("_Anisotropy", allProperties, false);
            specularSteps = FindProperty("_SpecularSteps", allProperties, false);
            
            // Hatching
            enableHatching = FindProperty("_EnableHatching", allProperties, false);
            hatchingTex = FindProperty("_HatchingTex", allProperties, false);
            crossHatchingTex = FindProperty("_CrossHatchingTex", allProperties, false);
            hatchingTex2 = FindProperty("_HatchingTex2", allProperties, false);
            hatchingDensity = FindProperty("_HatchingDensity", allProperties, false);
            hatchingIntensity = FindProperty("_HatchingIntensity", allProperties, false);
            hatchingThreshold = FindProperty("_HatchingThreshold", allProperties, false);
            crossHatchingThreshold = FindProperty("_CrossHatchingThreshold", allProperties, false);
            secondaryHatchingThreshold = FindProperty("_SecondaryHatchingThreshold", allProperties, false);
            hatchingRotation = FindProperty("_HatchingRotation", allProperties, false);
            hatchingAnimSpeed = FindProperty("_HatchingAnimSpeed", allProperties, false);
            enableScreenSpaceHatching = FindProperty("_ScreenSpaceHatching", allProperties, false);
            screenHatchScale = FindProperty("_ScreenHatchScale", allProperties, false);
            screenHatchBias = FindProperty("_ScreenHatchBias", allProperties, false);
            
            // Matcap
            enableMatcap = FindProperty("_EnableMatcap", allProperties, false);
            matcapTex = FindProperty("_MatcapTex", allProperties, false);
            matcapIntensity = FindProperty("_MatcapIntensity", allProperties, false);
            matcapBlendMode = FindProperty("_MatcapBlendMode", allProperties, false);
            matcapRotation = FindProperty("_MatcapRotation", allProperties, false);
            
            // Normal mapping
            enableNormalMap = FindProperty("_EnableNormalMap", allProperties, false);
            bumpMap = FindProperty("_BumpMap", allProperties, false);
            bumpScale = FindProperty("_BumpScale", allProperties, false);
            
            // Detail
            enableDetail = FindProperty("_EnableDetail", allProperties, false);
            detailMap = FindProperty("_DetailMap", allProperties, false);
            detailNormalMap = FindProperty("_DetailNormalMap", allProperties, false);
            detailScale = FindProperty("_DetailScale", allProperties, false);
            detailNormalScale = FindProperty("_DetailNormalScale", allProperties, false);
            
            // Emission
            enableEmission = FindProperty("_EnableEmission", allProperties, false);
            emissionMap = FindProperty("_EmissionMap", allProperties, false);
            emissionColor = FindProperty("_EmissionColor", allProperties, false);
            emissionIntensity = FindProperty("_EmissionIntensity", allProperties, false);
            emissionScrollSpeed = FindProperty("_EmissionScrollSpeed", allProperties, false);
            emissionPulseSpeed = FindProperty("_EmissionPulseSpeed", allProperties, false);
            emissionPulseIntensity = FindProperty("_EmissionPulseIntensity", allProperties, false);
            emissionTemperature = FindProperty("_EmissionTemperature", allProperties, false);
            
            // Fresnel
            enableFresnel = FindProperty("_EnableFresnel", allProperties, false);
            fresnelColor = FindProperty("_FresnelColor", allProperties, false);
            fresnelPower = FindProperty("_FresnelPower", allProperties, false);
            fresnelIntensity = FindProperty("_FresnelIntensity", allProperties, false);
            iridescenceIntensity = FindProperty("_IridescenceIntensity", allProperties, false);
            
            // Subsurface
            enableSubsurface = FindProperty("_EnableSubsurface", allProperties, false);
            subsurfaceColor = FindProperty("_SubsurfaceColor", allProperties, false);
            subsurfacePower = FindProperty("_SubsurfacePower", allProperties, false);
            subsurfaceIntensity = FindProperty("_SubsurfaceIntensity", allProperties, false);
            subsurfaceDistortion = FindProperty("_SubsurfaceDistortion", allProperties, false);
            subsurfaceThickness = FindProperty("_SubsurfaceThickness", allProperties, false);
            
            // Outline
            enableOutline = FindProperty("_EnableOutline", allProperties, false);
            outlineColor = FindProperty("_OutlineColor", allProperties, false);
            outlineWidth = FindProperty("_OutlineWidth", allProperties, false);
            outlineMode = FindProperty("_OutlineMode", allProperties, false);
            outlineFadeDistance = FindProperty("_OutlineFadeDistance", allProperties, false);
            outlineDepthBiasValue = FindProperty("_OutlineDepthBiasValue", allProperties, false);
            
            // Color grading
            hue = FindProperty("_Hue", allProperties, false);
            contrast = FindProperty("_Contrast", allProperties, false);
            gamma = FindProperty("_Gamma", allProperties, false);
            colorTemperature = FindProperty("_ColorTemperature", allProperties, false);
            colorTint = FindProperty("_ColorTint", allProperties, false);
            vibrance = FindProperty("_Vibrance", allProperties, false);
            
            // Stylization
            enablePosterize = FindProperty("_EnablePosterize", allProperties, false);
            posterizeLevels = FindProperty("_PosterizeLevels", allProperties, false);
            enableCelShading = FindProperty("_EnableCelShading", allProperties, false);
            celShadingSteps = FindProperty("_CelShadingSteps", allProperties, false);
            enableDithering = FindProperty("_EnableDithering", allProperties, false);
            ditheringIntensity = FindProperty("_DitheringIntensity", allProperties, false);
            
            // Advanced effects
            enableForceField = FindProperty("_EnableForceField", allProperties, false);
            forceFieldColor = FindProperty("_ForceFieldColor", allProperties, false);
            forceFieldIntensity = FindProperty("_ForceFieldIntensity", allProperties, false);
            forceFieldFrequency = FindProperty("_ForceFieldFrequency", allProperties, false);
            
            enableHologram = FindProperty("_EnableHologram", allProperties, false);
            hologramIntensity = FindProperty("_HologramIntensity", allProperties, false);
            hologramFlicker = FindProperty("_HologramFlicker", allProperties, false);
            hologramScanlines = FindProperty("_HologramScanlines", allProperties, false);
            
            enableDissolve = FindProperty("_EnableDissolve", allProperties, false);
            dissolveNoise = FindProperty("_DissolveNoise", allProperties, false);
            dissolveAmount = FindProperty("_DissolveAmount", allProperties, false);
            dissolveEdgeWidth = FindProperty("_DissolveEdgeWidth", allProperties, false);
            dissolveEdgeColor = FindProperty("_DissolveEdgeColor", allProperties, false);
            
            // Animation
            enableAnimatedProperties = FindProperty("_EnableAnimatedProperties", allProperties, false);
            animationSpeed = FindProperty("_AnimationSpeed", allProperties, false);
            enableVertexAnimation = FindProperty("_EnableVertexAnimation", allProperties, false);
            vertexAnimationIntensity = FindProperty("_VertexAnimationIntensity", allProperties, false);
            vertexAnimationFrequency = FindProperty("_VertexAnimationFrequency", allProperties, false);
            
            // Performance
            qualityLevel = FindProperty("_QualityLevel", allProperties, false);
            enableLODFade = FindProperty("_EnableLODFade", allProperties, false);
            lodFadeDistance = FindProperty("_LODFadeDistance", allProperties, false);
            enableInstancing = FindProperty("_EnableInstancing", allProperties, false);
            
            // Advanced rendering
            cutoff = FindProperty("_Cutoff", allProperties, false);
            cull = FindProperty("_Cull", allProperties, false);
            zwrite = FindProperty("_ZWrite", allProperties, false);
            ztest = FindProperty("_ZTest", allProperties, false);
            srcBlend = FindProperty("_SrcBlend", allProperties, false);
            dstBlend = FindProperty("_DstBlend", allProperties, false);
            
            // Debug
            debugMode = FindProperty("_DebugMode", allProperties, false);
            debugView = FindProperty("_DebugView", allProperties, false);
            showWireframe = FindProperty("_ShowWireframe", allProperties, false);
            wireframeColor = FindProperty("_WireframeColor", allProperties, false);
            wireframeThickness = FindProperty("_WireframeThickness", allProperties, false);
        }
        
        private void DrawHeader()
        {
            LoadLogo();
            
            if (logoTexture != null)
            {
                var rect = GUILayoutUtility.GetRect(0, 80, GUILayout.ExpandWidth(true));
                GUI.DrawTexture(rect, logoTexture, ScaleMode.ScaleToFit);
            }
            
            EditorGUILayout.LabelField("GORGONIZE TOON SHADER v4.0", CreateHeaderStyle());
            EditorGUILayout.LabelField("Unity 6 URP - Professional NPR Solution", CreateVersionStyle());
        }
        
        private void LoadLogo()
        {
            if (logoTexture != null) return;
            
            // Try to find logo in the same directory as this script
            string[] guids = AssetDatabase.FindAssets("t:Texture2D gorgonize_logo");
            if (guids.Length > 0)
            {
                string logoPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                logoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(logoPath);
            }
        }
        
        private void DrawPresetSystem(MaterialEditor materialEditor, Material material)
        {
            BeginStyledBox();
            EditorGUILayout.LabelField("Quick Style Presets", CreateBoldLabelStyle());
            
            var presetNames = GetBuiltInPresetNames();
            
            // First row
            BeginHorizontalLayout();
            for (int i = 0; i < 3 && i < presetNames.Length; i++)
            {
                if (DrawPresetButton(presetNames[i]))
                {
                    ApplyPresetByIndex(material, materialEditor, i);
                }
            }
            EndHorizontalLayout();
            
            // Second row
            if (presetNames.Length > 3)
            {
                BeginHorizontalLayout();
                for (int i = 3; i < 6 && i < presetNames.Length; i++)
                {
                    if (DrawPresetButton(presetNames[i]))
                    {
                        ApplyPresetByIndex(material, materialEditor, i);
                    }
                }
                EndHorizontalLayout();
            }
            
            // Third row
            if (presetNames.Length > 6)
            {
                BeginHorizontalLayout();
                for (int i = 6; i < presetNames.Length; i++)
                {
                    if (DrawPresetButton(presetNames[i]))
                    {
                        ApplyPresetByIndex(material, materialEditor, i);
                    }
                }
                EndHorizontalLayout();
            }
            
            DrawInfoBox("Click any preset to instantly apply predefined settings optimized for different art styles.", MessageType.Info);
            EndStyledBox();
        }
        
        private string[] GetBuiltInPresetNames()
        {
            return new string[]
            {
                "Anime Classic",
                "Cartoon Bold", 
                "Sketch Style",
                "Comic Book",
                "Hatched Drawing",
                "Realistic Toon",
                "Painterly"
            };
        }
        
        private void DrawPerformanceSection(Material material)
        {
            showPerformanceInfo = DrawStyledFoldout(showPerformanceInfo, "Performance Monitor", "⚡");
            
            if (showPerformanceInfo)
            {
                BeginStyledBox();
                
                // Calculate active features
                int activeFeatures = CountActiveFeatures(material);
                int totalFeatures = 12; // Total available features
                
                DrawFeatureCount(activeFeatures, totalFeatures);
                
                // Performance cost estimation
                float performanceCost = EstimatePerformanceCost(material);
                DrawPerformanceBar(performanceCost, "Estimated Cost");
                
                // Quality recommendation
                string recommendation = GetQualityRecommendation(performanceCost);
                if (!string.IsNullOrEmpty(recommendation))
                {
                    DrawInfoBox(recommendation, MessageType.Info);
                }
                
                EndStyledBox();
            }
        }
        
        private void DrawBaseProperties(MaterialEditor materialEditor)
        {
            showBase = DrawStyledFoldout(showBase, "Base Properties", "🎨");
            
            if (showBase)
            {
                BeginStyledBox();
                
                if (baseMap != null && baseColor != null)
                    materialEditor.TexturePropertySingleLine(new GUIContent("Albedo", "Main color texture"), baseMap, baseColor);
                else if (baseColor != null)
                    materialEditor.ColorProperty(baseColor, "Base Color");
                
                if (saturation != null)
                    materialEditor.RangeProperty(saturation, "Saturation");
                
                if (brightness != null)
                    materialEditor.RangeProperty(brightness, "Brightness");
                
                if (metallic != null)
                    materialEditor.RangeProperty(metallic, "Metallic");
                
                if (smoothness != null)
                    materialEditor.RangeProperty(smoothness, "Smoothness");
                
                if (occlusionStrength != null)
                    materialEditor.RangeProperty(occlusionStrength, "Occlusion Strength");
                
                EndStyledBox();
            }
        }
        
        private void DrawLightingSection(MaterialEditor materialEditor, Material material)
        {
            showLighting = DrawStyledFoldout(showLighting, "Toon Lighting", "💡");
            
            if (showLighting)
            {
                BeginStyledBox();
                
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
                showAdvancedLighting = DrawStyledFoldout(showAdvancedLighting, "Advanced Lighting", "⚡");
                if (showAdvancedLighting)
                {
                    EditorGUI.indentLevel++;
                    if (indirectLightingBoost != null)
                        materialEditor.RangeProperty(indirectLightingBoost, "Indirect Lighting Boost");
                    if (lightWrapping != null)
                        materialEditor.RangeProperty(lightWrapping, "Light Wrapping");
                    if (volumetricIntensity != null)
                        materialEditor.RangeProperty(volumetricIntensity, "Volumetric Intensity");
                    if (cookieInfluence != null)
                        materialEditor.RangeProperty(cookieInfluence, "Cookie Influence");
                    EditorGUI.indentLevel--;
                }
                
                EndStyledBox();
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
            DrawNormalMapSection(materialEditor, material);
            DrawDetailSection(materialEditor, material);
            DrawEmissionSection(materialEditor, material);
            DrawFresnelSection(materialEditor, material);
            DrawSubsurfaceSection(materialEditor, material);
            DrawOutlineSection(materialEditor, material);
        }
        
        private void DrawRimLightingSection(MaterialEditor materialEditor, Material material)
        {
            if (enableRimLighting == null) return;
            
            showRimLighting = DrawStyledFoldout(showRimLighting, "Rim Lighting", "🌟");
            
            if (showRimLighting)
            {
                BeginStyledBox();
                
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
                    
                    // Dual layer rim
                    if (rimColorSecondary != null && rimPowerSecondary != null)
                    {
                        EditorGUILayout.LabelField("Dual Layer Rim", EditorStyles.boldLabel);
                        materialEditor.ColorProperty(rimColorSecondary, "Secondary Rim Color");
                        materialEditor.RangeProperty(rimPowerSecondary, "Secondary Rim Power");
                        SetKeywordSafe(material, "_RIM_LIGHTING_DUAL_LAYER", true);
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                EndStyledBox();
            }
        }
        
        private void DrawSpecularSection(MaterialEditor materialEditor, Material material)
        {
            if (enableSpecular == null) return;
            
            showSpecular = DrawStyledFoldout(showSpecular, "Specular Highlights", "✨");
            
            if (showSpecular)
            {
                BeginStyledBox();
                
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

                    // Advanced specular options
                    if (anisotropy != null)
                    {
                        materialEditor.RangeProperty(anisotropy, "Anisotropy");
                        SetKeywordSafe(material, "_SPECULAR_ANISOTROPIC", Mathf.Abs(anisotropy.floatValue) > 0.001f);
                    }
                    
                    if (specularSteps != null)
                    {
                        materialEditor.RangeProperty(specularSteps, "Specular Steps");
                        SetKeywordSafe(material, "_SPECULAR_STEPPED", specularSteps.floatValue > 1);
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                EndStyledBox();
            }
        }
        
        private void DrawHatchingSection(MaterialEditor materialEditor, Material material)
        {
            if (enableHatching == null) return;
            
            showHatching = DrawStyledFoldout(showHatching, "Hatching Effects", "🖊️");
            
            if (showHatching)
            {
                BeginStyledBox();
                
                materialEditor.ShaderProperty(enableHatching, "Enable Hatching");
                SetKeywordSafe(material, "_HATCHING", enableHatching.floatValue > 0);
                
                if (enableHatching.floatValue > 0)
                {
                    EditorGUI.indentLevel++;
                    
                    // Textures
                    if (hatchingTex != null)
                        materialEditor.TexturePropertySingleLine(new GUIContent("Primary Hatching", "Main hatching pattern"), hatchingTex);
                    if (crossHatchingTex != null)
                        materialEditor.TexturePropertySingleLine(new GUIContent("Cross Hatching", "Cross hatching for deeper shadows"), crossHatchingTex);
                    if (hatchingTex2 != null)
                        materialEditor.TexturePropertySingleLine(new GUIContent("Secondary Hatching", "Additional hatching layer"), hatchingTex2);
                    
                    EditorGUILayout.Space(3f);
                    
                    // Parameters
                    if (hatchingDensity != null)
                        materialEditor.RangeProperty(hatchingDensity, "Hatching Density");
                    if (hatchingIntensity != null)
                        materialEditor.RangeProperty(hatchingIntensity, "Hatching Intensity");
                    if (hatchingThreshold != null)
                        materialEditor.RangeProperty(hatchingThreshold, "Primary Threshold");
                    if (crossHatchingThreshold != null)
                        materialEditor.RangeProperty(crossHatchingThreshold, "Cross Hatch Threshold");
                    if (secondaryHatchingThreshold != null)
                        materialEditor.RangeProperty(secondaryHatchingThreshold, "Secondary Threshold");
                    if (hatchingRotation != null)
                        materialEditor.RangeProperty(hatchingRotation, "Hatching Rotation");
                    
                    // Animation
                    if (hatchingAnimSpeed != null)
                    {
                        materialEditor.RangeProperty(hatchingAnimSpeed, "Animation Speed");
                        SetKeywordSafe(material, "_HATCHING_ANIMATED", hatchingAnimSpeed.floatValue > 0.001f);
                    }
                    
                    EditorGUILayout.Space(5f);
                    
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
                
                DrawInfoBox("Hatching adds sketch-like crosshatch patterns based on lighting. Lower thresholds = more hatching in dark areas.", MessageType.Info);
                EndStyledBox();
            }
        }
        
        private void DrawMatcapSection(MaterialEditor materialEditor, Material material)
        {
            if (enableMatcap == null) return;
            
            showMatcap = DrawStyledFoldout(showMatcap, "Matcap", "🎭");
            
            if (showMatcap)
            {
                BeginStyledBox();
                
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
                        string[] blendModes = {"Add", "Multiply", "Screen", "Overlay"};
                        int blendMode = (int)matcapBlendMode.floatValue;
                        blendMode = EditorGUILayout.Popup("Blend Mode", blendMode, blendModes);
                        matcapBlendMode.floatValue = blendMode;
                    }
                    
                    if (matcapRotation != null)
                        materialEditor.RangeProperty(matcapRotation, "Matcap Rotation");
                    
                    SetKeywordSafe(material, "_MATCAP_PERSPECTIVE_CORRECTION", true);
                    
                    EditorGUI.indentLevel--;
                }
                
                EndStyledBox();
            }
        }
        
        private void DrawNormalMapSection(MaterialEditor materialEditor, Material material)
        {
            if (enableNormalMap == null) return;
            
            showNormalMap = DrawStyledFoldout(showNormalMap, "Normal Mapping", "🗺️");
            
            if (showNormalMap)
            {
                BeginStyledBox();
                
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
                
                EndStyledBox();
            }
        }
        
        private void DrawDetailSection(MaterialEditor materialEditor, Material material)
        {
            if (enableDetail == null) return;
            
            showDetail = DrawStyledFoldout(showDetail, "Detail Mapping", "🔍");
            
            if (showDetail)
            {
                BeginStyledBox();
                
                materialEditor.ShaderProperty(enableDetail, "Enable Detail");
                SetKeywordSafe(material, "_DETAIL", enableDetail.floatValue > 0);
                
                if (enableDetail.floatValue > 0)
                {
                    EditorGUI.indentLevel++;
                    if (detailMap != null)
                        materialEditor.TexturePropertySingleLine(new GUIContent("Detail Albedo"), detailMap);
                    if (detailNormalMap != null)
                        materialEditor.TexturePropertySingleLine(new GUIContent("Detail Normal"), detailNormalMap);
                    if (detailScale != null)
                        materialEditor.RangeProperty(detailScale, "Detail Scale");
                    if (detailNormalScale != null)
                        materialEditor.RangeProperty(detailNormalScale, "Detail Normal Scale");
                    EditorGUI.indentLevel--;
                }
                
                EndStyledBox();
            }
        }
        
        private void DrawEmissionSection(MaterialEditor materialEditor, Material material)
        {
            if (enableEmission == null) return;
            
            showEmission = DrawStyledFoldout(showEmission, "Emission", "🔥");
            
            if (showEmission)
            {
                BeginStyledBox();
                
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
                    
                    // Pulsing emission
                    if (emissionPulseSpeed != null && emissionPulseIntensity != null)
                    {
                        EditorGUILayout.LabelField("Pulsing Emission", EditorStyles.boldLabel);
                        materialEditor.RangeProperty(emissionPulseSpeed, "Pulse Speed");
                        materialEditor.RangeProperty(emissionPulseIntensity, "Pulse Intensity");
                        SetKeywordSafe(material, "_EMISSION_PULSING", emissionPulseSpeed.floatValue > 0.001f);
                    }
                    
                    // Temperature-based emission
                    if (emissionTemperature != null)
                    {
                        materialEditor.RangeProperty(emissionTemperature, "Temperature (K)");
                        SetKeywordSafe(material, "_EMISSION_TEMPERATURE_BASED", emissionTemperature.floatValue != 6500f);
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                EndStyledBox();
            }
        }
        
        private void DrawFresnelSection(MaterialEditor materialEditor, Material material)
        {
            if (enableFresnel == null) return;
            
            showFresnel = DrawStyledFoldout(showFresnel, "Fresnel Effect", "🌀");
            
            if (showFresnel)
            {
                BeginStyledBox();
                
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
                    
                    // Iridescence
                    if (iridescenceIntensity != null)
                    {
                        materialEditor.RangeProperty(iridescenceIntensity, "Iridescence Intensity");
                        SetKeywordSafe(material, "_FRESNEL_IRIDESCENCE", iridescenceIntensity.floatValue > 0.001f);
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                EndStyledBox();
            }
        }
        
        private void DrawSubsurfaceSection(MaterialEditor materialEditor, Material material)
        {
            if (enableSubsurface == null) return;
            
            showSubsurface = DrawStyledFoldout(showSubsurface, "Subsurface Scattering", "🌸");
            
            if (showSubsurface)
            {
                BeginStyledBox();
                
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
                    if (subsurfaceDistortion != null)
                        materialEditor.RangeProperty(subsurfaceDistortion, "Subsurface Distortion");
                    if (subsurfaceThickness != null)
                        materialEditor.TexturePropertySingleLine(new GUIContent("Thickness Map"), subsurfaceThickness);
                    EditorGUI.indentLevel--;
                }
                
                EndStyledBox();
            }
        }
        
        private void DrawOutlineSection(MaterialEditor materialEditor, Material material)
        {
            if (enableOutline == null) return;
            
            showOutline = DrawStyledFoldout(showOutline, "Outline", "🖼️");
            
            if (showOutline)
            {
                BeginStyledBox();
                
                materialEditor.ShaderProperty(enableOutline, "Enable Outline");
                SetKeywordSafe(material, "_OUTLINE", enableOutline.floatValue > 0);
                
                if (enableOutline.floatValue > 0)
                {
                    EditorGUI.indentLevel++;
                    if (outlineColor != null)
                        materialEditor.ColorProperty(outlineColor, "Outline Color");
                    if (outlineWidth != null)
                        materialEditor.RangeProperty(outlineWidth, "Outline Width");
                    
                    if (outlineMode != null)
                    {
                        string[] outlineModes = {"Normal", "Position", "Clip"};
                        int mode = (int)outlineMode.floatValue;
                        mode = EditorGUILayout.Popup("Outline Mode", mode, outlineModes);
                        outlineMode.floatValue = mode;
                    }
                    
                    if (outlineFadeDistance != null)
                    {
                        materialEditor.RangeProperty(outlineFadeDistance, "Fade Distance");
                        SetKeywordSafe(material, "_OUTLINE_DISTANCE_FADE", outlineFadeDistance.floatValue > 0.001f);
                    }
                    
                    if (outlineDepthBiasValue != null)
                    {
                        materialEditor.RangeProperty(outlineDepthBiasValue, "Depth Bias Value");
                        SetKeywordSafe(material, "_OUTLINE_DEPTH_BIAS", outlineDepthBiasValue.floatValue > 0.001f);
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                EndStyledBox();
            }
        }
        
        private void DrawStylizationSection(MaterialEditor materialEditor, Material material)
        {
            showColorGrading = DrawStyledFoldout(showColorGrading, "Color Grading", "🎨");
            
            if (showColorGrading)
            {
                BeginStyledBox();
                
                if (hue != null)
                    materialEditor.RangeProperty(hue, "Hue Shift");
                if (contrast != null)
                    materialEditor.RangeProperty(contrast, "Contrast");
                if (gamma != null)
                    materialEditor.RangeProperty(gamma, "Gamma");
                if (colorTemperature != null)
                    materialEditor.RangeProperty(colorTemperature, "Color Temperature");
                if (colorTint != null)
                    materialEditor.RangeProperty(colorTint, "Color Tint");
                if (vibrance != null)
                    materialEditor.RangeProperty(vibrance, "Vibrance");
                
                EndStyledBox();
            }
            
            showStylization = DrawStyledFoldout(showStylization, "Stylization", "🎭");
            
            if (showStylization)
            {
                BeginStyledBox();
                
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
                
                // Dithering
                if (enableDithering != null)
                {
                    materialEditor.ShaderProperty(enableDithering, "Enable Dithering");
                    SetKeywordSafe(material, "_DITHERING", enableDithering.floatValue > 0);
                    
                    if (enableDithering.floatValue > 0 && ditheringIntensity != null)
                    {
                        EditorGUI.indentLevel++;
                        materialEditor.RangeProperty(ditheringIntensity, "Dithering Intensity");
                        EditorGUI.indentLevel--;
                    }
                }
                
                EndStyledBox();
            }
        }
        
        private void DrawAdvancedEffectsSection(MaterialEditor materialEditor, Material material)
        {
            showAdvancedEffects = DrawStyledFoldout(showAdvancedEffects, "Advanced Effects", "✨");
            
            if (showAdvancedEffects)
            {
                // Force Field Effect
                BeginStyledBox();
                EditorGUILayout.LabelField("Force Field Effect", CreateBoldLabelStyle());
                
                if (enableForceField != null)
                {
                    materialEditor.ShaderProperty(enableForceField, "Enable Force Field");
                    SetKeywordSafe(material, "_FORCE_FIELD", enableForceField.floatValue > 0);
                    
                    if (enableForceField.floatValue > 0)
                    {
                        EditorGUI.indentLevel++;
                        if (forceFieldColor != null)
                            materialEditor.ColorProperty(forceFieldColor, "Force Field Color");
                        if (forceFieldIntensity != null)
                            materialEditor.RangeProperty(forceFieldIntensity, "Force Field Intensity");
                        if (forceFieldFrequency != null)
                            materialEditor.RangeProperty(forceFieldFrequency, "Force Field Frequency");
                        EditorGUI.indentLevel--;
                    }
                }
                EndStyledBox();
                
                // Hologram Effect
                BeginStyledBox();
                EditorGUILayout.LabelField("Hologram Effect", CreateBoldLabelStyle());
                
                if (enableHologram != null)
                {
                    materialEditor.ShaderProperty(enableHologram, "Enable Hologram");
                    SetKeywordSafe(material, "_HOLOGRAM", enableHologram.floatValue > 0);
                    
                    if (enableHologram.floatValue > 0)
                    {
                        EditorGUI.indentLevel++;
                        if (hologramIntensity != null)
                            materialEditor.RangeProperty(hologramIntensity, "Hologram Intensity");
                        if (hologramFlicker != null)
                            materialEditor.RangeProperty(hologramFlicker, "Hologram Flicker");
                        if (hologramScanlines != null)
                            materialEditor.RangeProperty(hologramScanlines, "Hologram Scanlines");
                        EditorGUI.indentLevel--;
                    }
                }
                EndStyledBox();
                
                // Dissolve Effect
                BeginStyledBox();
                EditorGUILayout.LabelField("Dissolve Effect", CreateBoldLabelStyle());
                
                if (enableDissolve != null)
                {
                    materialEditor.ShaderProperty(enableDissolve, "Enable Dissolve");
                    SetKeywordSafe(material, "_DISSOLVE", enableDissolve.floatValue > 0);
                    
                    if (enableDissolve.floatValue > 0)
                    {
                        EditorGUI.indentLevel++;
                        if (dissolveNoise != null)
                            materialEditor.TexturePropertySingleLine(new GUIContent("Dissolve Noise"), dissolveNoise);
                        if (dissolveAmount != null)
                            materialEditor.RangeProperty(dissolveAmount, "Dissolve Amount");
                        if (dissolveEdgeWidth != null)
                            materialEditor.RangeProperty(dissolveEdgeWidth, "Dissolve Edge Width");
                        if (dissolveEdgeColor != null)
                            materialEditor.ColorProperty(dissolveEdgeColor, "Dissolve Edge Color");
                        EditorGUI.indentLevel--;
                    }
                }
                EndStyledBox();
            }
        }
        
        private void DrawAnimationSection(MaterialEditor materialEditor, Material material)
        {
            showAnimation = DrawStyledFoldout(showAnimation, "Animation System", "🎬");
            
            if (showAnimation)
            {
                BeginStyledBox();
                
                if (enableAnimatedProperties != null)
                {
                    materialEditor.ShaderProperty(enableAnimatedProperties, "Enable Animations");
                    SetKeywordSafe(material, "_ANIMATED_PROPERTIES", enableAnimatedProperties.floatValue > 0);
                    
                    if (enableAnimatedProperties.floatValue > 0 && animationSpeed != null)
                    {
                        EditorGUI.indentLevel++;
                        materialEditor.RangeProperty(animationSpeed, "Global Animation Speed");
                        EditorGUI.indentLevel--;
                    }
                }
                
                if (enableVertexAnimation != null)
                {
                    materialEditor.ShaderProperty(enableVertexAnimation, "Enable Vertex Animation");
                    SetKeywordSafe(material, "_VERTEX_ANIMATION", enableVertexAnimation.floatValue > 0);
                    
                    if (enableVertexAnimation.floatValue > 0)
                    {
                        EditorGUI.indentLevel++;
                        if (vertexAnimationIntensity != null)
                            materialEditor.RangeProperty(vertexAnimationIntensity, "Vertex Animation Intensity");
                        if (vertexAnimationFrequency != null)
                            materialEditor.RangeProperty(vertexAnimationFrequency, "Vertex Animation Frequency");
                        EditorGUI.indentLevel--;
                    }
                }
                
                EndStyledBox();
            }
        }
        
        private void DrawPerformancePropertiesSection(MaterialEditor materialEditor, Material material)
        {
            showPerformance = DrawStyledFoldout(showPerformance, "Performance & Quality", "⚡");
            
            if (showPerformance)
            {
                BeginStyledBox();
                
                if (qualityLevel != null)
                {
                    string[] qualityNames = { "Low", "Medium", "High", "Ultra" };
                    int quality = (int)qualityLevel.floatValue;
                    quality = EditorGUILayout.Popup("Quality Level", quality, qualityNames);
                    qualityLevel.floatValue = quality;
                }
                
                if (enableLODFade != null)
                {
                    materialEditor.ShaderProperty(enableLODFade, "Enable LOD Fade");
                    SetKeywordSafe(material, "_LOD_FADE", enableLODFade.floatValue > 0);
                    
                    if (enableLODFade.floatValue > 0 && lodFadeDistance != null)
                    {
                        EditorGUI.indentLevel++;
                        materialEditor.RangeProperty(lodFadeDistance, "LOD Fade Distance");
                        EditorGUI.indentLevel--;
                    }
                }
                
                if (enableInstancing != null)
                {
                    materialEditor.ShaderProperty(enableInstancing, "GPU Instancing");
                    SetKeywordSafe(material, "_INSTANCING_SUPPORT", enableInstancing.floatValue > 0);
                }
                
                EndStyledBox();
            }
        }
        
        private void DrawAdvancedSection(MaterialEditor materialEditor, Material material)
        {
            showAdvanced = DrawStyledFoldout(showAdvanced, "Advanced Rendering", "⚙️");
            
            if (showAdvanced)
            {
                BeginStyledBox();
                
                if (cutoff != null)
                    materialEditor.RangeProperty(cutoff, "Alpha Cutoff");
                if (cull != null)
                    materialEditor.ShaderProperty(cull, "Cull Mode");
                if (zwrite != null)
                    materialEditor.ShaderProperty(zwrite, "Z Write");
                if (ztest != null)
                    materialEditor.ShaderProperty(ztest, "Z Test");
                if (srcBlend != null)
                    materialEditor.ShaderProperty(srcBlend, "Src Blend");
                if (dstBlend != null)
                    materialEditor.ShaderProperty(dstBlend, "Dst Blend");
                
                EndStyledBox();
            }
        }
        
        private void DrawDebugSection(MaterialEditor materialEditor, Material material)
        {
            showDebug = DrawStyledFoldout(showDebug, "Debug & Visualization", "🔍");
            
            if (showDebug)
            {
                BeginStyledBox();
                
                if (debugMode != null)
                {
                    materialEditor.ShaderProperty(debugMode, "Debug Mode");
                    SetKeywordSafe(material, "_DEBUG_MODE", debugMode.floatValue > 0);
                    
                    if (debugMode.floatValue > 0 && debugView != null)
                    {
                        EditorGUI.indentLevel++;
                        string[] debugViews = {"None", "Normals", "Lighting", "Shadows", "Hatching"};
                        int view = (int)debugView.floatValue;
                        view = EditorGUILayout.Popup("Debug View", view, debugViews);
                        debugView.floatValue = view;
                        EditorGUI.indentLevel--;
                    }
                }
                
                if (showWireframe != null)
                {
                    materialEditor.ShaderProperty(showWireframe, "Show Wireframe");
                    SetKeywordSafe(material, "_WIREFRAME", showWireframe.floatValue > 0);
                    
                    if (showWireframe.floatValue > 0)
                    {
                        EditorGUI.indentLevel++;
                        if (wireframeColor != null)
                            materialEditor.ColorProperty(wireframeColor, "Wireframe Color");
                        if (wireframeThickness != null)
                            materialEditor.RangeProperty(wireframeThickness, "Wireframe Thickness");
                        EditorGUI.indentLevel--;
                    }
                }
                
                EndStyledBox();
            }
        }
        
        private void DrawFooter()
        {
            BeginStyledBox();
            EditorGUILayout.LabelField("Shader Information", CreateBoldLabelStyle());
            
            BeginHorizontalLayout();
            EditorGUILayout.LabelField("Unity 6 URP Compatible", CreateSmallLabelStyle());
            EditorGUILayout.LabelField("Mobile Optimized", CreateSmallLabelStyle());
            EndHorizontalLayout();
            
            BeginHorizontalLayout();
            EditorGUILayout.LabelField("Advanced NPR Features", CreateSmallLabelStyle());
            EditorGUILayout.LabelField("AAA Quality Rendering", CreateSmallLabelStyle());
            EndHorizontalLayout();
            
            EditorGUILayout.Space(5f);
            DrawInfoBox("Use the preset buttons for quick setup, then fine-tune individual properties below!", MessageType.Info);
            EndStyledBox();
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
            // Apply preset settings based on index
            switch (index)
            {
                case 0: // Anime Classic
                    ApplyAnimePreset(material);
                    break;
                case 1: // Cartoon Bold
                    ApplyCartoonPreset(material);
                    break;
                case 2: // Sketch Style
                    ApplySketchPreset(material);
                    break;
                case 3: // Comic Book
                    ApplyComicPreset(material);
                    break;
                case 4: // Hatched Drawing
                    ApplyHatchedPreset(material);
                    break;
                case 5: // Realistic Toon
                    ApplyRealisticPreset(material);
                    break;
                case 6: // Painterly
                    ApplyPainterlyPreset(material);
                    break;
            }
            
            materialEditor.PropertiesChanged();
        }
        
        private void ApplyAnimePreset(Material material)
        {
            // Anime style settings
            if (baseColor != null) baseColor.colorValue = Color.white;
            if (shadowThreshold != null) shadowThreshold.floatValue = 0.4f;
            if (shadowSmoothness != null) shadowSmoothness.floatValue = 0.1f;
            if (shadowColor != null) shadowColor.colorValue = new Color(0.8f, 0.8f, 0.9f, 1f);
            if (shadowIntensity != null) shadowIntensity.floatValue = 0.7f;
            
            // Enable rim lighting
            if (enableRimLighting != null) enableRimLighting.floatValue = 1f;
            SetKeywordSafe(material, "_RIM_LIGHTING", true);
            if (rimColor != null) rimColor.colorValue = new Color(1f, 0.95f, 0.8f, 1f);
            if (rimPower != null) rimPower.floatValue = 1.5f;
            if (rimIntensity != null) rimIntensity.floatValue = 2f;
            
            // Enable specular
            if (enableSpecular != null) enableSpecular.floatValue = 1f;
            SetKeywordSafe(material, "_SPECULAR", true);
            if (specularSize != null) specularSize.floatValue = 0.05f;
            if (specularIntensity != null) specularIntensity.floatValue = 2f;
            
            // Disable hatching
            if (enableHatching != null) enableHatching.floatValue = 0f;
            SetKeywordSafe(material, "_HATCHING", false);
        }
        
        private void ApplyCartoonPreset(Material material)
        {
            // Cartoon style settings
            if (baseColor != null) baseColor.colorValue = Color.white;
            if (shadowThreshold != null) shadowThreshold.floatValue = 0.6f;
            if (shadowSmoothness != null) shadowSmoothness.floatValue = 0.02f;
            if (shadowColor != null) shadowColor.colorValue = new Color(0.6f, 0.6f, 0.8f, 1f);
            if (shadowIntensity != null) shadowIntensity.floatValue = 0.9f;
            
            // Enable rim lighting
            if (enableRimLighting != null) enableRimLighting.floatValue = 1f;
            SetKeywordSafe(material, "_RIM_LIGHTING", true);
            if (rimIntensity != null) rimIntensity.floatValue = 3f;
            
            // Enable outline
            if (enableOutline != null) enableOutline.floatValue = 1f;
            SetKeywordSafe(material, "_OUTLINE", true);
            if (outlineColor != null) outlineColor.colorValue = Color.black;
            if (outlineWidth != null) outlineWidth.floatValue = 0.02f;
            
            // Enable cel shading
            if (enableCelShading != null) enableCelShading.floatValue = 1f;
            SetKeywordSafe(material, "_CEL_SHADING", true);
            if (celShadingSteps != null) celShadingSteps.floatValue = 4f;
        }
        
        private void ApplySketchPreset(Material material)
        {
            // Sketch style settings
            if (baseColor != null) baseColor.colorValue = Color.white;
            if (shadowThreshold != null) shadowThreshold.floatValue = 0.4f;
            if (shadowSmoothness != null) shadowSmoothness.floatValue = 0.05f;
            if (shadowColor != null) shadowColor.colorValue = new Color(0.95f, 0.95f, 0.95f, 1f);
            
            // Enable hatching
            if (enableHatching != null) enableHatching.floatValue = 1f;
            SetKeywordSafe(material, "_HATCHING", true);
            if (hatchingDensity != null) hatchingDensity.floatValue = 2f;
            if (hatchingIntensity != null) hatchingIntensity.floatValue = 0.8f;
            if (hatchingThreshold != null) hatchingThreshold.floatValue = 0.6f;
            if (crossHatchingThreshold != null) crossHatchingThreshold.floatValue = 0.3f;
            if (hatchingRotation != null) hatchingRotation.floatValue = 45f;
            
            // Minimal rim lighting
            if (enableRimLighting != null) enableRimLighting.floatValue = 1f;
            SetKeywordSafe(material, "_RIM_LIGHTING", true);
            if (rimIntensity != null) rimIntensity.floatValue = 1f;
            if (rimColor != null) rimColor.colorValue = new Color(0.8f, 0.8f, 0.8f, 1f);
        }
        
        private void ApplyComicPreset(Material material)
        {
            // Comic book style
            ApplyCartoonPreset(material); // Start with cartoon base
            
            // Enhance with posterization
            if (enablePosterize != null) enablePosterize.floatValue = 1f;
            SetKeywordSafe(material, "_POSTERIZE", true);
            if (posterizeLevels != null) posterizeLevels.floatValue = 8f;
            
            // Enhanced contrast
            if (contrast != null) contrast.floatValue = 1.4f;
            if (saturation != null) saturation.floatValue = 1.3f;
        }
        
        private void ApplyHatchedPreset(Material material)
        {
            ApplySketchPreset(material); // Start with sketch base
            
            // Enable screen space hatching
            if (enableScreenSpaceHatching != null) enableScreenSpaceHatching.floatValue = 1f;
            SetKeywordSafe(material, "_SCREEN_SPACE_HATCHING", true);
            if (screenHatchScale != null) screenHatchScale.floatValue = 3f;
            if (screenHatchBias != null) screenHatchBias.floatValue = 0.2f;
        }
        
        private void ApplyRealisticPreset(Material material)
        {
            // Realistic toon settings
            if (baseColor != null) baseColor.colorValue = Color.white;
            if (shadowThreshold != null) shadowThreshold.floatValue = 0.3f;
            if (shadowSmoothness != null) shadowSmoothness.floatValue = 0.2f;
            if (shadowColor != null) shadowColor.colorValue = new Color(0.7f, 0.7f, 0.8f, 1f);
            if (shadowIntensity != null) shadowIntensity.floatValue = 0.6f;
            
            // Enable subsurface scattering
            if (enableSubsurface != null) enableSubsurface.floatValue = 1f;
            SetKeywordSafe(material, "_SUBSURFACE", true);
            if (subsurfaceColor != null) subsurfaceColor.colorValue = new Color(1f, 0.7f, 0.7f, 1f);
            if (subsurfaceIntensity != null) subsurfaceIntensity.floatValue = 0.3f;
            
            // Enable fresnel
            if (enableFresnel != null) enableFresnel.floatValue = 1f;
            SetKeywordSafe(material, "_FRESNEL", true);
            if (fresnelIntensity != null) fresnelIntensity.floatValue = 0.5f;
            
            // Boost indirect lighting
            if (indirectLightingBoost != null) indirectLightingBoost.floatValue = 0.6f;
        }
        
        private void ApplyPainterlyPreset(Material material)
        {
            // Painterly style
            if (baseColor != null) baseColor.colorValue = Color.white;
            if (shadowThreshold != null) shadowThreshold.floatValue = 0.35f;
            if (shadowSmoothness != null) shadowSmoothness.floatValue = 0.3f;
            if (shadowColor != null) shadowColor.colorValue = new Color(0.6f, 0.7f, 0.8f, 1f);
            if (indirectLightingBoost != null) indirectLightingBoost.floatValue = 0.4f;
            
            // Enable matcap
            if (enableMatcap != null) enableMatcap.floatValue = 1f;
            SetKeywordSafe(material, "_MATCAP", true);
            if (matcapIntensity != null) matcapIntensity.floatValue = 0.8f;
            if (matcapBlendMode != null) matcapBlendMode.floatValue = 1f; // Multiply
            
            // Enable fresnel
            if (enableFresnel != null) enableFresnel.floatValue = 1f;
            SetKeywordSafe(material, "_FRESNEL", true);
            if (fresnelIntensity != null) fresnelIntensity.floatValue = 1.5f;
            if (fresnelPower != null) fresnelPower.floatValue = 2f;
            
            // Enable posterization
            if (enablePosterize != null) enablePosterize.floatValue = 1f;
            SetKeywordSafe(material, "_POSTERIZE", true);
            if (posterizeLevels != null) posterizeLevels.floatValue = 6f;
            
            // Color adjustments
            if (saturation != null) saturation.floatValue = 1.1f;
            if (brightness != null) brightness.floatValue = 1.1f;
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
            if (material.IsKeywordEnabled("_FORCE_FIELD")) cost += 0.15f;
            if (material.IsKeywordEnabled("_HOLOGRAM")) cost += 0.1f;
            if (material.IsKeywordEnabled("_DISSOLVE")) cost += 0.05f;
            
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
            
            return "Good performance balance for desktop and console platforms.";
        }
        
        #endregion
        
        #region GUI Helper Methods (Simplified versions of ToonGUIStyles)
        
        private GUIStyle CreateHeaderStyle()
        {
            return new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 18,
                normal = { textColor = new Color(0.3f, 0.7f, 1f, 1f) },
                wordWrap = true
            };
        }
        
        private GUIStyle CreateVersionStyle()
        {
            return new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic,
                fontSize = 10,
                normal = { textColor = Color.gray }
            };
        }
        
        private GUIStyle CreateBoldLabelStyle()
        {
            return new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold
            };
        }
        
        private GUIStyle CreateSmallLabelStyle()
        {
            return new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                normal = { textColor = Color.gray }
            };
        }
        
        private bool DrawStyledFoldout(bool foldout, string content, string emoji = "")
        {
            var style = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };
            
            return EditorGUILayout.Foldout(foldout, $"{emoji} {content}", style);
        }
        
        private bool DrawPresetButton(string text)
        {
            var style = new GUIStyle(GUI.skin.button)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                fixedHeight = 25,
                normal = { textColor = Color.white }
            };
            
            return GUILayout.Button(text, style);
        }
        
        private void BeginStyledBox()
        {
            var style = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(5, 5, 5, 5)
            };
            
            EditorGUILayout.BeginVertical(style);
        }
        
        private void EndStyledBox()
        {
            EditorGUILayout.EndVertical();
        }
        
        private void BeginHorizontalLayout()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5);
        }
        
        private void EndHorizontalLayout()
        {
            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawSeparator()
        {
            GUILayout.Space(5f);
            var rect = GUILayoutUtility.GetRect(0, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, Color.gray * 0.6f);
            GUILayout.Space(5f);
        }
        
        private void DrawInfoBox(string message, MessageType messageType)
        {
            EditorGUILayout.HelpBox(message, messageType);
        }
        
        private void DrawFeatureCount(int activeCount, int totalCount)
        {
            var color = activeCount > totalCount * 0.7f ? new Color(1f, 0.7f, 0.2f, 1f) : new Color(0.2f, 0.8f, 0.4f, 1f);
            var originalColor = GUI.contentColor;
            GUI.contentColor = color;
            EditorGUILayout.LabelField($"Active Features: {activeCount}/{totalCount}", CreateSmallLabelStyle());
            GUI.contentColor = originalColor;
        }
        
        private void DrawPerformanceBar(float value, string label)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(80));
            
            var rect = GUILayoutUtility.GetRect(0, 16, GUILayout.ExpandWidth(true));
            
            // Background
            EditorGUI.DrawRect(rect, Color.black * 0.3f);
            
            // Fill
            var fillRect = new Rect(rect.x, rect.y, rect.width * value, rect.height);
            Color fillColor = Color.green;
            if (value > 0.6f) fillColor = Color.yellow;
            if (value > 0.8f) fillColor = Color.red;
            
            EditorGUI.DrawRect(fillRect, fillColor);
            
            // Text
            var textStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10
            };
            EditorGUI.LabelField(rect, $"{value:P0}", textStyle);
            
            EditorGUILayout.EndHorizontal();
        }
        
        #endregion
    }
}