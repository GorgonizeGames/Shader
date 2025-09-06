using UnityEngine;
using Gorgonize.ToonShader.Core;

namespace Gorgonize.ToonShader.Settings
{
    /// <summary>
    /// Animation settings for the Ultimate Toon Shader
    /// Handles runtime animations for various shader properties
    /// </summary>
    [System.Serializable]
    public class ToonAnimationSettings
    {
        [Header("Rim Animation")]
        [Tooltip("Animate rim lighting intensity over time")]
        public bool animateRimLighting = false;
        
        [Range(0.1f, 5f)]
        [Tooltip("Speed of rim lighting animation")]
        public float rimAnimationSpeed = 2f;
        
        [Tooltip("Animation curve for rim intensity modulation")]
        public AnimationCurve rimAnimationCurve = AnimationCurve.EaseInOut(0, 0.5f, 1, 1.5f);
        
        [Header("Emission Animation")]
        [Tooltip("Animate emission intensity over time")]
        public bool animateEmission = false;
        
        [Range(0.1f, 3f)]
        [Tooltip("Speed of emission animation")]
        public float emissionAnimationSpeed = 1f;
        
        [Tooltip("Animation curve for emission intensity")]
        public AnimationCurve emissionAnimationCurve = AnimationCurve.EaseInOut(0, 0.3f, 1, 1f);
        
        [Header("Color Animation")]
        [Tooltip("Animate hue shift over time")]
        public bool animateHue = false;
        
        [Range(0.1f, 2f)]
        [Tooltip("Speed of hue animation")]
        public float hueAnimationSpeed = 0.5f;
        
        [Header("Hatching Animation")]
        [Tooltip("Animate hatching rotation")]
        public bool animateHatching = false;
        
        [Range(0.1f, 1f)]
        [Tooltip("Speed of hatching animation")]
        public float hatchingAnimationSpeed = 0.3f;
        
        [Header("Breathing Effect")]
        [Tooltip("Enable subtle breathing-like pulsing effect")]
        public bool enableBreathingEffect = false;
        
        [Range(0.5f, 3f)]
        [Tooltip("Speed of breathing effect")]
        public float breathingSpeed = 1f;
        
        [Range(0.05f, 0.3f)]
        [Tooltip("Intensity of breathing effect")]
        public float breathingIntensity = 0.1f;
        
        [Header("Texture Scrolling")]
        [Tooltip("Enable emission texture scrolling")]
        public bool enableEmissionScrolling = false;
        
        [Tooltip("Scroll speed for emission texture (X,Y)")]
        public Vector2 emissionScrollSpeed = Vector2.zero;

        /// <summary>
        /// Updates animated properties based on current time
        /// </summary>
        /// <param name="material">Target material to animate</param>
        /// <param name="deltaTime">Time since last update</param>
        /// <param name="totalTime">Total elapsed time</param>
        /// <param name="baseRimIntensity">Base rim intensity to modulate</param>
        /// <param name="baseEmissionIntensity">Base emission intensity to modulate</param>
        /// <param name="baseHatchingRotation">Base hatching rotation to modulate</param>
        public void UpdateAnimations(Material material, float deltaTime, float totalTime, 
                                   float baseRimIntensity, float baseEmissionIntensity, float baseHatchingRotation)
        {
            if (material == null) return;

            // Rim lighting animation
            if (animateRimLighting)
            {
                float animTime = totalTime * rimAnimationSpeed;
                float curveValue = rimAnimationCurve.Evaluate(animTime % 1f);
                float animatedIntensity = baseRimIntensity * curveValue;
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.RimIntensity, animatedIntensity);
            }

            // Emission animation
            if (animateEmission)
            {
                float animTime = totalTime * emissionAnimationSpeed;
                float curveValue = emissionAnimationCurve.Evaluate(animTime % 1f);
                float animatedIntensity = baseEmissionIntensity * curveValue;
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.EmissionIntensity, animatedIntensity);
            }

