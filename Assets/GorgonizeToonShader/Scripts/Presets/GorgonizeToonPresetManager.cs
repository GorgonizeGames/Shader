using UnityEngine;
using Gorgonize.ToonShader.Settings;

namespace Gorgonize.ToonShader.Presets
{
    /// <summary>
    /// Preset data structure for storing complete shader configurations
    /// </summary>
    [System.Serializable]
    public class ToonShaderPreset
    {
        [Header("Preset Info")]
        public string presetName = "Custom Preset";
        public string description = "Custom shader configuration";
        public Texture2D previewImage;
        
        [Header("Settings")]
        public Color baseColor = Color.white;
        public ToonLightingSettings lightingSettings = new ToonLightingSettings();
        public ToonVisualEffects visualEffects = new ToonVisualEffects();
        public ToonStylization stylization = new ToonStylization();
        public ToonAnimationSettings animationSettings = new ToonAnimationSettings();
        
        /// <summary>
        /// Applies this preset to a ToonMaterialController
        /// </summary>
        public void ApplyToController(ToonMaterialController controller)
        {
            if (controller == null) return;
            
            controller.baseColor = baseColor;
            controller.lightingSettings.CopyFrom(lightingSettings);
            controller.visualEffects.rimLighting.rimColor = visualEffects.rimLighting.rimColor;
            controller.visualEffects.rimLighting.rimIntensity = visualEffects.rimLighting.rimIntensity;
            controller.visualEffects.rimLighting.rimPower = visualEffects.rimLighting.rimPower;
            controller.visualEffects.rimLighting.enableRimLighting = visualEffects.rimLighting.enableRimLighting;
            // Copy other visual effects properties...
            
            controller.animationSettings.CopyFrom(animationSettings);
            controller.UpdateAllProperties();
        }
    }

    /// <summary>
    /// Manages shader presets and provides built-in configurations
    /// </summary>
    public static class ToonPresetManager
    {
        #region Built-in Presets
        
        /// <summary>
        /// Creates an anime-style preset
        /// </summary>
        public static ToonShaderPreset CreateAnimePreset()
        {
            var preset = new ToonShaderPreset
            {
                presetName = "Anime Classic",
                description = "Traditional anime/manga styling with smooth lighting and rim effects",
                baseColor = Color.white
            };
            
            // Lighting configuration
            preset.lightingSettings = ToonLightingSettings.CreateAnimePreset();
            
            // Visual effects
            preset.visualEffects.rimLighting.enableRimLighting = true;
            preset.visualEffects.rimLighting.rimColor = new Color(1f, 0.95f, 0.8f, 1f);
            preset.visualEffects.rimLighting.rimPower = 1.5f;
            preset.visualEffects.rimLighting.rimIntensity = 2f;
            preset.visualEffects.rimLighting.rimThreshold = 0.1f;
            
            preset.visualEffects.specular.enableSpecular = true;
            preset.visualEffects.specular.specularColor = Color.white;
            preset.visualEffects.specular.specularSize = 0.05f;
            preset.visualEffects.specular.specularIntensity = 2f;
            preset.visualEffects.specular.specularSmoothness = 0.02f;
            
            // Stylization
            preset.stylization.hatching.enableHatching = false;
            preset.stylization.colorGrading.saturation = 1.1f;
            preset.stylization.colorGrading.brightness = 1.05f;
            
            // Animation
            preset.animationSettings = ToonAnimationSettings.CreateGentlePreset();
            
            return preset;
        }
        
        /// <summary>
        /// Creates a cartoon-style preset
        /// </summary>
        public static ToonShaderPreset CreateCartoonPreset()
        {
            var preset = new ToonShaderPreset
            {
                presetName = "Cartoon Bold",
                description = "Bold cartoon styling with hard shadows and strong contrasts",
                baseColor = Color.white
            };
            
            // Lighting
            preset.lightingSettings = ToonLightingSettings.CreateCartoonPreset();
            
            // Visual effects
            preset.visualEffects.rimLighting.enableRimLighting = true;
            preset.visualEffects.rimLighting.rimColor = new Color(1f, 1f, 0.8f, 1f);
            preset.visualEffects.rimLighting.rimIntensity = 3f;
            preset.visualEffects.rimLighting.rimPower = 2f;
            
            preset.visualEffects.outline.enableOutline = true;
            preset.visualEffects.outline.outlineColor = Color.black;
            preset.visualEffects.outline.outlineWidth = 0.02f;
            
            // Stylization
            preset.stylization.quantization.enableCelShading = true;
            preset.stylization.quantization.celShadingSteps = 4f;
            preset.stylization.colorGrading.contrast = 1.3f;
            preset.stylization.colorGrading.saturation = 1.2f;
            
            return preset;
        }
        
