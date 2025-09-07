using UnityEngine;
using Gorgonize.ToonShader.Core;

namespace Gorgonize.ToonShader.Settings
{
    /// <summary>
    /// Hatching effect configuration for sketch-like rendering
    /// </summary>
    [System.Serializable]
    public class GorgonizeToonHatching
    {
        [Header("Basic Hatching")]
        [Tooltip("Enable hatching effects")]
        public bool enableHatching = false;
        
        [Tooltip("Main hatching pattern texture")]
        public Texture2D hatchingTexture;
        
        [Tooltip("Cross-hatching pattern for deeper shadows")]
        public Texture2D crossHatchingTexture;
        
        [Range(0.1f, 5f)]
        [Tooltip("Density/scale of hatching patterns")]
        public float hatchingDensity = 1f;
        
        [Range(0f, 2f)]
        [Tooltip("Intensity of hatching effect")]
        public float hatchingIntensity = 1f;
        
        [Range(0f, 1f)]
        [Tooltip("Light threshold where hatching appears")]
        public float hatchingThreshold = 0.5f;
        
        [Range(0f, 1f)]
        [Tooltip("Light threshold where cross-hatching appears")]
        public float crossHatchingThreshold = 0.3f;
        
        [Range(0f, 360f)]
        [Tooltip("Rotation angle of hatching patterns")]
        public float hatchingRotation = 45f;
        
        [Header("Screen Space Hatching")]
        [Tooltip("Enable screen-space hatching for consistent line density")]
        public bool enableScreenSpaceHatching = false;
        
        [Range(0.1f, 10f)]
        [Tooltip("Scale of screen-space hatching")]
        public float screenHatchScale = 2f;
        
        [Range(-1f, 1f)]
        [Tooltip("Bias for screen-space hatching")]
        public float screenHatchBias = 0f;

        /// <summary>
        /// Applies hatching settings to material
        /// </summary>
        public void ApplyToMaterial(Material material)
        {
            if (material == null) return;

            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.EnableHatching, enableHatching ? 1f : 0f);
            GorgonizeToonShaderProperties.SetKeywordSafe(material, GorgonizeToonShaderProperties.Keywords.Hatching, enableHatching);

