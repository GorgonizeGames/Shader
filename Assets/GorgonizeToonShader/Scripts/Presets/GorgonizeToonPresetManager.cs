using UnityEngine;
using Gorgonize.ToonShader.Settings;

namespace Gorgonize.ToonShader.Presets
{
    /// <summary>
    /// Preset data structure for storing complete shader configurations
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Toon Shader Preset", menuName = "Gorgonize/Toon Shader Preset")]
    public class GorgonizeToonShaderPreset : ScriptableObject
    {
        [Header("Preset Info")]
        public string presetName = "Custom Preset";
        public string description = "Custom shader configuration";
        public Texture2D previewImage;
        
        [Header("Settings")]
        public Color baseColor = Color.white;
        public GorgonizeToonLightingSettings lightingSettings = new GorgonizeToonLightingSettings();
        public GorgonizeToonVisualEffects visualEffects = new GorgonizeToonVisualEffects();
        public GorgonizeToonStylization stylization = new GorgonizeToonStylization();
        public GorgonizeToonAnimationSettings animationSettings = new GorgonizeToonAnimationSettings();
        
        /// <summary>
        /// Applies this preset to a ToonMaterialController
        /// </summary>
        public void ApplyToController(GorgonizeToonMaterialController controller)
        {
            if (controller == null) return;
            
            controller.baseColor = baseColor;
            controller.lightingSettings.CopyFrom(lightingSettings);
            controller.visualEffects.CopyFrom(visualEffects);
            controller.stylization.CopyFrom(stylization);
            controller.animationSettings.CopyFrom(animationSettings);
            
            controller.UpdateAllProperties();
        }
    }

    /// <summary>
    /// Manages shader presets and provides built-in configurations
    /// </summary>
    public static class GorgonizeToonPresetManager
    {
        #region Built-in Presets
        
        /// <summary>
        /// Creates an anime-style preset
        /// </summary>
        public static GorgonizeToonShaderPreset CreateAnimePreset()
        {
            var preset = ScriptableObject.CreateInstance<GorgonizeToonShaderPreset>();
            preset.presetName = "Anime Classic";
            preset.description = "Traditional anime/manga styling with smooth lighting and rim effects";
            preset.baseColor = Color.white;
            
            // Lighting configuration
            preset.lightingSettings = GorgonizeToonLightingSettings.CreateAnimePreset();
            
            // Visual effects
            preset.visualEffects = GorgonizeToonVisualEffects.CreateAnimePreset();
            
            // Stylization
            preset.stylization = new GorgonizeToonStylization();
            preset.stylization.hatching.enableHatching = false;
            preset.stylization.colorGrading.saturation = 1.1f;
            preset.stylization.colorGrading.brightness = 1.05f;
            
            // Animation
            preset.animationSettings = GorgonizeToonAnimationSettings.CreateGentlePreset();
            
            return preset;
        }
        
        /// <summary>
        /// Creates a cartoon-style preset
        /// </summary>
        public static GorgonizeToonShaderPreset CreateCartoonPreset()
        {
            var preset = ScriptableObject.CreateInstance<GorgonizeToonShaderPreset>();
            preset.presetName = "Cartoon Bold";
            preset.description = "Bold cartoon styling with hard shadows and strong contrasts";
            preset.baseColor = Color.white;
            
            // Lighting
            preset.lightingSettings = GorgonizeToonLightingSettings.CreateCartoonPreset();
            
            // Visual effects
            preset.visualEffects = GorgonizeToonVisualEffects.CreateCartoonPreset();
            
            // Stylization
            preset.stylization = GorgonizeToonStylization.CreateComicPreset();
            
            // Animation
            preset.animationSettings = GorgonizeToonAnimationSettings.CreateStaticPreset();
            
            return preset;
        }
        
        /// <summary>
        /// Creates a sketch-style preset
        /// </summary>
        public static GorgonizeToonShaderPreset CreateSketchPreset()
        {
            var preset = ScriptableObject.CreateInstance<GorgonizeToonShaderPreset>();
            preset.presetName = "Sketch Style";
            preset.description = "Hand-drawn sketch appearance with hatching effects";
            preset.baseColor = Color.white;
            
            // Lighting
            preset.lightingSettings = new GorgonizeToonLightingSettings();
            preset.lightingSettings.shadowThreshold = 0.4f;
            preset.lightingSettings.shadowSmoothness = 0.05f;
            preset.lightingSettings.shadowColor = new Color(0.95f, 0.95f, 0.95f, 1f);
            
            // Visual effects
            preset.visualEffects = new GorgonizeToonVisualEffects();
            preset.visualEffects.rimLighting.enableRimLighting = true;
            preset.visualEffects.rimLighting.rimIntensity = 1f;
            preset.visualEffects.rimLighting.rimColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            
            // Stylization - Heavy hatching
            preset.stylization = GorgonizeToonStylization.CreateSketchPreset();
            
            // Animation
            preset.animationSettings = GorgonizeToonAnimationSettings.CreateStaticPreset();
            
            return preset;
        }
        
