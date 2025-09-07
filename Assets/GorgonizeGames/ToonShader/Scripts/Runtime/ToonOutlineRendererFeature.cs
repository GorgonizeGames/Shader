using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
            public LayerMask outlineLayerMask = -1;
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
        private ToonOutlineRenderPass outlinePass;
        
        public override void Create()
        {
            outlinePass = new ToonOutlineRenderPass(settings);
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (outlinePass != null)
            {
                outlinePass.Setup(renderer.cameraColorTargetHandle);
                renderer.EnqueuePass(outlinePass);
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            outlinePass?.Dispose();
        }
    }
    
    public class ToonOutlineRenderPass : ScriptableRenderPass
    {
        private ToonOutlineRendererFeature.Settings settings;
        private Material outlineMaterial;
        private RenderTargetIdentifier cameraColorTarget;
        private RenderTargetHandle tempTexture;
        private RenderTargetHandle depthTexture;
        private RenderTargetHandle normalTexture;
        
        // Shader property IDs
        private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineWidthId = Shader.PropertyToID("_OutlineWidth");
        private static readonly int DepthThresholdId = Shader.PropertyToID("_DepthThreshold");
        private static readonly int NormalThresholdId = Shader.PropertyToID("_NormalThreshold");
        private static readonly int ColorThresholdId = Shader.PropertyToID("_ColorThreshold");
        private static readonly int CameraDepthTextureId = Shader.PropertyToID("_CameraDepthTexture");
        private static readonly int CameraNormalsTextureId = Shader.PropertyToID("_CameraNormalsTexture");
        private static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        
        private const string OutlineShaderName = "Hidden/GorgonizeGames/ToonOutlinePostProcess";
        
        public ToonOutlineRenderPass(ToonOutlineRendererFeature.Settings settings)
        {
            this.settings = settings;
            renderPassEvent = settings.renderPassEvent;
            
            tempTexture.Init("_TempOutlineTexture");
            depthTexture.Init("_TempDepthTexture");
            normalTexture.Init("_TempNormalTexture");
            
            // Create outline material
            Shader outlineShader = Shader.Find(OutlineShaderName);
            if (outlineShader != null)
            {
                outlineMaterial = new Material(outlineShader);
            }
            else
            {
                Debug.LogError($"Toon Outline Shader not found: {OutlineShaderName}");
            }
        }
        
        public void Setup(RenderTargetIdentifier cameraColorTarget)
        {
            this.cameraColorTarget = cameraColorTarget;
        }
        
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Configure render targets
            var descriptor = cameraTextureDescriptor;
            descriptor.depthBufferBits = 0; // No depth buffer needed for temp textures
            
            if (settings.downsample)
            {
                descriptor.width /= settings.downsampleFactor;
                descriptor.height /= settings.downsampleFactor;
            }
            
            cmd.GetTemporaryRT(tempTexture.id, descriptor, FilterMode.Bilinear);
            
            if (settings.useDepthTexture)
            {
                var depthDescriptor = descriptor;
                depthDescriptor.colorFormat = RenderTextureFormat.RFloat;
                cmd.GetTemporaryRT(depthTexture.id, depthDescriptor, FilterMode.Point);
            }
            
            if (settings.useNormalTexture)
            {
                var normalDescriptor = descriptor;
                normalDescriptor.colorFormat = RenderTextureFormat.ARGBHalf;
                cmd.GetTemporaryRT(normalTexture.id, normalDescriptor, FilterMode.Point);
            }
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (outlineMaterial == null)
                return;
            
            CommandBuffer cmd = CommandBufferPool.Get("Toon Outline");
            
            // Set material properties
            outlineMaterial.SetColor(OutlineColorId, settings.outlineColor);
            outlineMaterial.SetFloat(OutlineWidthId, settings.outlineWidth);
            outlineMaterial.SetFloat(DepthThresholdId, settings.depthThreshold);
            outlineMaterial.SetFloat(NormalThresholdId, settings.normalThreshold);
            outlineMaterial.SetFloat(ColorThresholdId, settings.colorThreshold);
            
            // Set shader keywords
            SetShaderKeywords();
            
            // Render outline effect
            if (settings.downsample)
            {
                // Downsample pass
                cmd.Blit(cameraColorTarget, tempTexture.Identifier(), outlineMaterial, 0);
                // Upsample pass
                cmd.Blit(tempTexture.Identifier(), cameraColorTarget, outlineMaterial, 1);
            }
            else
            {
                // Direct pass
                cmd.Blit(cameraColorTarget, tempTexture.Identifier(), outlineMaterial, 0);
                cmd.Blit(tempTexture.Identifier(), cameraColorTarget);
            }
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        private void SetShaderKeywords()
        {
            // Set shader keywords based on settings
            if (settings.useDepthTexture)
                outlineMaterial.EnableKeyword("USE_DEPTH_TEXTURE");
            else
                outlineMaterial.DisableKeyword("USE_DEPTH_TEXTURE");
                
            if (settings.useNormalTexture)
                outlineMaterial.EnableKeyword("USE_NORMAL_TEXTURE");
            else
                outlineMaterial.DisableKeyword("USE_NORMAL_TEXTURE");
                
            if (settings.useColorTexture)
                outlineMaterial.EnableKeyword("USE_COLOR_TEXTURE");
            else
                outlineMaterial.DisableKeyword("USE_COLOR_TEXTURE");
        }
        
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTexture.id);
            if (settings.useDepthTexture)
                cmd.ReleaseTemporaryRT(depthTexture.id);
            if (settings.useNormalTexture)
                cmd.ReleaseTemporaryRT(normalTexture.id);
        }
        
        public void Dispose()
        {
            if (outlineMaterial != null)
            {
                CoreUtils.Destroy(outlineMaterial);
            }
        }
    }
}