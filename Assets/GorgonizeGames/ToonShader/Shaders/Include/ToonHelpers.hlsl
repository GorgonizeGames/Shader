#ifndef TOON_HELPERS_INCLUDED
#define TOON_HELPERS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// Unity 6'da SafeNormalize doğrudan Core.hlsl'de tanımlı
// Kendi versiyonumuzu kullanmaya gerek yok, sadece wrapper yapalım
#ifndef SafeNormalize
    #define SafeNormalize(inVec) SafeNormalize(inVec)
#endif

// Simple noise function for procedural effects
half SimpleNoise(float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}

// Improved noise function with better distribution
half ImprovedNoise(float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    
    // Four corners of a tile
    half a = SimpleNoise(i);
    half b = SimpleNoise(i + float2(1.0, 0.0));
    half c = SimpleNoise(i + float2(0.0, 1.0));
    half d = SimpleNoise(i + float2(1.0, 1.0));
    
    // Smooth interpolation
    float2 u = f * f * (3.0 - 2.0 * f);
    
    return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

// Generate dither pattern for shadow transitions
half GenerateDitherPattern(float2 uv)
{
    // Bayer matrix 4x4 dithering
    const float bayerMatrix[16] = {
        0.0, 8.0, 2.0, 10.0,
        12.0, 4.0, 14.0, 6.0,
        3.0, 11.0, 1.0, 9.0,
        15.0, 7.0, 13.0, 5.0
    };
    
    int2 matrixPos = int2(fmod(uv, 4.0));
    int index = matrixPos.y * 4 + matrixPos.x;
    return bayerMatrix[index] / 16.0;
}

// Smoothstep with adjustable edge sharpness
half SmoothStep(half edge0, half edge1, half x, half sharpness)
{
    half t = saturate((x - edge0) / (edge1 - edge0));
    return pow(t, sharpness);
}

// Remap function to transform values from one range to another
half Remap(half value, half from1, half to1, half from2, half to2)
{
    return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
}

// Luminance calculation for color intensity
half Luminance(half3 color)
{
    return dot(color, half3(0.299, 0.587, 0.114));
}

// Convert RGB to HSV
half3 RGBtoHSV(half3 c)
{
    half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
    half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));
    
    half d = q.x - min(q.w, q.y);
    half e = 1.0e-10;
    return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

// Convert HSV to RGB
half3 HSVtoRGB(half3 c)
{
    half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
}

// Quantize color to specific number of levels
half3 QuantizeColor(half3 color, int levels)
{
    return floor(color * (float)levels) / (float)(levels - 1);
}

// Distance-based fade calculation
half CalculateDistanceFade(float3 worldPos, half fadeStart, half fadeEnd)
{
    half distance = length(_WorldSpaceCameraPos - worldPos);
    return saturate((fadeEnd - distance) / (fadeEnd - fadeStart));
}

// Screen-space position calculation
float2 GetScreenSpaceUV(float4 clipPos)
{
    float2 screenUV = clipPos.xy / clipPos.w;
    screenUV = screenUV * 0.5 + 0.5;
    
    #if UNITY_UV_STARTS_AT_TOP
        screenUV.y = 1.0 - screenUV.y;
    #endif
    
    return screenUV;
}

// World space to UV mapping for triplanar projection
float2 WorldToUV(float3 worldPos, half3 normal, half scale)
{
    // Determine dominant axis
    half3 absNormal = abs(normal);
    
    if (absNormal.y > absNormal.x && absNormal.y > absNormal.z)
    {
        // Y-axis dominant (top/bottom)
        return worldPos.xz * scale;
    }
    else if (absNormal.x > absNormal.z)
    {
        // X-axis dominant (left/right)
        return worldPos.yz * scale;
    }
    else
    {
        // Z-axis dominant (front/back)
        return worldPos.xy * scale;
    }
}

// Rotation matrix for 2D operations
float2x2 RotationMatrix(half angle)
{
    half c = cos(angle);
    half s = sin(angle);
    return float2x2(c, -s, s, c);
}

// Animate UV coordinates
float2 AnimateUV(float2 uv, half2 speed, half2 scale)
{
    return (uv + _Time.y * speed) * scale;
}

// Fresnel calculation with power control
half CalculateFresnel(half3 normal, half3 viewDir, half power)
{
    half fresnel = 1.0 - saturate(dot(normal, viewDir));
    return pow(fresnel, power);
}

// Edge mask calculation for various effects
half CalculateEdgeMask(half3 normal, half3 viewDir, half edgeWidth)
{
    half NdotV = dot(normal, viewDir);
    return 1.0 - smoothstep(edgeWidth - 0.1, edgeWidth, abs(NdotV));
}

// Simplified depth fade function
half CalculateDepthFade(float4 screenPos, half fadeDistance)
{
    // Simplified version that doesn't require depth texture
    // Returns constant value for now
    return 1.0;
}

// Sphere mask for localized effects
half SphereMask(float3 position, float3 center, half radius, half hardness)
{
    half distance = length(position - center);
    return 1.0 - smoothstep(radius * (1.0 - hardness), radius, distance);
}

// Box mask for localized effects
half BoxMask(float3 position, float3 center, half3 size, half hardness)
{
    half3 d = abs(position - center) - size;
    half distance = length(max(d, 0.0)) + min(max(d.x, max(d.y, d.z)), 0.0);
    return 1.0 - smoothstep(-hardness, 0.0, distance);
}

