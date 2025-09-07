#ifndef GORGONIZE_TOON_UTILITIES_INCLUDED
#define GORGONIZE_TOON_UTILITIES_INCLUDED

#include "GorgonizeToonProperties.hlsl"

// ===== MATHEMATICAL CONSTANTS =====
#define PI 3.14159265359
#define TWO_PI 6.28318530718
#define HALF_PI 1.57079632679
#define INV_PI 0.31830988618
#define EULER 2.71828182846
#define GOLDEN_RATIO 1.61803398875

// ===== COLOR SPACE CONVERSIONS =====

// RGB to HSV conversion
float3 RGBtoHSV(float3 rgb)
{
    float4 k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(rgb.bg, k.wz), float4(rgb.gb, k.xy), step(rgb.b, rgb.g));
    float4 q = lerp(float4(p.xyw, rgb.r), float4(rgb.r, p.yzx), step(p.x, rgb.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

// HSV to RGB conversion
float3 HSVtoRGB(float3 hsv)
{
    float4 k = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(hsv.xxx + k.xyz) * 6.0 - k.www);
    return hsv.z * lerp(k.xxx, saturate(p - k.xxx), hsv.y);
}

// RGB to YUV conversion
float3 RGBtoYUV(float3 rgb)
{
    const float3x3 rgbToYuv = float3x3(
        0.299, 0.587, 0.114,
        -0.147, -0.289, 0.436,
        0.615, -0.515, -0.100
    );
    return mul(rgbToYuv, rgb);
}

// YUV to RGB conversion
float3 YUVtoRGB(float3 yuv)
{
    const float3x3 yuvToRgb = float3x3(
        1.000, 0.000, 1.140,
        1.000, -0.395, -0.581,
        1.000, 2.032, 0.000
    );
    return mul(yuvToRgb, yuv);
}

// Linear to sRGB conversion
float3 LinearTosRGB(float3 linear)
{
    return pow(linear, 1.0 / 2.2);
}

// sRGB to Linear conversion
float3 sRGBToLinear(float3 srgb)
{
    return pow(srgb, 2.2);
}

// ===== ADVANCED COLOR GRADING =====

// Apply comprehensive color grading
float3 ApplyAdvancedColorGrading(float3 color, float hue, float saturation, float brightness, 
                                float contrast, float gamma, float temperature, float tint, float vibrance)
{
    // Gamma correction
    color = pow(max(color, 0.0), gamma);
    
    // Contrast adjustment
    color = ((color - 0.5) * contrast) + 0.5;
    
    // Brightness adjustment
    color = saturate(color * brightness);
    
    // Temperature and tint adjustments
    if (abs(temperature) > 0.001 || abs(tint) > 0.001)
    {
        // Convert to YUV for temperature/tint adjustments
        float3 yuv = RGBtoYUV(color);
        yuv.y += temperature * 0.01;
        yuv.z += tint * 0.01;
        color = YUVtoRGB(yuv);
    }
    
    // Hue shift
    if (abs(hue) > 0.001)
    {
        float3 hsv = RGBtoHSV(color);
        hsv.x += hue / 360.0;
        hsv.x = frac(hsv.x);
        color = HSVtoRGB(hsv);
    }
    
    // Saturation adjustment
    float luminance = dot(color, float3(0.299, 0.587, 0.114));
    color = lerp(luminance.xxx, color, saturation);
    
    // Vibrance adjustment (smart saturation)
    if (abs(vibrance) > 0.001)
    {
        float maxComponent = max(max(color.r, color.g), color.b);
        float minComponent = min(min(color.r, color.g), color.b);
        float saturationAmount = maxComponent - minComponent;
        float vibranceAmount = 1.0 - saturationAmount;
        color = lerp(color, color * (1.0 + vibrance * vibranceAmount), vibranceAmount);
    }
    
    return saturate(color);
}

// Black body radiation for temperature-based emission
float3 BlackBodyRadiation(float temperature)
{
    temperature = clamp(temperature, 1000.0, 12000.0);
    
    float t = temperature / 100.0;
    float3 color;
    
    // Red component
    if (t >= 66.0)
        color.r = 329.698727446 * pow(t - 60.0, -0.1332047592);
    else
        color.r = 1.0;
    
    // Green component
    if (t >= 66.0)
        color.g = 288.1221695283 * pow(t - 60.0, -0.0755148492);
    else
        color.g = 99.4708025861 * log(t) - 161.1195681661;
    
    // Blue component
    if (t >= 66.0)
        color.b = 1.0;
    else if (t >= 19.0)
        color.b = 138.5177312231 * log(t - 10.0) - 305.0447927307;
    else
        color.b = 0.0;
    
    return saturate(color / 255.0);
}

// ===== QUANTIZATION AND POSTERIZATION =====

// Enhanced posterization with dithering
float3 PosterizeWithDithering(float3 color, float levels, float2 screenPos, float ditheringIntensity)
{
    float3 posterized = floor(color * levels) / levels;
    
    #ifdef _DITHERING
    if (ditheringIntensity > 0.001)
    {
        // Blue noise dithering pattern
        float dither = frac(sin(dot(screenPos, float2(12.9898, 78.233))) * 43758.5453);
        dither = (dither - 0.5) * ditheringIntensity / levels;
        posterized += dither;
    }
    #endif
    
    return saturate(posterized);
}

// Quantize lighting with smooth transitions
float QuantizeLightingSmooth(float lightValue, float steps, float smoothness)
{
    float quantized = floor(lightValue * steps) / steps;
    float nextLevel = ceil(lightValue * steps) / steps;
    float blend = smoothstep(0.5 - smoothness, 0.5 + smoothness, frac(lightValue * steps));
    return lerp(quantized, nextLevel, blend);
}

// ===== UV MANIPULATION =====

// Enhanced UV rotation with pivot point
float2 RotateUVAdvanced(float2 uv, float rotation, float2 pivot)
{
    float rad = radians(rotation);
    float cosAngle = cos(rad);
    float sinAngle = sin(rad);
    uv -= pivot;
    float2 rotatedUV;
    rotatedUV.x = uv.x * cosAngle - uv.y * sinAngle;
    rotatedUV.y = uv.x * sinAngle + uv.y * cosAngle;
    return rotatedUV + pivot;
}

// UV scaling with center point
float2 ScaleUV(float2 uv, float2 scale, float2 center)
{
    uv -= center;
    uv *= scale;
    return uv + center;
}

// UV offset with wrapping
float2 OffsetUV(float2 uv, float2 offset, bool wrap)
{
    uv += offset;
    if (wrap)
    {
        uv = frac(uv);
    }
    return uv;
}

// Polar coordinates conversion
float2 CartesianToPolar(float2 cartesian, float2 center)
{
    float2 delta = cartesian - center;
    float radius = length(delta);
    float angle = atan2(delta.y, delta.x);
    return float2(radius, angle);
}

// ===== NOISE FUNCTIONS =====

// Improved Perlin noise
float PerlinNoise(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);
    
    float a = dot(hash22(i + float2(0.0, 0.0)), f - float2(0.0, 0.0));
    float b = dot(hash22(i + float2(1.0, 0.0)), f - float2(1.0, 0.0));
    float c = dot(hash22(i + float2(0.0, 1.0)), f - float2(0.0, 1.0));
    float d = dot(hash22(i + float2(1.0, 1.0)), f - float2(1.0, 1.0));
    
    float2 u = f * f * (3.0 - 2.0 * f);
    return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
}

