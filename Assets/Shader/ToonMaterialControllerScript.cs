using UnityEngine;
using System.Collections;

[System.Serializable]
public class ToonLightingSettings
{
    [Header("Shadow Settings")]
    [Range(0f, 1f)]
    public float shadowThreshold = 0.5f;
    
    [Range(0f, 0.5f)]
    public float shadowSmoothness = 0.05f;
    
    public Color shadowColor = new Color(0.7f, 0.7f, 0.8f, 1f);
    
    [Range(0f, 1f)]
    public float shadowIntensity = 0.8f;
    
    [Header("Advanced Lighting")]
    [Range(0f, 2f)]
    public float indirectLightingBoost = 0.3f;
    
    [Range(0f, 1f)]
    public float ambientOcclusion = 1f;
    
    [Range(0f, 1f)]
    public float lightWrapping = 0f;
    
    [Header("Ramp Texture")]
    public bool useRampTexture = false;
    public Texture2D lightRampTexture;
}

[System.Serializable]
public class ToonRimLighting
{
    public bool enableRimLighting = true;
    public Color rimColor = Color.white;
    
    [Range(0.1f, 10f)]
    public float rimPower = 2f;
    
    [Range(0f, 5f)]
    public float rimIntensity = 1f;
    
    [Range(0f, 1f)]
    public float rimThreshold = 0.1f;
}

[System.Serializable]
public class ToonSpecular
{
    public bool enableSpecular = true;
    public Color specularColor = Color.white;
    
    [Range(0.001f, 1f)]
    public float specularSize = 0.1f;
    
    [Range(0.001f, 0.5f)]
    public float specularSmoothness = 0.05f;
    
    [Range(0f, 5f)]
    public float specularIntensity = 1f;
}

[System.Serializable]
public class ToonHatching
{
    [Header("Basic Hatching")]
    public bool enableHatching = false;
    public Texture2D hatchingTexture;
    public Texture2D crossHatchingTexture;
    
    [Range(0.1f, 5f)]
    public float hatchingDensity = 1f;
    
    [Range(0f, 2f)]
    public float hatchingIntensity = 1f;
    
    [Range(0f, 1f)]
    public float hatchingThreshold = 0.5f;
    
    [Range(0f, 1f)]
    public float crossHatchingThreshold = 0.3f;
    
    [Range(0f, 360f)]
    public float hatchingRotation = 45f;
    
    [Header("Screen Space Hatching")]
    public bool enableScreenSpaceHatching = false;
    
    [Range(0.1f, 10f)]
    public float screenHatchScale = 2f;
    
    [Range(-1f, 1f)]
    public float screenHatchBias = 0f;
}

[System.Serializable]
public class ToonOutline
{
    public bool enableOutline = false;
    public Color outlineColor = Color.black;
    
    [Range(0f, 0.1f)]
    public float outlineWidth = 0.01f;
}

[System.Serializable]
public class ToonMatcap
{
    public bool enableMatcap = false;
    public Texture2D matcapTexture;
    
    [Range(0f, 2f)]
    public float matcapIntensity = 1f;
    
    [Header("Blend Mode")]
    public MatcapBlendMode blendMode = MatcapBlendMode.Add;
}

public enum MatcapBlendMode
{
    Add = 0,
    Multiply = 1,
    Screen = 2
}

[System.Serializable]
public class ToonFresnel
{
    public bool enableFresnel = false;
    public Color fresnelColor = Color.white;
    
    [Range(0.1f, 10f)]
    public float fresnelPower = 2f;
    
    [Range(0f, 3f)]
    public float fresnelIntensity = 1f;
}

[System.Serializable]
public class ToonSubsurface
{
    public bool enableSubsurface = false;
    public Color subsurfaceColor = new Color(1f, 0.5f, 0.5f, 1f);
    
    [Range(0.1f, 10f)]
    public float subsurfacePower = 3f;
    
