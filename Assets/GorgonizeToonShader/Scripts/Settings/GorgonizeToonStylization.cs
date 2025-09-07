using UnityEngine;
using Gorgonize.ToonShader.Core;

namespace Gorgonize.ToonShader.Settings
{
    /// <summary>
    /// Hatching effect configuration for sketch-like rendering
    /// </summary>
    [System.Serializable]
    public class ToonHatching
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

            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.EnableHatching, enableHatching ? 1f : 0f);
            ToonShaderProperties.SetKeywordSafe(material, ToonShaderProperties.Keywords.Hatching, enableHatching);

            if (enableHatching)
            {
                if (hatchingTexture != null)
                    ToonShaderProperties.SetTextureSafe(material, ToonShaderProperties.HatchingTex, hatchingTexture);
                
                if (crossHatchingTexture != null)
                    ToonShaderProperties.SetTextureSafe(material, ToonShaderProperties.CrossHatchingTex, crossHatchingTexture);

                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.HatchingDensity, hatchingDensity);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.HatchingIntensity, hatchingIntensity);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.HatchingThreshold, hatchingThreshold);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.CrossHatchingThreshold, crossHatchingThreshold);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.HatchingRotation, hatchingRotation);
            }

            // Screen space hatching
            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.EnableScreenSpaceHatching, enableScreenSpaceHatching ? 1f : 0f);
            ToonShaderProperties.SetKeywordSafe(material, ToonShaderProperties.Keywords.ScreenSpaceHatching, enableScreenSpaceHatching);

            if (enableScreenSpaceHatching)
            {
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.ScreenHatchScale, screenHatchScale);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.ScreenHatchBias, screenHatchBias);
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
    }

    /// <summary>
    /// Color grading configuration
    /// </summary>
    [System.Serializable]
    public class ToonColorGrading
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

            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.Saturation, saturation);
            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.Brightness, brightness);
            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.Hue, hueShift);
            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.Contrast, contrast);
            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.Gamma, gamma);
        }
    }

    /// <summary>
    /// Posterization and cel-shading effects
    /// </summary>
    [System.Serializable]
    public class ToonQuantization
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

            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.EnablePosterize, enablePosterize ? 1f : 0f);
            ToonShaderProperties.SetKeywordSafe(material, ToonShaderProperties.Keywords.Posterize, enablePosterize);
            
            if (enablePosterize)
            {
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.PosterizeLevels, posterizeLevels);
            }

            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.EnableCelShading, enableCelShading ? 1f : 0f);
            ToonShaderProperties.SetKeywordSafe(material, ToonShaderProperties.Keywords.CelShading, enableCelShading);
            
            if (enableCelShading)
            {
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.CelShadingSteps, celShadingSteps);
            }
        }
    }

    /// <summary>
    /// Complete stylization settings container
    /// </summary>
    [System.Serializable]
    public class ToonStylization
    {
        [Header("Hatching Effects")]
        public ToonHatching hatching = new ToonHatching();
        
        [Header("Color Processing")]
        public ToonColorGrading colorGrading = new ToonColorGrading();
        
        [Header("Quantization Effects")]
        public ToonQuantization quantization = new ToonQuantization();

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
            if (quantization.enablePosterize) count++;
            if (quantization.enableCelShading) count++;
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
        public static ToonStylization CreateSketchPreset()
        {
            var preset = new ToonStylization();
            
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
        public static ToonStylization CreateComicPreset()
        {
            var preset = new ToonStylization();
            
            preset.quantization.enableCelShading = true;
            preset.quantization.celShadingSteps = 4f;
            
            preset.colorGrading.saturation = 1.3f;
            preset.colorGrading.contrast = 1.4f;
            
            return preset;
        }

        /// <summary>
        /// Creates painterly style preset
        /// </summary>
        public static ToonStylization CreatePainterlyPreset()
        {
            var preset = new ToonStylization();
            
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
            
            colorGrading.saturation = Mathf.Clamp(colorGrading.saturation, 0f, 2f);
            colorGrading.brightness = Mathf.Clamp(colorGrading.brightness, 0f, 2f);
            colorGrading.hueShift = Mathf.Clamp(colorGrading.hueShift, -180f, 180f);
            colorGrading.contrast = Mathf.Clamp(colorGrading.contrast, 0.5f, 3f);
            colorGrading.gamma = Mathf.Clamp(colorGrading.gamma, 0.5f, 3f);
            
            quantization.posterizeLevels = Mathf.Clamp(quantization.posterizeLevels, 2f, 32f);
            quantization.celShadingSteps = Mathf.Clamp(quantization.celShadingSteps, 2f, 10f);
        }
    }
}