// Simple hash function for noise
float2 hash22(float2 p)
{
    p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
    return frac(sin(p) * 43758.5453123) * 2.0 - 1.0;
}

// Simplex noise (approximation)
float SimplexNoise(float2 p)
{
    const float K1 = 0.366025404; // (sqrt(3)-1)/2
    const float K2 = 0.211324865; // (3-sqrt(3))/6
    
    float2 i = floor(p + (p.x + p.y) * K1);
    float2 a = p - i + (i.x + i.y) * K2;
    float2 o = step(a.yx, a.xy);
    float2 b = a - o + K2;
    float2 c = a - 1.0 + 2.0 * K2;
    
    float3 h = max(0.5 - float3(dot(a, a), dot(b, b), dot(c, c)), 0.0);
    float3 n = h * h * h * h * float3(dot(a, hash22(i + 0.0)), dot(b, hash22(i + o)), dot(c, hash22(i + 1.0)));
    
    return dot(n, float3(70.0, 70.0, 70.0));
}

// Fractional Brownian Motion
float FBM(float2 p, int octaves, float lacunarity, float gain)
{
    float value = 0.0;
    float amplitude = 0.5;
    float frequency = 1.0;
    
    for (int i = 0; i < octaves; i++)
    {
        value += amplitude * PerlinNoise(p * frequency);
        frequency *= lacunarity;
        amplitude *= gain;
    }
    
    return value;
}