    [Range(0f, 2f)]
    public float subsurfaceIntensity = 0.5f;
}

[System.Serializable]
public class ToonColorGrading
{
    [Range(0f, 2f)]
    public float saturation = 1f;
    
    [Range(0f, 2f)]
    public float brightness = 1f;
    
    [Range(-180f, 180f)]
    public float hueShift = 0f;
    
    [Range(0.5f, 3f)]
    public float contrast = 1f;
    
    [Range(0.5f, 3f)]
    public float gamma = 1f;
}

[System.Serializable]
public class ToonStylization
{
    [Header("Posterization")]
    public bool enablePosterize = false;
    
    [Range(2f, 32f)]
    public float posterizeLevels = 8f;
    
    [Header("Cel Shading")]
    public bool enableCelShading = false;
    
    [Range(2f, 10f)]
    public float celShadingSteps = 3f;
}

[System.Serializable]
public class ToonAnimationSettings
{
    [Header("Rim Animation")]
    public bool animateRimLighting = false;
    public float rimAnimationSpeed = 2f;
    public AnimationCurve rimAnimationCurve = AnimationCurve.EaseInOut(0, 0.5f, 1, 1.5f);
    
    [Header("Emission Animation")]
    public bool animateEmission = false;
    public float emissionAnimationSpeed = 1f;
    public AnimationCurve emissionAnimationCurve = AnimationCurve.EaseInOut(0, 0.3f, 1, 1f);
    
    [Header("Color Animation")]
    public bool animateHue = false;
    public float hueAnimationSpeed = 0.5f;
    
    [Header("Hatching Animation")]
    public bool animateHatching = false;
    public float hatchingAnimationSpeed = 0.3f;
    
    [Header("Breathing Effect")]
    public bool enableBreathingEffect = false;
    public float breathingSpeed = 1f;
    public float breathingIntensity = 0.1f;
}

/// <summary>
/// Ultimate Toon Material Controller Pro
/// Enhanced runtime control system for the Ultimate Toon Shader with Hatching Effects
/// </summary>
[RequireComponent(typeof(Renderer))]
public class UltimateToonMaterialControllerPro : MonoBehaviour
{
    [Header("Material Settings")]
    public Material toonMaterial;
    
    [Header("Base Properties")]
    public Color baseColor = Color.white;
    public ToonColorGrading colorGrading = new ToonColorGrading();
    
    [Header("Lighting System")]
    public ToonLightingSettings lightingSettings = new ToonLightingSettings();
    
    [Header("Visual Effects")]
    public ToonRimLighting rimLighting = new ToonRimLighting();
    public ToonSpecular specular = new ToonSpecular();
    public ToonHatching hatching = new ToonHatching();
    public ToonOutline outline = new ToonOutline();
    public ToonMatcap matcap = new ToonMatcap();
    public ToonFresnel fresnel = new ToonFresnel();
    public ToonSubsurface subsurface = new ToonSubsurface();
    
    [Header("Stylization")]
    public ToonStylization stylization = new ToonStylization();
    
    [Header("Textures")]
    public Texture2D normalMap;
    [Range(0f, 2f)]
    public float normalScale = 1f;
    
    public Texture2D emissionMap;
    public Color emissionColor = Color.black;
    [Range(0f, 10f)]
    public float emissionIntensity = 1f;
    public Vector2 emissionScrollSpeed = Vector2.zero;
    
    [Header("Animation System")]
    public ToonAnimationSettings animationSettings = new ToonAnimationSettings();
    
    [Header("Quality Settings")]
    [Range(0, 3)]
    public int qualityLevel = 2; // 0=Low, 1=Medium, 2=High, 3=Ultra
    
    private Renderer objectRenderer;
    private Material materialInstance;
    private float animationTime;
    private Coroutine qualityTransitionCoroutine;
    
