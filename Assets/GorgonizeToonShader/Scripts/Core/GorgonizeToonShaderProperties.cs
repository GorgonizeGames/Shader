using UnityEngine;

namespace Gorgonize.ToonShader.Core
{
    /// <summary>
    /// Centralized shader property ID cache system for performance optimization
    /// Contains all shader property IDs used by the Ultimate Toon Shader
    /// </summary>
    public static class ToonShaderProperties
    {
        #region Base Properties
        public static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        public static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");
        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        #endregion

        #region Lighting Properties
        public static readonly int ShadowThreshold = Shader.PropertyToID("_ShadowThreshold");
        public static readonly int ShadowSmoothness = Shader.PropertyToID("_ShadowSmoothness");
        public static readonly int ShadowColor = Shader.PropertyToID("_ShadowColor");
        public static readonly int ShadowIntensity = Shader.PropertyToID("_ShadowIntensity");
        public static readonly int LightRampTex = Shader.PropertyToID("_LightRampTex");
        public static readonly int UseRampTexture = Shader.PropertyToID("_UseRampTexture");
        public static readonly int IndirectLightingBoost = Shader.PropertyToID("_IndirectLightingBoost");
        public static readonly int AmbientOcclusion = Shader.PropertyToID("_AmbientOcclusion");
        public static readonly int LightWrapping = Shader.PropertyToID("_LightWrapping");
        #endregion

        #region Rim Lighting Properties
        public static readonly int EnableRimLighting = Shader.PropertyToID("_EnableRimLighting");
        public static readonly int RimColor = Shader.PropertyToID("_RimColor");
        public static readonly int RimPower = Shader.PropertyToID("_RimPower");
        public static readonly int RimIntensity = Shader.PropertyToID("_RimIntensity");
        public static readonly int RimThreshold = Shader.PropertyToID("_RimThreshold");
        #endregion

        #region Specular Properties
        public static readonly int EnableSpecular = Shader.PropertyToID("_EnableSpecular");
        public static readonly int SpecularColor = Shader.PropertyToID("_SpecularColor");
        public static readonly int SpecularSize = Shader.PropertyToID("_SpecularSize");
        public static readonly int SpecularSmoothness = Shader.PropertyToID("_SpecularSmoothness");
        public static readonly int SpecularIntensity = Shader.PropertyToID("_SpecularIntensity");
        #endregion

        #region Hatching Properties
        public static readonly int EnableHatching = Shader.PropertyToID("_EnableHatching");
        public static readonly int HatchingTex = Shader.PropertyToID("_HatchingTex");
        public static readonly int CrossHatchingTex = Shader.PropertyToID("_CrossHatchingTex");
        public static readonly int HatchingDensity = Shader.PropertyToID("_HatchingDensity");
        public static readonly int HatchingIntensity = Shader.PropertyToID("_HatchingIntensity");
        public static readonly int HatchingThreshold = Shader.PropertyToID("_HatchingThreshold");
        public static readonly int CrossHatchingThreshold = Shader.PropertyToID("_CrossHatchingThreshold");
        public static readonly int HatchingRotation = Shader.PropertyToID("_HatchingRotation");
        public static readonly int EnableScreenSpaceHatching = Shader.PropertyToID("_EnableScreenSpaceHatching");
        public static readonly int ScreenHatchScale = Shader.PropertyToID("_ScreenHatchScale");
        public static readonly int ScreenHatchBias = Shader.PropertyToID("_ScreenHatchBias");
        #endregion

        #region Outline Properties
        public static readonly int EnableOutline = Shader.PropertyToID("_EnableOutline");
        public static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
        public static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        #endregion

        #region Matcap Properties
        public static readonly int EnableMatcap = Shader.PropertyToID("_EnableMatcap");
        public static readonly int MatcapTex = Shader.PropertyToID("_MatcapTex");
        public static readonly int MatcapIntensity = Shader.PropertyToID("_MatcapIntensity");
        public static readonly int MatcapBlendMode = Shader.PropertyToID("_MatcapBlendMode");
        #endregion

        #region Normal Mapping Properties
        public static readonly int EnableNormalMap = Shader.PropertyToID("_EnableNormalMap");
        public static readonly int BumpMap = Shader.PropertyToID("_BumpMap");
        public static readonly int BumpScale = Shader.PropertyToID("_BumpScale");
        #endregion

        #region Detail Properties
        public static readonly int EnableDetail = Shader.PropertyToID("_EnableDetail");
        public static readonly int DetailMap = Shader.PropertyToID("_DetailMap");
        public static readonly int DetailNormalMap = Shader.PropertyToID("_DetailNormalMap");
        public static readonly int DetailScale = Shader.PropertyToID("_DetailScale");
        public static readonly int DetailNormalScale = Shader.PropertyToID("_DetailNormalScale");
        #endregion

