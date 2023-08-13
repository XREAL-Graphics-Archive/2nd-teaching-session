using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ProceduralMipmapRendererFeature : ScriptableRendererFeature
{
    public enum BufferType
    {
        CameraColor,
        Custom 
    }
    
    [System.Serializable]
    public class MipmapSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public ComputeShader computeShader;

        [Range(0, 7)]
        public int mipLevel;
    
        public Material blitMaterial = null;
        public int blitMaterialPassIndex = -1;
        public BufferType sourceType      = BufferType.CameraColor;
        public BufferType destinationType = BufferType.CameraColor;
        public string sourceTextureId = "_SourceTexture";
        public string destinationTextureId = "_DestinationTexture";
    }

    ProceduralMipmapRenderPass scriptablePass;

    public MipmapSettings passSettings = new MipmapSettings();

    /// <inheritdoc/>
    public override void Create()
    {
        scriptablePass = new ProceduralMipmapRenderPass(passSettings, "Procedural Mipmap Feature");
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        ref CameraData cameraData = ref renderingData.cameraData; 
        RenderTargetIdentifier cameraColorTarget = cameraData.renderer.cameraColorTarget;
        scriptablePass.Setup(cameraColorTarget);
        
        renderer.EnqueuePass(scriptablePass);
    }
}