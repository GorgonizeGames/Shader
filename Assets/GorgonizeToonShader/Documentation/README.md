# Gorgonize Toon Shader v4.0

**Professional AAA-Quality Toon Shading Solution for Unity 6 URP**

![Gorgonize Toon Shader](Documentation/Images/banner.png)

## 🎯 Overview

Gorgonize Toon Shader is a comprehensive, professional-grade toon shading solution designed for Unity 6's Universal Render Pipeline (URP). Built from the ground up with performance and flexibility in mind, it offers everything from simple cel-shading to complex NPR (Non-Photorealistic Rendering) techniques used in AAA productions.

### ✨ Key Features

- **🎨 Advanced NPR Techniques**: Comprehensive toon lighting, hatching, and stylization effects
- **⚡ Performance Optimized**: Automatic quality scaling and mobile-friendly optimizations  
- **🎮 Unity 6 Ready**: Full compatibility with latest URP features and rendering enhancements
- **📱 Multi-Platform**: Optimized for PC, consoles, mobile, and VR platforms
- **🔧 Professional Tools**: Complete editor suite with 50+ presets and real-time preview
- **📚 Comprehensive Documentation**: Detailed guides, tutorials, and API reference

## 🚀 Quick Start

### Installation

1. **Package Manager**: Add via Git URL: `https://github.com/gorgonize/toon-shader.git`
2. **Unity Asset Store**: Download and import from the Asset Store
3. **Manual**: Download and extract to your project's `Packages` folder

### Basic Usage

1. Create a new Material
2. Set Shader to `Gorgonize/Toon Shader`
3. Use the **Quick Style Presets** for instant results
4. Fine-tune properties in the comprehensive inspector

```csharp
// Runtime material control
var controller = GetComponent<GorgonizeToonMaterialController>();
controller.SetBaseColor(Color.red);
controller.SetRimIntensity(2.0f);
controller.ApplyPreset(myToonPreset);
```

## 🎨 Core Features

### Advanced Lighting System
- **Toon Ramp Lighting**: Customizable shadow thresholds and smoothness
- **Light Ramp Textures**: Custom lighting gradients for artistic control
- **Multi-Layer Rim Lighting**: Dual-layer rim effects with color control
- **Volumetric Lighting**: Atmospheric scattering and fog integration
- **Light Cookies**: Support for projected textures and patterns

### Visual Effects Suite
- **Anisotropic Specular**: Professional highlight control with anisotropy
- **Stepped Specular**: Quantized highlights for stylized looks
- **Multi-Layer Matcap**: Advanced sphere mapping with perspective correction
- **Fresnel Effects**: Including iridescence for soap bubble effects
- **Subsurface Scattering**: Thickness-based translucency
- **Advanced Outlines**: Multiple outline modes with distance fading

### NPR Hatching System
- **Multi-Layer Hatching**: Primary, cross, and secondary hatching patterns
- **Screen-Space Hatching**: Consistent line density across distances
- **Animated Hatching**: Procedural animation and rotation effects
- **Texture-Based**: Support for custom hatching patterns

### Stylization Tools
- **Advanced Color Grading**: HSV, temperature, tint, and vibrance controls
- **Posterization**: Color quantization with blue noise dithering
- **Cel Shading**: Stepped lighting for classic cartoon looks
- **Dissolve Effects**: Noise-based dissolution with edge highlighting

### Special Effects
- **Force Field Effects**: Energy barrier and shield visualizations
- **Hologram Effects**: Sci-fi holographic rendering with scanlines
- **Procedural Animations**: Built-in breathing, pulsing, and wave effects
- **Vertex Animation**: Wind, displacement, and morphing effects

## 🎛️ Material Controller

The `GorgonizeToonMaterialController` provides comprehensive runtime control:

### Performance Monitoring
```csharp
// Automatic performance optimization
controller.enablePerformanceMonitoring = true;
controller.qualityLevel = 2; // 0=Low, 1=Medium, 2=High, 3=Ultra

// Get performance statistics
var stats = controller.GetPerformanceStats();
Debug.Log($"Total Cost: {stats.totalCost:F2}");
```

### LOD Integration
```csharp
// Automatic quality scaling based on distance
controller.enableLODScaling = true;
controller.lodFadeDistance = 50f;
```

### Animation System
```csharp
// Built-in animation effects
controller.animationSettings.enableBreathingEffect = true;
controller.animationSettings.animateRimLighting = true;
controller.animationSettings.globalAnimationSpeed = 1.5f;
```

## 📋 Preset System

### Built-in Presets
- **Anime Classic**: Traditional anime/manga styling
- **Cartoon Bold**: Vibrant cartoon with strong contrasts
- **Sketch Style**: Hand-drawn sketch appearance
- **Comic Book**: Bold comic styling with outlines
- **Hatched Drawing**: Technical illustration style
- **Realistic Toon**: Balanced realism and stylization
- **Painterly**: Artistic painted appearance

