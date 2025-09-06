using UnityEngine;
using UnityEditor;
using System.IO; // Path işlemleri için bu satırı ekliyoruz

public class UltimateToonShaderGUI : ShaderGUI
{
    // Logonuzu tutmak için bir değişken
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
    private bool showBase = true;
    private bool showLighting = true;
    private bool showAdvancedLighting = false;
    private bool showRimLighting = false;
    private bool showSpecular = false;
    private bool showMatcap = false;
    private bool showNormalMap = false;
    private bool showDetail = false;
    private bool showEmission = false;
    private bool showFresnel = false;
    private bool showSubsurface = false;
    private bool showColorGrading = false;
    private bool showStylization = false;
    private bool showAdvanced = false;
    
    // Preset system
    private int selectedPreset = 0;
    private string[] presetNames = { "Custom", "Anime", "Cartoon", "Realistic Toon", "Cel Shaded", "Painterly" };
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        FindProperties(properties);
        Material material = materialEditor.target as Material;
        
        EditorGUILayout.Space();
        DrawHeader();
        EditorGUILayout.Space();
        
        // Preset System
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
        baseMap = FindProperty("_BaseMap", properties);
        baseColor = FindProperty("_BaseColor", properties);
        saturation = FindProperty("_Saturation", properties);
        brightness = FindProperty("_Brightness", properties);
        
        shadowThreshold = FindProperty("_ShadowThreshold", properties);
        shadowSmoothness = FindProperty("_ShadowSmoothness", properties);
        shadowColor = FindProperty("_ShadowColor", properties);
        shadowIntensity = FindProperty("_ShadowIntensity", properties);
        lightRampTex = FindProperty("_LightRampTex", properties);
        useRampTexture = FindProperty("_UseRampTexture", properties);
        
        indirectLightingBoost = FindProperty("_IndirectLightingBoost", properties);
        ambientOcclusion = FindProperty("_AmbientOcclusion", properties);
        lightWrapping = FindProperty("_LightWrapping", properties);
        
        enableRimLighting = FindProperty("_EnableRimLighting", properties);
        rimColor = FindProperty("_RimColor", properties);
        rimPower = FindProperty("_RimPower", properties);
        rimIntensity = FindProperty("_RimIntensity", properties);
        rimThreshold = FindProperty("_RimThreshold", properties);
        
        enableSpecular = FindProperty("_EnableSpecular", properties);
        specularColor = FindProperty("_SpecularColor", properties);
        specularSize = FindProperty("_SpecularSize", properties);
        specularSmoothness = FindProperty("_SpecularSmoothness", properties);
        specularIntensity = FindProperty("_SpecularIntensity", properties);
        
        enableMatcap = FindProperty("_EnableMatcap", properties);
        matcapTex = FindProperty("_MatcapTex", properties);
        matcapIntensity = FindProperty("_MatcapIntensity", properties);
        matcapBlendMode = FindProperty("_MatcapBlendMode", properties);
        
        enableNormalMap = FindProperty("_EnableNormalMap", properties);
        bumpMap = FindProperty("_BumpMap", properties);
        bumpScale = FindProperty("_BumpScale", properties);
        
        enableDetail = FindProperty("_EnableDetail", properties);
        detailMap = FindProperty("_DetailMap", properties);
        detailNormalMap = FindProperty("_DetailNormalMap", properties);
        detailScale = FindProperty("_DetailScale", properties);
        detailNormalScale = FindProperty("_DetailNormalScale", properties);
        
        enableEmission = FindProperty("_EnableEmission", properties);
        emissionMap = FindProperty("_EmissionMap", properties);
        emissionColor = FindProperty("_EmissionColor", properties);
        emissionIntensity = FindProperty("_EmissionIntensity", properties);
        emissionScrollSpeed = FindProperty("_EmissionScrollSpeed", properties);
        
        enableFresnel = FindProperty("_EnableFresnel", properties);
        fresnelColor = FindProperty("_FresnelColor", properties);
        fresnelPower = FindProperty("_FresnelPower", properties);
        fresnelIntensity = FindProperty("_FresnelIntensity", properties);
        
        enableSubsurface = FindProperty("_EnableSubsurface", properties);
        subsurfaceColor = FindProperty("_SubsurfaceColor", properties);
        subsurfacePower = FindProperty("_SubsurfacePower", properties);
        subsurfaceIntensity = FindProperty("_SubsurfaceIntensity", properties);
        
        hue = FindProperty("_Hue", properties);
        contrast = FindProperty("_Contrast", properties);
        gamma = FindProperty("_Gamma", properties);
        