        /// <summary>
        /// Creates a comic book style preset
        /// </summary>
        public static GorgonizeToonShaderPreset CreateComicPreset()
        {
            var preset = ScriptableObject.CreateInstance<GorgonizeToonShaderPreset>();
            preset.presetName = "Comic Book";
            preset.description = "Bold comic book styling with strong outlines and vibrant colors";
            preset.baseColor = Color.white;
            
            // Lighting
            preset.lightingSettings = new GorgonizeToonLightingSettings();
            preset.lightingSettings.shadowThreshold = 0.5f;
            preset.lightingSettings.shadowSmoothness = 0.02f;
            preset.lightingSettings.shadowColor = new Color(0.3f, 0.3f, 0.5f, 1f);
            
            // Visual effects
            preset.visualEffects = GorgonizeToonVisualEffects.CreateCartoonPreset();
            
            // Stylization
            preset.stylization = GorgonizeToonStylization.CreateComicPreset();
            
            // Animation
            preset.animationSettings = GorgonizeToonAnimationSettings.CreateStaticPreset();
            
            return preset;
        }
        
        /// <summary>
        /// Creates a hatched drawing style preset
        /// </summary>
        public static GorgonizeToonShaderPreset CreateHatchedDrawingPreset()
        {
            var preset = ScriptableObject.CreateInstance<GorgonizeToonShaderPreset>();
            preset.presetName = "Hatched Drawing";
            preset.description = "Technical illustration style with extensive hatching effects";
            preset.baseColor = Color.white;
            
            // Lighting
            preset.lightingSettings = new GorgonizeToonLightingSettings();
            preset.lightingSettings.shadowThreshold = 0.45f;
            preset.lightingSettings.shadowSmoothness = 0.1f;
            preset.lightingSettings.shadowColor = new Color(0.95f, 0.95f, 0.95f, 1f);
            
            // Stylization - Advanced hatching
            preset.stylization = GorgonizeToonStylization.CreateSketchPreset();
            preset.stylization.hatching.enableScreenSpaceHatching = true;
            preset.stylization.hatching.screenHatchScale = 3f;
            preset.stylization.hatching.screenHatchBias = 0.2f;
            
            // Visual effects
            preset.visualEffects = new GorgonizeToonVisualEffects();
            
            // Animation
            preset.animationSettings = GorgonizeToonAnimationSettings.CreateStaticPreset();
            
            return preset;
        }
        
        /// <summary>
        /// Creates a realistic toon preset
        /// </summary>
        public static GorgonizeToonShaderPreset CreateRealisticToonPreset()
        {
            var preset = ScriptableObject.CreateInstance<GorgonizeToonShaderPreset>();
            preset.presetName = "Realistic Toon";
            preset.description = "Balanced approach between realism and stylization";
            preset.baseColor = Color.white;
            
            // Lighting
            preset.lightingSettings = GorgonizeToonLightingSettings.CreateRealisticPreset();
            
            // Visual effects
            preset.visualEffects = GorgonizeToonVisualEffects.CreateRealisticPreset();
            
            // Stylization
            preset.stylization = new GorgonizeToonStylization();
            
            // Animation
            preset.animationSettings = GorgonizeToonAnimationSettings.CreateGentlePreset();
            
            return preset;
        }
        
        /// <summary>
        /// Creates a painterly style preset
        /// </summary>
        public static GorgonizeToonShaderPreset CreatePainterlyPreset()
        {
            var preset = ScriptableObject.CreateInstance<GorgonizeToonShaderPreset>();
            preset.presetName = "Painterly";
            preset.description = "Artistic painted appearance with soft lighting";
            preset.baseColor = Color.white;
            
            // Lighting
            preset.lightingSettings = new GorgonizeToonLightingSettings();
            preset.lightingSettings.shadowThreshold = 0.35f;
            preset.lightingSettings.shadowSmoothness = 0.3f;
            preset.lightingSettings.shadowColor = new Color(0.6f, 0.7f, 0.8f, 1f);
            preset.lightingSettings.indirectLightingBoost = 0.4f;
            
            // Visual effects
            preset.visualEffects = new GorgonizeToonVisualEffects();
            preset.visualEffects.matcap.enableMatcap = true;
            preset.visualEffects.matcap.matcapIntensity = 0.8f;
            preset.visualEffects.matcap.blendMode = MatcapBlendMode.Multiply;
            
            preset.visualEffects.fresnel.enableFresnel = true;
            preset.visualEffects.fresnel.fresnelIntensity = 1.5f;
            preset.visualEffects.fresnel.fresnelPower = 2f;
            
            // Stylization
            preset.stylization = GorgonizeToonStylization.CreatePainterlyPreset();
            
            // Animation
            preset.animationSettings = GorgonizeToonAnimationSettings.CreateGentlePreset();
            
            return preset;
        }
        #endregion
        
