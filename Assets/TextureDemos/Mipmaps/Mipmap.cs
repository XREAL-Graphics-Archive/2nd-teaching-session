using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mipmap : MonoBehaviour
{
    // apply in inspector
    // [SerializeField] Material mipmapMat;
    [SerializeField] RenderTexture camOutput; // camera output, the image to render
    [SerializeField] [Range(0, 7)] int mipLvls = 0; // max mip levels
    [SerializeField] bool mipEnabled = true;
    
    // shader property id
    private static int _renderTexID = Shader.PropertyToID("_RenderTex");
    private static int _dstID = Shader.PropertyToID("_DST");
    private static int _mipID = Shader.PropertyToID("_MIP");
    private static int _MipEnabled = Shader.PropertyToID("_MipEnabled");

    // member variables for debugging
    [Header("Debug Params")]
    [SerializeField] Texture2D _dst;
    [SerializeField] Texture2D _mip;

    // event function to take picture
    public void Capture()
    {
        Shader.SetGlobalTexture(_renderTexID, camOutput);
        Shader.SetGlobalInteger(_MipEnabled, mipEnabled ? 1 : 0);
        Make(camOutput, mipLvls);
    }
    
    // main function to create mipmaps
    void Make(RenderTexture rt, int miplevel)
    {
        Texture2D copy = RTtoTex2D(rt);
        Shader.SetGlobalTexture(_mipID, copy);
        // Texture2D buffer = new Texture2D( // write to this tex
        //                         copy.width, 
        //                         copy.height, 
        //                         TextureFormat.RGBA32, 
        //                         false, 
        //                         true);
        // Texture2D mip = new Texture2D( // source tex
        //                         copy.width, 
        //                         copy.height, 
        //                         TextureFormat.RGBA32, 
        //                         false, 
        //                         true);
        
        // _dst = (Texture2D)(Shader.GetGlobalTexture(_mipID));
        // copy original render texture
        // Graphics.CopyTexture(copy, mip);
        // _dst = buffer;
        // return;

        // // downsample
        // for (int k = 1; k < miplevel; k++)
        // {
        //     // finer mipmap as source
        //     mip = new Texture2D(
        //         copy.width >> k - 1, 
        //         copy.height >> k - 1,
        //         TextureFormat.RGBA32, 
        //         false, 
        //         true);
        //     
        //     Graphics.CopyTexture(buffer, mip);
        //     // Graphics.Blit(src, dst);
        //     // destination texture
        //     buffer = new Texture2D(
        //             copy.width >> k, 
        //             copy.height >> k, 
        //             TextureFormat.RGBA32, 
        //             false, 
        //             true);
// 
        //     // bind textures to shader
        //     Shader.SetGlobalTexture(_dstID, buffer);
        //     Shader.SetGlobalTexture(_mipID, mip);
// 
        //     // check in inspector
        //     _dst = buffer;
        //     _mip = mip;
        //     
        //     // get processed texture from shader
        //     buffer = (Texture2D)(Shader.GetGlobalTexture(_mipID));
        // }
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
