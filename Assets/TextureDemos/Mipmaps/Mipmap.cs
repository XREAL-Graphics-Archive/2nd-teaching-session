using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mipmap : MonoBehaviour
{
    // apply in inspector
    [SerializeField] RenderTexture camOutput; // camera output, the image to render
    [SerializeField] [Range(0, 7)] int mipLvls = 0; // max mip levels
    
    // shader property id
    private static int _renderTexID = Shader.PropertyToID("_RenderTex");
    private static int _dstID = Shader.PropertyToID("_DST");
    private static int _mipID = Shader.PropertyToID("_MIP");
    
    // member variables for debugging
    [Header("Debug Params")]
    [SerializeField] Texture2D _dst;
    [SerializeField] Texture2D _mip;
    
    void Update()
    {
        Shader.SetGlobalTexture(_renderTexID, camOutput);
        Make(camOutput, mipLvls);
    }
    
    // main function to create mipmaps
    void Make(RenderTexture rt, int miplevel)
    {
        Texture2D copy = RTtoTex2D(rt);
        // tex2d = copy;
        Texture2D dst = new Texture2D(copy.width, copy.height, TextureFormat.RGBA32, false, true);
        Graphics.CopyTexture(copy, dst);
        
        // downsample
        for (int k = 1; k < miplevel; k++)
        {
            // destination texture
            dst = new Texture2D(
                copy.width >> k, 
                copy.height >> k, 
                TextureFormat.RGBA32, 
                false, 
                true);
            
            // finer mipmap as source
            Texture2D mip = new Texture2D(
                copy.width >> k - 1, 
                copy.height >> k - 1,
                TextureFormat.RGBA32, 
                false, 
                true);
            
            // bind textures to shader
            Shader.SetGlobalTexture(_dstID, dst);
            Shader.SetGlobalTexture(_mipID, mip);
            
            // check in inspector
            _dst = dst;
            _mip = mip;
            
            // get processed texture from shader
            copy = (Texture2D)(Shader.GetGlobalTexture(_mipID));
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
}