        #region Emission Properties
        public static readonly int EnableEmission = Shader.PropertyToID("_EnableEmission");
        public static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");
        public static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        public static readonly int EmissionIntensity = Shader.PropertyToID("_EmissionIntensity");
        public static readonly int EmissionScrollSpeed = Shader.PropertyToID("_EmissionScrollSpeed");
        #endregion

        #region Fresnel Properties
        public static readonly int EnableFresnel = Shader.PropertyToID("_EnableFresnel");
        public static readonly int FresnelColor = Shader.PropertyToID("_FresnelColor");
        public static readonly int FresnelPower = Shader.PropertyToID("_FresnelPower");
        public static readonly int FresnelIntensity = Shader.PropertyToID("_FresnelIntensity");
        #endregion

        #region Subsurface Properties
        public static readonly int EnableSubsurface = Shader.PropertyToID("_EnableSubsurface");
        public static readonly int SubsurfaceColor = Shader.PropertyToID("_SubsurfaceColor");
        public static readonly int SubsurfacePower = Shader.PropertyToID("_SubsurfacePower");
        public static readonly int SubsurfaceIntensity = Shader.PropertyToID("_SubsurfaceIntensity");
        #endregion

        #region Color Grading Properties
        public static readonly int Hue = Shader.PropertyToID("_Hue");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        #endregion

        #region Stylization Properties
        public static readonly int EnablePosterize = Shader.PropertyToID("_EnablePosterize");
        public static readonly int PosterizeLevels = Shader.PropertyToID("_PosterizeLevels");
        public static readonly int EnableCelShading = Shader.PropertyToID("_EnableCelShading");
        public static readonly int CelShadingSteps = Shader.PropertyToID("_CelShadingSteps");
        #endregion

        #region Advanced Properties
        public static readonly int Cutoff = Shader.PropertyToID("_Cutoff");
        public static readonly int Cull = Shader.PropertyToID("_Cull");
        public static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
        public static readonly int ZTest = Shader.PropertyToID("_ZTest");
        #endregion

        #region Shader Keywords
        public static class Keywords
        {
            public const string RimLighting = "_RIM_LIGHTING";
            public const string Specular = "_SPECULAR";
            public const string Hatching = "_HATCHING";
            public const string ScreenSpaceHatching = "_SCREEN_SPACE_HATCHING";
            public const string Outline = "_OUTLINE";
            public const string Matcap = "_MATCAP";
            public const string NormalMap = "_NORMALMAP";
            public const string Detail = "_DETAIL";
            public const string Emission = "_EMISSION";
            public const string Fresnel = "_FRESNEL";
            public const string Subsurface = "_SUBSURFACE";
            public const string UseRampTexture = "_USE_RAMP_TEXTURE";
            public const string Posterize = "_POSTERIZE";
            public const string CelShading = "_CEL_SHADING";
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Safely sets a keyword on the material
        /// </summary>
        /// <param name="material">Target material</param>
        /// <param name="keyword">Keyword to set</param>
        /// <param name="enabled">Enable or disable the keyword</param>
        public static void SetKeywordSafe(Material material, string keyword, bool enabled)
        {
            if (material == null) return;

            if (enabled)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }

        /// <summary>
        /// Safely sets a float property on the material
        /// </summary>
        /// <param name="material">Target material</param>
        /// <param name="propertyID">Property ID</param>
        /// <param name="value">Value to set</param>
        public static void SetFloatSafe(Material material, int propertyID, float value)
        {
            if (material == null || !material.HasProperty(propertyID)) return;
            material.SetFloat(propertyID, value);
        }

        /// <summary>
        /// Safely sets a color property on the material
        /// </summary>
        /// <param name="material">Target material</param>
        /// <param name="propertyID">Property ID</param>
        /// <param name="color">Color to set</param>
        public static void SetColorSafe(Material material, int propertyID, Color color)
        {
            if (material == null || !material.HasProperty(propertyID)) return;
            material.SetColor(propertyID, color);
        }

        /// <summary>
        /// Safely sets a texture property on the material
        /// </summary>
        /// <param name="material">Target material</param>
        /// <param name="propertyID">Property ID</param>
        /// <param name="texture">Texture to set</param>
        public static void SetTextureSafe(Material material, int propertyID, Texture texture)
        {
            if (material == null || !material.HasProperty(propertyID)) return;
            material.SetTexture(propertyID, texture);
        }

        /// <summary>
        /// Safely sets a vector property on the material
        /// </summary>
        /// <param name="material">Target material</param>
        /// <param name="propertyID">Property ID</param>
        /// <param name="vector">Vector to set</param>
        public static void SetVectorSafe(Material material, int propertyID, Vector4 vector)
        {
            if (material == null || !material.HasProperty(propertyID)) return;
            material.SetVector(propertyID, vector);
        }
        #endregion
    }
}