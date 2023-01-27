using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mipmap : MonoBehaviour
{
    // apply in inspector
    // [SerializeField] Material mipmapMat;
    [SerializeField] Camera captureCam;
    [SerializeField] ComputeShader mipmapKernel;
    [SerializeField] [Range(0, 7)] int mipLevels = 0; // max mip levels
    [SerializeField] bool mipEnabled = true;
    
    RenderTexture camOutput; // camera output, the image to render
    
    RenderTexture writeBuffer; // texture to write in compute shader
    int Resolution;

    // shader property id
    private static int _mipID = Shader.PropertyToID("_MIP");
    // private static int _renderTexID = Shader.PropertyToID("_RenderTex");
    // private static int _dstID = Shader.PropertyToID("_DST");
    // private static int _mipLevelID = Shader.PropertyToID("_MipLevels");
    // private static int _MipEnabled = Shader.PropertyToID("_MipEnabled");

    // member variables for debugging
    [Header("Debug Params")]
    [SerializeField] Texture2D _dst;
    [SerializeField] Texture2D _mip;

    private void Awake()
    {
        camOutput = new RenderTexture(1024, 1024, 24);
        camOutput.enableRandomWrite = true;
        camOutput.Create();

        captureCam.targetTexture = camOutput;
        Resolution = camOutput.width;
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
            
            writeBuffer = new RenderTexture(mipResolution, mipResolution, 24);
            writeBuffer.enableRandomWrite = true;
            writeBuffer.Create();
            
            Debug.Log("readSourceTx: " + readSourceTx.width + " * " + readSourceTx.height
            + ", writeBuffer: " + writeBuffer.width + " * " + writeBuffer.height);
            
            mipmapKernel.SetFloat("Resolution", readSourceTx.width * 1.0f); // float to convert tc from int to float
            mipmapKernel.SetTexture(0, "_ReadSourceTx", readSourceTx); // texture to read from
            mipmapKernel.SetTexture(0, "Result", writeBuffer); // texture to write
            mipmapKernel.Dispatch(
                0, 
                writeBuffer.width / 8, 
                writeBuffer.height / 8, 
                1
            );

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
        if (GUI.Button(new Rect(35, 25, 150, 50), "Capture"))
        {
            Capture();
        }
    }
}
