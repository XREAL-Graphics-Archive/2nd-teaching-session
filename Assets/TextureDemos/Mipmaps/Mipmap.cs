using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mipmap : MonoBehaviour
{
    [Header("Debug Params")]
    [SerializeField] Texture2D _dst;
    [SerializeField] Texture2D _mip;
    
    [SerializeField] RenderTexture camOutputTex;
    [SerializeField] [Range(0, 7)] int mipLvls = 0;

    private static int _renderTexID = Shader.PropertyToID("_RenderTex");
    private static int _dstID = Shader.PropertyToID("_DST");
    private static int _mipID = Shader.PropertyToID("_MIP");
    
    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalTexture(_renderTexID, camOutputTex);
        Make(camOutputTex, mipLvls);
    }

    void Make(RenderTexture rt, int miplevel)
    {
        Texture2D copy = RTtoTex2D(rt);
        // tex2d = copy;
        Texture2D dst = new Texture2D(copy.width, copy.height, TextureFormat.RGBA32, false, true);
        Graphics.CopyTexture(copy, dst);
        
        // repeat
        for (int k = 1; k < miplevel; k++)
        {
            Debug.Log("iter: " + k);
            
            // bind destination texture
            dst = new Texture2D(
                copy.width >> k, 
                copy.height >> k, 
                TextureFormat.RGBA32, 
                false, 
                true);
            
            // bind finer mipmap as source
            Texture2D mip = new Texture2D(
                copy.width >> k - 1, 
                copy.height >> k - 1,
                TextureFormat.RGBA32, 
                false, 
                true);
            
            Shader.SetGlobalTexture(_dstID, dst);
            Shader.SetGlobalTexture(_mipID, mip);
        
            _dst = dst;
            _mip = mip;
            
            // get processed texture
            copy = (Texture2D)(Shader.GetGlobalTexture(_mipID));
        }
    }
    
    // convert Render Texture to Texture2D
    Texture2D RTtoTex2D(RenderTexture rt)
    {
        Texture2D outputTex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, true);
        RenderTexture.active = rt;
        outputTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        outputTex.Apply();
        return outputTex;
    }
}
