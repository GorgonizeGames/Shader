using UnityEngine;
using Gorgonize.ToonShader.Core;

namespace Gorgonize.ToonShader.Settings
{
    /// <summary>
    /// Rim lighting effect configuration
    /// </summary>
    [System.Serializable]
    public class ToonRimLighting
    {
        [Tooltip("Enable rim lighting effect")]
        public bool enableRimLighting = true;
        
        [Tooltip("Color of the rim light")]
        public Color rimColor = Color.white;
        
        [Range(0.1f, 10f)]
        [Tooltip("Falloff curve of rim lighting")]
        public float rimPower = 2f;
        
        [Range(0f, 5f)]
        [Tooltip("Intensity of rim lighting")]
        public float rimIntensity = 1f;
        
        [Range(0f, 1f)]
        [Tooltip("Threshold for rim lighting visibility")]
        public float rimThreshold = 0.1f;

        public void ApplyToMaterial(Material material)
        {
            if (material == null) return;

            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.EnableRimLighting, enableRimLighting ? 1f : 0f);
            ToonShaderProperties.SetKeywordSafe(material, ToonShaderProperties.Keywords.RimLighting, enableRimLighting);
            
            if (enableRimLighting)
            {
                ToonShaderProperties.SetColorSafe(material, ToonShaderProperties.RimColor, rimColor);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.RimPower, rimPower);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.RimIntensity, rimIntensity);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.RimThreshold, rimThreshold);
            }
        }

        public void CopyFrom(ToonRimLighting other)
        {
            if (other == null) return;
            enableRimLighting = other.enableRimLighting;
            rimColor = other.rimColor;
            rimPower = other.rimPower;
            rimIntensity = other.rimIntensity;
            rimThreshold = other.rimThreshold;
        }

        public void ValidateSettings()
        {
            rimPower = Mathf.Clamp(rimPower, 0.1f, 10f);
            rimIntensity = Mathf.Clamp(rimIntensity, 0f, 5f);
            rimThreshold = Mathf.Clamp01(rimThreshold);
        }
    }

    /// <summary>
    /// Specular highlight configuration
    /// </summary>
    [System.Serializable]
    public class ToonSpecular
    {
        [Tooltip("Enable specular highlights")]
        public bool enableSpecular = true;
        
        [Tooltip("Color of specular highlights")]
        public Color specularColor = Color.white;
        
        [Range(0.001f, 1f)]
        [Tooltip("Size of specular highlights")]
        public float specularSize = 0.1f;
        
        [Range(0.001f, 0.5f)]
        [Tooltip("Smoothness of specular edges")]
        public float specularSmoothness = 0.05f;
        
        [Range(0f, 5f)]
        [Tooltip("Intensity of specular highlights")]
        public float specularIntensity = 1f;

        public void ApplyToMaterial(Material material)
        {
            if (material == null) return;

            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.EnableSpecular, enableSpecular ? 1f : 0f);
            ToonShaderProperties.SetKeywordSafe(material, ToonShaderProperties.Keywords.Specular, enableSpecular);
            
            if (enableSpecular)
            {
                ToonShaderProperties.SetColorSafe(material, ToonShaderProperties.SpecularColor, specularColor);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.SpecularSize, specularSize);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.SpecularSmoothness, specularSmoothness);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.SpecularIntensity, specularIntensity);
            }
        }

        public void CopyFrom(ToonSpecular other)
        {
            if (other == null) return;
            enableSpecular = other.enableSpecular;
            specularColor = other.specularColor;
            specularSize = other.specularSize;
            specularSmoothness = other.specularSmoothness;
            specularIntensity = other.specularIntensity;
        }

        public void ValidateSettings()
        {
            specularSize = Mathf.Clamp(specularSize, 0.001f, 1f);
            specularSmoothness = Mathf.Clamp(specularSmoothness, 0.001f, 0.5f);
            specularIntensity = Mathf.Clamp(specularIntensity, 0f, 5f);
        }
    }

    /// <summary>
    /// Matcap effect configuration
    /// </summary>
    [System.Serializable]
    public class ToonMatcap
    {
        [Tooltip("Enable matcap effect")]
        public bool enableMatcap = false;
        
        [Tooltip("Matcap texture for sphere-mapped lighting")]
        public Texture2D matcapTexture;
        
        [Range(0f, 2f)]
        [Tooltip("Intensity of matcap effect")]
        public float matcapIntensity = 1f;
        
        [Tooltip("How matcap blends with the surface")]
        public MatcapBlendMode blendMode = MatcapBlendMode.Add;

        public void ApplyToMaterial(Material material)
        {
            if (material == null) return;

            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.EnableMatcap, enableMatcap ? 1f : 0f);
            ToonShaderProperties.SetKeywordSafe(material, ToonShaderProperties.Keywords.Matcap, enableMatcap);
            
            if (enableMatcap)
            {
                if (matcapTexture != null)
                    ToonShaderProperties.SetTextureSafe(material, ToonShaderProperties.MatcapTex, matcapTexture);
                
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.MatcapIntensity, matcapIntensity);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.MatcapBlendMode, (float)blendMode);
            }
        }

        public void CopyFrom(ToonMatcap other)
        {
            if (other == null) return;
            enableMatcap = other.enableMatcap;
            matcapTexture = other.matcapTexture;
            matcapIntensity = other.matcapIntensity;
            blendMode = other.blendMode;
        }

        public void ValidateSettings()
        {
            matcapIntensity = Mathf.Clamp(matcapIntensity, 0f, 2f);
        }
    }

    /// <summary>
    /// Fresnel effect configuration
    /// </summary>
    [System.Serializable]
    public class ToonFresnel
    {
        [Tooltip("Enable fresnel effect")]
        public bool enableFresnel = false;
        
        [Tooltip("Color of fresnel effect")]
        public Color fresnelColor = Color.white;
        
        [Range(0.1f, 10f)]
        [Tooltip("Falloff curve of fresnel effect")]
        public float fresnelPower = 2f;
        
        [Range(0f, 3f)]
        [Tooltip("Intensity of fresnel effect")]
        public float fresnelIntensity = 1f;

        public void ApplyToMaterial(Material material)
        {
            if (material == null) return;

            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.EnableFresnel, enableFresnel ? 1f : 0f);
            ToonShaderProperties.SetKeywordSafe(material, ToonShaderProperties.Keywords.Fresnel, enableFresnel);
            
            if (enableFresnel)
            {
                ToonShaderProperties.SetColorSafe(material, ToonShaderProperties.FresnelColor, fresnelColor);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.FresnelPower, fresnelPower);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.FresnelIntensity, fresnelIntensity);
            }
        }

        public void CopyFrom(ToonFresnel other)
        {
            if (other == null) return;
            enableFresnel = other.enableFresnel;
            fresnelColor = other.fresnelColor;
            fresnelPower = other.fresnelPower;
            fresnelIntensity = other.fresnelIntensity;
        }

        public void ValidateSettings()
        {
            fresnelPower = Mathf.Clamp(fresnelPower, 0.1f, 10f);
            fresnelIntensity = Mathf.Clamp(fresnelIntensity, 0f, 3f);
        }
    }

    /// <summary>
    /// Subsurface scattering effect configuration
    /// </summary>
    [System.Serializable]
    public class ToonSubsurface
    {
        [Tooltip("Enable subsurface scattering")]
        public bool enableSubsurface = false;
        
        [Tooltip("Color of subsurface scattering")]
        public Color subsurfaceColor = new Color(1f, 0.5f, 0.5f, 1f);
        
        [Range(0.1f, 10f)]
        [Tooltip("Falloff curve of subsurface effect")]
        public float subsurfacePower = 3f;
        
        [Range(0f, 2f)]
        [Tooltip("Intensity of subsurface effect")]
        public float subsurfaceIntensity = 0.5f;

        public void ApplyToMaterial(Material material)
        {
            if (material == null) return;

            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.EnableSubsurface, enableSubsurface ? 1f : 0f);
            ToonShaderProperties.SetKeywordSafe(material, ToonShaderProperties.Keywords.Subsurface, enableSubsurface);
            
            if (enableSubsurface)
            {
                ToonShaderProperties.SetColorSafe(material, ToonShaderProperties.SubsurfaceColor, subsurfaceColor);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.SubsurfacePower, subsurfacePower);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.SubsurfaceIntensity, subsurfaceIntensity);
            }
        }

        public void CopyFrom(ToonSubsurface other)
        {
            if (other == null) return;
            enableSubsurface = other.enableSubsurface;
            subsurfaceColor = other.subsurfaceColor;
            subsurfacePower = other.subsurfacePower;
            subsurfaceIntensity = other.subsurfaceIntensity;
        }

        public void ValidateSettings()
        {
            subsurfacePower = Mathf.Clamp(subsurfacePower, 0.1f, 10f);
            subsurfaceIntensity = Mathf.Clamp(subsurfaceIntensity, 0f, 2f);
        }
    }

    /// <summary>
    /// Outline effect configuration
    /// </summary>
    [System.Serializable]
    public class ToonOutline
    {
        [Tooltip("Enable outline effect")]
        public bool enableOutline = false;
        
        [Tooltip("Color of the outline")]
        public Color outlineColor = Color.black;
        
        [Range(0f, 0.1f)]
        [Tooltip("Width of the outline")]
        public float outlineWidth = 0.01f;

        public void ApplyToMaterial(Material material)
        {
            if (material == null) return;

            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.EnableOutline, enableOutline ? 1f : 0f);
            ToonShaderProperties.SetKeywordSafe(material, ToonShaderProperties.Keywords.Outline, enableOutline);
            
            if (enableOutline)
            {
                ToonShaderProperties.SetColorSafe(material, ToonShaderProperties.OutlineColor, outlineColor);
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.OutlineWidth, outlineWidth);
            }
        }

        public void CopyFrom(ToonOutline other)
        {
            if (other == null) return;
            enableOutline = other.enableOutline;
            outlineColor = other.outlineColor;
            outlineWidth = other.outlineWidth;
        }

        public void ValidateSettings()
        {
            outlineWidth = Mathf.Clamp(outlineWidth, 0f, 0.1f);
        }
    }

    /// <summary>
    /// Matcap blend modes
    /// </summary>
    public enum MatcapBlendMode
    {
        Add = 0,
        Multiply = 1,
        Screen = 2
    }

    /// <summary>
    /// Container for all visual effects settings
    /// </summary>
    [System.Serializable]
    public class ToonVisualEffects
    {
        [Header("Surface Effects")]
        public ToonRimLighting rimLighting = new ToonRimLighting();
        public ToonSpecular specular = new ToonSpecular();
        public ToonMatcap matcap = new ToonMatcap();
        
        [Header("Advanced Effects")]
        public ToonFresnel fresnel = new ToonFresnel();
        public ToonSubsurface subsurface = new ToonSubsurface();
        public ToonOutline outline = new ToonOutline();

        /// <summary>
        /// Applies all visual effects to the material
        /// </summary>
        /// <param name="material">Target material</param>
        public void ApplyToMaterial(Material material)
        {
            rimLighting.ApplyToMaterial(material);
            specular.ApplyToMaterial(material);
            matcap.ApplyToMaterial(material);
            fresnel.ApplyToMaterial(material);
            subsurface.ApplyToMaterial(material);
            outline.ApplyToMaterial(material);
        }

        /// <summary>
        /// Returns the number of active effects
        /// </summary>
        public int GetActiveEffectCount()
        {
            int count = 0;
            if (rimLighting.enableRimLighting) count++;
            if (specular.enableSpecular) count++;
            if (matcap.enableMatcap) count++;
            if (fresnel.enableFresnel) count++;
            if (subsurface.enableSubsurface) count++;
            if (outline.enableOutline) count++;
            return count;
        }

        /// <summary>
        /// Estimates performance cost of active effects
        /// </summary>
        public float GetPerformanceCost()
        {
            float cost = 0f;
            
            if (rimLighting.enableRimLighting) cost += 0.1f;
            if (specular.enableSpecular) cost += 0.15f;
            if (matcap.enableMatcap) cost += 0.2f;
            if (fresnel.enableFresnel) cost += 0.1f;
            if (subsurface.enableSubsurface) cost += 0.25f;
            if (outline.enableOutline) cost += 0.3f;
            
            return Mathf.Clamp01(cost);
        }

        /// <summary>
        /// Copies all settings from another visual effects instance
        /// </summary>
        public void CopyFrom(ToonVisualEffects other)
        {
            if (other == null) return;
            
            rimLighting.CopyFrom(other.rimLighting);
            specular.CopyFrom(other.specular);
            matcap.CopyFrom(other.matcap);
            fresnel.CopyFrom(other.fresnel);
            subsurface.CopyFrom(other.subsurface);
            outline.CopyFrom(other.outline);
        }

        /// <summary>
        /// Validates all visual effects settings
        /// </summary>
        public void ValidateSettings()
        {
            rimLighting.ValidateSettings();
            specular.ValidateSettings();
            matcap.ValidateSettings();
            fresnel.ValidateSettings();
            subsurface.ValidateSettings();
            outline.ValidateSettings();
        }

        /// <summary>
        /// Creates anime-style visual effects preset
        /// </summary>
        public static ToonVisualEffects CreateAnimePreset()
        {
            var effects = new ToonVisualEffects();
            
            effects.rimLighting.enableRimLighting = true;
            effects.rimLighting.rimColor = new Color(1f, 0.95f, 0.8f, 1f);
            effects.rimLighting.rimPower = 1.5f;
            effects.rimLighting.rimIntensity = 2f;
            
            effects.specular.enableSpecular = true;
            effects.specular.specularSize = 0.05f;
            effects.specular.specularIntensity = 2f;
            
            return effects;
        }

        /// <summary>
        /// Creates cartoon-style visual effects preset
        /// </summary>
        public static ToonVisualEffects CreateCartoonPreset()
        {
            var effects = new ToonVisualEffects();
            
            effects.rimLighting.enableRimLighting = true;
            effects.rimLighting.rimIntensity = 3f;
            
            effects.outline.enableOutline = true;
            effects.outline.outlineColor = Color.black;
            effects.outline.outlineWidth = 0.02f;
            
            return effects;
        }

        /// <summary>
        /// Creates realistic toon visual effects preset
        /// </summary>
        public static ToonVisualEffects CreateRealisticPreset()
        {
            var effects = new ToonVisualEffects();
            
            effects.subsurface.enableSubsurface = true;
            effects.subsurface.subsurfaceColor = new Color(1f, 0.7f, 0.7f, 1f);
            effects.subsurface.subsurfaceIntensity = 0.3f;
            
            effects.fresnel.enableFresnel = true;
            effects.fresnel.fresnelIntensity = 0.5f;
            
            return effects;
        }

        /// <summary>
        /// Creates minimal visual effects preset for performance
        /// </summary>
        public static ToonVisualEffects CreateMinimalPreset()
        {
            var effects = new ToonVisualEffects();
            
            effects.rimLighting.enableRimLighting = false;
            effects.specular.enableSpecular = false;
            effects.matcap.enableMatcap = false;
            effects.fresnel.enableFresnel = false;
            effects.subsurface.enableSubsurface = false;
            effects.outline.enableOutline = false;
            
            return effects;
        }
    }
}