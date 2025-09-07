using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gorgonize.ToonShader.Core;
using Gorgonize.ToonShader.Settings;
using Gorgonize.ToonShader.Presets;

namespace Gorgonize.ToonShader
{
    /// <summary>
    /// Main controller for the Gorgonize Toon Shader
    /// Provides runtime control, performance monitoring, and advanced features
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    [HelpURL("https://docs.gorgonize.com/toon-shader/material-controller")]
    public class GorgonizeToonMaterialController : MonoBehaviour
    {
        [Header("Material Configuration")]
        [Tooltip("Source material to create instance from")]
        public Material sourceMaterial;
        
        [Tooltip("Enable automatic material instancing")]
        public bool enableMaterialInstancing = true;
        
        [Header("Base Properties")]
        [Tooltip("Main color tint")]
        public Color baseColor = Color.white;
        
        [Range(0f, 3f)]
        [Tooltip("Color saturation multiplier")]
        public float saturation = 1f;
        
        [Range(0f, 3f)]
        [Tooltip("Brightness multiplier")]
        public float brightness = 1f;
        
        [Range(0f, 1f)]
        [Tooltip("Metallic value")]
        public float metallic = 0f;
        
        [Range(0f, 1f)]
        [Tooltip("Smoothness value")]
        public float smoothness = 0.5f;
        
        [Header("Textures")]
        [Tooltip("Main albedo texture")]
        public Texture2D mainTexture;
        
        [Tooltip("Normal map texture")]
        public Texture2D normalMap;
        
        [Range(0f, 3f)]
        [Tooltip("Normal map intensity")]
        public float normalScale = 1f;
        
        [Tooltip("Emission texture")]
        public Texture2D emissionMap;
        
        [Tooltip("Emission color")]
        [ColorUsage(true, true)]
        public Color emissionColor = Color.black;
        
        [Range(0f, 20f)]
        [Tooltip("Emission intensity")]
        public float emissionIntensity = 1f;
        
        [Header("Shader Settings")]
        public GorgonizeToonLightingSettings lightingSettings = new GorgonizeToonLightingSettings();
        public GorgonizeToonVisualEffects visualEffects = new GorgonizeToonVisualEffects();
        public GorgonizeToonStylization stylization = new GorgonizeToonStylization();
        public GorgonizeToonAnimationSettings animationSettings = new GorgonizeToonAnimationSettings();
        
        [Header("Quality & Performance")]
        [Range(0, 3)]
        [Tooltip("Quality level: 0=Low, 1=Medium, 2=High, 3=Ultra")]
        public int qualityLevel = 2;
        
        [Tooltip("Enable automatic performance monitoring")]
        public bool enablePerformanceMonitoring = true;
        
        [Tooltip("Enable LOD-based quality scaling")]
        public bool enableLODScaling = true;
        
        [Range(1f, 100f)]
        [Tooltip("LOD fade distance")]
        public float lodFadeDistance = 20f;
        
        [Header("Advanced Features")]
        [Tooltip("Enable GPU instancing support")]
        public bool enableGPUInstancing = true;
        
        [Tooltip("Enable temporal effects")]
        public bool enableTemporalEffects = false;
        
        [Tooltip("Enable procedural animations")]
        public bool enableProceduralAnimations = false;
        
        [Header("Debug Options")]
        [Tooltip("Enable debug mode")]
        public bool debugMode = false;
        
        [Tooltip("Debug visualization mode")]
        public DebugVisualizationMode debugVisualization = DebugVisualizationMode.None;
        
        [Tooltip("Show performance statistics")]
        public bool showPerformanceStats = false;

        // Private fields
        private Renderer objectRenderer;
        private Material materialInstance;
        private float animationTime;
        private Coroutine qualityTransitionCoroutine;
        private List<MaterialPropertyBlock> propertyBlocks = new List<MaterialPropertyBlock>();
        
        // Performance tracking
        private float lastPerformanceCheck;
        private PerformanceStats performanceStats = new PerformanceStats();
        private const float PERFORMANCE_CHECK_INTERVAL = 1f;
        