// ===== LIGHTING UTILITIES =====

// Enhanced toon ramp calculation with multiple transition points
float CalculateAdvancedToonRamp(float NdotL, float threshold, float smoothness, int steps)
{
    #ifdef _CEL_SHADING
    if (steps > 1)
    {
        return QuantizeLightingSmooth(NdotL, steps, smoothness);
    }
    #endif
    
    return smoothstep(threshold - smoothness, threshold + smoothness, NdotL);
}

// Enhanced light wrapping with falloff
float ApplyAdvancedLightWrapping(float NdotL, float wrapAmount, float falloff)
{
    float wrapped = (NdotL + wrapAmount) / (1.0 + wrapAmount);
    return pow(saturate(wrapped), falloff);
}

// Fresnel calculation with multiple models
float CalculateFresnelAdvanced(float3 viewDir, float3 normal, float power, int model)
{
    float fresnel = 1.0 - saturate(dot(viewDir, normal));
    
    switch (model)
    {
        case 0: // Standard
            return pow(fresnel, power);
        case 1: // Schlick approximation
            return fresnel + (1.0 - fresnel) * pow(1.0 - dot(viewDir, normal), 5.0);
        case 2: // Spherical Gaussian
            return exp2((-5.55473 * dot(viewDir, normal) - 6.98316) * dot(viewDir, normal));
        default:
            return pow(fresnel, power);
    }
}

// Enhanced rim lighting with multiple layers
float3 CalculateMultiLayerRimLighting(float3 viewDir, float3 normal, float3 lightDir, 
                                     float3 rimColor1, float power1, float intensity1, float threshold1,
                                     float3 rimColor2, float power2, float intensity2, float threshold2)
{
    float rim1 = CalculateRimLighting(viewDir, normal, power1, threshold1) * intensity1;
    float rim2 = CalculateRimLighting(viewDir, normal, power2, threshold2) * intensity2;
    
    // Light-dependent rim enhancement
    float lightInfluence = saturate(dot(normal, lightDir)) * 0.5 + 0.5;
    
    return (rim1 * rimColor1 + rim2 * rimColor2) * lightInfluence;
}

// Standard rim lighting calculation
float CalculateRimLighting(float3 viewDir, float3 normal, float power, float threshold)
{
    float rim = 1.0 - saturate(dot(viewDir, normal));
    rim = smoothstep(threshold, 1.0, rim);
    return pow(rim, power);
}

// ===== SPECULAR CALCULATIONS =====

// Anisotropic specular highlight
float CalculateAnisotropicSpecular(float3 lightDir, float3 viewDir, float3 normal, 
                                  float3 tangent, float roughnessX, float roughnessY)
{
    float3 halfVector = normalize(lightDir + viewDir);
    float3 bitangent = cross(normal, tangent);
    
    float NdotH = saturate(dot(normal, halfVector));
    float TdotH = dot(tangent, halfVector);
    float BdotH = dot(bitangent, halfVector);
    
    float roughnessX2 = roughnessX * roughnessX;
    float roughnessY2 = roughnessY * roughnessY;
    
    float exponent = -2.0 * ((TdotH * TdotH) / roughnessX2 + (BdotH * BdotH) / roughnessY2) / (1.0 + NdotH);
    
    return exp(exponent) / (PI * roughnessX * roughnessY * NdotH * NdotH * NdotH * NdotH);
}

