using UnityEngine;
using UnityEditor;
using System.IO;

public class UltimateToonShaderProGUI : ShaderGUI
{
    private static Texture2D logoTexture;
    
    // Material Properties
    private MaterialProperty baseMap;
    private MaterialProperty baseColor;
    private MaterialProperty saturation;
    private MaterialProperty brightness;
    
    private MaterialProperty shadowThreshold;
    private MaterialProperty shadowSmoothness;
    private MaterialProperty shadowColor;
    private MaterialProperty shadowIntensity;
    private MaterialProperty lightRampTex;
    private MaterialProperty useRampTexture;
    
    private MaterialProperty indirectLightingBoost;
    private MaterialProperty ambientOcclusion;
    private MaterialProperty lightWrapping;
    
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
    
    private MaterialProperty enableMatcap;
    private MaterialProperty matcapTex;
    private MaterialProperty matcapIntensity;
    private MaterialProperty matcapBlendMode;
    
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
    
    private MaterialProperty hue;
    private MaterialProperty contrast;
    private MaterialProperty gamma;
    
    private MaterialProperty enablePosterize;
    private MaterialProperty posterizeLevels;
    private MaterialProperty enableCelShading;
    private MaterialProperty celShadingSteps;
    
    private MaterialProperty cutoff;
    private MaterialProperty cull;
    private MaterialProperty zwrite;
    private MaterialProperty ztest;
    
    // Foldout states
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
    private static bool showAdvanced = false;
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        FindProperties(properties);
        Material material = materialEditor.target as Material;
        
        EditorGUILayout.Space();
        DrawHeader();
        EditorGUILayout.Space();
        
        // Preset System with fixed functionality
        DrawPresetSystem(materialEditor, material);
        EditorGUILayout.Space();
        
        // Base Properties
        showBase = EditorGUILayout.BeginFoldoutHeaderGroup(showBase, "🎨 Base Properties");
        if (showBase)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.TexturePropertySingleLine(new GUIContent("Albedo", "Main color texture"), baseMap, baseColor);
            materialEditor.RangeProperty(saturation, "Saturation");
            materialEditor.RangeProperty(brightness, "Brightness");
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Toon Lighting
        showLighting = EditorGUILayout.BeginFoldoutHeaderGroup(showLighting, "💡 Toon Lighting");
        if (showLighting)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.ShaderProperty(useRampTexture, "Use Ramp Texture");
            
            if (useRampTexture.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.TexturePropertySingleLine(new GUIContent("Light Ramp", "Custom lighting ramp texture"), lightRampTex);
                EditorGUI.indentLevel--;
                SetKeyword(material, "_USE_RAMP_TEXTURE", true);
            }
            else
            {
                materialEditor.RangeProperty(shadowThreshold, "Shadow Threshold");
                materialEditor.RangeProperty(shadowSmoothness, "Shadow Smoothness");
                SetKeyword(material, "_USE_RAMP_TEXTURE", false);
            }
            
            materialEditor.ColorProperty(shadowColor, "Shadow Color");
            materialEditor.RangeProperty(shadowIntensity, "Shadow Intensity");
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Advanced Lighting
        showAdvancedLighting = EditorGUILayout.BeginFoldoutHeaderGroup(showAdvancedLighting, "⚡ Advanced Lighting");
        if (showAdvancedLighting)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.RangeProperty(indirectLightingBoost, "Indirect Lighting Boost");
            materialEditor.RangeProperty(ambientOcclusion, "Ambient Occlusion");
            materialEditor.RangeProperty(lightWrapping, "Light Wrapping");
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Rim Lighting
        showRimLighting = EditorGUILayout.BeginFoldoutHeaderGroup(showRimLighting, "🌟 Rim Lighting");
        if (showRimLighting)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.ShaderProperty(enableRimLighting, "Enable Rim Lighting");
            