        // Animation base values for restoration
        private float baseRimIntensity;
        private float baseEmissionIntensity;
        private float baseHatchingRotation;
        private float baseHue;
        
        // LOD system
        private LODGroup lodGroup;
        private float currentLODLevel = 0f;
        
        // Event system
        public System.Action<int> OnQualityLevelChanged;
        public System.Action<PerformanceStats> OnPerformanceStatsUpdated;
        public System.Action<Material> OnMaterialChanged;

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            InitializeMaterial();
            CacheBaseValues();
            UpdateAllProperties();
            ApplyQualitySettings();
            SetupLODSystem();
            
            if (enablePerformanceMonitoring)
                InvokeRepeating(nameof(MonitorPerformance), PERFORMANCE_CHECK_INTERVAL, PERFORMANCE_CHECK_INTERVAL);
        }

        private void Update()
        {
            if (animationSettings.HasActiveAnimations() || enableProceduralAnimations)
            {
                animationTime += Time.deltaTime;
                HandleAnimations();
            }
            
            if (enableLODScaling)
            {
                UpdateLODSystem();
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

        private void OnDrawGizmosSelected()
        {
            if (showPerformanceStats && Application.isPlaying)
            {
                DrawPerformanceGizmos();
            }
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            objectRenderer = GetComponent<Renderer>();
            lodGroup = GetComponentInParent<LODGroup>();
            
            // Initialize settings if null
            if (lightingSettings == null) lightingSettings = new GorgonizeToonLightingSettings();
            if (visualEffects == null) visualEffects = new GorgonizeToonVisualEffects();
            if (stylization == null) stylization = new GorgonizeToonStylization();
            if (animationSettings == null) animationSettings = new GorgonizeToonAnimationSettings();
        }

        private void InitializeMaterial()
        {
            if (enableMaterialInstancing)
            {
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
            else
            {
                materialInstance = objectRenderer.sharedMaterial;
            }
            
            OnMaterialChanged?.Invoke(materialInstance);
        }

        private void CacheBaseValues()
        {
            baseRimIntensity = visualEffects.rimLighting.rimIntensity;
            baseEmissionIntensity = emissionIntensity;
            baseHatchingRotation = stylization.hatching.hatchingRotation;
            baseHue = stylization.colorGrading.hueShift;
        }

        private void SetupLODSystem()
        {
            if (lodGroup != null && enableLODScaling)
            {
                // Setup automatic quality scaling based on LOD
                // This can be customized based on your LOD setup
            }
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
            UpdateAdvancedFeatures();
            
            lightingSettings.ApplyToMaterial(materialInstance);
            visualEffects.ApplyToMaterial(materialInstance);
            stylization.ApplyToMaterial(materialInstance);
            
            UpdateQualityKeywords();
            UpdateDebugSettings();
        }

        private void UpdateBaseProperties()
        {
            GorgonizeToonShaderProperties.SetColorSafe(materialInstance, GorgonizeToonShaderProperties.BaseColor, baseColor);
            GorgonizeToonShaderProperties.SetFloatSafe(materialInstance, GorgonizeToonShaderProperties.Saturation, saturation);
            GorgonizeToonShaderProperties.SetFloatSafe(materialInstance, GorgonizeToonShaderProperties.Brightness, brightness);
            
            // Set metallic and smoothness if properties exist
            if (materialInstance.HasProperty("_Metallic"))
                materialInstance.SetFloat("_Metallic", metallic);
            if (materialInstance.HasProperty("_Smoothness"))
                materialInstance.SetFloat("_Smoothness", smoothness);
        }

        private void UpdateTextures()
        {
            if (mainTexture != null)
                GorgonizeToonShaderProperties.SetTextureSafe(materialInstance, GorgonizeToonShaderProperties.BaseMap, mainTexture);

            // Normal mapping
            bool hasNormalMap = normalMap != null;
            GorgonizeToonShaderProperties.SetFloatSafe(materialInstance, GorgonizeToonShaderProperties.EnableNormalMap, hasNormalMap ? 1f : 0f);
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, GorgonizeToonShaderProperties.Keywords.NormalMap, hasNormalMap);
            
            if (hasNormalMap)
            {
                GorgonizeToonShaderProperties.SetTextureSafe(materialInstance, GorgonizeToonShaderProperties.BumpMap, normalMap);
                GorgonizeToonShaderProperties.SetFloatSafe(materialInstance, GorgonizeToonShaderProperties.BumpScale, normalScale);
            }

            // Emission
            bool hasEmission = emissionMap != null || emissionColor != Color.black;
            GorgonizeToonShaderProperties.SetFloatSafe(materialInstance, GorgonizeToonShaderProperties.EnableEmission, hasEmission ? 1f : 0f);
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, GorgonizeToonShaderProperties.Keywords.Emission, hasEmission);
            
            if (hasEmission)
            {
                if (emissionMap != null)
                    GorgonizeToonShaderProperties.SetTextureSafe(materialInstance, GorgonizeToonShaderProperties.EmissionMap, emissionMap);
                
                GorgonizeToonShaderProperties.SetColorSafe(materialInstance, GorgonizeToonShaderProperties.EmissionColor, emissionColor);
                GorgonizeToonShaderProperties.SetFloatSafe(materialInstance, GorgonizeToonShaderProperties.EmissionIntensity, emissionIntensity);
            }
        }

        private void UpdateAdvancedFeatures()
        {
            // Quality level
            if (materialInstance.HasProperty("_QualityLevel"))
                materialInstance.SetFloat("_QualityLevel", qualityLevel);
            
            // LOD fade distance
            if (materialInstance.HasProperty("_LODFadeDistance"))
                materialInstance.SetFloat("_LODFadeDistance", lodFadeDistance);
            
            // Animation speed
            if (materialInstance.HasProperty("_AnimationSpeed"))
                materialInstance.SetFloat("_AnimationSpeed", animationSettings.globalAnimationSpeed);
            
            // GPU Instancing
            if (materialInstance.HasProperty("_EnableInstancing"))
                materialInstance.SetFloat("_EnableInstancing", enableGPUInstancing ? 1f : 0f);
            
            // Temporal effects
            if (materialInstance.HasProperty("_EnableTemporalEffects"))
                materialInstance.SetFloat("_EnableTemporalEffects", enableTemporalEffects ? 1f : 0f);
        }

        private void UpdateQualityKeywords()
        {
            // Enable/disable features based on quality level
            switch (qualityLevel)
            {
                case 0: // Low
                    DisableExpensiveFeatures();
                    break;
                case 1: // Medium
                    EnableBasicFeatures();
                    break;
                case 2: // High
                    EnableAdvancedFeatures();
                    break;
                case 3: // Ultra
                    EnableAllFeatures();
                    break;
            }
        }

        private void UpdateDebugSettings()
        {
            if (materialInstance.HasProperty("_DebugMode"))
                materialInstance.SetFloat("_DebugMode", debugMode ? 1f : 0f);
            if (materialInstance.HasProperty("_DebugView"))
                materialInstance.SetFloat("_DebugView", (float)debugVisualization);
        }
        #endregion

        #region Animation System
        private void HandleAnimations()
        {
            if (animationSettings.HasActiveAnimations())
            {
                animationSettings.UpdateAnimations(materialInstance, Time.deltaTime, animationTime,
                    baseRimIntensity, baseEmissionIntensity, baseHatchingRotation);
            }
            
            if (enableProceduralAnimations)
            {
                HandleProceduralAnimations();
            }
        }

        private void HandleProceduralAnimations()
        {
            // Example procedural animations
            if (visualEffects.rimLighting.enableRimLighting)
            {
                float proceduralRim = baseRimIntensity * (1f + Mathf.Sin(animationTime * 2f) * 0.1f);
                GorgonizeToonShaderProperties.SetFloatSafe(materialInstance, GorgonizeToonShaderProperties.RimIntensity, proceduralRim);
            }
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
            OnQualityLevelChanged?.Invoke(qualityLevel);
        }

        private void ApplyLowQuality()
        {
            // Disable expensive features for mobile/low-end
            visualEffects.fresnel.enableFresnel = false;
            visualEffects.subsurface.enableSubsurface = false;
            visualEffects.matcap.enableMatcap = false;
            stylization.quantization.enablePosterize = false;
            stylization.hatching.enableScreenSpaceHatching = false;
            
            // Reduce quality settings
            lightingSettings.shadowSmoothness = Mathf.Min(lightingSettings.shadowSmoothness, 0.05f);
            visualEffects.specular.specularSmoothness = Mathf.Min(visualEffects.specular.specularSmoothness, 0.02f);
        }

        private void ApplyMediumQuality()
        {
            // Enable basic features
            visualEffects.rimLighting.enableRimLighting = true;
            visualEffects.specular.enableSpecular = true;
            stylization.hatching.enableHatching = true;
            stylization.hatching.enableScreenSpaceHatching = false;
            
            lightingSettings.shadowSmoothness = Mathf.Min(lightingSettings.shadowSmoothness, 0.1f);
        }

        private void ApplyHighQuality()
        {
            // Enable most features
            visualEffects.rimLighting.enableRimLighting = true;
            visualEffects.specular.enableSpecular = true;
            stylization.hatching.enableHatching = true;
            stylization.hatching.enableScreenSpaceHatching = true;
            visualEffects.fresnel.enableFresnel = true;
            visualEffects.matcap.enableMatcap = true;
        }

        private void ApplyUltraQuality()
        {
            // Enable all features
            EnableAllFeatures();
        }

        private void DisableExpensiveFeatures()
        {
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_FRESNEL", false);
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_SUBSURFACE", false);
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_MATCAP", false);
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_SCREEN_SPACE_HATCHING", false);
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_FORCE_FIELD", false);
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_HOLOGRAM", false);
        }

        private void EnableBasicFeatures()
        {
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_RIM_LIGHTING", visualEffects.rimLighting.enableRimLighting);
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_SPECULAR", visualEffects.specular.enableSpecular);
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_HATCHING", stylization.hatching.enableHatching);
        }

        private void EnableAdvancedFeatures()
        {
            EnableBasicFeatures();
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_FRESNEL", visualEffects.fresnel.enableFresnel);
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_MATCAP", visualEffects.matcap.enableMatcap);
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_SCREEN_SPACE_HATCHING", stylization.hatching.enableScreenSpaceHatching);
        }

        private void EnableAllFeatures()
        {
            EnableAdvancedFeatures();
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_SUBSURFACE", visualEffects.subsurface.enableSubsurface);
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_FORCE_FIELD", visualEffects.specialEffects.enableForceField);
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_HOLOGRAM", visualEffects.specialEffects.enableHologram);
            GorgonizeToonShaderProperties.SetKeywordSafe(materialInstance, "_DISSOLVE", visualEffects.specialEffects.enableDissolve);
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

        #region LOD System
        private void UpdateLODSystem()
        {
            if (lodGroup != null)
            {
                // Get current LOD level from LOD group
                float relativeHeight = lodGroup.GetWorldSpaceSize() / QualitySettings.lodBias;
                currentLODLevel = Mathf.Clamp01(relativeHeight);
                
                // Adjust quality based on LOD
                AdjustQualityBasedOnLOD();
            }
            else
            {
                // Fallback: distance-based LOD
                float distance = Vector3.Distance(transform.position, Camera.main ? Camera.main.transform.position : Vector3.zero);
                currentLODLevel = Mathf.Clamp01(1f - (distance / lodFadeDistance));
                AdjustQualityBasedOnDistance(distance);
            }
        }

        private void AdjustQualityBasedOnLOD()
        {
            // Automatically adjust features based on LOD level
            if (currentLODLevel < 0.25f) // Very far
            {
                if (qualityLevel > 0) SetQualityLevel(0);
            }
            else if (currentLODLevel < 0.5f) // Far
            {
                if (qualityLevel > 1) SetQualityLevel(1);
            }
            else if (currentLODLevel < 0.75f) // Medium
            {
                if (qualityLevel > 2) SetQualityLevel(2);
            }
            // Close objects can use full quality
        }

        private void AdjustQualityBasedOnDistance(float distance)
        {
            // Set LOD fade in shader
            float lodFade = Mathf.Clamp01(1f - (distance / lodFadeDistance));
            if (materialInstance.HasProperty("_LODFadeDistance"))
                materialInstance.SetFloat("_LODFadeDistance", lodFadeDistance);
        }
        #endregion

        #region Performance Monitoring
        private void MonitorPerformance()
        {
            if (Time.time - lastPerformanceCheck < PERFORMANCE_CHECK_INTERVAL)
                return;

            lastPerformanceCheck = Time.time;
            
            UpdatePerformanceStats();
            
            // Auto-adjust quality if performance is poor
            if (performanceStats.totalCost > 0.8f && qualityLevel > 0)
            {
                Debug.LogWarning($"High performance cost detected ({performanceStats.totalCost:F2}). Consider lowering quality level on {name}.");
                
                if (enablePerformanceMonitoring)
                {
                    SetQualityLevel(qualityLevel - 1);
                }
            }
            
            OnPerformanceStatsUpdated?.Invoke(performanceStats);
        }

        private void UpdatePerformanceStats()
        {
            performanceStats.lightingCost = lightingSettings.GetPerformanceCost();
            performanceStats.visualEffectsCost = visualEffects.GetPerformanceCost();
            performanceStats.stylizationCost = stylization.GetPerformanceCost();
            performanceStats.animationCost = animationSettings.GetPerformanceCost();
            performanceStats.totalCost = performanceStats.lightingCost + performanceStats.visualEffectsCost + 
                                        performanceStats.stylizationCost + performanceStats.animationCost;
            
            performanceStats.activeEffectCount = visualEffects.GetActiveEffectCount() + stylization.GetActiveEffectCount();
            performanceStats.activeAnimationCount = animationSettings.GetActiveAnimationCount();
            performanceStats.qualityLevel = qualityLevel;
            performanceStats.lodLevel = currentLODLevel;
        }

        /// <summary>
        /// Returns performance information as formatted string
        /// </summary>
        public string GetPerformanceInfo()
        {
            return $"Effects: {performanceStats.activeEffectCount}, Animations: {performanceStats.activeAnimationCount}, " +
                   $"Quality: {qualityLevel}, LOD: {currentLODLevel:F2}, Cost: {performanceStats.totalCost:F2}";
        }

        private void DrawPerformanceGizmos()
        {
            if (performanceStats.totalCost > 0.8f)
            {
                Gizmos.color = Color.red;
            }
            else if (performanceStats.totalCost > 0.6f)
            {
                Gizmos.color = Color.yellow;
            }
            else
            {
                Gizmos.color = Color.green;
            }
            
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
        #endregion

        #region Validation
        private void ValidateAllSettings()
        {
            lightingSettings.ValidateSettings();
            visualEffects.ValidateSettings();
            stylization.ValidateSettings();
            animationSettings.ValidateSettings();
            
            qualityLevel = Mathf.Clamp(qualityLevel, 0, 3);
            normalScale = Mathf.Clamp(normalScale, 0f, 3f);
            emissionIntensity = Mathf.Clamp(emissionIntensity, 0f, 20f);
            saturation = Mathf.Clamp(saturation, 0f, 3f);
            brightness = Mathf.Clamp(brightness, 0f, 3f);
            metallic = Mathf.Clamp01(metallic);
            smoothness = Mathf.Clamp01(smoothness);
            lodFadeDistance = Mathf.Clamp(lodFadeDistance, 1f, 100f);
        }
        #endregion

        #region Public API
        /// <summary>
        /// Sets base color
        /// </summary>
        public void SetBaseColor(Color color)
        {
            baseColor = color;
            GorgonizeToonShaderProperties.SetColorSafe(materialInstance, GorgonizeToonShaderProperties.BaseColor, baseColor);
        }

        /// <summary>
        /// Sets shadow threshold
        /// </summary>
        public void SetShadowThreshold(float threshold)
        {
            lightingSettings.shadowThreshold = Mathf.Clamp01(threshold);
            GorgonizeToonShaderProperties.SetFloatSafe(materialInstance, GorgonizeToonShaderProperties.ShadowThreshold, lightingSettings.shadowThreshold);
        }

        /// <summary>
        /// Sets rim lighting intensity
        /// </summary>
        public void SetRimIntensity(float intensity)
        {
            visualEffects.rimLighting.rimIntensity = Mathf.Clamp(intensity, 0f, 5f);
            baseRimIntensity = visualEffects.rimLighting.rimIntensity;
            
            if (!animationSettings.animateRimLighting)
            {
                GorgonizeToonShaderProperties.SetFloatSafe(materialInstance, GorgonizeToonShaderProperties.RimIntensity, visualEffects.rimLighting.rimIntensity);
            }
        }

        /// <summary>
        /// Sets emission intensity
        /// </summary>
        public void SetEmissionIntensity(float intensity)
        {
            emissionIntensity = Mathf.Clamp(intensity, 0f, 20f);
            baseEmissionIntensity = emissionIntensity;
            
            if (!animationSettings.animateEmission)
            {
                GorgonizeToonShaderProperties.SetFloatSafe(materialInstance, GorgonizeToonShaderProperties.EmissionIntensity, emissionIntensity);
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

        /// <summary>
        /// Applies a preset to this material controller
        /// </summary>
        public void ApplyPreset(GorgonizeToonShaderPreset preset)
        {
            if (preset == null) return;
            
            preset.ApplyToController(this);
            UpdateAllProperties();
        }

        /// <summary>
        /// Creates a preset from current settings
        /// </summary>
        public GorgonizeToonShaderPreset CreatePresetFromCurrent(string presetName = "Custom Preset")
        {
            return GorgonizeToonPresetManager.CreatePresetFromController(this, presetName);
        }

        /// <summary>
        /// Gets the current material instance
        /// </summary>
        public Material GetMaterialInstance()
        {
            return materialInstance;
        }

        /// <summary>
        /// Forces a complete material update
        /// </summary>
        public void ForceUpdate()
        {
            UpdateAllProperties();
        }

        /// <summary>
        /// Enables or disables a specific effect
        /// </summary>
        public void SetEffectEnabled(string effectName, bool enabled)
        {
            switch (effectName.ToLower())
            {
                case "rimlighting":
                    visualEffects.rimLighting.enableRimLighting = enabled;
                    break;
                case "specular":
                    visualEffects.specular.enableSpecular = enabled;
                    break;
                case "hatching":
                    stylization.hatching.enableHatching = enabled;
                    break;
                case "matcap":
                    visualEffects.matcap.enableMatcap = enabled;
                    break;
                case "fresnel":
                    visualEffects.fresnel.enableFresnel = enabled;
                    break;
                case "subsurface":
                    visualEffects.subsurface.enableSubsurface = enabled;
                    break;
                case "outline":
                    visualEffects.outline.enableOutline = enabled;
                    break;
                default:
                    Debug.LogWarning($"Unknown effect: {effectName}");
                    return;
            }
            
            UpdateAllProperties();
        }

        /// <summary>
        /// Gets the current performance statistics
        /// </summary>
        public PerformanceStats GetPerformanceStats()
        {
            return performanceStats;
        }
        #endregion

        #region Cleanup
        private void CleanupResources()
        {
            if (qualityTransitionCoroutine != null)
                StopCoroutine(qualityTransitionCoroutine);

            if (enablePerformanceMonitoring)
                CancelInvoke(nameof(MonitorPerformance));
                
            if (materialInstance != null && enableMaterialInstancing)
                DestroyImmediate(materialInstance);
        }
        #endregion
    }

    #region Supporting Classes
    [System.Serializable]
    public class PerformanceStats
    {
        public float lightingCost;
        public float visualEffectsCost;
        public float stylizationCost;
        public float animationCost;
        public float totalCost;
        public int activeEffectCount;
        public int activeAnimationCount;
        public int qualityLevel;
        public float lodLevel;
        public float frameTime;
        public long memoryUsage;
    }

    public enum DebugVisualizationMode
    {
        None = 0,
        Normals = 1,
        Lighting = 2,
        Shadows = 3,
        Hatching = 4,
        Performance = 5
    }
    #endregion
}