// Stepped specular calculation
float CalculateSteppedSpecular(float3 lightDir, float3 viewDir, float3 normal, 
                              float size, float smoothness, float steps)
{
    float3 halfVector = normalize(lightDir + viewDir);
    float NdotH = saturate(dot(normal, halfVector));
    float specular = pow(NdotH, (1.0 - size) * 128.0);
    
    // Apply stepping
    specular = floor(specular * steps) / steps;
    
    // Apply smoothness
    return smoothstep(0.5 - smoothness, 0.5 + smoothness, specular);
}

// Standard toon specular
float CalculateToonSpecular(float3 lightDir, float3 viewDir, float3 normal, float size, float smoothness)
{
    float3 halfVector = normalize(lightDir + viewDir);
    float NdotH = saturate(dot(normal, halfVector));
    float specular = pow(NdotH, (1.0 - size) * 128.0);
    return smoothstep(0.5 - smoothness, 0.5 + smoothness, specular);
}

// ===== ANIMATION FUNCTIONS =====

// Smooth pulse animation
float PulseAnimation(float time, float frequency, float sharpness)
{
    float t = frac(time * frequency);
    return pow(sin(t * PI), sharpness);
}

// Breathing animation with customizable curve
float BreathingAnimation(float time, float speed, float intensity, float curve)
{
    return 1.0 + pow(sin(time * speed), curve) * intensity;
}

// Wave animation for vertex displacement
float3 WaveAnimation(float3 worldPos, float time, float frequency, float amplitude, float3 direction)
{
    float wave = sin(dot(worldPos, direction) * frequency + time) * amplitude;
    return worldPos + direction * wave;
}

// Noise-based animation
float NoiseAnimation(float2 position, float time, float scale, float speed)
{
    return PerlinNoise((position + time * speed) * scale);
}

// ===== DISTANCE AND FADE CALCULATIONS =====

// Calculate LOD fade based on distance
float CalculateLODFade(float3 worldPos, float fadeDistance)
{
    float distance = length(worldPos - _WorldSpaceCameraPos);
    return 1.0 - saturate(distance / fadeDistance);
}

// Distance-based detail fade
float CalculateDetailFade(float3 worldPos, float3 cameraPos, float fadeStart, float fadeEnd)
{
    float distance = length(worldPos - cameraPos);
    return 1.0 - saturate((distance - fadeStart) / (fadeEnd - fadeStart));
}

// Screen space fade for effects
float CalculateScreenSpaceFade(float4 screenPos, float fadeWidth)
{
    float2 screenUV = screenPos.xy / screenPos.w;
    float2 fade = min(screenUV, 1.0 - screenUV);
    float fadeFactor = min(fade.x, fade.y) / fadeWidth;
    return saturate(fadeFactor);
}

// ===== HATCHING UTILITIES =====

