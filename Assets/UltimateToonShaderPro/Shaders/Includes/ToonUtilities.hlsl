#ifndef TOON_UTILITIES_INCLUDED
#define TOON_UTILITIES_INCLUDED

// Color space conversion functions
float3 RGBtoHSV(float3 rgb)
{
    float4 k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(rgb.bg, k.wz), float4(rgb.gb, k.xy), step(rgb.b, rgb.g));
    float4 q = lerp(float4(p.xyw, rgb.r), float4(rgb.r, p.yzx), step(p.x, rgb.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float3 HSVtoRGB(float3 hsv)
{
    float4 k = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(hsv.xxx + k.xyz) * 6.0 - k.www);
    return hsv.z * lerp(k.xxx, saturate(p - k.xxx), hsv.y);
}

// Color grading functions
float3 ApplyColorGrading(float3 color, float hue, float saturation, float brightness, float contrast, float gamma)
{
    // Gamma correction
    color = pow(color, gamma);
    
    // Contrast
    color = ((color - 0.5) * contrast) + 0.5;
    
    // Hue shift
    if (abs(hue) > 0.001)
    {
        float3 hsv = RGBtoHSV(color);
        hsv.x += hue / 360.0;
        hsv.x = frac(hsv.x);
        color = HSVtoRGB(hsv);
    }
    
    // Brightness
    color = saturate(color * brightness);
    
    // Saturation
    float luminance = dot(color, float3(0.299, 0.587, 0.114));
    color = lerp(luminance.xxx, color, saturation);
    
    return saturate(color);
}

// Posterization function
float3 Posterize(float3 color, float levels)
{
    return floor(color * levels) / levels;
}

// UV rotation function
float2 RotateUV(float2 uv, float rotation)
{
    float rad = radians(rotation);
    float cosAngle = cos(rad);
    float sinAngle = sin(rad);
    float2 center = float2(0.5, 0.5);
    uv -= center;
    float2 rotatedUV;
    rotatedUV.x = uv.x * cosAngle - uv.y * sinAngle;
    rotatedUV.y = uv.x * sinAngle + uv.y * cosAngle;
    return rotatedUV + center;
}

// Toon ramp calculation
float CalculateToonRamp(float NdotL, float threshold, float smoothness)
{
    return smoothstep(threshold - smoothness, threshold + smoothness, NdotL);
}

// Light wrapping function
float ApplyLightWrapping(float NdotL, float wrapAmount)
{
    return saturate((NdotL + wrapAmount) / (1.0 + wrapAmount));
}

// Fresnel calculation
float CalculateFresnel(float3 viewDir, float3 normal, float power)
{
    float fresnel = 1.0 - saturate(dot(viewDir, normal));
    return pow(fresnel, power);
}

// Rim lighting calculation
float CalculateRimLighting(float3 viewDir, float3 normal, float power, float threshold)
{
    float rim = 1.0 - saturate(dot(viewDir, normal));
    rim = smoothstep(threshold, 1.0, rim);
    return pow(rim, power);
}

// Specular calculation for toon shading
float CalculateToonSpecular(float3 lightDir, float3 viewDir, float3 normal, float size, float smoothness)
{
    float3 halfVector = normalize(lightDir + viewDir);
    float NdotH = saturate(dot(normal, halfVector));
    float specular = pow(NdotH, (1.0 - size) * 128.0);
    return smoothstep(0.5 - smoothness, 0.5 + smoothness, specular);
}

// Screen-space hatching calculation
float CalculateScreenSpaceHatching(float4 screenPos, float scale, float bias, float lightValue, float threshold, float intensity)
{
    float2 screenUV = screenPos.xy / screenPos.w;
    screenUV *= _ScreenParams.xy / scale;
    
    float hatch = sin(screenUV.x * 3.14159) * sin(screenUV.y * 3.14159);
    hatch = saturate(hatch + bias);
    
    float hatchMask = step(lightValue, threshold);
    return lerp(1.0, hatch, hatchMask * intensity);
}

// World-space hatching calculation
float CalculateWorldSpaceHatching(float2 uv, float density, float rotation, float lightValue, float threshold, float crossThreshold, float intensity, sampler2D hatchTex, sampler2D crossHatchTex)
{
    float hatching = 1.0;
    
    // Main hatching
    float2 hatchUV = RotateUV(uv * density, rotation);
    float hatch1 = tex2D(hatchTex, hatchUV).r;
    
    if (lightValue < threshold)
    {
        hatching *= lerp(1.0, hatch1, intensity);
    }
    
    // Cross hatching for deeper shadows
    if (lightValue < crossThreshold)
    {
        float2 crossHatchUV = RotateUV(uv * density, rotation + 90);
        float crossHatch = tex2D(crossHatchTex, crossHatchUV).r;
        hatching *= lerp(1.0, crossHatch, intensity);
    }
    
    return hatching;
}

// Matcap calculation
float3 CalculateMatcap(float3 normalWS, float3 viewDirWS, sampler2D matcapTex, float intensity, float blendMode)
{
    // Convert normal to view space
    float3 normalVS = mul((float3x3)UNITY_MATRIX_V, normalWS);
    float2 matcapUV = normalVS.xy * 0.5 + 0.5;
    float3 matcap = tex2D(matcapTex, matcapUV).rgb;
    
    // Apply blend mode
    if (blendMode < 0.5) // Add
    {
        return matcap * intensity;
    }
    else if (blendMode < 1.5) // Multiply
    {
        return lerp(float3(1, 1, 1), matcap, intensity);
    }
    else // Screen
    {
        return lerp(float3(0, 0, 0), matcap, intensity);
    }
}

// Subsurface scattering approximation
float3 CalculateSubsurface(float3 lightDir, float3 viewDir, float3 normal, float3 subsurfaceColor, float power, float intensity)
{
    float3 lightDirViewSpace = mul((float3x3)UNITY_MATRIX_V, -lightDir);
    float3 viewDirViewSpace = mul((float3x3)UNITY_MATRIX_V, -viewDir);
    float subsurface = pow(saturate(dot(lightDirViewSpace, -viewDirViewSpace)), power);
    return subsurface * subsurfaceColor * intensity;
}

// Cel shading quantization
float QuantizeLighting(float lightValue, float steps)
{
    return floor(lightValue * steps) / steps;
}

// Noise function for procedural effects
float Noise(float2 p)
{
    return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
}

// Simple hash function
float Hash(float2 p)
{
    float3 p3 = frac(float3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 19.19);
    return frac((p3.x + p3.y) * p3.z);
}

// Animated noise for dynamic effects
float AnimatedNoise(float2 p, float time)
{
    return Noise(p + time * 0.1);
}

// Distance-based detail fade
float CalculateDetailFade(float3 worldPos, float3 cameraPos, float fadeStart, float fadeEnd)
{
    float distance = length(worldPos - cameraPos);
    return 1.0 - saturate((distance - fadeStart) / (fadeEnd - fadeStart));
}

// Edge detection for outline generation
float EdgeDetection(float depth, float2 uv, float2 texelSize)
{
    float depthN = tex2D(_CameraDepthTexture, uv + float2(0, texelSize.y)).r;
    float depthS = tex2D(_CameraDepthTexture, uv - float2(0, texelSize.y)).r;
    float depthE = tex2D(_CameraDepthTexture, uv + float2(texelSize.x, 0)).r;
    float depthW = tex2D(_CameraDepthTexture, uv - float2(texelSize.x, 0)).r;
    
    float edge = abs(depthN - depth) + abs(depthS - depth) + abs(depthE - depth) + abs(depthW - depth);
    return saturate(edge * 100);
}

// Smooth minimum function for blending effects
float SmoothMin(float a, float b, float k)
{
    float h = max(k - abs(a - b), 0.0) / k;
    return min(a, b) - h * h * h * k * (1.0 / 6.0);
}

// Breathing animation function
float BreathingAnimation(float time, float speed, float intensity)
{
    return 1.0 + sin(time * speed) * intensity;
}

// Pulse animation function
float PulseAnimation(float time, float speed, float sharpness)
{
    float t = frac(time * speed);
    return pow(sin(t * 3.14159), sharpness);
}

#endif