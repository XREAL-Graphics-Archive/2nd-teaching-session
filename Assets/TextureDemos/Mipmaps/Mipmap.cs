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
    RenderTexture camOutput; // camera output, the image to render
    [SerializeField] bool mipEnabled = true;

    // shader property id
    private static int _renderTexID = Shader.PropertyToID("_RenderTex");
    // private static int _dstID = Shader.PropertyToID("_DST");
    private static int _mipID = Shader.PropertyToID("_MIP");
    private static int _mipLevelID = Shader.PropertyToID("_MipLevels");
    private static int _MipEnabled = Shader.PropertyToID("_MipEnabled");

    // member variables for debugging
    [Header("Debug Params")]
    [SerializeField] Texture2D _dst;
    [SerializeField] Texture2D _mip;

    private void Start()
    {
        camOutput = new RenderTexture(1024, 1024, 24);
        camOutput.enableRandomWrite = true;
        camOutput.Create();

        captureCam.targetTexture = camOutput;
    }

    // event function to take picture
    public void Capture()
    {
        // Shader.SetGlobalInteger(_MipEnabled, mipEnabled ? 1 : 0);
        Make(camOutput, mipLevels);
        Shader.SetGlobalTexture(_mipID, camOutput);
    }
    
    // main function to create mipmaps
    void Make(RenderTexture rt, int miplevel)
    {
        Texture2D copy = RTtoTex2D(rt);
        mipmapKernel.SetFloat("Resolution", camOutput.width);
        mipmapKernel.SetTexture(0, "Result", camOutput);
        mipmapKernel.Dispatch(
            0, 
            camOutput.width / 8, 
            camOutput.height / 8, 
            1
        );
        
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