    // Cached shader property IDs for performance
    private static class ShaderIDs
    {
        public static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");
        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int ShadowThreshold = Shader.PropertyToID("_ShadowThreshold");
        public static readonly int ShadowSmoothness = Shader.PropertyToID("_ShadowSmoothness");
        public static readonly int ShadowColor = Shader.PropertyToID("_ShadowColor");
        public static readonly int ShadowIntensity = Shader.PropertyToID("_ShadowIntensity");
        public static readonly int LightRampTex = Shader.PropertyToID("_LightRampTex");
        public static readonly int IndirectLightingBoost = Shader.PropertyToID("_IndirectLightingBoost");
        public static readonly int AmbientOcclusion = Shader.PropertyToID("_AmbientOcclusion");
        public static readonly int LightWrapping = Shader.PropertyToID("_LightWrapping");
        public static readonly int RimColor = Shader.PropertyToID("_RimColor");
        public static readonly int RimPower = Shader.PropertyToID("_RimPower");
        public static readonly int RimIntensity = Shader.PropertyToID("_RimIntensity");
        public static readonly int RimThreshold = Shader.PropertyToID("_RimThreshold");
        public static readonly int SpecularColor = Shader.PropertyToID("_SpecularColor");
        public static readonly int SpecularSize = Shader.PropertyToID("_SpecularSize");
        public static readonly int SpecularSmoothness = Shader.PropertyToID("_SpecularSmoothness");
        public static readonly int SpecularIntensity = Shader.PropertyToID("_SpecularIntensity");
        
        // Hatching properties
        public static readonly int HatchingTex = Shader.PropertyToID("_HatchingTex");
        public static readonly int CrossHatchingTex = Shader.PropertyToID("_CrossHatchingTex");
        public static readonly int HatchingDensity = Shader.PropertyToID("_HatchingDensity");
        public static readonly int HatchingIntensity = Shader.PropertyToID("_HatchingIntensity");
        public static readonly int HatchingThreshold = Shader.PropertyToID("_HatchingThreshold");
        public static readonly int CrossHatchingThreshold = Shader.PropertyToID("_CrossHatchingThreshold");
        public static readonly int HatchingRotation = Shader.PropertyToID("_HatchingRotation");
        public static readonly int ScreenHatchScale = Shader.PropertyToID("_ScreenHatchScale");
        public static readonly int ScreenHatchBias = Shader.PropertyToID("_ScreenHatchBias");
        
        // Outline properties
        public static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
        public static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        
        public static readonly int MatcapTex = Shader.PropertyToID("_MatcapTex");
        public static readonly int MatcapIntensity = Shader.PropertyToID("_MatcapIntensity");
        public static readonly int MatcapBlendMode = Shader.PropertyToID("_MatcapBlendMode");
        public static readonly int BumpMap = Shader.PropertyToID("_BumpMap");
        public static readonly int BumpScale = Shader.PropertyToID("_BumpScale");
        public static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");
        public static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        public static readonly int EmissionIntensity = Shader.PropertyToID("_EmissionIntensity");
        public static readonly int EmissionScrollSpeed = Shader.PropertyToID("_EmissionScrollSpeed");
        public static readonly int FresnelColor = Shader.PropertyToID("_FresnelColor");
        public static readonly int FresnelPower = Shader.PropertyToID("_FresnelPower");
        public static readonly int FresnelIntensity = Shader.PropertyToID("_FresnelIntensity");
        public static readonly int SubsurfaceColor = Shader.PropertyToID("_SubsurfaceColor");
        public static readonly int SubsurfacePower = Shader.PropertyToID("_SubsurfacePower");
        public static readonly int SubsurfaceIntensity = Shader.PropertyToID("_SubsurfaceIntensity");
        public static readonly int Hue = Shader.PropertyToID("_Hue");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        public static readonly int PosterizeLevels = Shader.PropertyToID("_PosterizeLevels");
        public static readonly int CelShadingSteps = Shader.PropertyToID("_CelShadingSteps");
    }
    