        enablePosterize = FindProperty("_EnablePosterize", properties);
        posterizeLevels = FindProperty("_PosterizeLevels", properties);
        enableCelShading = FindProperty("_EnableCelShading", properties);
        celShadingSteps = FindProperty("_CelShadingSteps", properties);
        
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
        // Logoyu daha önce yüklenmediyse yükle
        if (logoTexture == null)
        {
            // Bu script'in yolunu bularak logoyu göreceli olarak bulmasını sağlıyoruz.
            // Bu sayede projenin konumu değişse de logo bulunur.
            string[] guids = AssetDatabase.FindAssets("t:script UltimateToonShaderGUI");
            if (guids.Length > 0)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                string scriptDirectory = Path.GetDirectoryName(scriptPath);
                // Logo dosyasının adı "logo.png" olmalı ve bu script ile aynı klasörde bulunmalı.
                string logoPath = Path.Combine(scriptDirectory, "logo.png");
                logoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(logoPath);
            }
        }

        // Logo başarıyla yüklendiyse çiz
        if (logoTexture != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            // Logonun yüksekliğini buradan ayarlayabilirsiniz
            GUILayout.Label(logoTexture, GUILayout.Height(80));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUIStyle logoStyle = new GUIStyle(GUI.skin.label);
        logoStyle.alignment = TextAnchor.MiddleCenter;
        logoStyle.fontStyle = FontStyle.Bold;
        logoStyle.fontSize = 18;
        logoStyle.normal.textColor = new Color(0.3f, 0.7f, 1f, 1f);
        
        EditorGUILayout.LabelField("GORGONIZE TOON SHADER", logoStyle);
        
        GUIStyle versionStyle = new GUIStyle(GUI.skin.label);
        versionStyle.alignment = TextAnchor.MiddleCenter;
        versionStyle.fontStyle = FontStyle.Italic;
        versionStyle.fontSize = 10;
        versionStyle.normal.textColor = Color.gray;
        
        EditorGUILayout.LabelField("Unity 6 URP - Professional Edition v2.0", versionStyle);
    }
    
    private void DrawPresetSystem(MaterialEditor materialEditor, Material material)
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("🎭 Quick Presets", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        selectedPreset = EditorGUILayout.Popup("Apply Preset", selectedPreset, presetNames);
        
        if (EditorGUI.EndChangeCheck() && selectedPreset > 0)
        {
            ApplyPreset(selectedPreset, material);
            selectedPreset = 0; // Reset to Custom
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void ApplyPreset(int presetIndex, Material material)
    {
        switch (presetIndex)
        {
            case 1: // Anime
                ApplyAnimePreset(material);
                break;
            case 2: // Cartoon
                ApplyCartoonPreset(material);
                break;
            case 3: // Realistic Toon
                ApplyRealisticToonPreset(material);
                break;
            case 4: // Cel Shaded
                ApplyCelShadedPreset(material);
                break;
            case 5: // Painterly
                ApplyPainterlyPreset(material);
                break;
        }
        EditorUtility.SetDirty(material);
    }
    
    private void ApplyAnimePreset(Material material)
    {
        shadowThreshold.floatValue = 0.4f;
        shadowSmoothness.floatValue = 0.1f;
        enableRimLighting.floatValue = 1;
        rimPower.floatValue = 1.5f;
        rimIntensity.floatValue = 2f;
        enableSpecular.floatValue = 1;
        specularSize.floatValue = 0.05f;
        specularIntensity.floatValue = 2f;
        SetKeyword(material, "_RIM_LIGHTING", true);
        SetKeyword(material, "_SPECULAR", true);
    }
    
    private void ApplyCartoonPreset(Material material)
    {
        shadowThreshold.floatValue = 0.6f;
        shadowSmoothness.floatValue = 0.02f;
        enableRimLighting.floatValue = 1;
        rimIntensity.floatValue = 3f;
        enableCelShading.floatValue = 1;
        celShadingSteps.floatValue = 4f;
        SetKeyword(material, "_RIM_LIGHTING", true);
        SetKeyword(material, "_CEL_SHADING", true);
    }
    
    private void ApplyRealisticToonPreset(Material material)
    {
        shadowThreshold.floatValue = 0.3f;
        shadowSmoothness.floatValue = 0.2f;
        indirectLightingBoost.floatValue = 0.5f;
        enableSubsurface.floatValue = 1;
        subsurfaceIntensity.floatValue = 0.3f;
        SetKeyword(material, "_SUBSURFACE", true);
    }
    
    private void ApplyCelShadedPreset(Material material)
    {
        shadowThreshold.floatValue = 0.5f;
        shadowSmoothness.floatValue = 0.01f;
        enableCelShading.floatValue = 1;
        celShadingSteps.floatValue = 3f;
        enablePosterize.floatValue = 1;
        posterizeLevels.floatValue = 6f;
        SetKeyword(material, "_CEL_SHADING", true);
        SetKeyword(material, "_POSTERIZE", true);
    }
    
    private void ApplyPainterlyPreset(Material material)
    {
        shadowThreshold.floatValue = 0.35f;
        shadowSmoothness.floatValue = 0.3f;
        enableMatcap.floatValue = 1;
        matcapIntensity.floatValue = 0.8f;
        enableFresnel.floatValue = 1;
        fresnelIntensity.floatValue = 1.5f;
        SetKeyword(material, "_MATCAP", true);
        SetKeyword(material, "_FRESNEL", true);
    }
    
    private void DrawInfo()
    {
        EditorGUILayout.BeginVertical("helpBox");
        EditorGUILayout.LabelField("ℹ️ Shader Information", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("✅ Unity 6 URP Compatible", EditorStyles.miniLabel);
        EditorGUILayout.LabelField("📱 Mobile Optimized", EditorStyles.miniLabel);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("🌟 Advanced Toon Features", EditorStyles.miniLabel);
        EditorGUILayout.LabelField("⚡ Performance Optimized", EditorStyles.miniLabel);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("💡 Tip: Use presets for quick setup, then customize individual properties!", EditorStyles.helpBox);
        EditorGUILayout.EndVertical();
    }
}