            if (enableHatching)
            {
                if (hatchingTexture != null)
                    GorgonizeToonShaderProperties.SetTextureSafe(material, GorgonizeToonShaderProperties.HatchingTex, hatchingTexture);
                
                if (crossHatchingTexture != null)
                    GorgonizeToonShaderProperties.SetTextureSafe(material, GorgonizeToonShaderProperties.CrossHatchingTex, crossHatchingTexture);

                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.HatchingDensity, hatchingDensity);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.HatchingIntensity, hatchingIntensity);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.HatchingThreshold, hatchingThreshold);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.CrossHatchingThreshold, crossHatchingThreshold);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.HatchingRotation, hatchingRotation);
            }

            // Screen space hatching
            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.EnableScreenSpaceHatching, enableScreenSpaceHatching ? 1f : 0f);
            GorgonizeToonShaderProperties.SetKeywordSafe(material, GorgonizeToonShaderProperties.Keywords.ScreenSpaceHatching, enableScreenSpaceHatching);

            if (enableScreenSpaceHatching)
            {
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.ScreenHatchScale, screenHatchScale);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.ScreenHatchBias, screenHatchBias);
            }
        }

        /// <summary>
        /// Validates hatching settings
        /// </summary>
        public void ValidateSettings()
        {
            hatchingDensity = Mathf.Clamp(hatchingDensity, 0.1f, 5f);
            hatchingIntensity = Mathf.Clamp(hatchingIntensity, 0f, 2f);
            hatchingThreshold = Mathf.Clamp01(hatchingThreshold);
            crossHatchingThreshold = Mathf.Clamp01(crossHatchingThreshold);
            hatchingRotation = hatchingRotation % 360f;
            screenHatchScale = Mathf.Clamp(screenHatchScale, 0.1f, 10f);
            screenHatchBias = Mathf.Clamp(screenHatchBias, -1f, 1f);
        }

        public void CopyFrom(GorgonizeToonHatching other)
        {
            if (other == null) return;
            enableHatching = other.enableHatching;
            hatchingTexture = other.hatchingTexture;
            crossHatchingTexture = other.crossHatchingTexture;
            hatchingDensity = other.hatchingDensity;
            hatchingIntensity = other.hatchingIntensity;
            hatchingThreshold = other.hatchingThreshold;
            crossHatchingThreshold = other.crossHatchingThreshold;
            hatchingRotation = other.hatchingRotation;
            enableScreenSpaceHatching = other.enableScreenSpaceHatching;
            screenHatchScale = other.screenHatchScale;
            screenHatchBias = other.screenHatchBias;
        }
    }

    /// <summary>
    /// Color grading configuration
    /// </summary>
    [System.Serializable]
    public class GorgonizeToonColorGrading
    {
        [Range(0f, 2f)]
        [Tooltip("Color saturation multiplier")]
        public float saturation = 1f;
        
        [Range(0f, 2f)]
        [Tooltip("Brightness multiplier")]
        public float brightness = 1f;
        
        [Range(-180f, 180f)]
        [Tooltip("Hue shift in degrees")]
        public float hueShift = 0f;
        
        [Range(0.5f, 3f)]
        [Tooltip("Contrast adjustment")]
        public float contrast = 1f;
        
        [Range(0.5f, 3f)]
        [Tooltip("Gamma correction")]
        public float gamma = 1f;

        /// <summary>
        /// Applies color grading to material
        /// </summary>
        public void ApplyToMaterial(Material material)
        {
            if (material == null) return;

            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.Saturation, saturation);
            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.Brightness, brightness);
            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.Hue, hueShift);
            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.Contrast, contrast);
            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.Gamma, gamma);
        }

        public void CopyFrom(GorgonizeToonColorGrading other)
        {
            if (other == null) return;
            saturation = other.saturation;
            brightness = other.brightness;
            hueShift = other.hueShift;
            contrast = other.contrast;
            gamma = other.gamma;
        }

        public void ValidateSettings()
        {
            saturation = Mathf.Clamp(saturation, 0f, 2f);
            brightness = Mathf.Clamp(brightness, 0f, 2f);
            hueShift = Mathf.Clamp(hueShift, -180f, 180f);
            contrast = Mathf.Clamp(contrast, 0.5f, 3f);
            gamma = Mathf.Clamp(gamma, 0.5f, 3f);
        }
    }

    /// <summary>
    /// Posterization and cel-shading effects
    /// </summary>
    [System.Serializable]
    public class GorgonizeToonQuantization
    {
        [Header("Posterization")]
        [Tooltip("Enable color posterization")]
        public bool enablePosterize = false;
        
        [Range(2f, 32f)]
        [Tooltip("Number of color levels for posterization")]
        public float posterizeLevels = 8f;
        
        [Header("Cel Shading")]
        [Tooltip("Enable cel shading steps")]
        public bool enableCelShading = false;
        
        [Range(2f, 10f)]
        [Tooltip("Number of lighting steps for cel shading")]
        public float celShadingSteps = 3f;

        /// <summary>
        /// Applies quantization effects to material
        /// </summary>
        public void ApplyToMaterial(Material material)
        {
            if (material == null) return;

            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.EnablePosterize, enablePosterize ? 1f : 0f);
            GorgonizeToonShaderProperties.SetKeywordSafe(material, GorgonizeToonShaderProperties.Keywords.Posterize, enablePosterize);
            
            if (enablePosterize)
            {
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.PosterizeLevels, posterizeLevels);
            }

            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.EnableCelShading, enableCelShading ? 1f : 0f);
            GorgonizeToonShaderProperties.SetKeywordSafe(material, GorgonizeToonShaderProperties.Keywords.CelShading, enableCelShading);
            
            if (enableCelShading)
            {
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.CelShadingSteps, celShadingSteps);
            }
        }

        public void CopyFrom(GorgonizeToonQuantization other)
        {
            if (other == null) return;
            enablePosterize = other.enablePosterize;
            posterizeLevels = other.posterizeLevels;
            enableCelShading = other.enableCelShading;
            celShadingSteps = other.celShadingSteps;
        }

        public void ValidateSettings()
        {
            posterizeLevels = Mathf.Clamp(posterizeLevels, 2f, 32f);
            celShadingSteps = Mathf.Clamp(celShadingSteps, 2f, 10f);
        }

        public int GetActiveEffectCount()
        {
            int count = 0;
            if (enablePosterize) count++;
            if (enableCelShading) count++;
            return count;
        }
    }

    /// <summary>
    /// Complete stylization settings container
    /// </summary>
    [System.Serializable]
    public class GorgonizeToonStylization
    {
        [Header("Hatching Effects")]
        public GorgonizeToonHatching hatching = new GorgonizeToonHatching();
        
        [Header("Color Processing")]
        public GorgonizeToonColorGrading colorGrading = new GorgonizeToonColorGrading();
        
        [Header("Quantization Effects")]
        public GorgonizeToonQuantization quantization = new GorgonizeToonQuantization();

        /// <summary>
        /// Applies all stylization effects to material
        /// </summary>
        public void ApplyToMaterial(Material material)
        {
            hatching.ApplyToMaterial(material);
            colorGrading.ApplyToMaterial(material);
            quantization.ApplyToMaterial(material);
        }

        /// <summary>
        /// Returns number of active stylization effects
        /// </summary>
        public int GetActiveEffectCount()
        {
            int count = 0;
            if (hatching.enableHatching) count++;
            if (hatching.enableScreenSpaceHatching) count++;
            count += quantization.GetActiveEffectCount();
            return count;
        }

        /// <summary>
        /// Estimates performance cost of stylization effects
        /// </summary>
        public float GetPerformanceCost()
        {
            float cost = 0f;
            
            // Color grading is relatively cheap
            cost += 0.05f;
            
            if (hatching.enableHatching) cost += 0.2f;
            if (hatching.enableScreenSpaceHatching) cost += 0.15f;
            if (quantization.enablePosterize) cost += 0.1f;
            if (quantization.enableCelShading) cost += 0.05f;
            
            return Mathf.Clamp01(cost);
        }

        /// <summary>
        /// Creates sketch-style preset
        /// </summary>
        public static GorgonizeToonStylization CreateSketchPreset()
        {
            var preset = new GorgonizeToonStylization();
            
            preset.hatching.enableHatching = true;
            preset.hatching.hatchingDensity = 2f;
            preset.hatching.hatchingIntensity = 0.8f;
            preset.hatching.hatchingThreshold = 0.6f;
            preset.hatching.crossHatchingThreshold = 0.3f;
            preset.hatching.hatchingRotation = 45f;
            
            preset.colorGrading.saturation = 0.8f;
            preset.colorGrading.contrast = 1.2f;
            
            return preset;
        }

        /// <summary>
        /// Creates comic book style preset
        /// </summary>
        public static GorgonizeToonStylization CreateComicPreset()
        {
            var preset = new GorgonizeToonStylization();
            
            preset.quantization.enableCelShading = true;
            preset.quantization.celShadingSteps = 4f;
            
            preset.colorGrading.saturation = 1.3f;
            preset.colorGrading.contrast = 1.4f;
            
            return preset;
        }

        /// <summary>
        /// Creates painterly style preset
        /// </summary>
        public static GorgonizeToonStylization CreatePainterlyPreset()
        {
            var preset = new GorgonizeToonStylization();
            
            preset.quantization.enablePosterize = true;
            preset.quantization.posterizeLevels = 6f;
            
            preset.colorGrading.saturation = 1.1f;
            preset.colorGrading.brightness = 1.1f;
            
            return preset;
        }

        /// <summary>
        /// Validates all stylization settings
        /// </summary>
        public void ValidateSettings()
        {
            hatching.ValidateSettings();
            colorGrading.ValidateSettings();
            quantization.ValidateSettings();
        }

        /// <summary>
        /// Copy settings from another stylization instance
        /// </summary>
        public void CopyFrom(GorgonizeToonStylization other)
        {
            if (other == null) return;
            hatching.CopyFrom(other.hatching);
            colorGrading.CopyFrom(other.colorGrading);
            quantization.CopyFrom(other.quantization);
        }
    }
}