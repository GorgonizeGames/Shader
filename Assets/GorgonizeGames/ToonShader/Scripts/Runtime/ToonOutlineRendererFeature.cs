using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace GorgonizeGames
{
    public class ToonOutlineRendererFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            [Header("Outline Settings")]
            public Color outlineColor = Color.black;
            [Range(0f, 10f)]
            public float outlineWidth = 1f;
            [Range(0f, 1f)]
            public float depthThreshold = 0.1f;
            [Range(0f, 1f)]
            public float normalThreshold = 0.4f;
            [Range(0f, 1f)]
            public float colorThreshold = 0.2f;

            [Header("Advanced")]
            public bool useDepthTexture = true;
            public bool useNormalTexture = true;
            public bool useColorTexture = false;

            [Header("Performance")]
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            public bool downsample = false;
            [Range(1, 4)]
            public int downsampleFactor = 2;
        }

        public Settings settings = new Settings();
        private ToonOutlineRenderPass m_OutlinePass;
        private Material m_Material;
        private const string ShaderName = "Hidden/GorgonizeGames/ToonOutlinePostProcess";

        public override void Create()
        {
            m_OutlinePass = new ToonOutlineRenderPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Preview || renderingData.cameraData.cameraType == CameraType.Reflection)
                return;

            if (m_Material == null)
            {
                m_Material = CoreUtils.CreateEngineMaterial(ShaderName);
                if (m_Material == null)
                {
                    Debug.LogError($"Material could not be created for shader: {ShaderName}. Outline effect will not be rendered.");
                    return;
                }
            }
            
            m_OutlinePass.Setup(settings, m_Material);
            renderer.EnqueuePass(m_OutlinePass);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(m_Material);
            base.Dispose(disposing);
        }

        private class ToonOutlineRenderPass : ScriptableRenderPass
        {
            private Settings m_Settings;
            private Material m_Material;

            private class PassData
            {
                internal Settings settings;
                internal Material material;
                internal UniversalResourceData resourceData;
                internal TextureHandle source;
            }
            
            public ToonOutlineRenderPass()
            {
                profilingSampler = new ProfilingSampler(nameof(ToonOutlineRenderPass));
            }

            public void Setup(Settings settings, Material material)
            {
                this.m_Settings = settings;
                this.m_Material = material;
                this.renderPassEvent = settings.renderPassEvent;
            }
            
            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                if (m_Material == null) return;
                
                var resourceData = frameData.Get<UniversalResourceData>();
                
                TextureHandle tempTexture;
                {
                    var tempDesc = frameData.Get<UniversalCameraData>().cameraTargetDescriptor;
                    tempDesc.depthBufferBits = 0;
                    if (m_Settings.downsample)
                    {
                        tempDesc.width /= m_Settings.downsampleFactor;
                        tempDesc.height /= m_Settings.downsampleFactor;
                    }
                    tempTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, tempDesc, "_TempOutlineTexture", true);
                }

                using (var builder = renderGraph.AddRasterRenderPass<PassData>("Toon Outline Pass", out var data, profilingSampler))
                {
                    data.settings = this.m_Settings;
                    data.material = this.m_Material;
                    data.resourceData = resourceData;
                    data.source = resourceData.activeColorTexture;

                    builder.UseTexture(data.source);
                    if (m_Settings.useDepthTexture) builder.UseTexture(resourceData.cameraDepthTexture);
                    if (m_Settings.useNormalTexture) builder.UseTexture(resourceData.cameraNormalsTexture);
                    
                    builder.SetRenderAttachment(tempTexture, 0);
                    
                    builder.SetRenderFunc((PassData d, RasterGraphContext context) =>
                    {
                        d.material.SetColor("_OutlineColor", d.settings.outlineColor);
                        d.material.SetFloat("_OutlineWidth", d.settings.outlineWidth);
                        d.material.SetFloat("_DepthThreshold", d.settings.depthThreshold);
                        d.material.SetFloat("_NormalThreshold", d.settings.normalThreshold);
                        d.material.SetFloat("_ColorThreshold", d.settings.colorThreshold);

                        CoreUtils.SetKeyword(d.material, "USE_DEPTH_TEXTURE", d.settings.useDepthTexture);
                        CoreUtils.SetKeyword(d.material, "USE_NORMAL_TEXTURE", d.settings.useNormalTexture);
                        CoreUtils.SetKeyword(d.material, "USE_COLOR_TEXTURE", d.settings.useColorTexture);
                        
                        Blitter.BlitTexture(context.cmd, d.source, new Vector4(1, 1, 0, 0), d.material, 0);
                    });
                }
                
                using (var builder = renderGraph.AddRasterRenderPass<PassData>("Copy To Camera Target", out var data, profilingSampler))
                {
                    data.resourceData = resourceData;
                    data.source = tempTexture;
                   
                    builder.UseTexture(data.source);
                    builder.SetRenderAttachment(resourceData.activeColorTexture, 0);

                    builder.SetRenderFunc((PassData d, RasterGraphContext context) =>
                    {
                        Blitter.BlitTexture(context.cmd, d.source, new Vector4(1, 1, 0, 0), Blitter.GetBlitMaterial(TextureDimension.Tex2D), 0);
                    });
                }
            }
            
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData){}
        }
    }
}