        #region Preset Operations
        
        /// <summary>
        /// Gets all built-in presets
        /// </summary>
        public static GorgonizeToonShaderPreset[] GetBuiltInPresets()
        {
            return new GorgonizeToonShaderPreset[]
            {
                CreateAnimePreset(),
                CreateCartoonPreset(),
                CreateSketchPreset(),
                CreateComicPreset(),
                CreateHatchedDrawingPreset(),
                CreateRealisticToonPreset(),
                CreatePainterlyPreset()
            };
        }
        
        /// <summary>
        /// Gets preset names for UI display
        /// </summary>
        public static string[] GetPresetNames()
        {
            var presets = GetBuiltInPresets();
            string[] names = new string[presets.Length];
            
            for (int i = 0; i < presets.Length; i++)
            {
                names[i] = presets[i].presetName;
            }
            
            return names;
        }
        
        /// <summary>
        /// Gets a preset by name
        /// </summary>
        public static GorgonizeToonShaderPreset GetPresetByName(string name)
        {
            var presets = GetBuiltInPresets();
            
            foreach (var preset in presets)
            {
                if (preset.presetName.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                {
                    return preset;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Gets a preset by index
        /// </summary>
        public static GorgonizeToonShaderPreset GetPresetByIndex(int index)
        {
            var presets = GetBuiltInPresets();
            
            if (index >= 0 && index < presets.Length)
            {
                return presets[index];
            }
            
            return null;
        }
        
        /// <summary>
        /// Creates a preset from current controller settings
        /// </summary>
        public static GorgonizeToonShaderPreset CreatePresetFromController(GorgonizeToonMaterialController controller, string presetName = "Custom Preset")
        {
            if (controller == null) return null;
            
            var preset = ScriptableObject.CreateInstance<GorgonizeToonShaderPreset>();
            preset.presetName = presetName;
            preset.description = "Custom configuration created from current settings";
            preset.baseColor = controller.baseColor;
            
            // Copy settings
            preset.lightingSettings.CopyFrom(controller.lightingSettings);
            preset.visualEffects.CopyFrom(controller.visualEffects);
            preset.stylization.CopyFrom(controller.stylization);
            preset.animationSettings.CopyFrom(controller.animationSettings);
            
            return preset;
        }
        
        /// <summary>
        /// Applies a preset to a material controller
        /// </summary>
        public static void ApplyPreset(GorgonizeToonMaterialController controller, GorgonizeToonShaderPreset preset)
        {
            if (controller == null || preset == null) return;
            
            preset.ApplyToController(controller);
        }
        
        /// <summary>
        /// Applies a preset by name
        /// </summary>
        public static void ApplyPresetByName(GorgonizeToonMaterialController controller, string presetName)
        {
            var preset = GetPresetByName(presetName);
            if (preset != null)
            {
                ApplyPreset(controller, preset);
            }
        }
        
        /// <summary>
        /// Applies a preset by index
        /// </summary>
        public static void ApplyPresetByIndex(GorgonizeToonMaterialController controller, int index)
        {
            var preset = GetPresetByIndex(index);
            if (preset != null)
            {
                ApplyPreset(controller, preset);
            }
        }
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// Validates a preset for completeness
        /// </summary>
        public static bool ValidatePreset(GorgonizeToonShaderPreset preset)
        {
            if (preset == null) return false;
            if (string.IsNullOrEmpty(preset.presetName)) return false;
            if (preset.lightingSettings == null) return false;
            if (preset.visualEffects == null) return false;
            if (preset.stylization == null) return false;
            if (preset.animationSettings == null) return false;
            
            return true;
        }
        
        /// <summary>
        /// Gets performance estimation for a preset
        /// </summary>
        public static float EstimatePresetPerformance(GorgonizeToonShaderPreset preset)
        {
            if (preset == null) return 0f;
            
            float cost = 0f;
            
            cost += preset.lightingSettings.GetPerformanceCost();
            cost += preset.visualEffects.GetPerformanceCost();
            cost += preset.stylization.GetPerformanceCost();
            cost += preset.animationSettings.GetPerformanceCost();
            
            return Mathf.Clamp01(cost);
        }
        
        /// <summary>
        /// Gets a performance category for a preset
        /// </summary>
        public static string GetPerformanceCategory(GorgonizeToonShaderPreset preset)
        {
            float cost = EstimatePresetPerformance(preset);
            
            if (cost < 0.3f) return "Light";
            if (cost < 0.6f) return "Medium";
            if (cost < 0.8f) return "Heavy";
            return "Ultra";
        }
        
        /// <summary>
        /// Creates a preset optimized for mobile platforms
        /// </summary>
        public static GorgonizeToonShaderPreset CreateMobileOptimizedPreset()
        {
            var preset = ScriptableObject.CreateInstance<GorgonizeToonShaderPreset>();
            preset.presetName = "Mobile Optimized";
            preset.description = "Lightweight preset optimized for mobile devices";
            preset.baseColor = Color.white;
            
            // Basic lighting only
            preset.lightingSettings = new GorgonizeToonLightingSettings();
            preset.lightingSettings.shadowThreshold = 0.5f;
            preset.lightingSettings.shadowSmoothness = 0.02f; // Hard shadows for performance
            preset.lightingSettings.useRampTexture = false;
            preset.lightingSettings.indirectLightingBoost = 0.2f;
            
            // Minimal visual effects
            preset.visualEffects = GorgonizeToonVisualEffects.CreateMinimalPreset();
            preset.visualEffects.rimLighting.enableRimLighting = true;
            preset.visualEffects.rimLighting.rimIntensity = 1f;
            
            // No expensive stylization
            preset.stylization = new GorgonizeToonStylization();
            preset.stylization.hatching.enableHatching = false;
            preset.stylization.hatching.enableScreenSpaceHatching = false;
            preset.stylization.quantization.enablePosterize = false;
            
            // No animations for performance
            preset.animationSettings = GorgonizeToonAnimationSettings.CreateStaticPreset();
            
            return preset;
        }
        
        /// <summary>
        /// Creates a high-quality preset for desktop/console
        /// </summary>
        public static GorgonizeToonShaderPreset CreateHighQualityPreset()
        {
            var preset = ScriptableObject.CreateInstance<GorgonizeToonShaderPreset>();
            preset.presetName = "High Quality";
            preset.description = "High-end preset with all features enabled for desktop/console";
            preset.baseColor = Color.white;
            
            // Advanced lighting
            preset.lightingSettings = GorgonizeToonLightingSettings.CreateRealisticPreset();
            preset.lightingSettings.shadowSmoothness = 0.15f;
            preset.lightingSettings.indirectLightingBoost = 0.6f;
            
            // All visual effects
            preset.visualEffects = new GorgonizeToonVisualEffects();
            preset.visualEffects.rimLighting.enableRimLighting = true;
            preset.visualEffects.rimLighting.rimIntensity = 2f;
            preset.visualEffects.specular.enableSpecular = true;
            preset.visualEffects.matcap.enableMatcap = true;
            preset.visualEffects.fresnel.enableFresnel = true;
            preset.visualEffects.subsurface.enableSubsurface = true;
            preset.visualEffects.specialEffects.enableForceField = false; // Keep disabled by default
            
            // Advanced stylization
            preset.stylization = new GorgonizeToonStylization();
            preset.stylization.hatching.enableHatching = true;
            preset.stylization.hatching.enableScreenSpaceHatching = true;
            preset.stylization.quantization.enablePosterize = true;
            preset.stylization.quantization.posterizeLevels = 12f;
            
            // Dynamic animations
            preset.animationSettings = GorgonizeToonAnimationSettings.CreateDynamicPreset();
            
            return preset;
        }
        
        /// <summary>
        /// Creates a VR-optimized preset
        /// </summary>
        public static GorgonizeToonShaderPreset CreateVROptimizedPreset()
        {
            var preset = ScriptableObject.CreateInstance<GorgonizeToonShaderPreset>();
            preset.presetName = "VR Optimized";
            preset.description = "Balanced preset optimized for VR rendering";
            preset.baseColor = Color.white;
            
            // Medium quality lighting
            preset.lightingSettings = new GorgonizeToonLightingSettings();
            preset.lightingSettings.shadowThreshold = 0.45f;
            preset.lightingSettings.shadowSmoothness = 0.05f;
            preset.lightingSettings.indirectLightingBoost = 0.3f;
            
            // Basic visual effects
            preset.visualEffects = new GorgonizeToonVisualEffects();
            preset.visualEffects.rimLighting.enableRimLighting = true;
            preset.visualEffects.rimLighting.rimIntensity = 1.5f;
            preset.visualEffects.specular.enableSpecular = true;
            preset.visualEffects.specular.specularIntensity = 1f;
            preset.visualEffects.outline.enableOutline = true; // Good for VR clarity
            preset.visualEffects.outline.outlineWidth = 0.015f;
            
            // Minimal stylization
            preset.stylization = new GorgonizeToonStylization();
            preset.stylization.hatching.enableHatching = false;
            preset.stylization.quantization.enableCelShading = true;
            preset.stylization.quantization.celShadingSteps = 3f;
            
            // Minimal animations to maintain framerate
            preset.animationSettings = GorgonizeToonAnimationSettings.CreateGentlePreset();
            
            return preset;
        }
        #endregion
    }
}