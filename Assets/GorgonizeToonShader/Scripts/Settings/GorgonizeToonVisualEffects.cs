using UnityEngine;
using Gorgonize.ToonShader.Core;

namespace Gorgonize.ToonShader.Settings
{
    /// <summary>
    /// Rim lighting effect configuration
    /// </summary>
    [System.Serializable]
    public class GorgonizeToonRimLighting
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

            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.EnableRimLighting, enableRimLighting ? 1f : 0f);
            GorgonizeToonShaderProperties.SetKeywordSafe(material, GorgonizeToonShaderProperties.Keywords.RimLighting, enableRimLighting);
            
            if (enableRimLighting)
            {
                GorgonizeToonShaderProperties.SetColorSafe(material, GorgonizeToonShaderProperties.RimColor, rimColor);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.RimPower, rimPower);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.RimIntensity, rimIntensity);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.RimThreshold, rimThreshold);
            }
        }

        public void CopyFrom(GorgonizeToonRimLighting other)
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
    public class GorgonizeToonSpecular
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

            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.EnableSpecular, enableSpecular ? 1f : 0f);
            GorgonizeToonShaderProperties.SetKeywordSafe(material, GorgonizeToonShaderProperties.Keywords.Specular, enableSpecular);
            
            if (enableSpecular)
            {
                GorgonizeToonShaderProperties.SetColorSafe(material, GorgonizeToonShaderProperties.SpecularColor, specularColor);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.SpecularSize, specularSize);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.SpecularSmoothness, specularSmoothness);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.SpecularIntensity, specularIntensity);
            }
        }

        public void CopyFrom(GorgonizeToonSpecular other)
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
    public class GorgonizeToonMatcap
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

            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.EnableMatcap, enableMatcap ? 1f : 0f);
            GorgonizeToonShaderProperties.SetKeywordSafe(material, GorgonizeToonShaderProperties.Keywords.Matcap, enableMatcap);
            
            if (enableMatcap)
            {
                if (matcapTexture != null)
                    GorgonizeToonShaderProperties.SetTextureSafe(material, GorgonizeToonShaderProperties.MatcapTex, matcapTexture);
                
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.MatcapIntensity, matcapIntensity);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.MatcapBlendMode, (float)blendMode);
            }
        }

        public void CopyFrom(GorgonizeToonMatcap other)
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
    public class GorgonizeToonFresnel
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

            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.EnableFresnel, enableFresnel ? 1f : 0f);
            GorgonizeToonShaderProperties.SetKeywordSafe(material, GorgonizeToonShaderProperties.Keywords.Fresnel, enableFresnel);
            
            if (enableFresnel)
            {
                GorgonizeToonShaderProperties.SetColorSafe(material, GorgonizeToonShaderProperties.FresnelColor, fresnelColor);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.FresnelPower, fresnelPower);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.FresnelIntensity, fresnelIntensity);
            }
        }

        public void CopyFrom(GorgonizeToonFresnel other)
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
    public class GorgonizeToonSubsurface
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

            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.EnableSubsurface, enableSubsurface ? 1f : 0f);
            GorgonizeToonShaderProperties.SetKeywordSafe(material, GorgonizeToonShaderProperties.Keywords.Subsurface, enableSubsurface);
            
            if (enableSubsurface)
            {
                GorgonizeToonShaderProperties.SetColorSafe(material, GorgonizeToonShaderProperties.SubsurfaceColor, subsurfaceColor);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.SubsurfacePower, subsurfacePower);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.SubsurfaceIntensity, subsurfaceIntensity);
            }
        }

        public void CopyFrom(GorgonizeToonSubsurface other)
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
    public class GorgonizeToonOutline
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

            GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.EnableOutline, enableOutline ? 1f : 0f);
            GorgonizeToonShaderProperties.SetKeywordSafe(material, GorgonizeToonShaderProperties.Keywords.Outline, enableOutline);
            
            if (enableOutline)
            {
                GorgonizeToonShaderProperties.SetColorSafe(material, GorgonizeToonShaderProperties.OutlineColor, outlineColor);
                GorgonizeToonShaderProperties.SetFloatSafe(material, GorgonizeToonShaderProperties.OutlineWidth, outlineWidth);
            }
        }

        public void CopyFrom(GorgonizeToonOutline other)
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
    /// Special effects configuration
    /// </summary>
    [System.Serializable]
    public class GorgonizeToonSpecialEffects
    {
        [Header("Force Field")]
        [Tooltip("Enable force field effect")]
        public bool enableForceField = false;
        
        [Tooltip("Color of force field effect")]
        public Color forceFieldColor = new Color(0.2f, 0.8f, 1f, 1f);
        
        [Range(0f, 2f)]
        [Tooltip("Intensity of force field effect")]
        public float forceFieldIntensity = 1f;

        [Header("Hologram")]
        [Tooltip("Enable hologram effect")]
        public bool enableHologram = false;
        
        [Range(0f, 2f)]
        [Tooltip("Intensity of hologram effect")]
        public float hologramIntensity = 1f;

        [Header("Dissolve")]
        [Tooltip("Enable dissolve effect")]
        public bool enableDissolve = false;
        
        [Range(0f, 1f)]
        [Tooltip("Amount of dissolution")]
        public float dissolveAmount = 0f;

        public void ApplyToMaterial(Material material)
        {
            if (material == null) return;

            // Force field
            if (material.HasProperty("_EnableForceField"))
                material.SetFloat("_EnableForceField", enableForceField ? 1f : 0f);
            
            if (enableForceField)
            {
                if (material.HasProperty("_ForceFieldColor"))
                    material.SetColor("_ForceFieldColor", forceFieldColor);
                if (material.HasProperty("_ForceFieldIntensity"))
                    material.SetFloat("_ForceFieldIntensity", forceFieldIntensity);
                material.EnableKeyword("_FORCE_FIELD");
            }
            else
            {
                material.DisableKeyword("_FORCE_FIELD");
            }

            // Hologram
            if (material.HasProperty("_EnableHologram"))
                material.SetFloat("_EnableHologram", enableHologram ? 1f : 0f);
            
            if (enableHologram)
            {
                if (material.HasProperty("_HologramIntensity"))
                    material.SetFloat("_HologramIntensity", hologramIntensity);
                material.EnableKeyword("_HOLOGRAM");
            }
            else
            {
                material.DisableKeyword("_HOLOGRAM");
            }

            // Dissolve
            if (material.HasProperty("_EnableDissolve"))
                material.SetFloat("_EnableDissolve", enableDissolve ? 1f : 0f);
            
            if (enableDissolve)
            {
                if (material.HasProperty("_DissolveAmount"))
                    material.SetFloat("_DissolveAmount", dissolveAmount);
                material.EnableKeyword("_DISSOLVE");
            }
            else
            {
                material.DisableKeyword("_DISSOLVE");
            }
        }

        public void CopyFrom(GorgonizeToonSpecialEffects other)
        {
            if (other == null) return;
            enableForceField = other.enableForceField;
            forceFieldColor = other.forceFieldColor;
            forceFieldIntensity = other.forceFieldIntensity;
            enableHologram = other.enableHologram;
            hologramIntensity = other.hologramIntensity;
            enableDissolve = other.enableDissolve;
            dissolveAmount = other.dissolveAmount;
        }

        public void ValidateSettings()
        {
            forceFieldIntensity = Mathf.Clamp(forceFieldIntensity, 0f, 2f);
            hologramIntensity = Mathf.Clamp(hologramIntensity, 0f, 2f);
            dissolveAmount = Mathf.Clamp01(dissolveAmount);
        }

        public int GetActiveEffectCount()
        {
            int count = 0;
            if (enableForceField) count++;
            if (enableHologram) count++;
            if (enableDissolve) count++;
            return count;
        }

        public float GetPerformanceCost()
        {
            float cost = 0f;
            if (enableForceField) cost += 0.15f;
            if (enableHologram) cost += 0.1f;
            if (enableDissolve) cost += 0.05f;
            return Mathf.Clamp01(cost);
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
    public class GorgonizeToonVisualEffects
    {
        [Header("Surface Effects")]
        public GorgonizeToonRimLighting rimLighting = new GorgonizeToonRimLighting();
        public GorgonizeToonSpecular specular = new GorgonizeToonSpecular();
        public GorgonizeToonMatcap matcap = new GorgonizeToonMatcap();
        
        [Header("Advanced Effects")]
        public GorgonizeToonFresnel fresnel = new GorgonizeToonFresnel();
        public GorgonizeToonSubsurface subsurface = new GorgonizeToonSubsurface();
        public GorgonizeToonOutline outline = new GorgonizeToonOutline();

        [Header("Special Effects")]
        public GorgonizeToonSpecialEffects specialEffects = new GorgonizeToonSpecialEffects();

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
            specialEffects.ApplyToMaterial(material);
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
            count += specialEffects.GetActiveEffectCount();
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
            cost += specialEffects.GetPerformanceCost();
            
            return Mathf.Clamp01(cost);
        }

        /// <summary>
        /// Copies all settings from another visual effects instance
        /// </summary>
        public void CopyFrom(GorgonizeToonVisualEffects other)
        {
            if (other == null) return;
            
            rimLighting.CopyFrom(other.rimLighting);
            specular.CopyFrom(other.specular);
            matcap.CopyFrom(other.matcap);
            fresnel.CopyFrom(other.fresnel);
            subsurface.CopyFrom(other.subsurface);
            outline.CopyFrom(other.outline);
            specialEffects.CopyFrom(other.specialEffects);
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
            specialEffects.ValidateSettings();
        }

        /// <summary>
        /// Creates anime-style visual effects preset
        /// </summary>
        public static GorgonizeToonVisualEffects CreateAnimePreset()
        {
            var effects = new GorgonizeToonVisualEffects();
            
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
        public static GorgonizeToonVisualEffects CreateCartoonPreset()
        {
            var effects = new GorgonizeToonVisualEffects();
            
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
        public static GorgonizeToonVisualEffects CreateRealisticPreset()
        {
            var effects = new GorgonizeToonVisualEffects();
            
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
        public static GorgonizeToonVisualEffects CreateMinimalPreset()
        {
            var effects = new GorgonizeToonVisualEffects();
            
            effects.rimLighting.enableRimLighting = false;
            effects.specular.enableSpecular = false;
            effects.matcap.enableMatcap = false;
            effects.fresnel.enableFresnel = false;
            effects.subsurface.enableSubsurface = false;
            effects.outline.enableOutline = false;
            effects.specialEffects.enableForceField = false;
            effects.specialEffects.enableHologram = false;
            effects.specialEffects.enableDissolve = false;
            
            return effects;
        }
    }
}