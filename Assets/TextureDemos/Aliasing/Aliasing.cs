using UnityEngine;

[ExecuteInEditMode]
public class Aliasing : MonoBehaviour
{
    private Texture2D _texture;
    private static int _texID = Shader.PropertyToID("_SRC");
    
    void Start()
    {
        _texture = new Texture2D(1024, 1024);
        Shader.SetGlobalTexture(_texID, _texture);
    }
}
