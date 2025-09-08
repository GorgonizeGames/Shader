using UnityEngine;
using UnityEditor;
using System;

namespace GorgonizeGames.ToonShader
{
    /// <summary>
    /// Custom Material Editor for GorgonizeGames Toon Shader
    /// Provides an organized, user-friendly interface with conditional property display
    /// </summary>
    public class ToonShaderGUI : ShaderGUI
    {
        // Foldout states
        private static bool showBaseSettings = true;
        private static bool showLightingSettings = true;
        private static bool showSpecularSettings = false;
        private static bool showRimSettings = false;
        private static bool showOutlineSettings = false;
        private static bool showShadowSettings = false;
        private static bool showEffectsSettings = false;
        private static bool showEmissionSettings = false;
        private static bool showRenderingSettings = false;

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
            GUILayout.Label("GorgonizeGames Toon Shader", EditorStyles.boldLabel);
            GUILayout.Label("Unity URP Toon Shader with Advanced Features", EditorStyles.miniLabel);
            EditorGUILayout.Space();
            
            // Quick Mobile Optimization Button
            if (GUILayout.Button("Enable Mobile Optimizations"))
            {
                EnableMobileOptimizations(material);
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
            
            // Set shader keywords based on properties
            SetMaterialKeywords(material);
        }

        private void FindProperties(MaterialProperty[] properties)
        {
            baseMap = FindProperty("_BaseMap", properties);
            baseColor = FindProperty("_BaseColor", properties);
            
            useRampShading = FindProperty("_UseRampShading", properties);
            rampMap = FindProperty("_RampMap", properties);
            rampSteps = FindProperty("_RampSteps", properties);
            shadowColor = FindProperty("_ShadowColor", properties);
            highlightColor = FindProperty("_HighlightColor", properties);
            shadowBlendMode = FindProperty("_ShadowBlendMode", properties);
            shadowIntensity = FindProperty("_ShadowIntensity", properties);
            lightSmoothness = FindProperty("_LightSmoothness", properties);
            
            enableSpecular = FindProperty("_EnableSpecular", properties);
            specularColor = FindProperty("_SpecularColor", properties);
            specularSize = FindProperty("_SpecularSize", properties);
            specularSmoothness = FindProperty("_SpecularSmoothness", properties);
            anisotropicSpecular = FindProperty("_AnisotropicSpecular", properties);
            anisotropy = FindProperty("_Anisotropy", properties);
            useMatCap = FindProperty("_UseMatCap", properties);
            matCapMap = FindProperty("_MatCapMap", properties);
            
            enableRim = FindProperty("_EnableRim", properties);
            rimColor = FindProperty("_RimColor", properties);
            rimPower = FindProperty("_RimPower", properties);
            rimIntensity = FindProperty("_RimIntensity", properties);
            enableSecondaryRim = FindProperty("_EnableSecondaryRim", properties);
            secondaryRimColor = FindProperty("_SecondaryRimColor", properties);
            secondaryRimPower = FindProperty("_SecondaryRimPower", properties);
            secondaryRimIntensity = FindProperty("_SecondaryRimIntensity", properties);
            lightBasedRim = FindProperty("_LightBasedRim", properties);
            
            enableOutline = FindProperty("_EnableOutline", properties);
            outlineWidth = FindProperty("_OutlineWidth", properties);
            outlineColor = FindProperty("_OutlineColor", properties);
            outlineDistanceFade = FindProperty("_OutlineDistanceFade", properties);
            cornerRounding = FindProperty("_CornerRounding", properties);
            cornerRoundness = FindProperty("_CornerRoundness", properties);
            
            stylizedShadows = FindProperty("_StylizedShadows", properties);
            shadowTint = FindProperty("_ShadowTint", properties);
            shadowSharpness = FindProperty("_ShadowSharpness", properties);
            shadowDithering = FindProperty("_ShadowDithering", properties);
            ditherScale = FindProperty("_DitherScale", properties);
            
            enableHatching = FindProperty("_EnableHatching", properties);
            hatchingMap = FindProperty("_HatchingMap", properties);
            hatchingScale = FindProperty("_HatchingScale", properties);
            hatchingIntensity = FindProperty("_HatchingIntensity", properties);
            enableDithering = FindProperty("_EnableDithering", properties);
            colorDitherScale = FindProperty("_ColorDitherScale", properties);
            
            enableEmission = FindProperty("_EnableEmission", properties);
            emissionMap = FindProperty("_EmissionMap", properties);
            emissionColor = FindProperty("_EmissionColor", properties);
            
            mobileMode = FindProperty("_MobileMode", properties);
            
            cull = FindProperty("_Cull", properties);
            srcBlend = FindProperty("_SrcBlend", properties);
            dstBlend = FindProperty("_DstBlend", properties);
            zWrite = FindProperty("_ZWrite", properties);
            zTest = FindProperty("_ZTest", properties);
        }

