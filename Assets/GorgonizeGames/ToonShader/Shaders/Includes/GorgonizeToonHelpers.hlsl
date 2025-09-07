#ifndef GORGONIZE_TOON_HELPERS_INCLUDED
#define GORGONIZE_TOON_HELPERS_INCLUDED

// Safe normalize function to avoid division by zero
half3 SafeNormalize(half3 inVec)
{
    half dp3 = max(FLT_MIN, dot(inVec, inVec));
    return inVec * rsqrt(dp3);
}

// Transform world space normal to view space
half3 TransformWorldToViewNormal(half3 normalWS, bool doNormalize)
{
    half3 normalVS = mul((half3x3)GetWorldToViewMatrix(), normalWS);
    if (doNormalize)
        return normalize(normalVS);
    return normalVS;
}

// Dithering pattern generation
half3 ApplyDithering(half3 color, float2 screenPos, half scale)
{
    #if !ENABLE_DITHERING
        return color;
    #endif
    
    // Generate dithering pattern using screen coordinates
    float2 ditherCoord = screenPos / scale;
    
    // Simple ordered dithering pattern (Bayer matrix 4x4)
    const half ditherPattern[16] = {
        0.0/16.0,  8.0/16.0,  2.0/16.0, 10.0/16.0,
        12.0/16.0, 4.0/16.0, 14.0/16.0,  6.0/16.0,
        3.0/16.0, 11.0/16.0,  1.0/16.0,  9.0/16.0,
        15.0/16.0, 7.0/16.0, 13.0/16.0,  5.0/16.0
    };
    
    int x = int(ditherCoord.x) % 4;
    int y = int(ditherCoord.y) % 4;
    half dither = ditherPattern[y * 4 + x];
    
    // Apply dithering to color
    return color + (dither - 0.5) * 0.02; // Subtle dithering effect
}

// Shadow dithering for soft shadow edges
half ApplyShadowDithering(half shadow, float2 screenPos)
{
    #if !SHADOW_DITHERING
        return shadow;
    #endif
    
    // Generate noise for dithering
    float2 noise = frac(sin(dot(screenPos, float2(12.9898, 78.233))) * 43758.5453);
    half dither = noise.x;
    
    // Apply dithering to shadow edges
    half shadowEdge = abs(shadow - 0.5) * 2.0;
    shadowEdge = 1.0 - shadowEdge;
    
    shadow += (dither - 0.5) * shadowEdge * 0.1;
    
    return saturate(shadow);
}

// Linear to sRGB conversion (for mobile optimization)
half3 LinearTosRGB(half3 color)
{
    #if MOBILE_OPTIMIZED
        // Simplified gamma correction for mobile
        return sqrt(color);
    #else
        // Accurate linear to sRGB conversion
        return pow(abs(color), 1.0/2.2);
    #endif
}

// sRGB to Linear conversion
half3 sRGBToLinear(half3 color)
{
    #if MOBILE_OPTIMIZED
        // Simplified inverse gamma for mobile
        return color * color;
    #else
        // Accurate sRGB to linear conversion
        return pow(abs(color), 2.2);
    #endif
}

// Smooth step with customizable edge sharpness
half SmoothStep(half edge0, half edge1, half x, half sharpness)
{
    half range = edge1 - edge0;
    half normalizedX = saturate((x - edge0) / range);
    
    // Apply sharpness
    normalizedX = pow(normalizedX, 1.0 / max(0.01, sharpness));
    
    return normalizedX;
}

// Remap value from one range to another
half Remap(half value, half oldMin, half oldMax, half newMin, half newMax)
{
    half oldRange = oldMax - oldMin;
    half newRange = newMax - newMin;
    return (((value - oldMin) * newRange) / oldRange) + newMin;
}

// Calculate luminance of color
half Luminance(half3 color)
{
    return dot(color, half3(0.299, 0.587, 0.114));
}

// HSV to RGB conversion
half3 HSVToRGB(half3 hsv)
{
    half4 k = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    half3 p = abs(frac(hsv.xxx + k.xyz) * 6.0 - k.www);
    return hsv.z * lerp(k.xxx, saturate(p - k.xxx), hsv.y);
}

