using UnityEngine;

public class Mipmap : MonoBehaviour
{
    [SerializeField] Camera captureCam; // cam that outputs to render texture
    [SerializeField] ComputeShader mipmapKernel; // mipmap kernel to dispatch
    int mipLevels = 0; // max mip levels

    private GUIStyle style;
    private string GUIMipLevel;
    RenderTexture camOutput; // camera output, the image to render
    
    RenderTexture writeBuffer; // texture to write in compute shader
    int Resolution; // texture resolution, bind to shader

    // shader property id
    private static int _mipID = Shader.PropertyToID("_MIP");

    // create render textures
    private void Awake()
    {
        camOutput = new RenderTexture(1024, 1024, 24);
        camOutput.enableRandomWrite = true;
        camOutput.Create();

        captureCam.targetTexture = camOutput;
        Resolution = camOutput.width;

        mipLevels = 0;

        style = new GUIStyle();
        style.fontSize = 50;
        style.normal.textColor = Color.black;

        Capture();
    }

    // event function to take picture
    public void Capture()
    {
        if (mipLevels > 0) 
            Make(camOutput, mipLevels); // execute if mip level is above 0
        Shader.SetGlobalTexture(_mipID, mipLevels == 0 ? camOutput : writeBuffer); // set output
    }
    
    // main function to create mipmaps
    void Make(RenderTexture rt, int miplevel)
    {
        Texture2D readSourceTx = RTtoTex2D(rt);

        for (int k = 1; k <= miplevel; k++)
        {
            int mipResolution = Resolution >> k;
            if (mipResolution <= 0) break; // break at max possible mip level
            
            // render texture to write
            writeBuffer = new RenderTexture(mipResolution, mipResolution, 24);
            writeBuffer.enableRandomWrite = true;
            writeBuffer.Create();

            // dispatch compute shader
            mipmapKernel.SetFloat("Resolution", readSourceTx.width * 1.0f); // texture resolution
            mipmapKernel.SetTexture(0, "_ReadSourceTx", readSourceTx); // texture to read from
            mipmapKernel.SetTexture(0, "Result", writeBuffer); // texture to write
            mipmapKernel.Dispatch(
                0, 
                writeBuffer.width / 8, 
                writeBuffer.height / 8, 
                1
            );

            // save output for next iteration
            readSourceTx = RTtoTex2D(writeBuffer);
        }
    }
    
    // convert render texture to texture2D
    Texture2D RTtoTex2D(RenderTexture rt)
    {
        Texture2D outputTex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, true);
        
        RenderTexture.active = rt;
        outputTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        outputTex.Apply();
        
        return outputTex;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(340, 90, 30, 30), "+") && mipLevels < 7)
        {
            mipLevels++;
            Capture();
        }
        if (GUI.Button(new Rect(340, 130, 30, 30), "-") && mipLevels > 0)
        {
            mipLevels--;
            Capture();
        }
        
        GUIMipLevel = "Mip Level  " + mipLevels;
        GUI.Label(new Rect(35, 100, 100, 50), GUIMipLevel, style);
    }
}
