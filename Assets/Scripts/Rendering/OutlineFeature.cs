using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class OutlineFeature : ScriptableRendererFeature
{
    [Serializable]
    public class EdgeDetectionSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        
        [Header("Ana Hat Ayarları")]
        [Range(0, 20)] public float outlineThickness = 8f;
        public Color outlineColor = Color.black;
        
        [Header("Hassasiyet Ayarları")]
        [Tooltip("Nesneler arasındaki ana hatların hassasiyeti.")]
        [Range(0.001f, 10f)] public float depthThreshold = 0.01f;
        [Tooltip("Bir nesnenin kendi kıvrımlarındaki ana hatların hassasiyeti.")]
        [Range(0.1f, 10f)] public float normalsThreshold = 0.4f;

        [Header("Fırça Dokusu Ayarları")]
        [Tooltip("Ana hatlara uygulanacak fırça darbesi dokusu (Siyah-Beyaz).")]
        public Texture2D brushTexture;
        [Tooltip("Fırça dokusunun desen tekrarı (boyutu).")]
        [Range(0.1f, 10f)] public float brushTiling = 1.0f;
    }

    [SerializeField] private EdgeDetectionSettings settings;
    private Material edgeDetectionMaterial;
    private EdgeDetectionPass edgeDetectionPass;

    public override void Create()
    {
        edgeDetectionPass ??= new EdgeDetectionPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Preview
            || renderingData.cameraData.cameraType == CameraType.Reflection
            || UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
            return;
        
        if (edgeDetectionMaterial == null)
        {
            edgeDetectionMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/Edge Detection"));
            if (edgeDetectionMaterial == null)
            {
                Debug.LogWarning("Gerekli materyal oluşturulamadı. Kenar çizgisi efekti çalışmayacak.");
                return;
            }
        }

        edgeDetectionPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Color);
        edgeDetectionPass.requiresIntermediateTexture = true;
        edgeDetectionPass.Setup(ref settings, ref edgeDetectionMaterial);

        renderer.EnqueuePass(edgeDetectionPass);
    }

    override protected void Dispose(bool disposing)
    {
        edgeDetectionPass = null;
        CoreUtils.Destroy(edgeDetectionMaterial);
    }
    
    private class EdgeDetectionPass : ScriptableRenderPass
    {
        private Material material;
        
        private static readonly int OutlineThicknessProperty = Shader.PropertyToID("_OutlineThickness");
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        private static readonly int DepthThresholdProperty = Shader.PropertyToID("_DepthThreshold");
        private static readonly int NormalsThresholdProperty = Shader.PropertyToID("_NormalsThreshold");
        private static readonly int BrushTexProperty = Shader.PropertyToID("_BrushTex");
        private static readonly int BrushTilingProperty = Shader.PropertyToID("_BrushTiling");


        public EdgeDetectionPass()
        {
            profilingSampler = new ProfilingSampler(nameof(EdgeDetectionPass));
        }

        public void Setup(ref EdgeDetectionSettings settings, ref Material edgeDetectionMaterial)
        {
            material = edgeDetectionMaterial;
            renderPassEvent = settings.renderPassEvent;

            material.SetFloat(OutlineThicknessProperty, settings.outlineThickness);
            material.SetColor(OutlineColorProperty, settings.outlineColor);
            material.SetFloat(DepthThresholdProperty, settings.depthThreshold);
            material.SetFloat(NormalsThresholdProperty, settings.normalsThreshold);
            material.SetFloat(BrushTilingProperty, settings.brushTiling);

            // Dokuyu atarken null kontrolü yap
            if (settings.brushTexture != null)
            {
                material.SetTexture(BrushTexProperty, settings.brushTexture);
            }
            else
            {
                // Eğer doku atanmamışsa, varsayılan beyaz dokuyu kullan (düz çizgi)
                material.SetTexture(BrushTexProperty, Texture2D.whiteTexture);
            }
        }

        private class PassData { }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();

            using var builder = renderGraph.AddRasterRenderPass<PassData>("Edge Detection", out _);

            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
            builder.UseAllGlobalTextures(true);
            builder.AllowPassCulling(false);
            builder.SetRenderFunc((PassData _, RasterGraphContext context) => 
            {
                Blitter.BlitTexture(context.cmd, Vector2.one, material, 0); 
            });
        }
    }
}