        /// <summary>
        /// Creates a sketch-style preset
        /// </summary>
        public static ToonShaderPreset CreateSketchPreset()
        {
            var preset = new ToonShaderPreset
            {
                presetName = "Sketch Style",
                description = "Hand-drawn sketch appearance with hatching effects",
                baseColor = Color.white
            };
            
            // Lighting
            preset.lightingSettings.shadowThreshold = 0.4f;
            preset.lightingSettings.shadowSmoothness = 0.05f;
            preset.lightingSettings.shadowColor = new Color(0.95f, 0.95f, 0.95f, 1f);
            
            // Visual effects
            preset.visualEffects.rimLighting.enableRimLighting = true;
            preset.visualEffects.rimLighting.rimIntensity = 1f;
            preset.visualEffects.rimLighting.rimColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            
            // Stylization - Heavy hatching
            preset.stylization.hatching.enableHatching = true;
            preset.stylization.hatching.hatchingDensity = 2f;
            preset.stylization.hatching.hatchingIntensity = 0.8f;
            preset.stylization.hatching.hatchingThreshold = 0.6f;
            preset.stylization.hatching.crossHatchingThreshold = 0.3f;
            preset.stylization.hatching.hatchingRotation = 45f;
            
            preset.stylization.colorGrading.saturation = 0.8f;
            preset.stylization.colorGrading.contrast = 1.2f;
            
            return preset;
        }
        
        /// <summary>
        /// Creates a comic book style preset
        /// </summary>
        public static ToonShaderPreset CreateComicPreset()
        {
            var preset = new ToonShaderPreset
            {
                presetName = "Comic Book",
                description = "Bold comic book styling with strong outlines and vibrant colors",
                baseColor = Color.white
            };
            
            // Lighting
            preset.lightingSettings.shadowThreshold = 0.5f;
            preset.lightingSettings.shadowSmoothness = 0.02f;
            preset.lightingSettings.shadowColor = new Color(0.3f, 0.3f, 0.5f, 1f);
            
            // Visual effects
            preset.visualEffects.outline.enableOutline = true;
            preset.visualEffects.outline.outlineColor = Color.black;
            preset.visualEffects.outline.outlineWidth = 0.02f;
            
            preset.visualEffects.rimLighting.enableRimLighting = true;
            preset.visualEffects.rimLighting.rimIntensity = 2.5f;
            preset.visualEffects.rimLighting.rimColor = new Color(1f, 0.9f, 0.7f, 1f);
            
            preset.visualEffects.specular.enableSpecular = true;
            preset.visualEffects.specular.specularIntensity = 3f;
            preset.visualEffects.specular.specularSize = 0.03f;
            
            // Stylization
            preset.stylization.colorGrading.saturation = 1.3f;
            preset.stylization.colorGrading.contrast = 1.4f;
            
            return preset;
        }
        
        /// <summary>
        /// Creates a hatched drawing style preset
        /// </summary>
        public static ToonShaderPreset CreateHatchedDrawingPreset()
        {
            var preset = new ToonShaderPreset
            {
                presetName = "Hatched Drawing",
                description = "Technical illustration style with extensive hatching effects",
                baseColor = Color.white
            };
            
            // Lighting
            preset.lightingSettings.shadowThreshold = 0.45f;
            preset.lightingSettings.shadowSmoothness = 0.1f;
            preset.lightingSettings.shadowColor = new Color(0.95f, 0.95f, 0.95f, 1f);
            
            // Stylization - Advanced hatching
            preset.stylization.hatching.enableHatching = true;
            preset.stylization.hatching.hatchingDensity = 1.5f;
            preset.stylization.hatching.hatchingIntensity = 1.2f;
            preset.stylization.hatching.hatchingThreshold = 0.7f;
            preset.stylization.hatching.crossHatchingThreshold = 0.4f;
            preset.stylization.hatching.hatchingRotation = 30f;
            
            preset.stylization.hatching.enableScreenSpaceHatching = true;
            preset.stylization.hatching.screenHatchScale = 3f;
            preset.stylization.hatching.screenHatchBias = 0.2f;
            
            preset.stylization.colorGrading.saturation = 0.7f;
            preset.stylization.colorGrading.contrast = 1.3f;
            preset.stylization.colorGrading.gamma = 1.2f;
            
            return preset;
        }
        