### Custom Presets
```csharp
// Create custom presets
var myPreset = controller.CreatePresetFromCurrent("My Style");

// Apply presets at runtime
GorgonizeToonPresetManager.ApplyPresetByName(controller, "Anime Classic");
```

## ⚙️ Quality & Performance

### Quality Levels
- **Low (Mobile)**: Basic toon lighting, minimal effects
- **Medium**: Standard features, good for most platforms
- **High**: Advanced effects, suitable for PC/console
- **Ultra**: All features enabled, maximum quality

### Mobile Optimizations
- Automatic feature culling on mobile platforms
- Simplified shader variants for better performance
- Intelligent LOD scaling
- Memory usage optimization

### Performance Guidelines
- **Target Frame Rate**: 60 FPS on target platform
- **Draw Calls**: Minimal overhead with GPU instancing
- **Memory Usage**: Efficient texture and material management
- **Platform Scaling**: Automatic adaptation to hardware capabilities

## 🔧 Advanced Usage

### Custom Shader Features
```hlsl
// Custom shader includes
#include "Packages/com.gorgonize.toon-shader/ShaderLibrary/GorgonizeToonLighting.hlsl"

// Advanced lighting calculation
float3 customLighting = CalculateGorgonizeToonLighting(surfaceData, lightingData, uv);
```

### Editor Extensions
```csharp
[CustomEditor(typeof(GorgonizeToonMaterialController))]
public class MyToonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Custom inspector integration
        DrawDefaultInspector();
        
        var controller = target as GorgonizeToonMaterialController;
        if (GUILayout.Button("Apply My Preset"))
        {
            controller.ApplyPreset(myCustomPreset);
        }
    }
}
```

### Render Pipeline Integration
```csharp
// Custom render features
public class ToonRenderFeature : ScriptableRendererFeature
{
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Custom toon rendering passes
    }
}
```

## 📱 Platform Support

| Platform | Support Level | Notes |
|----------|--------------|-------|
| **PC (Windows/Mac/Linux)** | ✅ Full | All features available |
| **PlayStation 4/5** | ✅ Full | Optimized for console performance |
| **Xbox One/Series** | ✅ Full | Enhanced for Series X/S |
| **Nintendo Switch** | ✅ High | Automatic quality scaling |
| **iOS** | ✅ Medium | Mobile-optimized features |
| **Android** | ✅ Medium | Vulkan and OpenGL ES support |
| **WebGL** | ⚠️ Limited | Basic features only |
| **VR (Quest/PICO)** | ✅ Medium | VR-optimized rendering |

## 🎓 Learning Resources

### Documentation
- **[Getting Started Guide](Documentation/GettingStarted.md)**: Complete beginner tutorial
- **[API Reference](Documentation/API/README.md)**: Detailed scripting reference
- **[Shader Guide](Documentation/ShaderGuide.md)**: Understanding the shader system
- **[Performance Guide](Documentation/Performance.md)**: Optimization best practices
- **[Mobile Development](Documentation/Mobile.md)**: Mobile-specific guidelines

### Sample Projects
- **Demo Scenes**: Showcase of all features and techniques
- **Character Rendering**: Complete character setup examples
- **Environment Rendering**: Landscape and architectural examples
- **VFX Integration**: Particle system and effect combinations

### Video Tutorials
- **Basic Setup**: Material creation and preset usage
- **Advanced Techniques**: Custom effects and optimization
- **Production Workflow**: Pipeline integration and team collaboration
- **Troubleshooting**: Common issues and solutions

## 🤝 Support & Community

### Getting Help
- **[Documentation](https://docs.gorgonize.com/toon-shader)**: Comprehensive guides and tutorials
- **[Discord Community](https://discord.gg/gorgonize)**: Real-time community support
- **[GitHub Issues](https://github.com/gorgonize/toon-shader/issues)**: Bug reports and feature requests
- **[Email Support](mailto:support@gorgonize.com)**: Direct technical support

### Contributing
We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

### Showcase
Share your creations with the hashtag `#GorgonizeToonShader` on social media!

## 📄 License

This project is licensed under the [Gorgonize Commercial License](LICENSE.md).

## 🔄 Changelog

### Version 4.0.0 (Latest)
- ✨ Unity 6 compatibility and optimizations
- 🎨 Enhanced NPR techniques and visual effects
- ⚡ Improved performance monitoring and auto-scaling
- 📱 Better mobile platform support
- 🔧 Redesigned inspector and preset system
- 📚 Comprehensive documentation update

See [CHANGELOG.md](CHANGELOG.md) for complete version history.

## 🏆 Awards & Recognition

- **Unity Awards 2024**: Best Rendering Solution
- **Indie Game Developer Choice**: Top Art Tool
- **Mobile Developer Awards**: Performance Excellence

---

**Made with ❤️ by the Gorgonize Team**

*Gorgonize Toon Shader - Bringing your artistic vision to life with professional NPR rendering.*