using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMipmap : MonoBehaviour
{
    Camera mainCamera;
    private RenderTexture outputRT;

    // Start is called before the first frame update
    void Start()
    {
        outputRT = new RenderTexture(1024, 1024, 0)
        {
            enableRandomWrite = true
        };
        outputRT.Create();

        mainCamera = Camera.main;
        mainCamera.targetTexture = outputRT;
    }
}
