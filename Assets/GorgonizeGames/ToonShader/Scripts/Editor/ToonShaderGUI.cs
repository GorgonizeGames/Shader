using UnityEngine;
using UnityEditor;

namespace GorgonizeGames
{
    public class ToonShaderGUI : ShaderGUI
    {
        // Property references
        private MaterialProperty mainTex;
        private MaterialProperty color;
        
        // Lighting properties
        private MaterialProperty useRampTexture;
        private MaterialProperty rampTex;
        private MaterialProperty rampSteps;
        private MaterialProperty rampSmoothness;
        private MaterialProperty shadowColor;
        private MaterialProperty highlightColor;
        private MaterialProperty shadowBlendMode;
        
        // Specular properties
        private MaterialProperty enableSpecular;
        private MaterialProperty specColor;
        private MaterialProperty glossiness;
        private MaterialProperty specularSize;
        private MaterialProperty enableAnisotropic;
        private MaterialProperty anisotropyDirection;
        
        // MatCap properties
        private MaterialProperty enableMatCap;
        private MaterialProperty matCapTex;
        private MaterialProperty matCapIntensity;
        
        // Rim properties
        private MaterialProperty enableRim;
        private MaterialProperty rimColor;
        private MaterialProperty rimPower;
        private MaterialProperty rimIntensity;
        private MaterialProperty enableRimSecondary;
        private MaterialProperty rimColorSecondary;
        private MaterialProperty rimPowerSecondary;
        private MaterialProperty rimIntensitySecondary;
        
        // Outline properties
        private MaterialProperty enableOutline;
        private MaterialProperty outlineColor;
        private MaterialProperty outlineWidth;
        private MaterialProperty outlineDistanceFade;
        
        // Shadow properties
        private MaterialProperty stylizedShadows;
        private MaterialProperty shadowTint;
        private MaterialProperty shadowSharpness;
        private MaterialProperty shadowDithering;
        
        // Effects properties
        private MaterialProperty enableHatching;
        private MaterialProperty hatchingTex;
        private MaterialProperty hatchingScale;
        private MaterialProperty hatchingIntensity;
        private MaterialProperty enableDithering;
        private MaterialProperty ditheringScale;
        private MaterialProperty enableEmission;
        private MaterialProperty emissionMap;
        private MaterialProperty emissionColor;
        
        // Advanced properties
        private MaterialProperty mobileOptimized;
        private MaterialProperty additionalLights;
        
        // Foldout states
        private bool baseFoldout = true;
        private bool lightingFoldout = true;
        private bool specularFoldout = false;
        private bool rimFoldout = false;
        private bool outlineFoldout = false;
        private bool shadowsFoldout = false;
        private bool effectsFoldout = false;
        private bool advancedFoldout = false;
        
        // GUI Styles
        private static GUIStyle boldFoldoutStyle;
        private static GUIStyle helpBoxStyle;
        
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            InitializeStyles();
            FindProperties(properties);
            
            Material material = materialEditor.target as Material;
            
            EditorGUI.BeginChangeCheck();
            
            // Header
            DrawHeader();
            
            // Base Settings
            DrawBaseSettings(materialEditor);
            
            // Lighting Settings
            DrawLightingSettings(materialEditor);
            
            // Specular Settings
            DrawSpecularSettings(materialEditor);
            
            // Rim Lighting Settings
            DrawRimSettings(materialEditor);
            
            // Outline Settings
            DrawOutlineSettings(materialEditor);
            
            // Shadow Settings
            DrawShadowSettings(materialEditor);
            
            // Effects Settings
            DrawEffectsSettings(materialEditor);
            
            // Advanced Settings
            DrawAdvancedSettings(materialEditor);
            
            // Quick Setup Buttons
            DrawQuickSetupButtons(material);
            
            if (EditorGUI.EndChangeCheck())
            {
                SetKeywords(material);
            }
        }
        
