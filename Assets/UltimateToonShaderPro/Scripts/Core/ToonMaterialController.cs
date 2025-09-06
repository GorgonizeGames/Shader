using UnityEngine;
using System.Collections;
using Gorgonize.ToonShader.Core;
using Gorgonize.ToonShader.Settings;

namespace Gorgonize.ToonShader
{
    /// <summary>
    /// Main controller for the Ultimate Toon Shader Pro
    /// Manages all shader properties and provides runtime control
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class ToonMaterialController : MonoBehaviour
    {
        [Header("Material Configuration")]
        [Tooltip("Source material to create instance from")]
        public Material sourceMaterial;
        
        [Header("Base Properties")]
        [Tooltip("Main color tint")]
        public Color baseColor = Color.white;
        
        [Header("Textures")]
        [Tooltip("Main albedo texture")]
        public Texture2D mainTexture;
        
        [Tooltip("Normal map texture")]
        public Texture2D normalMap;
        
        [Range(0f, 2f)]
        [Tooltip("Normal map intensity")]
        public float normalScale = 1f;
        
        [Tooltip("Emission texture")]
        public Texture2D emissionMap;
        
        [Tooltip("Emission color")]
        public Color emissionColor = Color.black;
        
        [Range(0f, 10f)]
        [Tooltip("Emission intensity")]
        public float emissionIntensity = 1f;
        
        [Header("Shader Settings")]
        public ToonLightingSettings lightingSettings = new ToonLightingSettings();
        public ToonVisualEffects visualEffects = new ToonVisualEffects();
        public ToonStylization stylization = new ToonStylization();
        public ToonAnimationSettings animationSettings = new ToonAnimationSettings();
        
        [Header("Quality & Performance")]
        [Range(0, 3)]
        [Tooltip("Quality level: 0=Low, 1=Medium, 2=High, 3=Ultra")]
        public int qualityLevel = 2;
        
        [Tooltip("Enable automatic performance monitoring")]
        public bool enablePerformanceMonitoring = true;

        // Private fields
        private Renderer objectRenderer;
        private Material materialInstance;
        private float animationTime;
        private Coroutine qualityTransitionCoroutine;
        
        // Performance tracking
        private float lastPerformanceCheck;
        private const float PERFORMANCE_CHECK_INTERVAL = 1f;
        
        // Animation base values for restoration
        private float baseRimIntensity;
        private float baseEmissionIntensity;
        private float baseHatchingRotation;
        private float baseHue;

        #region Unity Lifecycle
        private void Start()
        {
            InitializeMaterial();
            CacheBaseValues();
            UpdateAllProperties();
            ApplyQualitySettings();
            
            if (enablePerformanceMonitoring)
                InvokeRepeating(nameof(MonitorPerformance), PERFORMANCE_CHECK_INTERVAL, PERFORMANCE_CHECK_INTERVAL);
        }

        private void Update()
        {
            if (animationSettings.HasActiveAnimations())
            {
                animationTime += Time.deltaTime;
                HandleAnimations();
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying && materialInstance != null)
            {
                ValidateAllSettings();
                UpdateAllProperties();
            }
        }

        private void OnDestroy()
        {
            CleanupResources();
        }
        #endregion

        #region Initialization
        private void InitializeMaterial()
        {
            objectRenderer = GetComponent<Renderer>();
            
            if (sourceMaterial != null)
            {
                materialInstance = new Material(sourceMaterial);
                objectRenderer.material = materialInstance;
            }
            else
            {
                materialInstance = objectRenderer.material;
                if (materialInstance != null)
                {
                    // Create instance to avoid modifying shared material
                    materialInstance = new Material(materialInstance);
                    objectRenderer.material = materialInstance;
                }
            }
        }

        private void CacheBaseValues()
        {
            baseRimIntensity = visualEffects.rimLighting.rimIntensity;
            baseEmissionIntensity = emissionIntensity;
            baseHatchingRotation = stylization.hatching.hatchingRotation;
            baseHue = stylization.colorGrading.hueShift;
        }
        #endregion

        #region Property Updates
        /// <summary>
        /// Updates all shader properties on the material
        /// </summary>
        public void UpdateAllProperties()
        {
            if (materialInstance == null) return;

            UpdateBaseProperties();
            UpdateTextures();
            
            lightingSettings.ApplyToMaterial(materialInstance);
            visualEffects.ApplyToMaterial(materialInstance);
            stylization.ApplyToMaterial(materialInstance);
        }

        private void UpdateBaseProperties()
        {
            ToonShaderProperties.SetColorSafe(materialInstance, ToonShaderProperties.BaseColor, baseColor);
        }

        private void UpdateTextures()
        {
            if (mainTexture != null)
                ToonShaderProperties.SetTextureSafe(materialInstance, ToonShaderProperties.BaseMap, mainTexture);

            // Normal mapping
            bool hasNormalMap = normalMap != null;
            ToonShaderProperties.SetFloatSafe(materialInstance, ToonShaderProperties.EnableNormalMap, hasNormalMap ? 1f : 0f);
            ToonShaderProperties.SetKeywordSafe(materialInstance, ToonShaderProperties.Keywords.NormalMap, hasNormalMap);
            
            if (hasNormalMap)
            {
                ToonShaderProperties.SetTextureSafe(materialInstance, ToonShaderProperties.BumpMap, normalMap);
                ToonShaderProperties.SetFloatSafe(materialInstance, ToonShaderProperties.BumpScale, normalScale);
            }

            // Emission
            bool hasEmission = emissionMap != null || emissionColor != Color.black;
            ToonShaderProperties.SetFloatSafe(materialInstance, ToonShaderProperties.EnableEmission, hasEmission ? 1f : 0f);
            ToonShaderProperties.SetKeywordSafe(materialInstance, ToonShaderProperties.Keywords.Emission, hasEmission);
            
            if (hasEmission)
            {
                if (emissionMap != null)
                    ToonShaderProperties.SetTextureSafe(materialInstance, ToonShaderProperties.EmissionMap, emissionMap);
                
                ToonShaderProperties.SetColorSafe(materialInstance, ToonShaderProperties.EmissionColor, emissionColor);
                ToonShaderProperties.SetFloatSafe(materialInstance, ToonShaderProperties.EmissionIntensity, emissionIntensity);
            }
        }
        #endregion

        #region Animation System
        private void HandleAnimations()
        {
            animationSettings.UpdateAnimations(materialInstance, Time.deltaTime, animationTime,
                baseRimIntensity, baseEmissionIntensity, baseHatchingRotation);
        }

        /// <summary>
        /// Resets all animations to base values
        /// </summary>
        public void ResetAnimations()
        {
            animationSettings.ResetAnimations(materialInstance, baseRimIntensity, baseEmissionIntensity, 
                baseHatchingRotation, baseHue);
        }
        #endregion

        #region Quality Management
        /// <summary>
        /// Applies quality settings based on current quality level
        /// </summary>
        public void ApplyQualitySettings()
        {
            switch (qualityLevel)
            {
                case 0: ApplyLowQuality(); break;
                case 1: ApplyMediumQuality(); break;
                case 2: ApplyHighQuality(); break;
                case 3: ApplyUltraQuality(); break;
            }
            
            UpdateAllProperties();
        }

        private void ApplyLowQuality()
        {
            // Disable expensive features for mobile
            visualEffects.fresnel.enableFresnel = false;
            visualEffects.subsurface.enableSubsurface = false;
            visualEffects.matcap.enableMatcap = false;
            stylization.quantization.enablePosterize = false;
            stylization.hatching.enableScreenSpaceHatching = false;
            
            // Reduce smoothness for performance
            lightingSettings.shadowSmoothness = Mathf.Min(lightingSettings.shadowSmoothness, 0.1f);
        }

        private void ApplyMediumQuality()
        {
            // Enable basic features
            visualEffects.rimLighting.enableRimLighting = true;
            visualEffects.specular.enableSpecular = true;
            stylization.hatching.enableHatching = true;
            stylization.hatching.enableScreenSpaceHatching = false;
            
            lightingSettings.shadowSmoothness = Mathf.Min(lightingSettings.shadowSmoothness, 0.2f);
        }

        private void ApplyHighQuality()
        {
            // Enable most features
            visualEffects.rimLighting.enableRimLighting = true;
            visualEffects.specular.enableSpecular = true;
            stylization.hatching.enableHatching = true;
            stylization.hatching.enableScreenSpaceHatching = true;
            visualEffects.fresnel.enableFresnel = true;
        }

        private void ApplyUltraQuality()
        {
            // Enable all features
            visualEffects.rimLighting.enableRimLighting = true;
            visualEffects.specular.enableSpecular = true;
            stylization.hatching.enableHatching = true;
            stylization.hatching.enableScreenSpaceHatching = true;
            visualEffects.fresnel.enableFresnel = true;
            visualEffects.subsurface.enableSubsurface = true;
            visualEffects.matcap.enableMatcap = true;
        }

        /// <summary>
        /// Sets quality level with smooth transition
        /// </summary>
        public void SetQualityLevel(int level)
        {
            qualityLevel = Mathf.Clamp(level, 0, 3);
            
            if (qualityTransitionCoroutine != null)
                StopCoroutine(qualityTransitionCoroutine);
            
            qualityTransitionCoroutine = StartCoroutine(SmoothQualityTransition());
        }

        private IEnumerator SmoothQualityTransition()
        {
            float transitionTime = 0.5f;
            float elapsedTime = 0f;
            
            while (elapsedTime < transitionTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            ApplyQualitySettings();
        }
        #endregion

        #region Performance Monitoring
        private void MonitorPerformance()
        {
            if (Time.time - lastPerformanceCheck < PERFORMANCE_CHECK_INTERVAL)
                return;

            lastPerformanceCheck = Time.time;
            
            float totalCost = GetTotalPerformanceCost();
            
            // Auto-adjust quality if performance is poor
            if (totalCost > 0.8f && qualityLevel > 0)
            {
                Debug.LogWarning($"High performance cost detected ({totalCost:F2}). Consider lowering quality level.");
            }
        }

        /// <summary>
        /// Calculates total performance cost of current settings
        /// </summary>
        public float GetTotalPerformanceCost()
        {
            float cost = 0f;
            
            cost += lightingSettings.GetPerformanceCost();
            cost += visualEffects.GetPerformanceCost();
            cost += stylization.GetPerformanceCost();
            cost += animationSettings.GetPerformanceCost();
            
            return Mathf.Clamp01(cost);
        }

        /// <summary>
        /// Returns performance information as string
        /// </summary>
        public string GetPerformanceInfo()
        {
            int activeEffects = visualEffects.GetActiveEffectCount() + stylization.GetActiveEffectCount();
            int activeAnimations = animationSettings.GetActiveAnimationCount();
            float totalCost = GetTotalPerformanceCost();
            
            return $"Effects: {activeEffects}, Animations: {activeAnimations}, Quality: {qualityLevel}, Cost: {totalCost:F2}";
        }
        #endregion

        #region Validation
        private void ValidateAllSettings()
        {
            lightingSettings.ValidateSettings();
            stylization.ValidateSettings();
            animationSettings.ValidateSettings();
            
            qualityLevel = Mathf.Clamp(qualityLevel, 0, 3);
            normalScale = Mathf.Clamp(normalScale, 0f, 2f);
            emissionIntensity = Mathf.Clamp(emissionIntensity, 0f, 10f);
        }
        #endregion

        #region Public API
        /// <summary>
        /// Sets base color
        /// </summary>
        public void SetBaseColor(Color color)
        {
            baseColor = color;
            ToonShaderProperties.SetColorSafe(materialInstance, ToonShaderProperties.BaseColor, baseColor);
        }

        /// <summary>
        /// Sets shadow threshold
        /// </summary>
        public void SetShadowThreshold(float threshold)
        {
            lightingSettings.shadowThreshold = Mathf.Clamp01(threshold);
            ToonShaderProperties.SetFloatSafe(materialInstance, ToonShaderProperties.ShadowThreshold, lightingSettings.shadowThreshold);
        }

        /// <summary>
        /// Sets rim lighting intensity
        /// </summary>
        public void SetRimIntensity(float intensity)
        {
            visualEffects.rimLighting.rimIntensity = Mathf.Clamp(intensity, 0f, 5f);
            baseRimIntensity = visualEffects.rimLighting.rimIntensity;
            
            if (!animationSettings.animateRimLighting && !animationSettings.enableBreathingEffect)
            {
                ToonShaderProperties.SetFloatSafe(materialInstance, ToonShaderProperties.RimIntensity, visualEffects.rimLighting.rimIntensity);
            }
        }

        /// <summary>
        /// Sets emission intensity
        /// </summary>
        public void SetEmissionIntensity(float intensity)
        {
            emissionIntensity = Mathf.Clamp(intensity, 0f, 10f);
            baseEmissionIntensity = emissionIntensity;
            
            if (!animationSettings.animateEmission)
            {
                ToonShaderProperties.SetFloatSafe(materialInstance, ToonShaderProperties.EmissionIntensity, emissionIntensity);
            }
        }

        /// <summary>
        /// Toggles rim lighting
        /// </summary>
        public void ToggleRimLighting()
        {
            visualEffects.rimLighting.enableRimLighting = !visualEffects.rimLighting.enableRimLighting;
            visualEffects.rimLighting.ApplyToMaterial(materialInstance);
        }

        /// <summary>
        /// Toggles hatching effect
        /// </summary>
        public void ToggleHatching()
        {
            stylization.hatching.enableHatching = !stylization.hatching.enableHatching;
            stylization.hatching.ApplyToMaterial(materialInstance);
        }
        #endregion

        #region Cleanup
        private void CleanupResources()
        {
            if (qualityTransitionCoroutine != null)
                StopCoroutine(qualityTransitionCoroutine);

            if (enablePerformanceMonitoring)
                CancelInvoke(nameof(MonitorPerformance));
                
            if (materialInstance != null)
                DestroyImmediate(materialInstance);
        }
        #endregion
    }
}