    private void Start()
    {
        InitializeMaterial();
        UpdateAllProperties();
        ApplyQualitySettings();
    }
    
    private void Update()
    {
        HandleAnimations();
    }
    
    private void OnValidate()
    {
        if (Application.isPlaying && materialInstance != null)
        {
            UpdateAllProperties();
        }
    }
    
    private void InitializeMaterial()
    {
        objectRenderer = GetComponent<Renderer>();
        
        if (toonMaterial != null)
        {
            materialInstance = new Material(toonMaterial);
            objectRenderer.material = materialInstance;
        }
        else
        {
            materialInstance = objectRenderer.material;
        }
    }
    
    private void UpdateAllProperties()
    {
        if (materialInstance == null) return;
        
        // Base properties
        materialInstance.SetColor(ShaderIDs.BaseColor, baseColor);
        materialInstance.SetFloat(ShaderIDs.Saturation, colorGrading.saturation);
        materialInstance.SetFloat(ShaderIDs.Brightness, colorGrading.brightness);
        
        // Lighting settings
        materialInstance.SetFloat(ShaderIDs.ShadowThreshold, lightingSettings.shadowThreshold);
        materialInstance.SetFloat(ShaderIDs.ShadowSmoothness, lightingSettings.shadowSmoothness);
        materialInstance.SetColor(ShaderIDs.ShadowColor, lightingSettings.shadowColor);
        materialInstance.SetFloat(ShaderIDs.ShadowIntensity, lightingSettings.shadowIntensity);
        materialInstance.SetFloat(ShaderIDs.IndirectLightingBoost, lightingSettings.indirectLightingBoost);
        materialInstance.SetFloat(ShaderIDs.AmbientOcclusion, lightingSettings.ambientOcclusion);
        materialInstance.SetFloat(ShaderIDs.LightWrapping, lightingSettings.lightWrapping);
        
        // Ramp texture
        if (lightingSettings.lightRampTexture != null)
        {
            materialInstance.SetTexture(ShaderIDs.LightRampTex, lightingSettings.lightRampTexture);
        }
        
        // Rim lighting
        materialInstance.SetColor(ShaderIDs.RimColor, rimLighting.rimColor);
        materialInstance.SetFloat(ShaderIDs.RimPower, rimLighting.rimPower);
        materialInstance.SetFloat(ShaderIDs.RimIntensity, rimLighting.rimIntensity);
        materialInstance.SetFloat(ShaderIDs.RimThreshold, rimLighting.rimThreshold);
        
        // Specular
        materialInstance.SetColor(ShaderIDs.SpecularColor, specular.specularColor);
        materialInstance.SetFloat(ShaderIDs.SpecularSize, specular.specularSize);
        materialInstance.SetFloat(ShaderIDs.SpecularSmoothness, specular.specularSmoothness);
        materialInstance.SetFloat(ShaderIDs.SpecularIntensity, specular.specularIntensity);
        
        // Hatching
        if (hatching.hatchingTexture != null)
        {
            materialInstance.SetTexture(ShaderIDs.HatchingTex, hatching.hatchingTexture);
        }
        if (hatching.crossHatchingTexture != null)
        {
            materialInstance.SetTexture(ShaderIDs.CrossHatchingTex, hatching.crossHatchingTexture);
        }
        materialInstance.SetFloat(ShaderIDs.HatchingDensity, hatching.hatchingDensity);
        materialInstance.SetFloat(ShaderIDs.HatchingIntensity, hatching.hatchingIntensity);
        materialInstance.SetFloat(ShaderIDs.HatchingThreshold, hatching.hatchingThreshold);
        materialInstance.SetFloat(ShaderIDs.CrossHatchingThreshold, hatching.crossHatchingThreshold);
        materialInstance.SetFloat(ShaderIDs.HatchingRotation, hatching.hatchingRotation);
        materialInstance.SetFloat(ShaderIDs.ScreenHatchScale, hatching.screenHatchScale);
        materialInstance.SetFloat(ShaderIDs.ScreenHatchBias, hatching.screenHatchBias);
        
        // Outline
        materialInstance.SetColor(ShaderIDs.OutlineColor, outline.outlineColor);
        materialInstance.SetFloat(ShaderIDs.OutlineWidth, outline.outlineWidth);
        
        // Matcap
        if (matcap.matcapTexture != null)
        {
            materialInstance.SetTexture(ShaderIDs.MatcapTex, matcap.matcapTexture);
        }
        materialInstance.SetFloat(ShaderIDs.MatcapIntensity, matcap.matcapIntensity);
        materialInstance.SetFloat(ShaderIDs.MatcapBlendMode, (float)matcap.blendMode);
        
        // Normal mapping
        if (normalMap != null)
        {
            materialInstance.SetTexture(ShaderIDs.BumpMap, normalMap);
        }
        materialInstance.SetFloat(ShaderIDs.BumpScale, normalScale);
        
        // Emission
        if (emissionMap != null)
        {
            materialInstance.SetTexture(ShaderIDs.EmissionMap, emissionMap);
        }
        materialInstance.SetColor(ShaderIDs.EmissionColor, emissionColor);
        materialInstance.SetFloat(ShaderIDs.EmissionIntensity, emissionIntensity);
        materialInstance.SetVector(ShaderIDs.EmissionScrollSpeed, new Vector4(emissionScrollSpeed.x, emissionScrollSpeed.y, 0, 0));
        
        // Fresnel
        materialInstance.SetColor(ShaderIDs.FresnelColor, fresnel.fresnelColor);
        materialInstance.SetFloat(ShaderIDs.FresnelPower, fresnel.fresnelPower);
        materialInstance.SetFloat(ShaderIDs.FresnelIntensity, fresnel.fresnelIntensity);
        
        // Subsurface
        materialInstance.SetColor(ShaderIDs.SubsurfaceColor, subsurface.subsurfaceColor);
        materialInstance.SetFloat(ShaderIDs.SubsurfacePower, subsurface.subsurfacePower);
        materialInstance.SetFloat(ShaderIDs.SubsurfaceIntensity, subsurface.subsurfaceIntensity);
        
        // Color grading
        materialInstance.SetFloat(ShaderIDs.Hue, colorGrading.hueShift);
        materialInstance.SetFloat(ShaderIDs.Contrast, colorGrading.contrast);
        materialInstance.SetFloat(ShaderIDs.Gamma, colorGrading.gamma);
        
        // Stylization
        materialInstance.SetFloat(ShaderIDs.PosterizeLevels, stylization.posterizeLevels);
        materialInstance.SetFloat(ShaderIDs.CelShadingSteps, stylization.celShadingSteps);
        
        // Update keywords
        UpdateShaderKeywords();
    }
    
