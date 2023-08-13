using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ProceduralMipmapRenderPass : ScriptableRenderPass
{
    private string profilerTag;
    private ProceduralMipmapRendererFeature.MipmapSettings settings;

    private RenderTargetIdentifier cameraColorTargetIdentifier;
    private int mipLevel;
    private ComputeShader computeShader;
    
    RenderTargetIdentifier source;
    RenderTargetIdentifier destination;
    int temporaryRTId = Shader.PropertyToID("_TempRT");

    int sourceId;
    int destinationId;
    bool isSourceAndDestinationSameTarget;
    
    public ProceduralMipmapRenderPass(ProceduralMipmapRendererFeature.MipmapSettings passSettings, string profilerTag)
    {
        renderPassEvent = passSettings.renderPassEvent;

        this.profilerTag = profilerTag;
        settings = passSettings;
        mipLevel = passSettings.mipLevel;
        computeShader = passSettings.computeShader;
    }

    public void Setup(RenderTargetIdentifier cameraColorTargetIdentifier)
    {
        this.cameraColorTargetIdentifier = cameraColorTargetIdentifier;
    }
    
    // This method is called before executing the render pass.
    // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
    // When empty this render pass will render to the active camera render target.
    // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    // The render pipeline will ensure target setup and clearing happens in a performant manner.
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        blitTargetDescriptor.depthBufferBits = 0;

        isSourceAndDestinationSameTarget = settings.sourceType == settings.destinationType &&
                                           (settings.sourceType == ProceduralMipmapRendererFeature.BufferType.CameraColor || settings.sourceTextureId == settings.destinationTextureId);

        var renderer = renderingData.cameraData.renderer;

        if (settings.sourceType == ProceduralMipmapRendererFeature.BufferType.CameraColor)
        {
            sourceId = -1;
            source = renderer.cameraColorTarget;
        }
        else
        {
            sourceId = Shader.PropertyToID(settings.sourceTextureId);
            cmd.GetTemporaryRT(sourceId, blitTargetDescriptor, FilterMode.Bilinear);
            source = new RenderTargetIdentifier(sourceId);
        }

        if (isSourceAndDestinationSameTarget)
        {
            destinationId = temporaryRTId;
            cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, FilterMode.Bilinear);
            destination = new RenderTargetIdentifier(destinationId);
        }
        else if (settings.destinationType == ProceduralMipmapRendererFeature.BufferType.CameraColor)
        {
            destinationId = -1;
            destination = renderer.cameraColorTarget;
        }
        else
        {
            destinationId = Shader.PropertyToID(settings.destinationTextureId);
            cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, FilterMode.Bilinear);
            destination = new RenderTargetIdentifier(destinationId);
        }
    }

    // Here you can implement the rendering logic.
    // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
    // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
    // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        cmd.Clear();

        using (new ProfilingScope(cmd, new ProfilingSampler("Mip Pass")))
        {
            if (mipLevel == 0)
            {
                // Blitter.BlitCameraTexture(cmd, cameraColorTargetIdentifier, depthStoreAction);
                // Blit(cmd, rendertexture, cameraColorTargetIdentifier);
            }
            else
            {
                //Blit(cmd, src, cameraColorTargetIdentifier);
            }
        }
        
        context.ExecuteCommandBuffer(cmd);
        
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    // Cleanup any allocated resources that were created during the execution of this render pass.
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (destinationId != -1)
            cmd.ReleaseTemporaryRT(destinationId);

        if (source == destination && sourceId != -1)
            cmd.ReleaseTemporaryRT(sourceId);
    }
}