        private void DrawBaseSettings(MaterialEditor materialEditor, Material material)
        {
            showBaseSettings = EditorGUILayout.Foldout(showBaseSettings, "Base Settings", true);
            if (showBaseSettings)
            {
                EditorGUI.indentLevel++;
                
                materialEditor.TexturePropertySingleLine(new GUIContent("Base Map"), baseMap, baseColor);
                
                EditorGUI.indentLevel--;
            }
        }

        private void DrawLightingSettings(MaterialEditor materialEditor, Material material)
        {
            showLightingSettings = EditorGUILayout.Foldout(showLightingSettings, "Lighting Settings", true);
            if (showLightingSettings)
            {
                EditorGUI.indentLevel++;
                
                materialEditor.ShaderProperty(useRampShading, "Use Ramp Shading");
                
                if (useRampShading.floatValue > 0.5f)
                {
                    materialEditor.TexturePropertySingleLine(new GUIContent("Ramp Map"), rampMap);
                    EditorGUILayout.HelpBox("Use a 1D ramp texture for custom lighting transitions.", MessageType.Info);
                }
                else
                {
                    materialEditor.ShaderProperty(rampSteps, "Ramp Steps (Procedural)");
                    EditorGUILayout.HelpBox("Procedural gradient with 2-5 color steps.", MessageType.Info);
                }
                
                materialEditor.ShaderProperty(shadowColor, "Shadow Color");
                materialEditor.ShaderProperty(highlightColor, "Highlight Color");
                materialEditor.ShaderProperty(shadowBlendMode, "Shadow Blend Mode");
                materialEditor.ShaderProperty(shadowIntensity, "Shadow Intensity");
                materialEditor.ShaderProperty(lightSmoothness, "Light Smoothness");
                
                EditorGUI.indentLevel--;
            }
        }