// Advanced hatching calculation with multiple layers
float CalculateAdvancedHatching(float2 uv, float lightValue, float4 screenPos, float time)
{
    float hatching = 1.0;
    
    #ifdef _HATCHING
        // Primary hatching layer
        float2 hatchUV1 = RotateUVAdvanced(uv * _HatchingDensity, _HatchingRotation, float2(0.5, 0.5));
        
        #ifdef _HATCHING_ANIMATED
        if (_HatchingAnimSpeed > 0.001)
        {
            float animOffset = time * _HatchingAnimSpeed;
            hatchUV1 += float2(sin(animOffset), cos(animOffset)) * 0.02;
        }
        #endif
        
        float hatch1 = GORGONIZE_TOON_SAMPLE_2D(_HatchingTex, sampler_HatchingTex, hatchUV1).r;
        
        if (lightValue < _HatchingThreshold)
        {
            hatching *= lerp(1.0, hatch1, _HatchingIntensity);
        }
        
        // Cross hatching layer
        if (lightValue < _CrossHatchingThreshold)
        {
            float2 crossHatchUV = RotateUVAdvanced(uv * _HatchingDensity, _HatchingRotation + 90, float2(0.5, 0.5));
            float crossHatch = GORGONIZE_TOON_SAMPLE_2D(_CrossHatchingTex, sampler_CrossHatchingTex, crossHatchUV).r;
            hatching *= lerp(1.0, crossHatch, _HatchingIntensity);
        }
        
        // Secondary hatching layer (for ultra quality)
        #if GORGONIZE_HAS_EXPENSIVE_EFFECTS
        if (lightValue < _SecondaryHatchingThreshold)
        {
            float2 secondaryHatchUV = RotateUVAdvanced(uv * _HatchingDensity * 1.5, _HatchingRotation + 45, float2(0.5, 0.5));
            float secondaryHatch = GORGONIZE_TOON_SAMPLE_2D(_HatchingTex2, sampler_HatchingTex2, secondaryHatchUV).r;
            hatching *= lerp(1.0, secondaryHatch, _HatchingIntensity * 0.5);
        }
        #endif
    #endif
    
    #ifdef _SCREEN_SPACE_HATCHING
        // Screen space hatching
        float2 screenUV = screenPos.xy / screenPos.w;
        screenUV *= _ScreenParams.xy / _ScreenHatchScale;
        
        float screenHatch = sin(screenUV.x * PI) * sin(screenUV.y * PI);
        screenHatch = saturate(screenHatch + _ScreenHatchBias);
        
        if (lightValue < _HatchingThreshold)
        {
            hatching *= lerp(1.0, screenHatch, _HatchingIntensity);
        }
    #endif
    
    return hatching;
}

// ===== MATCAP UTILITIES =====

// Advanced matcap calculation with perspective correction
float3 CalculateAdvancedMatcap(float3 normalWS, float3 viewDirWS, float3 positionWS, 
                              float intensity, float blendMode, float rotation)
{
    float3 matcap = 0;
    
    #ifdef _MATCAP
        float3 normalVS;
        
        #ifdef _MATCAP_PERSPECTIVE_CORRECTION
            // Perspective-corrected matcap
            float3 upVector = float3(0, 1, 0);
            float3 rightVector = normalize(cross(viewDirWS, upVector));
            upVector = cross(rightVector, viewDirWS);
            
            float3x3 viewMatrix = float3x3(rightVector, upVector, viewDirWS);
            normalVS = mul(viewMatrix, normalWS);
        #else
            // Standard view space normal
            normalVS = mul((float3x3)UNITY_MATRIX_V, normalWS);
        #endif
        
        float2 matcapUV = normalVS.xy * 0.5 + 0.5;
        
        // Apply rotation if specified
        if (abs(rotation) > 0.001)
        {
            matcapUV = RotateUVAdvanced(matcapUV, rotation, float2(0.5, 0.5));
        }
        
        float3 matcapColor = GORGONIZE_TOON_SAMPLE_2D(_MatcapTex, sampler_MatcapTex, matcapUV).rgb;
        
        // Apply blend mode
        if (blendMode < 0.5) // Add
        {
            matcap = matcapColor * intensity;
        }
        else if (blendMode < 1.5) // Multiply
        {
            matcap = lerp(float3(1, 1, 1), matcapColor, intensity);
        }
        else if (blendMode < 2.5) // Screen
        {
            matcap = lerp(float3(0, 0, 0), matcapColor, intensity);
        }
        else // Overlay
        {
            float3 base = float3(0.5, 0.5, 0.5);
            matcap = lerp(base, matcapColor < 0.5 ? 2.0 * matcapColor * base : 1.0 - 2.0 * (1.0 - matcapColor) * (1.0 - base), intensity);
        }
    #endif
    
    return matcap;
}

// ===== SUBSURFACE SCATTERING =====

// Enhanced subsurface scattering calculation
float3 CalculateAdvancedSubsurface(float3 lightDir, float3 viewDir, float3 normal, 
                                  float3 subsurfaceColor, float power, float intensity, 
                                  float distortion, float thickness)
{
    float3 subsurface = 0;
    
    #ifdef _SUBSURFACE
        // Distorted light direction for subsurface effect
        float3 distortedLightDir = lightDir + normal * distortion;
        
        // Calculate subsurface term
        float subsurfaceTerm = pow(saturate(dot(viewDir, -distortedLightDir)), power);
        
        // Apply thickness attenuation
        subsurfaceTerm *= thickness;
        
        subsurface = subsurfaceTerm * subsurfaceColor * intensity;
    #endif
    
    return subsurface;
}