    private void UpdateShaderKeywords()
    {
        if (materialInstance == null) return;
        
        // Set shader keywords based on enabled features
        SetKeyword("_RIM_LIGHTING", rimLighting.enableRimLighting);
        SetKeyword("_SPECULAR", specular.enableSpecular);
        SetKeyword("_HATCHING", hatching.enableHatching);
        SetKeyword("_SCREEN_SPACE_HATCHING", hatching.enableScreenSpaceHatching);
        SetKeyword("_OUTLINE", outline.enableOutline);
        SetKeyword("_MATCAP", matcap.enableMatcap);
        SetKeyword("_NORMALMAP", normalMap != null);
        SetKeyword("_EMISSION", emissionMap != null || emissionColor != Color.black);
        SetKeyword("_FRESNEL", fresnel.enableFresnel);
        SetKeyword("_SUBSURFACE", subsurface.enableSubsurface);
        SetKeyword("_USE_RAMP_TEXTURE", lightingSettings.useRampTexture);
        SetKeyword("_POSTERIZE", stylization.enablePosterize);
        SetKeyword("_CEL_SHADING", stylization.enableCelShading);
    }
    
    private void SetKeyword(string keyword, bool enabled)
    {
        if (enabled)
            materialInstance.EnableKeyword(keyword);
        else
            materialInstance.DisableKeyword(keyword);
    }
    