        /// <summary>
        /// Creates a realistic toon preset
        /// </summary>
        public static ToonShaderPreset CreateRealisticToonPreset()
        {
            var preset = new ToonShaderPreset
            {
                presetName = "Realistic Toon",
                description = "Balanced approach between realism and stylization",
                baseColor = Color.white
            };
            
            // Lighting
            preset.lightingSettings = ToonLightingSettings.CreateRealisticPreset();
            
            // Visual effects
            preset.visualEffects.subsurface.enableSubsurface = true;
            preset.visualEffects.subsurface.subsurfaceColor = new Color(1f, 0.7f, 0.7f, 1f);
            preset.visualEffects.subsurface.subsurfaceIntensity = 0.3f;
            preset.visualEffects.subsurface.subsurfacePower = 2f;
            
            preset.visualEffects.fresnel.enableFresnel = true;
            preset.visualEffects.fresnel.fresnelIntensity = 0.5f;
            preset.visualEffects.fresnel.fresnelPower = 3f;
            
            preset.visualEffects.rimLighting.enableRimLighting = true;
            preset.visualEffects.rimLighting.rimIntensity = 1.5f;
            
            return preset;
        }
        
        /// <summary>
        /// Creates a painterly style preset
        /// </summary>
        public static ToonShaderPreset CreatePainterlyPreset()
        {
            var preset = new ToonShaderPreset
            {
                presetName = "Painterly",
                description = "Artistic painted appearance with soft lighting",
                baseColor = Color.white
            };
            
            // Lighting
            preset.lightingSettings.shadowThreshold = 0.35f;
            preset.lightingSettings.shadowSmoothness = 0.3f;
            preset.lightingSettings.shadowColor = new Color(0.6f, 0.7f, 0.8f, 1f);
            preset.lightingSettings.indirectLightingBoost = 0.4f;
            
            // Visual effects
            preset.visualEffects.matcap.enableMatcap = true;
            preset.visualEffects.matcap.matcapIntensity = 0.8f;
            preset.visualEffects.matcap.blendMode = MatcapBlendMode.Multiply;
            
            preset.visualEffects.fresnel.enableFresnel = true;
            preset.visualEffects.fresnel.fresnelIntensity = 1.5f;
            preset.visualEffects.fresnel.fresnelPower = 2f;
            
            // Stylization
            preset.stylization.colorGrading.saturation = 1.1f;
            preset.stylization.colorGrading.brightness = 1.1f;
            
            return preset;
        }
        #endregion
        
        #region Preset Operations
        
        /// <summary>
        /// Gets all built-in presets
        /// </summary>
        public static ToonShaderPreset[] GetBuiltInPresets()
        {
            return new ToonShaderPreset[]
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
        public static ToonShaderPreset GetPresetByName(string name)
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
        public static ToonShaderPreset GetPresetByIndex(int index)
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
        public static ToonShaderPreset CreatePresetFromController(ToonMaterialController controller, string presetName = "Custom Preset")
        {
            if (controller == null) return null;
            
            var preset = new ToonShaderPreset
            {
                presetName = presetName,
                description = "Custom configuration created from current settings",
                baseColor = controller.baseColor
            };
            
            // Copy settings
            preset.lightingSettings.CopyFrom(controller.lightingSettings);
            preset.animationSettings.CopyFrom(controller.animationSettings);
            
            // Note: In a full implementation, you'd need to add copy methods to visual effects
            // and stylization classes similar to what we have for lighting and animation
            
            return preset;
        }
        
        /// <summary>
        /// Applies a preset to a material controller
        /// </summary>
        public static void ApplyPreset(ToonMaterialController controller, ToonShaderPreset preset)
        {
            if (controller == null || preset == null) return;
            
            preset.ApplyToController(controller);
        }
        
        /// <summary>
        /// Applies a preset by name
        /// </summary>
        public static void ApplyPresetByName(ToonMaterialController controller, string presetName)
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
        public static void ApplyPresetByIndex(ToonMaterialController controller, int index)
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
        public static bool ValidatePreset(ToonShaderPreset preset)
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
        public static float EstimatePresetPerformance(ToonShaderPreset preset)
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
        public static string GetPerformanceCategory(ToonShaderPreset preset)
        {
            float cost = EstimatePresetPerformance(preset);
            
            if (cost < 0.3f) return "Light";
            if (cost < 0.6f) return "Medium";
            if (cost < 0.8f) return "Heavy";
            return "Ultra";
        }
        #endregion
    }
}