            // Hue animation
            if (animateHue)
            {
                float animatedHue = (totalTime * hueAnimationSpeed * 360f) % 360f - 180f;
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.Hue, animatedHue);
            }

            // Hatching animation
            if (animateHatching)
            {
                float animTime = totalTime * hatchingAnimationSpeed;
                float rotationOffset = Mathf.Sin(animTime) * 15f; // Oscillate ±15 degrees
                float animatedRotation = baseHatchingRotation + rotationOffset;
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.HatchingRotation, animatedRotation);
            }

            // Breathing effect
            if (enableBreathingEffect)
            {
                float breathingValue = 1f + Mathf.Sin(totalTime * breathingSpeed) * breathingIntensity;
                float currentRimIntensity = baseRimIntensity;
                
                // If rim animation is also active, get the current animated value
                if (animateRimLighting)
                {
                    float animTime = totalTime * rimAnimationSpeed;
                    float curveValue = rimAnimationCurve.Evaluate(animTime % 1f);
                    currentRimIntensity = baseRimIntensity * curveValue;
                }
                
                ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.RimIntensity, currentRimIntensity * breathingValue);
            }

            // Emission scrolling
            if (enableEmissionScrolling && emissionScrollSpeed != Vector2.zero)
            {
                Vector4 scrollVector = new Vector4(emissionScrollSpeed.x, emissionScrollSpeed.y, 0, 0);
                ToonShaderProperties.SetVectorSafe(material, ToonShaderProperties.EmissionScrollSpeed, scrollVector);
            }
        }

        /// <summary>
        /// Resets all animations to their base state
        /// </summary>
        /// <param name="material">Target material</param>
        /// <param name="baseRimIntensity">Base rim intensity</param>
        /// <param name="baseEmissionIntensity">Base emission intensity</param>
        /// <param name="baseHatchingRotation">Base hatching rotation</param>
        /// <param name="baseHue">Base hue value</param>
        public void ResetAnimations(Material material, float baseRimIntensity, float baseEmissionIntensity, 
                                  float baseHatchingRotation, float baseHue)
        {
            if (material == null) return;

            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.RimIntensity, baseRimIntensity);
            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.EmissionIntensity, baseEmissionIntensity);
            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.HatchingRotation, baseHatchingRotation);
            ToonShaderProperties.SetFloatSafe(material, ToonShaderProperties.Hue, baseHue);
        }

        /// <summary>
        /// Returns whether any animations are currently active
        /// </summary>
        public bool HasActiveAnimations()
        {
            return animateRimLighting || animateEmission || animateHue || 
                   animateHatching || enableBreathingEffect || enableEmissionScrolling;
        }

        /// <summary>
        /// Returns the number of active animations
        /// </summary>
        public int GetActiveAnimationCount()
        {
            int count = 0;
            if (animateRimLighting) count++;
            if (animateEmission) count++;
            if (animateHue) count++;
            if (animateHatching) count++;
            if (enableBreathingEffect) count++;
            if (enableEmissionScrolling) count++;
            return count;
        }

        /// <summary>
        /// Estimates performance cost of active animations
        /// </summary>
        public float GetPerformanceCost()
        {
            if (!HasActiveAnimations()) return 0f;

            float cost = 0.05f; // Base cost for animation system
            
            if (animateRimLighting) cost += 0.02f;
            if (animateEmission) cost += 0.02f;
            if (animateHue) cost += 0.03f;
            if (animateHatching) cost += 0.02f;
            if (enableBreathingEffect) cost += 0.01f;
            if (enableEmissionScrolling) cost += 0.01f;
            
            return Mathf.Clamp01(cost);
        }

        /// <summary>
        /// Validates all animation settings
        /// </summary>
        public void ValidateSettings()
        {
            rimAnimationSpeed = Mathf.Clamp(rimAnimationSpeed, 0.1f, 5f);
            emissionAnimationSpeed = Mathf.Clamp(emissionAnimationSpeed, 0.1f, 3f);
            hueAnimationSpeed = Mathf.Clamp(hueAnimationSpeed, 0.1f, 2f);
            hatchingAnimationSpeed = Mathf.Clamp(hatchingAnimationSpeed, 0.1f, 1f);
            breathingSpeed = Mathf.Clamp(breathingSpeed, 0.5f, 3f);
            breathingIntensity = Mathf.Clamp(breathingIntensity, 0.05f, 0.3f);

            // Ensure animation curves are not null
            if (rimAnimationCurve == null)
                rimAnimationCurve = AnimationCurve.EaseInOut(0, 0.5f, 1, 1.5f);
            
            if (emissionAnimationCurve == null)
                emissionAnimationCurve = AnimationCurve.EaseInOut(0, 0.3f, 1, 1f);
        }

        /// <summary>
        /// Creates a preset for gentle animations
        /// </summary>
        public static ToonAnimationSettings CreateGentlePreset()
        {
            return new ToonAnimationSettings
            {
                enableBreathingEffect = true,
                breathingSpeed = 1f,
                breathingIntensity = 0.1f,
                animateHue = false,
                animateRimLighting = false,
                animateEmission = false,
                animateHatching = false
            };
        }

        /// <summary>
        /// Creates a preset for dynamic animations
        /// </summary>
        public static ToonAnimationSettings CreateDynamicPreset()
        {
            return new ToonAnimationSettings
            {
                animateRimLighting = true,
                rimAnimationSpeed = 1.5f,
                animateEmission = true,
                emissionAnimationSpeed = 1.2f,
                enableBreathingEffect = true,
                breathingSpeed = 1.5f,
                breathingIntensity = 0.15f,
                animateHue = false,
                animateHatching = false
            };
        }

        /// <summary>
        /// Creates a preset for psychedelic animations
        /// </summary>
        public static ToonAnimationSettings CreatePsychedelicPreset()
        {
            return new ToonAnimationSettings
            {
                animateRimLighting = true,
                rimAnimationSpeed = 2.5f,
                animateEmission = true,
                emissionAnimationSpeed = 2f,
                animateHue = true,
                hueAnimationSpeed = 1f,
                enableBreathingEffect = true,
                breathingSpeed = 2f,
                breathingIntensity = 0.2f,
                animateHatching = true,
                hatchingAnimationSpeed = 0.5f
            };
        }

        /// <summary>
        /// Creates a preset with no animations
        /// </summary>
        public static ToonAnimationSettings CreateStaticPreset()
        {
            return new ToonAnimationSettings
            {
                animateRimLighting = false,
                animateEmission = false,
                animateHue = false,
                animateHatching = false,
                enableBreathingEffect = false,
                enableEmissionScrolling = false
            };
        }

        /// <summary>
        /// Copies settings from another animation settings instance
        /// </summary>
        public void CopyFrom(ToonAnimationSettings other)
        {
            if (other == null) return;

            animateRimLighting = other.animateRimLighting;
            rimAnimationSpeed = other.rimAnimationSpeed;
            rimAnimationCurve = other.rimAnimationCurve;
            
            animateEmission = other.animateEmission;
            emissionAnimationSpeed = other.emissionAnimationSpeed;
            emissionAnimationCurve = other.emissionAnimationCurve;
            
            animateHue = other.animateHue;
            hueAnimationSpeed = other.hueAnimationSpeed;
            
            animateHatching = other.animateHatching;
            hatchingAnimationSpeed = other.hatchingAnimationSpeed;
            
            enableBreathingEffect = other.enableBreathingEffect;
            breathingSpeed = other.breathingSpeed;
            breathingIntensity = other.breathingIntensity;
            
            enableEmissionScrolling = other.enableEmissionScrolling;
            emissionScrollSpeed = other.emissionScrollSpeed;
        }
    }
}