    private void HandleAnimations()
    {
        animationTime += Time.deltaTime;
        
        // Rim lighting animation
        if (animationSettings.animateRimLighting && rimLighting.enableRimLighting)
        {
            float animTime = animationTime * animationSettings.rimAnimationSpeed;
            float animatedIntensity = rimLighting.rimIntensity * animationSettings.rimAnimationCurve.Evaluate(animTime % 1f);
            materialInstance.SetFloat(ShaderIDs.RimIntensity, animatedIntensity);
        }
        
        // Emission animation
        if (animationSettings.animateEmission && (emissionMap != null || emissionColor != Color.black))
        {
            float animTime = animationTime * animationSettings.emissionAnimationSpeed;
            float animatedIntensity = emissionIntensity * animationSettings.emissionAnimationCurve.Evaluate(animTime % 1f);
            materialInstance.SetFloat(ShaderIDs.EmissionIntensity, animatedIntensity);
        }
        
        // Hue animation
        if (animationSettings.animateHue)
        {
            float animatedHue = (animationTime * animationSettings.hueAnimationSpeed * 360f) % 360f - 180f;
            materialInstance.SetFloat(ShaderIDs.Hue, animatedHue);
        }
        
        // Hatching animation
        if (animationSettings.animateHatching && hatching.enableHatching)
        {
            float animTime = animationTime * animationSettings.hatchingAnimationSpeed;
            float animatedRotation = hatching.hatchingRotation + (Mathf.Sin(animTime) * 15f);
            materialInstance.SetFloat(ShaderIDs.HatchingRotation, animatedRotation);
        }
        
        // Breathing effect
        if (animationSettings.enableBreathingEffect)
        {
            float breathingValue = 1f + Mathf.Sin(animationTime * animationSettings.breathingSpeed) * animationSettings.breathingIntensity;
            materialInstance.SetFloat(ShaderIDs.RimIntensity, rimLighting.rimIntensity * breathingValue);
        }
    }
    
    private void ApplyQualitySettings()
    {
        switch (qualityLevel)
        {
            case 0: // Low
                SetLowQuality();
                break;
            case 1: // Medium
                SetMediumQuality();
                break;
            case 2: // High
                SetHighQuality();
                break;
            case 3: // Ultra
                SetUltraQuality();
                break;
        }
    }
    
    private void SetLowQuality()
    {
        // Disable expensive features for mobile
        fresnel.enableFresnel = false;
        subsurface.enableSubsurface = false;
        matcap.enableMatcap = false;
        stylization.enablePosterize = false;
        hatching.enableScreenSpaceHatching = false;
        
        lightingSettings.shadowSmoothness = Mathf.Min(lightingSettings.shadowSmoothness, 0.1f);
        UpdateShaderKeywords();
    }
    
    private void SetMediumQuality()
    {
        // Enable some features
        rimLighting.enableRimLighting = true;
        specular.enableSpecular = true;
        hatching.enableHatching = true;
        hatching.enableScreenSpaceHatching = false;
        
        lightingSettings.shadowSmoothness = Mathf.Min(lightingSettings.shadowSmoothness, 0.2f);
        UpdateShaderKeywords();
    }
    
    private void SetHighQuality()
    {
        // Enable most features
        rimLighting.enableRimLighting = true;
        specular.enableSpecular = true;
        hatching.enableHatching = true;
        hatching.enableScreenSpaceHatching = true;
        fresnel.enableFresnel = true;
        
        UpdateShaderKeywords();
    }
    