// RGB to HSV conversion
half3 RGBToHSV(half3 rgb)
{
    half4 k = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    half4 p = rgb.g < rgb.b ? half4(rgb.bg, k.wz) : half4(rgb.gb, k.xy);
    half4 q = rgb.r < p.x ? half4(p.xyw, rgb.r) : half4(rgb.r, p.yzx);
    
    half d = q.x - min(q.w, q.y);
    half e = 1.0e-10;
    return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

// Distance-based outline width calculation
half CalculateOutlineWidth(float3 positionWS, half baseWidth, half fadeFactor)
{
    float distanceToCamera = distance(positionWS, GetCameraPositionWS());
    half fadeMultiplier = lerp(1.0, 0.1, saturate(distanceToCamera * fadeFactor));
    return baseWidth * fadeMultiplier;
}

// Normal-based outline calculation for better quality
half3 CalculateNormalBasedOutline(half3 normalOS, half3 positionOS, half outlineWidth)
{
    // Use object space normals for more consistent outline thickness
    half3 outlineOffset = normalize(normalOS) * outlineWidth;
    return positionOS + outlineOffset;
}

// Screen space outline calculation (for post-processing)
half CalculateScreenSpaceOutline(float2 screenUV, sampler2D depthTexture, sampler2D normalTexture)
{
    // Sample neighboring pixels for edge detection
    float2 texelSize = 1.0 / _ScreenParams.xy;
    
    // Depth-based edge detection
    half depth = tex2D(depthTexture, screenUV).r;
    half depthN = tex2D(depthTexture, screenUV + float2(0, texelSize.y)).r;
    half depthE = tex2D(depthTexture, screenUV + float2(texelSize.x, 0)).r;
    
    half depthDiff = abs(depth - depthN) + abs(depth - depthE);
    
    // Normal-based edge detection
    half3 normal = tex2D(normalTexture, screenUV).xyz * 2.0 - 1.0;
    half3 normalN = tex2D(normalTexture, screenUV + float2(0, texelSize.y)).xyz * 2.0 - 1.0;
    half3 normalE = tex2D(normalTexture, screenUV + float2(texelSize.x, 0)).xyz * 2.0 - 1.0;
    
    half normalDiff = length(normal - normalN) + length(normal - normalE);
    
    // Combine depth and normal edges
    return saturate(depthDiff + normalDiff);
}

// Triplanar mapping for seamless texturing
half4 TriplanarMapping(TEXTURE2D(tex), SAMPLER(samp), float3 positionWS, half3 normalWS, half scale)
{
    // Calculate triplanar weights
    half3 blendWeights = abs(normalWS);
    blendWeights = pow(blendWeights, 4);
    blendWeights /= (blendWeights.x + blendWeights.y + blendWeights.z);
    
    // Sample texture from three directions
    half4 xAxis = SAMPLE_TEXTURE2D(tex, samp, positionWS.zy * scale);
    half4 yAxis = SAMPLE_TEXTURE2D(tex, samp, positionWS.xz * scale);
    half4 zAxis = SAMPLE_TEXTURE2D(tex, samp, positionWS.xy * scale);
    
    // Blend samples based on normal weights
    return xAxis * blendWeights.x + yAxis * blendWeights.y + zAxis * blendWeights.z;
}

// Noise generation functions
half Random(float2 seed)
{
    return frac(sin(dot(seed, float2(12.9898, 78.233))) * 43758.5453);
}

half PerlinNoise(float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    
    half a = Random(i);
    half b = Random(i + float2(1.0, 0.0));
    half c = Random(i + float2(0.0, 1.0));
    half d = Random(i + float2(1.0, 1.0));
    
    float2 u = f * f * (3.0 - 2.0 * f);
    
    return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

// Utility function for mobile optimization
bool IsMobileOptimized()
{
    #if MOBILE_OPTIMIZED
        return true;
    #else
        return false;
    #endif
}

#endif