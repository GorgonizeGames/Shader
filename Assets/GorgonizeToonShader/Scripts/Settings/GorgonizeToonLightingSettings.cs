using UnityEngine;
using Gorgonize.ToonShader.Core;

namespace Gorgonize.ToonShader.Settings
{
    /// <summary>
    /// Comprehensive lighting settings for the Ultimate Toon Shader
    /// Handles shadow calculation, light ramps, and advanced lighting features
    /// </summary>
    [System.Serializable]
    public class GorgonizeToonLightingSettings
    {
        [Header("Shadow Configuration")]
        [Range(0f, 1f)]
        [Tooltip("Threshold where shadows appear. Lower values = more shadows")]
        public float shadowThreshold = 0.5f;
        
        [Range(0f, 0.5f)]
        [Tooltip("Softness of shadow edges. 0 = hard shadows, 0.5 = very soft")]
        public float shadowSmoothness = 0.05f;
        
        [Tooltip("Color tint applied to shadowed areas")]
        public Color shadowColor = new Color(0.7f, 0.7f, 0.8f, 1f);
        
        [Range(0f, 1f)]
        [Tooltip("How strong the shadow effect is")]
        public float shadowIntensity = 0.8f;
        
        [Header("Light Ramp System")]
        [Tooltip("Use custom ramp texture for lighting instead of threshold-based")]
        public bool useRampTexture = false;
        
        [Tooltip("Custom lighting ramp texture (gradient from dark to light)")]
        public Texture2D lightRampTexture;
        
        [Header("Advanced Lighting")]
        [Range(0f, 2f)]
        [Tooltip("Boosts indirect/ambient lighting contribution")]
        public float indirectLightingBoost = 0.3f;
        
        [Range(0f, 1f)]
        [Tooltip("Ambient occlusion strength")]
        public float ambientOcclusion = 1f;
        
        [Range(0f, 1f)]
        [Tooltip("How much light wraps around the surface")]
        public float lightWrapping = 0f;

        /// <summary>
        /// Applies lighting settings to the given material
        /// </summary>
        /// <param name="material">Target material to update</param>
        public void ApplyToMaterial(Material material)
        {
            if (material == null) return;

            // Basic shadow properties
            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.ShadowThreshold, shadowThreshold);
            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.ShadowSmoothness, shadowSmoothness);
            GorgonizeToonShaderProperties.SetColorSafe(material, GorgonizeToonShaderProperties.ShadowColor, shadowColor);
            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.ShadowIntensity, shadowIntensity);

            // Ramp texture system
            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.UseRampTexture, useRampTexture ? 1f : 0f);
            GorgonizeToonShaderProperties.SetKeywordSafe(material, GorgonizeToonShaderProperties.Keywords.UseRampTexture, useRampTexture);
            
            if (useRampTexture && lightRampTexture != null)
            {
                GorgonizeToonShaderProperties.SetTextureSafe(material, GorgonizeToonShaderProperties.LightRampTex, lightRampTexture);
            }

            // Advanced lighting
            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.IndirectLightingBoost, indirectLightingBoost);
            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.AmbientOcclusion, ambientOcclusion);
            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.LightWrapping, lightWrapping);
        }

        /// <summary>
        /// Creates a preset for anime-style lighting
        /// </summary>
        public static GorgonizeToonLightingSettings CreateAnimePreset()
        {
            return new GorgonizeToonLightingSettings
            {
                shadowThreshold = 0.4f,
                shadowSmoothness = 0.1f,
                shadowColor = new Color(0.8f, 0.8f, 0.9f, 1f),
                shadowIntensity = 0.7f,
                useRampTexture = false,
                indirectLightingBoost = 0.5f,
                ambientOcclusion = 1f,
                lightWrapping = 0.2f
            };
        }

        /// <summary>
        /// Creates a preset for cartoon-style lighting
        /// </summary>
        public static GorgonizeToonLightingSettings CreateCartoonPreset()
        {
            return new GorgonizeToonLightingSettings
            {
                shadowThreshold = 0.6f,
                shadowSmoothness = 0.02f,
                shadowColor = new Color(0.6f, 0.6f, 0.8f, 1f),
                shadowIntensity = 0.9f,
                useRampTexture = false,
                indirectLightingBoost = 0.3f,
                ambientOcclusion = 1f,
                lightWrapping = 0f
            };
        }

        /// <summary>
        /// Creates a preset for realistic toon lighting
        /// </summary>
        public static GorgonizeToonLightingSettings CreateRealisticPreset()
        {
            return new GorgonizeToonLightingSettings
            {
                shadowThreshold = 0.3f,
                shadowSmoothness = 0.2f,
                shadowColor = new Color(0.7f, 0.7f, 0.8f, 1f),
                shadowIntensity = 0.6f,
                useRampTexture = false,
                indirectLightingBoost = 0.6f,
                ambientOcclusion = 0.8f,
                lightWrapping = 0.4f
            };
        }

        /// <summary>
        /// Validates the current settings and fixes any invalid values
        /// </summary>
        public void ValidateSettings()
        {
            shadowThreshold = Mathf.Clamp01(shadowThreshold);
            shadowSmoothness = Mathf.Clamp(shadowSmoothness, 0f, 0.5f);
            shadowIntensity = Mathf.Clamp01(shadowIntensity);
            indirectLightingBoost = Mathf.Clamp(indirectLightingBoost, 0f, 2f);
            ambientOcclusion = Mathf.Clamp01(ambientOcclusion);
            lightWrapping = Mathf.Clamp01(lightWrapping);
        }

        /// <summary>
        /// Returns performance cost estimation for current settings
        /// </summary>
        /// <returns>Performance cost from 0 (free) to 1 (expensive)</returns>
        public float GetPerformanceCost()
        {
            float cost = 0f;
            
            // Base cost for toon lighting
            cost += 0.1f;
            
            // Ramp texture adds minimal cost
            if (useRampTexture) cost += 0.05f;
            
            // Advanced lighting features
            if (indirectLightingBoost > 0.1f) cost += 0.1f;
            if (lightWrapping > 0.1f) cost += 0.05f;
            
            // Smooth shadows cost more than hard shadows
            cost += shadowSmoothness * 0.1f;
            
            return Mathf.Clamp01(cost);
        }

        /// <summary>
        /// Copy settings from another ToonLightingSettings instance
        /// </summary>
        /// <param name="other">Source settings to copy from</param>
        public void CopyFrom(GorgonizeToonLightingSettings other)
        {
            if (other == null) return;

            shadowThreshold = other.shadowThreshold;
            shadowSmoothness = other.shadowSmoothness;
            shadowColor = other.shadowColor;
            shadowIntensity = other.shadowIntensity;
            useRampTexture = other.useRampTexture;
            lightRampTexture = other.lightRampTexture;
            indirectLightingBoost = other.indirectLightingBoost;
            ambientOcclusion = other.ambientOcclusion;
            lightWrapping = other.lightWrapping;
        }
    }
}