    private void SetUltraQuality()
    {
        // Enable all features
        rimLighting.enableRimLighting = true;
        specular.enableSpecular = true;
        hatching.enableHatching = true;
        hatching.enableScreenSpaceHatching = true;
        fresnel.enableFresnel = true;
        subsurface.enableSubsurface = true;
        matcap.enableMatcap = true;
        
        UpdateShaderKeywords();
    }
    
    // Public API methods for runtime control
    public void SetBaseColor(Color color)
    {
        baseColor = color;
        if (materialInstance != null)
            materialInstance.SetColor(ShaderIDs.BaseColor, baseColor);
    }
    
    public void SetShadowThreshold(float threshold)
    {
        lightingSettings.shadowThreshold = Mathf.Clamp01(threshold);
        if (materialInstance != null)
            materialInstance.SetFloat(ShaderIDs.ShadowThreshold, lightingSettings.shadowThreshold);
    }
    
    public void SetRimIntensity(float intensity)
    {
        rimLighting.rimIntensity = Mathf.Clamp(intensity, 0f, 5f);
        if (materialInstance != null)
            materialInstance.SetFloat(ShaderIDs.RimIntensity, rimLighting.rimIntensity);
    }
    
    public void SetHatchingIntensity(float intensity)
    {
        hatching.hatchingIntensity = Mathf.Clamp(intensity, 0f, 2f);
        if (materialInstance != null)
            materialInstance.SetFloat(ShaderIDs.HatchingIntensity, hatching.hatchingIntensity);
    }
    
    public void SetHatchingThreshold(float threshold)
    {
        hatching.hatchingThreshold = Mathf.Clamp01(threshold);
        if (materialInstance != null)
            materialInstance.SetFloat(ShaderIDs.HatchingThreshold, hatching.hatchingThreshold);
    }
    
    public void SetEmissionIntensity(float intensity)
    {
        emissionIntensity = Mathf.Clamp(intensity, 0f, 10f);
        if (materialInstance != null)
            materialInstance.SetFloat(ShaderIDs.EmissionIntensity, emissionIntensity);
    }
    
    public void SetQualityLevel(int level)
    {
        qualityLevel = Mathf.Clamp(level, 0, 3);
        
        if (qualityTransitionCoroutine != null)
            StopCoroutine(qualityTransitionCoroutine);
        
        qualityTransitionCoroutine = StartCoroutine(SmoothQualityTransition(level));
    }
    
    private IEnumerator SmoothQualityTransition(int targetLevel)
    {
        float transitionTime = 0.5f;
        float elapsedTime = 0f;
        
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionTime;
            
            // Smooth transition logic here
            yield return null;
        }
        