// Gradient noise (more complex than simple noise)
half GradientNoise(float2 uv, half scale)
{
    uv *= scale;
    float2 i = floor(uv);
    float2 f = frac(uv);
    
    float2 u = f * f * (3.0 - 2.0 * f);
    
    return lerp(lerp(SimpleNoise(i + float2(0.0, 0.0)), 
                     SimpleNoise(i + float2(1.0, 0.0)), u.x),
                lerp(SimpleNoise(i + float2(0.0, 1.0)), 
                     SimpleNoise(i + float2(1.0, 1.0)), u.x), u.y);
}

// Fractal noise (multiple octaves)
half FractalNoise(float2 uv, int octaves, half frequency, half amplitude)
{
    half value = 0.0;
    half totalAmplitude = 0.0;
    
    for (int i = 0; i < octaves; i++)
    {
        value += GradientNoise(uv, frequency) * amplitude;
        totalAmplitude += amplitude;
        frequency *= 2.0;
        amplitude *= 0.5;
    }
    
    return value / totalAmplitude;
}

// Voronoi noise for cellular patterns
half VoronoiNoise(float2 uv, half scale)
{
    uv *= scale;
    float2 i = floor(uv);
    float2 f = frac(uv);
    
    half minDist = 1.0;
    
    for (int y = -1; y <= 1; y++)
    {
        for (int x = -1; x <= 1; x++)
        {
            float2 neighbor = float2(float(x), float(y));
            float2 pointOffset = float2(SimpleNoise(i + neighbor), SimpleNoise(i + neighbor + float2(17.0, 42.0)));
            float2 point = neighbor + pointOffset;
            float2 diff = point - f;
            half dist = length(diff);
            minDist = min(minDist, dist);
        }
    }
    
    return minDist;
}

// Bias and gain functions for curve control
half Bias(half t, half bias)
{
    return t / ((1.0 / bias - 2.0) * (1.0 - t) + 1.0);
}

half Gain(half t, half gain)
{
    return (t < 0.5) ? Bias(t * 2.0, gain) / 2.0 : Bias(t * 2.0 - 1.0, 1.0 - gain) / 2.0 + 0.5;
}

// Color temperature adjustment
half3 AdjustColorTemperature(half3 color, half temperature)
{
    // Temperature range: 0 = cool (blue), 1 = warm (orange)
    half3 cool = half3(0.8, 0.9, 1.2);
    half3 warm = half3(1.2, 1.0, 0.8);
    half3 tempFilter = lerp(cool, warm, temperature);
    
    return color * tempFilter;
}

// Saturation adjustment
half3 AdjustSaturation(half3 color, half saturation)
{
    half gray = Luminance(color);
    return lerp(half3(gray, gray, gray), color, saturation);
}

// Contrast adjustment
half3 AdjustContrast(half3 color, half contrast)
{
    return saturate(lerp(half3(0.5, 0.5, 0.5), color, contrast));
}

// Gamma correction
half3 ApplyGamma(half3 color, half gamma)
{
    return pow(abs(color), 1.0 / gamma);
}

// Dithering patterns - simplified version
half OrderedDithering(float2 screenPos, int size)
{
    // Simple dithering pattern without arrays
    float2 uv = screenPos / (float)size;
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}

// Blue noise approximation
half BlueNoise(float2 uv)
{
    // This is a simplified blue noise approximation
    // In practice, you'd use a blue noise texture
    float2 seed = uv + _Time.y * 0.1;
    return frac(sin(dot(seed, float2(12.9898, 78.233))) * 43758.5453);
}

// Smooth minimum for blending operations
half SmoothMin(half a, half b, half k)
{
    half h = saturate(0.5 + 0.5 * (b - a) / k);
    return lerp(b, a, h) - k * h * (1.0 - h);
}

// Smooth maximum for blending operations
half SmoothMax(half a, half b, half k)
{
    return -SmoothMin(-a, -b, k);
}

// Easing functions for animations
half EaseInQuad(half t) { return t * t; }
half EaseOutQuad(half t) { return 1.0 - (1.0 - t) * (1.0 - t); }
half EaseInOutQuad(half t) { return t < 0.5 ? 2.0 * t * t : 1.0 - 2.0 * (1.0 - t) * (1.0 - t); }

half EaseInCubic(half t) { return t * t * t; }
half EaseOutCubic(half t) { return 1.0 - pow(1.0 - t, 3.0); }

// Utility for checking if a feature is enabled
bool IsFeatureEnabled(half featureToggle)
{
    return featureToggle > 0.5;
}

// Blend modes
half3 BlendMultiply(half3 base, half3 blend) { return base * blend; }
half3 BlendScreen(half3 base, half3 blend) { return 1.0 - (1.0 - base) * (1.0 - blend); }
half3 BlendOverlay(half3 base, half3 blend)
{
    return base < 0.5 ? 2.0 * base * blend : 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
}
half3 BlendSoftLight(half3 base, half3 blend)
{
    return base < 0.5 ? 2.0 * base * blend + base * base * (1.0 - 2.0 * blend) 
                      : sqrt(base) * (2.0 * blend - 1.0) + 2.0 * base * (1.0 - blend);
}

#endif