        private void InitializeStyles()
        {
            if (boldFoldoutStyle == null)
            {
                boldFoldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 12
                };
            }
            
            if (helpBoxStyle == null)
            {
                helpBoxStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    fontSize = 10,
                    padding = new RectOffset(8, 8, 4, 4)
                };
            }
        }
        
        private void FindProperties(MaterialProperty[] properties)
        {
            // Base properties
            mainTex = FindProperty("_MainTex", properties);
            color = FindProperty("_Color", properties);
            
            // Lighting properties
            useRampTexture = FindProperty("_UseRampTexture", properties);
            rampTex = FindProperty("_RampTex", properties);
            rampSteps = FindProperty("_RampSteps", properties);
            rampSmoothness = FindProperty("_RampSmoothness", properties);
            shadowColor = FindProperty("_ShadowColor", properties);
            highlightColor = FindProperty("_HighlightColor", properties);
            shadowBlendMode = FindProperty("_ShadowBlendMode", properties);
            
            // Specular properties
            enableSpecular = FindProperty("_EnableSpecular", properties);
            specColor = FindProperty("_SpecColor", properties);
            glossiness = FindProperty("_Glossiness", properties);
            specularSize = FindProperty("_SpecularSize", properties);
            enableAnisotropic = FindProperty("_EnableAnisotropic", properties);
            anisotropyDirection = FindProperty("_AnisotropyDirection", properties);
            
            // MatCap properties
            enableMatCap = FindProperty("_EnableMatCap", properties);
            matCapTex = FindProperty("_MatCapTex", properties);
            matCapIntensity = FindProperty("_MatCapIntensity", properties);
            
            // Rim properties
            enableRim = FindProperty("_EnableRim", properties);
            rimColor = FindProperty("_RimColor", properties);
            rimPower = FindProperty("_RimPower", properties);
            rimIntensity = FindProperty("_RimIntensity", properties);
            enableRimSecondary = FindProperty("_EnableRimSecondary", properties);
            rimColorSecondary = FindProperty("_RimColorSecondary", properties);
            rimPowerSecondary = FindProperty("_RimPowerSecondary", properties);
            rimIntensitySecondary = FindProperty("_RimIntensitySecondary", properties);
            
            // Outline properties
            enableOutline = FindProperty("_EnableOutline", properties);
            outlineColor = FindProperty("_OutlineColor", properties);
            outlineWidth = FindProperty("_OutlineWidth", properties);
            outlineDistanceFade = FindProperty("_OutlineDistanceFade", properties);
            
            // Shadow properties
            stylizedShadows = FindProperty("_StylizedShadows", properties);
            shadowTint = FindProperty("_ShadowTint", properties);
            shadowSharpness = FindProperty("_ShadowSharpness", properties);
            shadowDithering = FindProperty("_ShadowDithering", properties);
            
            // Effects properties
            enableHatching = FindProperty("_EnableHatching", properties);
            hatchingTex = FindProperty("_HatchingTex", properties);
            hatchingScale = FindProperty("_HatchingScale", properties);
            hatchingIntensity = FindProperty("_HatchingIntensity", properties);
            enableDithering = FindProperty("_EnableDithering", properties);
            ditheringScale = FindProperty("_DitheringScale", properties);
            enableEmission = FindProperty("_EnableEmission", properties);
            emissionMap = FindProperty("_EmissionMap", properties);
            emissionColor = FindProperty("_EmissionColor", properties);
            
            // Advanced properties
            mobileOptimized = FindProperty("_MobileOptimized", properties);
            additionalLights = FindProperty("_AdditionalLights", properties);
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("GorgonizeGames Toon Shader", EditorStyles.largeLabel);
            EditorGUILayout.LabelField("Professional Toon Rendering for URP", EditorStyles.miniLabel);
            EditorGUILayout.Space();
        }
        
        private void DrawBaseSettings(MaterialEditor materialEditor)
        {
            baseFoldout = EditorGUILayout.Foldout(baseFoldout, "Base Settings", boldFoldoutStyle);
            if (baseFoldout)
            {
                EditorGUI.indentLevel++;
                
                materialEditor.TexturePropertySingleLine(
                    new GUIContent("Base Color", "Main texture and color tint"), 
                    mainTex, color);
                
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }
        
        private void DrawLightingSettings(MaterialEditor materialEditor)
        {
            lightingFoldout = EditorGUILayout.Foldout(lightingFoldout, "Lighting", boldFoldoutStyle);
            if (lightingFoldout)
            {
                EditorGUI.indentLevel++;
                
                // Ramp Settings
                materialEditor.ShaderProperty(useRampTexture, "Use Ramp Texture");
                
                if (useRampTexture.floatValue > 0.5f)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.TexturePropertySingleLine(
                        new GUIContent("Ramp Texture", "1D gradient texture for lighting ramp"),
                        rampTex);
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(rampSteps, "Ramp Steps");
                    materialEditor.ShaderProperty(rampSmoothness, "Ramp Smoothness");
                    EditorGUI.indentLevel--;
                }
                
                // Shadow and Highlight Colors
                materialEditor.ShaderProperty(shadowColor, "Shadow Color");
                materialEditor.ShaderProperty(highlightColor, "Highlight Color");
                
                // Shadow Blend Mode
                string[] blendModeNames = { "Multiply", "Additive", "Replace" };
                shadowBlendMode.floatValue = EditorGUILayout.Popup(
                    "Shadow Blend Mode", 
                    (int)shadowBlendMode.floatValue, 
                    blendModeNames);
                
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }
        
        private void DrawSpecularSettings(MaterialEditor materialEditor)
        {
            specularFoldout = EditorGUILayout.Foldout(specularFoldout, "Specular & MatCap", boldFoldoutStyle);
            if (specularFoldout)
            {
                EditorGUI.indentLevel++;
                
                // Specular
                materialEditor.ShaderProperty(enableSpecular, "Enable Specular");
                if (enableSpecular.floatValue > 0.5f)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(specColor, "Specular Color");
                    materialEditor.ShaderProperty(glossiness, "Glossiness");
                    materialEditor.ShaderProperty(specularSize, "Specular Size");
                    
                    materialEditor.ShaderProperty(enableAnisotropic, "Enable Anisotropic");
                    if (enableAnisotropic.floatValue > 0.5f)
                    {
                        materialEditor.ShaderProperty(anisotropyDirection, "Anisotropy Direction");
                    }
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.Space(5);
                
                // MatCap
                materialEditor.ShaderProperty(enableMatCap, "Enable MatCap");
                if (enableMatCap.floatValue > 0.5f)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.TexturePropertySingleLine(
                        new GUIContent("MatCap Texture", "Spherical environment map"),
                        matCapTex);
                    materialEditor.ShaderProperty(matCapIntensity, "MatCap Intensity");
                    EditorGUI.indentLevel--;
                }
                
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }
        
        private void DrawRimSettings(MaterialEditor materialEditor)
        {
            rimFoldout = EditorGUILayout.Foldout(rimFoldout, "Rim Lighting", boldFoldoutStyle);
            if (rimFoldout)
            {
                EditorGUI.indentLevel++;
                
                materialEditor.ShaderProperty(enableRim, "Enable Rim Lighting");
                if (enableRim.floatValue > 0.5f)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(rimColor, "Rim Color");
                    materialEditor.ShaderProperty(rimPower, "Rim Power");
                    materialEditor.ShaderProperty(rimIntensity, "Rim Intensity");
                    
                    EditorGUILayout.Space(5);
                    
                    materialEditor.ShaderProperty(enableRimSecondary, "Enable Secondary Rim");
                    if (enableRimSecondary.floatValue > 0.5f)
                    {
                        materialEditor.ShaderProperty(rimColorSecondary, "Secondary Rim Color");
                        materialEditor.ShaderProperty(rimPowerSecondary, "Secondary Rim Power");
                        materialEditor.ShaderProperty(rimIntensitySecondary, "Secondary Rim Intensity");
                    }
                    EditorGUI.indentLevel--;
                }
                
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }
        
        private void DrawOutlineSettings(MaterialEditor materialEditor)
        {
            outlineFoldout = EditorGUILayout.Foldout(outlineFoldout, "Outline", boldFoldoutStyle);
            if (outlineFoldout)
            {
                EditorGUI.indentLevel++;
                
                materialEditor.ShaderProperty(enableOutline, "Enable Outline");
                if (enableOutline.floatValue > 0.5f)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(outlineColor, "Outline Color");
                    materialEditor.ShaderProperty(outlineWidth, "Outline Width");
                    materialEditor.ShaderProperty(outlineDistanceFade, "Distance Fade");
                    
                    EditorGUILayout.HelpBox(
                        "Outline uses the Inverted Hull method. For post-processing outlines, use the GorgonizeToon Renderer Feature.",
                        MessageType.Info);
                    EditorGUI.indentLevel--;
                }
                
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }
        
        private void DrawShadowSettings(MaterialEditor materialEditor)
        {
            shadowsFoldout = EditorGUILayout.Foldout(shadowsFoldout, "Shadows", boldFoldoutStyle);
            if (shadowsFoldout)
            {
                EditorGUI.indentLevel++;
                
                materialEditor.ShaderProperty(stylizedShadows, "Stylized Shadows");
                if (stylizedShadows.floatValue > 0.5f)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(shadowTint, "Shadow Tint");
                    materialEditor.ShaderProperty(shadowSharpness, "Shadow Sharpness");
                    EditorGUI.indentLevel--;
                }
                
                materialEditor.ShaderProperty(shadowDithering, "Shadow Dithering");
                
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }
        
        private void DrawEffectsSettings(MaterialEditor materialEditor)
        {
            effectsFoldout = EditorGUILayout.Foldout(effectsFoldout, "Effects", boldFoldoutStyle);
            if (effectsFoldout)
            {
                EditorGUI.indentLevel++;
                
                // Hatching
                materialEditor.ShaderProperty(enableHatching, "Enable Hatching");
                if (enableHatching.floatValue > 0.5f)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.TexturePropertySingleLine(
                        new GUIContent("Hatching Texture", "Texture for hatching pattern"),
                        hatchingTex);
                    materialEditor.ShaderProperty(hatchingScale, "Hatching Scale");
                    materialEditor.ShaderProperty(hatchingIntensity, "Hatching Intensity");
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.Space(5);
                
                // Dithering
                materialEditor.ShaderProperty(enableDithering, "Enable Dithering");
                if (enableDithering.floatValue > 0.5f)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(ditheringScale, "Dithering Scale");
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.Space(5);
                
                // Emission
                materialEditor.ShaderProperty(enableEmission, "Enable Emission");
                if (enableEmission.floatValue > 0.5f)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.TexturePropertySingleLine(
                        new GUIContent("Emission Map", "Emission texture"),
                        emissionMap, emissionColor);
                    EditorGUI.indentLevel--;
                }
                
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }
        
        private void DrawAdvancedSettings(MaterialEditor materialEditor)
        {
            advancedFoldout = EditorGUILayout.Foldout(advancedFoldout, "Advanced", boldFoldoutStyle);
            if (advancedFoldout)
            {
                EditorGUI.indentLevel++;
                
                materialEditor.ShaderProperty(mobileOptimized, "Mobile Optimized");
                materialEditor.ShaderProperty(additionalLights, "Additional Lights");
                
                EditorGUILayout.HelpBox(
                    "Mobile Optimized mode disables some features for better performance on mobile devices.",
                    MessageType.Info);
                
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }
        
        private void DrawQuickSetupButtons(Material material)
        {
            EditorGUILayout.LabelField("Quick Setup", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Character Setup"))
            {
                SetupForCharacter(material);
            }
            
            if (GUILayout.Button("Environment Setup"))
            {
                SetupForEnvironment(material);
            }
            
            if (GUILayout.Button("Mobile Setup"))
            {
                SetupForMobile(material);
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
        }
        
        private void SetupForCharacter(Material material)
        {
            // Character-focused setup
            material.SetFloat("_EnableSpecular", 1f);
            material.SetFloat("_EnableRim", 1f);
            material.SetFloat("_EnableOutline", 1f);
            material.SetFloat("_StylizedShadows", 1f);
            material.SetFloat("_RimPower", 2f);
            material.SetColor("_RimColor", new Color(1f, 0.8f, 0.6f, 1f));
            material.SetFloat("_OutlineWidth", 0.003f);
            material.SetColor("_OutlineColor", Color.black);
        }
        
        private void SetupForEnvironment(Material material)
        {
            // Environment-focused setup
            material.SetFloat("_EnableSpecular", 0f);
            material.SetFloat("_EnableRim", 0f);
            material.SetFloat("_EnableOutline", 0f);
            material.SetFloat("_EnableHatching", 1f);
            material.SetFloat("_StylizedShadows", 1f);
            material.SetFloat("_HatchingScale", 2f);
            material.SetFloat("_HatchingIntensity", 0.5f);
        }
        
        private void SetupForMobile(Material material)
        {
            // Mobile-optimized setup
            material.SetFloat("_MobileOptimized", 1f);
            material.SetFloat("_AdditionalLights", 0f);
            material.SetFloat("_EnableSpecular", 0f);
            material.SetFloat("_EnableMatCap", 0f);
            material.SetFloat("_EnableRimSecondary", 0f);
            material.SetFloat("_EnableAnisotropic", 0f);
            material.SetFloat("_EnableHatching", 0f);
            material.SetFloat("_EnableDithering", 0f);
            material.SetFloat("_ShadowDithering", 0f);
        }
        
        private void SetKeywords(Material material)
        {
            // Set shader keywords based on material properties
            SetKeyword(material, "USE_RAMP_TEXTURE", material.GetFloat("_UseRampTexture") > 0.5f);
            SetKeyword(material, "ENABLE_SPECULAR", material.GetFloat("_EnableSpecular") > 0.5f);
            SetKeyword(material, "ENABLE_ANISOTROPIC", material.GetFloat("_EnableAnisotropic") > 0.5f);
            SetKeyword(material, "ENABLE_MATCAP", material.GetFloat("_EnableMatCap") > 0.5f);
            SetKeyword(material, "ENABLE_RIM", material.GetFloat("_EnableRim") > 0.5f);
            SetKeyword(material, "ENABLE_RIM_SECONDARY", material.GetFloat("_EnableRimSecondary") > 0.5f);
            SetKeyword(material, "ENABLE_OUTLINE", material.GetFloat("_EnableOutline") > 0.5f);
            SetKeyword(material, "STYLIZED_SHADOWS", material.GetFloat("_StylizedShadows") > 0.5f);
            SetKeyword(material, "SHADOW_DITHERING", material.GetFloat("_ShadowDithering") > 0.5f);
            SetKeyword(material, "ENABLE_HATCHING", material.GetFloat("_EnableHatching") > 0.5f);
            SetKeyword(material, "ENABLE_DITHERING", material.GetFloat("_EnableDithering") > 0.5f);
            SetKeyword(material, "ENABLE_EMISSION", material.GetFloat("_EnableEmission") > 0.5f);
            SetKeyword(material, "MOBILE_OPTIMIZED", material.GetFloat("_MobileOptimized") > 0.5f);
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