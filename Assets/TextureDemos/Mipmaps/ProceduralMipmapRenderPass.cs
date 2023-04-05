using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ProceduralMipmapRenderPass : ScriptableRenderPass
{
    private string profilerTag;

    private RenderTargetIdentifier cameraColorTargetIdentifier;
    private int mipLevel;
    private ComputeShader computeShader;
    
    public ProceduralMipmapRenderPass(MipmapSettings passSettings, string profilerTag)
    {
        renderPassEvent = passSettings.renderPassEvent;

        this.profilerTag = profilerTag;
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
    }

    // Here you can implement the rendering logic.
    // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
    // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
    // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        cmd.Clear();

        if (mipLevel == 0)
        {
            // Blit(cmd, rendertexture, cameraColorTargetIdentifier);
        }
        else
        {
            //Blit(cmd, src, cameraColorTargetIdentifier);
        }
    }

    // Cleanup any allocated resources that were created during the execution of this render pass.
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
    }
}