        ApplyQualitySettings();
        UpdateAllProperties();
    }
    
    // Toggle functions
    public void ToggleRimLighting()
    {
        rimLighting.enableRimLighting = !rimLighting.enableRimLighting;
        SetKeyword("_RIM_LIGHTING", rimLighting.enableRimLighting);
    }
    
    public void ToggleSpecular()
    {
        specular.enableSpecular = !specular.enableSpecular;
        SetKeyword("_SPECULAR", specular.enableSpecular);
    }
    
    public void ToggleHatching()
    {
        hatching.enableHatching = !hatching.enableHatching;
        SetKeyword("_HATCHING", hatching.enableHatching);
    }
    
    public void ToggleScreenSpaceHatching()
    {
        hatching.enableScreenSpaceHatching = !hatching.enableScreenSpaceHatching;
        SetKeyword("_SCREEN_SPACE_HATCHING", hatching.enableScreenSpaceHatching);
    }
    
    public void ToggleOutline()
    {
        outline.enableOutline = !outline.enableOutline;
        SetKeyword("_OUTLINE", outline.enableOutline);
    }
    
    public void ToggleMatcap()
    {
        matcap.enableMatcap = !matcap.enableMatcap;
        SetKeyword("_MATCAP", matcap.enableMatcap);
    }
    
    // Enhanced preset system
    public void ApplyAnimePreset()
    {
        lightingSettings.shadowThreshold = 0.4f;
        lightingSettings.shadowSmoothness = 0.1f;
        rimLighting.enableRimLighting = true;
        rimLighting.rimPower = 1.5f;
        rimLighting.rimIntensity = 2f;
        specular.enableSpecular = true;
        specular.specularSize = 0.05f;
        specular.specularIntensity = 2f;
        hatching.enableHatching = false;
        outline.enableOutline = false;
        UpdateAllProperties();
    }
    
    public void ApplyCartoonPreset()
    {
        lightingSettings.shadowThreshold = 0.6f;
        lightingSettings.shadowSmoothness = 0.02f;
        rimLighting.enableRimLighting = true;
        rimLighting.rimIntensity = 3f;
        stylization.enableCelShading = true;
        stylization.celShadingSteps = 4f;
        outline.enableOutline = true;
        outline.outlineWidth = 0.02f;
        hatching.enableHatching = false;
        UpdateAllProperties();
    }
    
    public void ApplySketchPreset()
    {
        lightingSettings.shadowThreshold = 0.4f;
        lightingSettings.shadowSmoothness = 0.05f;
        hatching.enableHatching = true;
        hatching.hatchingDensity = 2f;
        hatching.hatchingIntensity = 0.8f;
        hatching.hatchingThreshold = 0.6f;
        hatching.crossHatchingThreshold = 0.3f;
        hatching.hatchingRotation = 45f;
        colorGrading.saturation = 0.8f;
        colorGrading.contrast = 1.2f;
        UpdateAllProperties();
    }
    
    public void ApplyHatchedDrawingPreset()
    {
        lightingSettings.shadowThreshold = 0.45f;
        lightingSettings.shadowSmoothness = 0.1f;
        hatching.enableHatching = true;
        hatching.hatchingDensity = 1.5f;
        hatching.hatchingIntensity = 1.2f;
        hatching.hatchingThreshold = 0.7f;
        hatching.crossHatchingThreshold = 0.4f;
        hatching.enableScreenSpaceHatching = true;
        hatching.screenHatchScale = 3f;
        hatching.screenHatchBias = 0.2f;
        colorGrading.saturation = 0.7f;
        colorGrading.contrast = 1.3f;
        UpdateAllProperties();
    }
    
    public void ApplyComicBookPreset()
    {
        lightingSettings.shadowThreshold = 0.5f;
        lightingSettings.shadowSmoothness = 0.02f;
        outline.enableOutline = true;
        outline.outlineColor = Color.black;
        outline.outlineWidth = 0.02f;
        rimLighting.enableRimLighting = true;
        rimLighting.rimIntensity = 2.5f;
        specular.enableSpecular = true;
        specular.specularIntensity = 3f;
        colorGrading.saturation = 1.3f;
        colorGrading.contrast = 1.4f;
        UpdateAllProperties();
    }
    
    // Performance monitoring
    public int GetActiveFeatureCount()
    {
        int count = 0;
        if (rimLighting.enableRimLighting) count++;
        if (specular.enableSpecular) count++;
        if (hatching.enableHatching) count++;
        if (hatching.enableScreenSpaceHatching) count++;
        if (outline.enableOutline) count++;
        if (matcap.enableMatcap) count++;
        if (fresnel.enableFresnel) count++;
        if (subsurface.enableSubsurface) count++;
        if (stylization.enablePosterize) count++;
        if (stylization.enableCelShading) count++;
        return count;
    }
    
    public string GetPerformanceInfo()
    {
        return $"Active Features: {GetActiveFeatureCount()}/10, Quality: {qualityLevel}, Animated: {(animationSettings.animateRimLighting || animationSettings.animateEmission || animationSettings.animateHatching)}";
    }
    
    private void OnDestroy()
    {
        if (qualityTransitionCoroutine != null)
            StopCoroutine(qualityTransitionCoroutine);
            
        if (materialInstance != null)
            DestroyImmediate(materialInstance);
    }
}