            if (enableRimLighting.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.ColorProperty(rimColor, "Rim Color");
                materialEditor.RangeProperty(rimPower, "Rim Power");
                materialEditor.RangeProperty(rimIntensity, "Rim Intensity");
                materialEditor.RangeProperty(rimThreshold, "Rim Threshold");
                EditorGUI.indentLevel--;
                SetKeyword(material, "_RIM_LIGHTING", true);
            }
            else
            {
                SetKeyword(material, "_RIM_LIGHTING", false);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Specular
        showSpecular = EditorGUILayout.BeginFoldoutHeaderGroup(showSpecular, "✨ Specular Highlights");
        if (showSpecular)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.ShaderProperty(enableSpecular, "Enable Specular");
            
            if (enableSpecular.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.ColorProperty(specularColor, "Specular Color");
                materialEditor.RangeProperty(specularSize, "Specular Size");
                materialEditor.RangeProperty(specularSmoothness, "Specular Smoothness");
                materialEditor.RangeProperty(specularIntensity, "Specular Intensity");
                EditorGUI.indentLevel--;
                SetKeyword(material, "_SPECULAR", true);
            }
            else
            {
                SetKeyword(material, "_SPECULAR", false);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Hatching Effects - NEW SECTION
        showHatching = EditorGUILayout.BeginFoldoutHeaderGroup(showHatching, "🖊️ Hatching Effects");
        if (showHatching)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.ShaderProperty(enableHatching, "Enable Hatching");
            
            if (enableHatching.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.TexturePropertySingleLine(new GUIContent("Hatching Texture", "Main hatching texture"), hatchingTex);
                materialEditor.TexturePropertySingleLine(new GUIContent("Cross Hatching Texture", "Cross hatching texture for deeper shadows"), crossHatchingTex);
                
                EditorGUILayout.Space(3);
                materialEditor.RangeProperty(hatchingDensity, "Hatching Density");
                materialEditor.RangeProperty(hatchingIntensity, "Hatching Intensity");
                materialEditor.RangeProperty(hatchingThreshold, "Hatching Threshold");
                materialEditor.RangeProperty(crossHatchingThreshold, "Cross Hatching Threshold");
                materialEditor.RangeProperty(hatchingRotation, "Hatching Rotation");
                
                EditorGUILayout.Space(5);
                materialEditor.ShaderProperty(enableScreenSpaceHatching, "Enable Screen Space Hatching");
                
                if (enableScreenSpaceHatching.floatValue > 0)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.RangeProperty(screenHatchScale, "Screen Hatch Scale");
                    materialEditor.RangeProperty(screenHatchBias, "Screen Hatch Bias");
                    EditorGUI.indentLevel--;
                    SetKeyword(material, "_SCREEN_SPACE_HATCHING", true);
                }
                else
                {
                    SetKeyword(material, "_SCREEN_SPACE_HATCHING", false);
                }
                
                EditorGUI.indentLevel--;
                SetKeyword(material, "_HATCHING", true);
            }
            else
            {
                SetKeyword(material, "_HATCHING", false);
                SetKeyword(material, "_SCREEN_SPACE_HATCHING", false);
            }
            
            EditorGUILayout.HelpBox("💡 Hatching adds sketch-like crosshatch patterns based on lighting. Lower thresholds = more hatching in dark areas.", MessageType.Info);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Outline - NEW SECTION
        showOutline = EditorGUILayout.BeginFoldoutHeaderGroup(showOutline, "🖼️ Outline");
        if (showOutline)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.ShaderProperty(enableOutline, "Enable Outline");
            
            if (enableOutline.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.ColorProperty(outlineColor, "Outline Color");
                materialEditor.RangeProperty(outlineWidth, "Outline Width");
                EditorGUI.indentLevel--;
                SetKeyword(material, "_OUTLINE", true);
            }
            else
            {
                SetKeyword(material, "_OUTLINE", false);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Matcap
        showMatcap = EditorGUILayout.BeginFoldoutHeaderGroup(showMatcap, "🎭 Matcap");
        if (showMatcap)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.ShaderProperty(enableMatcap, "Enable Matcap");
            
            if (enableMatcap.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.TexturePropertySingleLine(new GUIContent("Matcap Texture"), matcapTex);
                materialEditor.RangeProperty(matcapIntensity, "Matcap Intensity");
                
                string[] blendModes = {"Add", "Multiply", "Screen"};
                int blendMode = (int)matcapBlendMode.floatValue;
                blendMode = EditorGUILayout.Popup("Blend Mode", blendMode, blendModes);
                matcapBlendMode.floatValue = blendMode;
                
                EditorGUI.indentLevel--;
                SetKeyword(material, "_MATCAP", true);
            }
            else
            {
                SetKeyword(material, "_MATCAP", false);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Normal Mapping
        showNormalMap = EditorGUILayout.BeginFoldoutHeaderGroup(showNormalMap, "🗺️ Normal Mapping");
        if (showNormalMap)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.ShaderProperty(enableNormalMap, "Enable Normal Map");
            
            if (enableNormalMap.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.TexturePropertySingleLine(new GUIContent("Normal Map"), bumpMap);
                if (bumpMap.textureValue != null)
                {
                    materialEditor.RangeProperty(bumpScale, "Normal Scale");
                }
                EditorGUI.indentLevel--;
                SetKeyword(material, "_NORMALMAP", true);
            }
            else
            {
                SetKeyword(material, "_NORMALMAP", false);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Detail Textures
        showDetail = EditorGUILayout.BeginFoldoutHeaderGroup(showDetail, "🔍 Detail Textures");
        if (showDetail)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.ShaderProperty(enableDetail, "Enable Detail");
            
            if (enableDetail.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.TexturePropertySingleLine(new GUIContent("Detail Texture"), detailMap);
                materialEditor.TexturePropertySingleLine(new GUIContent("Detail Normal"), detailNormalMap);
                materialEditor.RangeProperty(detailScale, "Detail Scale");
                materialEditor.RangeProperty(detailNormalScale, "Detail Normal Scale");
                EditorGUI.indentLevel--;
                SetKeyword(material, "_DETAIL", true);
            }
            else
            {
                SetKeyword(material, "_DETAIL", false);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Emission
        showEmission = EditorGUILayout.BeginFoldoutHeaderGroup(showEmission, "🔥 Emission");
        if (showEmission)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.ShaderProperty(enableEmission, "Enable Emission");
            
            if (enableEmission.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.TexturePropertyWithHDRColor(new GUIContent("Emission"), emissionMap, emissionColor, false);
                materialEditor.RangeProperty(emissionIntensity, "Emission Intensity");
                materialEditor.VectorProperty(emissionScrollSpeed, "Scroll Speed (X,Y)");
                EditorGUI.indentLevel--;
                SetKeyword(material, "_EMISSION", true);
            }
            else
            {
                SetKeyword(material, "_EMISSION", false);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Fresnel Effect
        showFresnel = EditorGUILayout.BeginFoldoutHeaderGroup(showFresnel, "🌀 Fresnel Effect");
        if (showFresnel)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.ShaderProperty(enableFresnel, "Enable Fresnel");
            
            if (enableFresnel.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.ColorProperty(fresnelColor, "Fresnel Color");
                materialEditor.RangeProperty(fresnelPower, "Fresnel Power");
                materialEditor.RangeProperty(fresnelIntensity, "Fresnel Intensity");
                EditorGUI.indentLevel--;
                SetKeyword(material, "_FRESNEL", true);
            }
            else
            {
                SetKeyword(material, "_FRESNEL", false);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Subsurface Scattering
        showSubsurface = EditorGUILayout.BeginFoldoutHeaderGroup(showSubsurface, "🌸 Subsurface Scattering");
        if (showSubsurface)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.ShaderProperty(enableSubsurface, "Enable Subsurface");
            
            if (enableSubsurface.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.ColorProperty(subsurfaceColor, "Subsurface Color");
                materialEditor.RangeProperty(subsurfacePower, "Subsurface Power");
                materialEditor.RangeProperty(subsurfaceIntensity, "Subsurface Intensity");
                EditorGUI.indentLevel--;
                SetKeyword(material, "_SUBSURFACE", true);
            }
            else
            {
                SetKeyword(material, "_SUBSURFACE", false);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Color Grading
        showColorGrading = EditorGUILayout.BeginFoldoutHeaderGroup(showColorGrading, "🎨 Color Grading");
        if (showColorGrading)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.RangeProperty(hue, "Hue Shift");
            materialEditor.RangeProperty(contrast, "Contrast");
            materialEditor.RangeProperty(gamma, "Gamma");
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Stylization
        showStylization = EditorGUILayout.BeginFoldoutHeaderGroup(showStylization, "🎭 Stylization");
        if (showStylization)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.ShaderProperty(enablePosterize, "Enable Posterize");
            if (enablePosterize.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.RangeProperty(posterizeLevels, "Posterize Levels");
                EditorGUI.indentLevel--;
                SetKeyword(material, "_POSTERIZE", true);
            }
            else
            {
                SetKeyword(material, "_POSTERIZE", false);
            }
            
            materialEditor.ShaderProperty(enableCelShading, "Enable Cel Shading");
            if (enableCelShading.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.RangeProperty(celShadingSteps, "Cel Shading Steps");
                EditorGUI.indentLevel--;
                SetKeyword(material, "_CEL_SHADING", true);
            }
            else
            {
                SetKeyword(material, "_CEL_SHADING", false);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        // Advanced Settings
        showAdvanced = EditorGUILayout.BeginFoldoutHeaderGroup(showAdvanced, "⚙️ Advanced Settings");
        if (showAdvanced)
        {
            EditorGUILayout.BeginVertical("helpBox");
            materialEditor.RangeProperty(cutoff, "Alpha Cutoff");
            materialEditor.ShaderProperty(cull, "Cull Mode");
            materialEditor.ShaderProperty(zwrite, "Z Write");
            materialEditor.ShaderProperty(ztest, "Z Test");
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        EditorGUILayout.Space();
        DrawInfo();
    }
    
    private void FindProperties(MaterialProperty[] properties)
    {
        // Base properties
        baseMap = FindProperty("_BaseMap", properties);
        baseColor = FindProperty("_BaseColor", properties);
        saturation = FindProperty("_Saturation", properties);
        brightness = FindProperty("_Brightness", properties);
        
        // Lighting properties
        shadowThreshold = FindProperty("_ShadowThreshold", properties);
        shadowSmoothness = FindProperty("_ShadowSmoothness", properties);
        shadowColor = FindProperty("_ShadowColor", properties);
        shadowIntensity = FindProperty("_ShadowIntensity", properties);
        lightRampTex = FindProperty("_LightRampTex", properties);
        useRampTexture = FindProperty("_UseRampTexture", properties);
        
        indirectLightingBoost = FindProperty("_IndirectLightingBoost", properties);
        ambientOcclusion = FindProperty("_AmbientOcclusion", properties);
        lightWrapping = FindProperty("_LightWrapping", properties);
        
        // Rim lighting
        enableRimLighting = FindProperty("_EnableRimLighting", properties);
        rimColor = FindProperty("_RimColor", properties);
        rimPower = FindProperty("_RimPower", properties);
        rimIntensity = FindProperty("_RimIntensity", properties);
        rimThreshold = FindProperty("_RimThreshold", properties);
        
        // Specular
        enableSpecular = FindProperty("_EnableSpecular", properties);
        specularColor = FindProperty("_SpecularColor", properties);
        specularSize = FindProperty("_SpecularSize", properties);
        specularSmoothness = FindProperty("_SpecularSmoothness", properties);
        specularIntensity = FindProperty("_SpecularIntensity", properties);
        
        // Hatching properties
        enableHatching = FindProperty("_EnableHatching", properties);
        hatchingTex = FindProperty("_HatchingTex", properties);
        crossHatchingTex = FindProperty("_CrossHatchingTex", properties);
        hatchingDensity = FindProperty("_HatchingDensity", properties);
        hatchingIntensity = FindProperty("_HatchingIntensity", properties);
        hatchingThreshold = FindProperty("_HatchingThreshold", properties);
        crossHatchingThreshold = FindProperty("_CrossHatchingThreshold", properties);
        hatchingRotation = FindProperty("_HatchingRotation", properties);
        
        enableScreenSpaceHatching = FindProperty("_EnableScreenSpaceHatching", properties);
        screenHatchScale = FindProperty("_ScreenHatchScale", properties);
        screenHatchBias = FindProperty("_ScreenHatchBias", properties);
        
        // Outline
        enableOutline = FindProperty("_EnableOutline", properties);
        outlineColor = FindProperty("_OutlineColor", properties);
        outlineWidth = FindProperty("_OutlineWidth", properties);
        
        // Matcap
        enableMatcap = FindProperty("_EnableMatcap", properties);
        matcapTex = FindProperty("_MatcapTex", properties);
        matcapIntensity = FindProperty("_MatcapIntensity", properties);
        matcapBlendMode = FindProperty("_MatcapBlendMode", properties);
        
        // Normal mapping
        enableNormalMap = FindProperty("_EnableNormalMap", properties);
        bumpMap = FindProperty("_BumpMap", properties);
        bumpScale = FindProperty("_BumpScale", properties);
        
        // Detail
        enableDetail = FindProperty("_EnableDetail", properties);
        detailMap = FindProperty("_DetailMap", properties);
        detailNormalMap = FindProperty("_DetailNormalMap", properties);
        detailScale = FindProperty("_DetailScale", properties);
        detailNormalScale = FindProperty("_DetailNormalScale", properties);
        
        // Emission
        enableEmission = FindProperty("_EnableEmission", properties);
        emissionMap = FindProperty("_EmissionMap", properties);
        emissionColor = FindProperty("_EmissionColor", properties);
        emissionIntensity = FindProperty("_EmissionIntensity", properties);
        emissionScrollSpeed = FindProperty("_EmissionScrollSpeed", properties);
        
        // Fresnel
        enableFresnel = FindProperty("_EnableFresnel", properties);
        fresnelColor = FindProperty("_FresnelColor", properties);
        fresnelPower = FindProperty("_FresnelPower", properties);
        fresnelIntensity = FindProperty("_FresnelIntensity", properties);
        
        // Subsurface
        enableSubsurface = FindProperty("_EnableSubsurface", properties);
        subsurfaceColor = FindProperty("_SubsurfaceColor", properties);
        subsurfacePower = FindProperty("_SubsurfacePower", properties);
        subsurfaceIntensity = FindProperty("_SubsurfaceIntensity", properties);
        
        // Color grading
        hue = FindProperty("_Hue", properties);
        contrast = FindProperty("_Contrast", properties);
        gamma = FindProperty("_Gamma", properties);
        
        // Stylization
        enablePosterize = FindProperty("_EnablePosterize", properties);
        posterizeLevels = FindProperty("_PosterizeLevels", properties);
        enableCelShading = FindProperty("_EnableCelShading", properties);
        celShadingSteps = FindProperty("_CelShadingSteps", properties);
        
        // Advanced
        cutoff = FindProperty("_Cutoff", properties);
        cull = FindProperty("_Cull", properties);
        zwrite = FindProperty("_ZWrite", properties);
        ztest = FindProperty("_ZTest", properties);
    }
    
    private void SetKeyword(Material material, string keyword, bool enabled)
    {
        if (enabled)
            material.EnableKeyword(keyword);
        else
            material.DisableKeyword(keyword);
    }
    
    private void DrawHeader()
    {
        if (logoTexture == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:script UltimateToonShaderProGUI");
            if (guids.Length > 0)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                string scriptDirectory = Path.GetDirectoryName(scriptPath);
                string logoPath = Path.Combine(scriptDirectory, "logo.png");
                logoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(logoPath);
            }
        }

        if (logoTexture != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(logoTexture, GUILayout.Height(80));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUIStyle logoStyle = new GUIStyle(GUI.skin.label);
        logoStyle.alignment = TextAnchor.MiddleCenter;
        logoStyle.fontStyle = FontStyle.Bold;
        logoStyle.fontSize = 18;
        logoStyle.normal.textColor = new Color(0.3f, 0.7f, 1f, 1f);
        
        EditorGUILayout.LabelField("GORGONIZE TOON SHADER PRO", logoStyle);
        
        GUIStyle versionStyle = new GUIStyle(GUI.skin.label);
        versionStyle.alignment = TextAnchor.MiddleCenter;
        versionStyle.fontStyle = FontStyle.Italic;
        versionStyle.fontSize = 10;
        versionStyle.normal.textColor = Color.gray;
        
        EditorGUILayout.LabelField("Unity 6 URP - Professional Edition v3.0 with Hatching", versionStyle);
    }
    
    private void DrawPresetSystem(MaterialEditor materialEditor, Material material)
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Quick Presets", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Anime Classic", GUILayout.Height(25)))
        {
            ApplyAnimePreset(material);
            materialEditor.PropertiesChanged();
        }
        
        if (GUILayout.Button("Cartoon Bold", GUILayout.Height(25)))
        {
            ApplyCartoonPreset(material);
            materialEditor.PropertiesChanged();
        }
        
        if (GUILayout.Button("Realistic Toon", GUILayout.Height(25)))
        {
            ApplyRealisticToonPreset(material);
            materialEditor.PropertiesChanged();
        }
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Cel Shaded", GUILayout.Height(25)))
        {
            ApplyCelShadedPreset(material);
            materialEditor.PropertiesChanged();
        }
        
        if (GUILayout.Button("Painterly", GUILayout.Height(25)))
        {
            ApplyPainterlyPreset(material);
            materialEditor.PropertiesChanged();
        }
        
        if (GUILayout.Button("Sketch Style", GUILayout.Height(25)))
        {
            ApplySketchPreset(material);
            materialEditor.PropertiesChanged();
        }
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Comic Book", GUILayout.Height(25)))
        {
            ApplyComicPreset(material);
            materialEditor.PropertiesChanged();
        }
        
        if (GUILayout.Button("Hatched Drawing", GUILayout.Height(25)))
        {
            ApplyHatchedPreset(material);
            materialEditor.PropertiesChanged();
        }
        
        if (GUILayout.Button("Reset All", GUILayout.Height(25)))
        {
            ApplyResetPreset(material);
            materialEditor.PropertiesChanged();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.HelpBox("Click any preset button to instantly apply predefined settings. Each preset is optimized for different art styles.", MessageType.Info);
        EditorGUILayout.EndVertical();
    }
    
    private void ApplyAnimePreset(Material material)
    {
        // Reset all first
        ApplyResetPreset(material);
        
        // Anime-specific settings
        shadowThreshold.floatValue = 0.4f;
        shadowSmoothness.floatValue = 0.1f;
        shadowColor.colorValue = new Color(0.8f, 0.8f, 0.9f, 1f);
        shadowIntensity.floatValue = 0.7f;
        
        enableRimLighting.floatValue = 1f;
        rimColor.colorValue = new Color(1f, 0.95f, 0.8f, 1f);
        rimPower.floatValue = 1.5f;
        rimIntensity.floatValue = 2f;
        rimThreshold.floatValue = 0.1f;
        
        enableSpecular.floatValue = 1f;
        specularColor.colorValue = Color.white;
        specularSize.floatValue = 0.05f;
        specularIntensity.floatValue = 2f;
        specularSmoothness.floatValue = 0.02f;
        
        // Update keywords
        SetKeyword(material, "_RIM_LIGHTING", true);
        SetKeyword(material, "_SPECULAR", true);
        
        EditorUtility.SetDirty(material);
    }
    
    private void ApplyCartoonPreset(Material material)
    {
        ApplyResetPreset(material);
        
        shadowThreshold.floatValue = 0.6f;
        shadowSmoothness.floatValue = 0.02f;
        shadowColor.colorValue = new Color(0.6f, 0.6f, 0.8f, 1f);
        shadowIntensity.floatValue = 0.9f;
        
        enableRimLighting.floatValue = 1f;
        rimColor.colorValue = new Color(1f, 1f, 0.8f, 1f);
        rimIntensity.floatValue = 3f;
        rimPower.floatValue = 2f;
        
        enableCelShading.floatValue = 1f;
        celShadingSteps.floatValue = 4f;
        
        contrast.floatValue = 1.3f;
        saturation.floatValue = 1.2f;
        
        SetKeyword(material, "_RIM_LIGHTING", true);
        SetKeyword(material, "_CEL_SHADING", true);
        
        EditorUtility.SetDirty(material);
    }
    
    private void ApplyRealisticToonPreset(Material material)
    {
        ApplyResetPreset(material);
        
        shadowThreshold.floatValue = 0.3f;
        shadowSmoothness.floatValue = 0.2f;
        shadowColor.colorValue = new Color(0.7f, 0.7f, 0.8f, 1f);
        shadowIntensity.floatValue = 0.6f;
        
        indirectLightingBoost.floatValue = 0.5f;
        lightWrapping.floatValue = 0.3f;
        
        enableSubsurface.floatValue = 1f;
        subsurfaceColor.colorValue = new Color(1f, 0.7f, 0.7f, 1f);
        subsurfaceIntensity.floatValue = 0.3f;
        subsurfacePower.floatValue = 2f;
        
        enableFresnel.floatValue = 1f;
        fresnelIntensity.floatValue = 0.5f;
        fresnelPower.floatValue = 3f;
        
        SetKeyword(material, "_SUBSURFACE", true);
        SetKeyword(material, "_FRESNEL", true);
        
        EditorUtility.SetDirty(material);
    }
    
    private void ApplyCelShadedPreset(Material material)
    {
        ApplyResetPreset(material);
        
        shadowThreshold.floatValue = 0.5f;
        shadowSmoothness.floatValue = 0.01f;
        shadowColor.colorValue = new Color(0.5f, 0.5f, 0.7f, 1f);
        
        enableCelShading.floatValue = 1f;
        celShadingSteps.floatValue = 3f;
        
        enablePosterize.floatValue = 1f;
        posterizeLevels.floatValue = 6f;
        
        contrast.floatValue = 1.4f;
        saturation.floatValue = 1.1f;
        
        SetKeyword(material, "_CEL_SHADING", true);
        SetKeyword(material, "_POSTERIZE", true);
        
        EditorUtility.SetDirty(material);
    }
    
    private void ApplyPainterlyPreset(Material material)
    {
        ApplyResetPreset(material);
        
        shadowThreshold.floatValue = 0.35f;
        shadowSmoothness.floatValue = 0.3f;
        shadowColor.colorValue = new Color(0.6f, 0.7f, 0.8f, 1f);
        
        enableMatcap.floatValue = 1f;
        matcapIntensity.floatValue = 0.8f;
        matcapBlendMode.floatValue = 1f; // Multiply
        
        enableFresnel.floatValue = 1f;
        fresnelIntensity.floatValue = 1.5f;
        fresnelPower.floatValue = 2f;
        
        indirectLightingBoost.floatValue = 0.4f;
        
        SetKeyword(material, "_MATCAP", true);
        SetKeyword(material, "_FRESNEL", true);
        
        EditorUtility.SetDirty(material);
    }
    
    private void ApplySketchPreset(Material material)
    {
        ApplyResetPreset(material);
        
        shadowThreshold.floatValue = 0.4f;
        shadowSmoothness.floatValue = 0.05f;
        shadowColor.colorValue = new Color(0.9f, 0.9f, 0.9f, 1f);
        
        enableHatching.floatValue = 1f;
        hatchingDensity.floatValue = 2f;
        hatchingIntensity.floatValue = 0.8f;
        hatchingThreshold.floatValue = 0.6f;
        crossHatchingThreshold.floatValue = 0.3f;
        hatchingRotation.floatValue = 45f;
        
        enableRimLighting.floatValue = 1f;
        rimIntensity.floatValue = 1f;
        rimColor.colorValue = new Color(0.8f, 0.8f, 0.8f, 1f);
        
        saturation.floatValue = 0.8f;
        contrast.floatValue = 1.2f;
        
        SetKeyword(material, "_HATCHING", true);
        SetKeyword(material, "_RIM_LIGHTING", true);
        
        EditorUtility.SetDirty(material);
    }
    
    private void ApplyComicPreset(Material material)
    {
        ApplyResetPreset(material);
        
        shadowThreshold.floatValue = 0.5f;
        shadowSmoothness.floatValue = 0.02f;
        shadowColor.colorValue = new Color(0.3f, 0.3f, 0.5f, 1f);
        
        enableOutline.floatValue = 1f;
        outlineColor.colorValue = Color.black;
        outlineWidth.floatValue = 0.02f;
        
        enableRimLighting.floatValue = 1f;
        rimIntensity.floatValue = 2.5f;
        rimColor.colorValue = new Color(1f, 0.9f, 0.7f, 1f);
        
        enableSpecular.floatValue = 1f;
        specularIntensity.floatValue = 3f;
        specularSize.floatValue = 0.03f;
        
        saturation.floatValue = 1.3f;
        contrast.floatValue = 1.4f;
        
        SetKeyword(material, "_OUTLINE", true);
        SetKeyword(material, "_RIM_LIGHTING", true);
        SetKeyword(material, "_SPECULAR", true);
        
        EditorUtility.SetDirty(material);
    }
    
    private void ApplyHatchedPreset(Material material)
    {
        ApplyResetPreset(material);
        
        shadowThreshold.floatValue = 0.45f;
        shadowSmoothness.floatValue = 0.1f;
        shadowColor.colorValue = new Color(0.95f, 0.95f, 0.95f, 1f);
        
        enableHatching.floatValue = 1f;
        hatchingDensity.floatValue = 1.5f;
        hatchingIntensity.floatValue = 1.2f;
        hatchingThreshold.floatValue = 0.7f;
        crossHatchingThreshold.floatValue = 0.4f;
        hatchingRotation.floatValue = 30f;
        
        enableScreenSpaceHatching.floatValue = 1f;
        screenHatchScale.floatValue = 3f;
        screenHatchBias.floatValue = 0.2f;
        
        saturation.floatValue = 0.7f;
        contrast.floatValue = 1.3f;
        gamma.floatValue = 1.2f;
        
        SetKeyword(material, "_HATCHING", true);
        SetKeyword(material, "_SCREEN_SPACE_HATCHING", true);
        
        EditorUtility.SetDirty(material);
    }
    
    private void ApplyResetPreset(Material material)
    {
        // Reset all properties to default values
        baseColor.colorValue = Color.white;
        saturation.floatValue = 1f;
        brightness.floatValue = 1f;
        
        shadowThreshold.floatValue = 0.5f;
        shadowSmoothness.floatValue = 0.05f;
        shadowColor.colorValue = new Color(0.7f, 0.7f, 0.8f, 1f);
        shadowIntensity.floatValue = 0.8f;
        
        indirectLightingBoost.floatValue = 0.3f;
        ambientOcclusion.floatValue = 1f;
        lightWrapping.floatValue = 0f;
        
        // Disable all features
        enableRimLighting.floatValue = 0f;
        enableSpecular.floatValue = 0f;
        enableHatching.floatValue = 0f;
        enableScreenSpaceHatching.floatValue = 0f;
        enableOutline.floatValue = 0f;
        enableMatcap.floatValue = 0f;
        enableNormalMap.floatValue = 0f;
        enableDetail.floatValue = 0f;
        enableEmission.floatValue = 0f;
        enableFresnel.floatValue = 0f;
        enableSubsurface.floatValue = 0f;
        enablePosterize.floatValue = 0f;
        enableCelShading.floatValue = 0f;
        useRampTexture.floatValue = 0f;
        
        // Reset color grading
        hue.floatValue = 0f;
        contrast.floatValue = 1f;
        gamma.floatValue = 1f;
        
        // Disable all keywords
        SetKeyword(material, "_RIM_LIGHTING", false);
        SetKeyword(material, "_SPECULAR", false);
        SetKeyword(material, "_HATCHING", false);
        SetKeyword(material, "_SCREEN_SPACE_HATCHING", false);
        SetKeyword(material, "_OUTLINE", false);
        SetKeyword(material, "_MATCAP", false);
        SetKeyword(material, "_NORMALMAP", false);
        SetKeyword(material, "_DETAIL", false);
        SetKeyword(material, "_EMISSION", false);
        SetKeyword(material, "_FRESNEL", false);
        SetKeyword(material, "_SUBSURFACE", false);
        SetKeyword(material, "_POSTERIZE", false);
        SetKeyword(material, "_CEL_SHADING", false);
        SetKeyword(material, "_USE_RAMP_TEXTURE", false);
    }
    
    private void DrawInfo()
    {
        EditorGUILayout.BeginVertical("helpBox");
        EditorGUILayout.LabelField("Shader Information", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Unity 6 URP Compatible", EditorStyles.miniLabel);
        EditorGUILayout.LabelField("Mobile Optimized", EditorStyles.miniLabel);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Advanced Toon Features", EditorStyles.miniLabel);
        EditorGUILayout.LabelField("Hatching Effects", EditorStyles.miniLabel);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Tip: Use the preset buttons above for quick setup, then fine-tune individual properties below!", EditorStyles.helpBox);
        EditorGUILayout.EndVertical();
    }
}