// ===== DEBUG VISUALIZATION =====

// Apply debug visualization modes
float3 ApplyDebugVisualization(float3 color, ToonSurfaceData surfaceData, ToonLightingData lightingData, float debugMode)
{
    #ifdef _DEBUG_MODE
    if (debugMode > 0.5)
    {
        if (debugMode < 1.5) // Normals
        {
            return surfaceData.normalWS * 0.5 + 0.5;
        }
        else if (debugMode < 2.5) // Lighting
        {
            Light mainLight = GetMainLight(lightingData.shadowCoord);
            float NdotL = saturate(dot(surfaceData.normalWS, mainLight.direction));
            return NdotL.xxx;
        }
        else if (debugMode < 3.5) // Shadows
        {
            Light mainLight = GetMainLight(lightingData.shadowCoord);
            return mainLight.shadowAttenuation.xxx;
        }
        else if (debugMode < 4.5) // Hatching
        {
            Light mainLight = GetMainLight(lightingData.shadowCoord);
            float NdotL = saturate(dot(surfaceData.normalWS, mainLight.direction));
            float lightValue = CalculateAdvancedToonRamp(NdotL, _ShadowThreshold, _ShadowSmoothness, _CelShadingSteps);
            float hatching = CalculateAdvancedHatching(lightingData.screenPos.xy / lightingData.screenPos.w, lightValue, lightingData.screenPos, _Time.y);
            return hatching.xxx;
        }
    }
    #endif
    
    return color;
}

// Wireframe visualization
float3 ApplyWireframe(float3 color, float3 worldPos, float3 wireframeColor, float thickness)
{
    #ifdef _WIREFRAME
        // Simple wireframe based on world position
        float3 derivative = fwidth(worldPos);
        float wireframe = min(min(derivative.x, derivative.y), derivative.z);
        wireframe = 1.0 - smoothstep(0.0, thickness, wireframe);
        return lerp(color, wireframeColor, wireframe);
    #endif
    
    return color;
}

// ===== UTILITY FUNCTIONS =====

// Safe normalize that handles zero vectors
float3 SafeNormalize(float3 vector)
{
    float length = max(dot(vector, vector), 1e-6);
    return vector * rsqrt(length);
}

// Smooth minimum function for blending
float SmoothMin(float a, float b, float k)
{
    float h = max(k - abs(a - b), 0.0) / k;
    return min(a, b) - h * h * h * k * (1.0 / 6.0);
}

// Smooth maximum function
float SmoothMax(float a, float b, float k)
{
    return -SmoothMin(-a, -b, k);
}

// Remap function
float Remap(float value, float inMin, float inMax, float outMin, float outMax)
{
    return outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);
}

// Clamp with smooth falloff
float SoftClamp(float value, float min, float max, float softness)
{
    if (value < min)
        return min - (min - value) / (1.0 + (min - value) / softness);
    else if (value > max)
        return max + (value - max) / (1.0 + (value - max) / softness);
    else
        return value;
}

// Exponential falloff
float ExponentialFalloff(float distance, float falloffStart, float falloffEnd)
{
    if (distance <= falloffStart)
        return 1.0;
    
    float normalizedDistance = (distance - falloffStart) / (falloffEnd - falloffStart);
    return exp(-normalizedDistance * normalizedDistance);
}

// ===== PERFORMANCE OPTIMIZATIONS =====

// Quality level branching
bool ShouldUseHighQualityFeature()
{
    return _QualityLevel >= QUALITY_HIGH;
}

bool ShouldUseUltraQualityFeature()
{
    return _QualityLevel >= QUALITY_ULTRA;
}

// LOD-based feature culling
bool ShouldCullDetailAtDistance(float3 worldPos, float cullDistance)
{
    float distance = length(worldPos - _WorldSpaceCameraPos);
    return distance > cullDistance;
}

#endif