        private void DrawSpecularSettings(MaterialEditor materialEditor, Material material)
        {
            showSpecularSettings = EditorGUILayout.Foldout(showSpecularSettings, "Specular Settings", true);
            if (showSpecularSettings)
            {
                EditorGUI.indentLevel++;
                
                materialEditor.ShaderProperty(enableSpecular, "Enable Specular");
                
                if (enableSpecular.floatValue > 0.5f)
                {
                    materialEditor.ShaderProperty(specularColor, "Specular Color");
                    materialEditor.ShaderProperty(specularSize, "Specular Size");
                    materialEditor.ShaderProperty(specularSmoothness, "Specular Smoothness");
                    
                    materialEditor.ShaderProperty(anisotropicSpecular, "Anisotropic Specular");
                    if (anisotropicSpecular.floatValue > 0.5f)
                    {
                        materialEditor.ShaderProperty(anisotropy, "Anisotropy");
                        EditorGUILayout.HelpBox("Great for hair and fabric materials.", MessageType.Info);
                    }
                    
                    materialEditor.ShaderProperty(useMatCap, "Use MatCap");
                    if (useMatCap.floatValue > 0.5f)
                    {
                        materialEditor.TexturePropertySingleLine(new GUIContent("MatCap Map"), matCapMap);
                        EditorGUILayout.HelpBox("MatCap provides fake reflections for better performance.", MessageType.Info);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }

        private void DrawRimLightingSettings(MaterialEditor materialEditor, Material material)
        {
            showRimSettings = EditorGUILayout.Foldout(showRimSettings, "Rim Lighting Settings", true);
            if (showRimSettings)
            {
                EditorGUI.indentLevel++;
                
                materialEditor.ShaderProperty(enableRim, "Enable Rim Lighting");
                
                if (enableRim.floatValue > 0.5f)
                {
                    materialEditor.ShaderProperty(rimColor, "Primary Rim Color");
                    materialEditor.ShaderProperty(rimPower, "Primary Rim Power");
                    materialEditor.ShaderProperty(rimIntensity, "Primary Rim Intensity");
                    
                    materialEditor.ShaderProperty(enableSecondaryRim, "Enable Secondary Rim");
                    if (enableSecondaryRim.floatValue > 0.5f)
                    {
                        materialEditor.ShaderProperty(secondaryRimColor, "Secondary Rim Color");
                        materialEditor.ShaderProperty(secondaryRimPower, "Secondary Rim Power");
                        materialEditor.ShaderProperty(secondaryRimIntensity, "Secondary Rim Intensity");
                    }
                    
                    materialEditor.ShaderProperty(lightBasedRim, "Light Based Rim");
                    if (lightBasedRim.floatValue > 0.5f)
                    {
                        EditorGUILayout.HelpBox("Rim lighting will be influenced by light direction.", MessageType.Info);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }

        private void DrawOutlineSettings(MaterialEditor materialEditor, Material material)
        {
            showOutlineSettings = EditorGUILayout.Foldout(showOutlineSettings, "Outline Settings", true);
            if (showOutlineSettings)
            {
                EditorGUI.indentLevel++;
                
                materialEditor.ShaderProperty(enableOutline, "Enable Outline");
                
                if (enableOutline.floatValue > 0.5f)
                {
                    materialEditor.ShaderProperty(outlineWidth, "Outline Width");
                    materialEditor.ShaderProperty(outlineColor, "Outline Color");
                    materialEditor.ShaderProperty(outlineDistanceFade, "Distance Fade");
                    
                    materialEditor.ShaderProperty(cornerRounding, "Corner Rounding");
                    if (cornerRounding.floatValue > 0.5f)
                    {
                        materialEditor.ShaderProperty(cornerRoundness, "Corner Roundness");
                    }
                    
                    EditorGUILayout.HelpBox("Uses Inverted Hull technique for consistent outline rendering.", MessageType.Info);
                }
                
                EditorGUI.indentLevel--;
            }
        }

        private void DrawShadowSettings(MaterialEditor materialEditor, Material material)
        {
            showShadowSettings = EditorGUILayout.Foldout(showShadowSettings, "Shadow Settings", true);
            if (showShadowSettings)
            {
                EditorGUI.indentLevel++;
                
                materialEditor.ShaderProperty(stylizedShadows, "Stylized Shadows");
                
                if (stylizedShadows.floatValue > 0.5f)
                {
                    materialEditor.ShaderProperty(shadowTint, "Shadow Tint");
                    materialEditor.ShaderProperty(shadowSharpness, "Shadow Sharpness");
                    
                    materialEditor.ShaderProperty(shadowDithering, "Shadow Dithering");
                    if (shadowDithering.floatValue > 0.5f)
                    {
                        materialEditor.ShaderProperty(ditherScale, "Dither Scale");
                        EditorGUILayout.HelpBox("Dithering creates smoother shadow transitions.", MessageType.Info);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }

        private void DrawEffectsSettings(MaterialEditor materialEditor, Material material)
        {
            showEffectsSettings = EditorGUILayout.Foldout(showEffectsSettings, "Effects Settings", true);
            if (showEffectsSettings)
            {
                EditorGUI.indentLevel++;
                
                materialEditor.ShaderProperty(enableHatching, "Enable Hatching");
                if (enableHatching.floatValue > 0.5f)
                {
                    materialEditor.TexturePropertySingleLine(new GUIContent("Hatching Map"), hatchingMap);
                    materialEditor.ShaderProperty(hatchingScale, "Hatching Scale");
                    materialEditor.ShaderProperty(hatchingIntensity, "Hatching Intensity");
                    EditorGUILayout.HelpBox("Use RGB channels for different hatching densities.", MessageType.Info);
                }
                
                materialEditor.ShaderProperty(enableDithering, "Enable Color Dithering");
                if (enableDithering.floatValue > 0.5f)
                {
                    materialEditor.ShaderProperty(colorDitherScale, "Color Dither Scale");
                }
                
                EditorGUI.indentLevel--;
            }
        }

        private void DrawEmissionSettings(MaterialEditor materialEditor, Material material)
        {
            showEmissionSettings = EditorGUILayout.Foldout(showEmissionSettings, "Emission Settings", true);
            if (showEmissionSettings)
            {
                EditorGUI.indentLevel++;
                
                materialEditor.ShaderProperty(enableEmission, "Enable Emission");
                
                if (enableEmission.floatValue > 0.5f)
                {
                    materialEditor.TexturePropertySingleLine(new GUIContent("Emission Map"), emissionMap);
                    materialEditor.ShaderProperty(emissionColor, "Emission Color");
                }
                
                EditorGUI.indentLevel--;
            }
        }

        private void DrawRenderingSettings(MaterialEditor materialEditor, Material material)
        {
            showRenderingSettings = EditorGUILayout.Foldout(showRenderingSettings, "Rendering Settings", true);
            if (showRenderingSettings)
            {
                EditorGUI.indentLevel++;
                
                materialEditor.ShaderProperty(mobileMode, "Mobile Mode");
                if (mobileMode.floatValue > 0.5f)
                {
                    EditorGUILayout.HelpBox("Mobile mode reduces shader complexity for better performance.", MessageType.Info);
                }
                
                EditorGUILayout.Space();
                
                materialEditor.ShaderProperty(cull, "Cull Mode");
                materialEditor.ShaderProperty(srcBlend, "Src Blend");
                materialEditor.ShaderProperty(dstBlend, "Dst Blend");
                materialEditor.ShaderProperty(zWrite, "ZWrite");
                materialEditor.ShaderProperty(zTest, "ZTest");
                
                EditorGUI.indentLevel--;
            }
        }

        private void EnableMobileOptimizations(Material material)
        {
            // Enable mobile mode
            material.SetFloat("_MobileMode", 1.0f);
            
            // Disable expensive features
            material.SetFloat("_EnableSpecular", 0.0f);
            material.SetFloat("_EnableSecondaryRim", 0.0f);
            material.SetFloat("_ShadowDithering", 0.0f);
            material.SetFloat("_EnableHatching", 0.0f);
            material.SetFloat("_EnableDithering", 0.0f);
            material.SetFloat("_CornerRounding", 0.0f);
            
            // Reduce outline width
            if (material.GetFloat("_EnableOutline") > 0.5f)
            {
                material.SetFloat("_OutlineWidth", Mathf.Min(material.GetFloat("_OutlineWidth"), 0.005f));
            }
            
            // Simplify lighting
            material.SetFloat("_UseRampShading", 0.0f);
            material.SetFloat("_RampSteps", 3.0f);
            
            EditorUtility.SetDirty(material);
            Debug.Log("Mobile optimizations applied to " + material.name);
        }

        private void SetMaterialKeywords(Material material)
        {
            // Lighting keywords
            SetKeyword(material, "_USE_RAMP_SHADING", material.GetFloat("_UseRampShading") > 0.5f);
            
            // Specular keywords
            SetKeyword(material, "_ENABLE_SPECULAR", material.GetFloat("_EnableSpecular") > 0.5f);
            SetKeyword(material, "_ANISOTROPIC_SPECULAR", material.GetFloat("_AnisotropicSpecular") > 0.5f);
            SetKeyword(material, "_USE_MATCAP", material.GetFloat("_UseMatCap") > 0.5f);
            
            // Rim lighting keywords
            SetKeyword(material, "_ENABLE_RIM", material.GetFloat("_EnableRim") > 0.5f);
            SetKeyword(material, "_ENABLE_SECONDARY_RIM", material.GetFloat("_EnableSecondaryRim") > 0.5f);
            SetKeyword(material, "_LIGHT_BASED_RIM", material.GetFloat("_LightBasedRim") > 0.5f);
            
            // Outline keywords
            SetKeyword(material, "_ENABLE_OUTLINE", material.GetFloat("_EnableOutline") > 0.5f);
            SetKeyword(material, "_CORNER_ROUNDING", material.GetFloat("_CornerRounding") > 0.5f);
            
            // Shadow keywords
            SetKeyword(material, "_STYLIZED_SHADOWS", material.GetFloat("_StylizedShadows") > 0.5f);
            SetKeyword(material, "_SHADOW_DITHERING", material.GetFloat("_ShadowDithering") > 0.5f);
            
            // Effects keywords
            SetKeyword(material, "_ENABLE_HATCHING", material.GetFloat("_EnableHatching") > 0.5f);
            SetKeyword(material, "_ENABLE_DITHERING", material.GetFloat("_EnableDithering") > 0.5f);
            
            // Emission keywords
            SetKeyword(material, "_EMISSION", material.GetFloat("_EnableEmission") > 0.5f);
            
            // Mobile keywords
            SetKeyword(material, "_MOBILE_MODE", material.GetFloat("_MobileMode") > 0.5f);
        }

        private void SetKeyword(Material material, string keyword, bool state)
        {
            if (state)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }
    }

    /// <summary>
    /// Custom Ramp Texture Creator Window
    /// Allows users to create ramp textures directly in the editor
    /// </summary>
    public class RampTextureCreator : EditorWindow
    {
        private Gradient gradient = new Gradient();
        private int textureWidth = 256;
        private int textureHeight = 4;
        private string savePath = "Assets/";

        [MenuItem("GorgonizeGames/Toon Shader/Create Ramp Texture")]
        public static void ShowWindow()
        {
            GetWindow<RampTextureCreator>("Ramp Texture Creator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Ramp Texture Creator", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            gradient = EditorGUILayout.GradientField("Gradient", gradient);
            
            textureWidth = EditorGUILayout.IntSlider("Width", textureWidth, 32, 1024);
            textureHeight = EditorGUILayout.IntSlider("Height", textureHeight, 1, 16);
            
            savePath = EditorGUILayout.TextField("Save Path", savePath);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create Ramp Texture"))
            {
                CreateRampTexture();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Create custom ramp textures for unique lighting styles. The gradient will be converted to a horizontal ramp texture.", MessageType.Info);
        }

        private void CreateRampTexture()
        {
            Texture2D rampTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
            rampTexture.wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < textureWidth; x++)
            {
                float t = (float)x / (textureWidth - 1);
                Color color = gradient.Evaluate(t);
                
                for (int y = 0; y < textureHeight; y++)
                {
                    rampTexture.SetPixel(x, y, color);
                }
            }

            rampTexture.Apply();

            string path = savePath + "RampTexture_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
            byte[] pngData = rampTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, pngData);
            
            AssetDatabase.Refresh();
            
            // Set import settings
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Default;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.filterMode = FilterMode.Bilinear;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
            }
            
            Debug.Log("Ramp texture created at: " + path);
            
            // Select the created texture
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }
}