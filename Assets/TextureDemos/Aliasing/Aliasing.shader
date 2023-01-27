Shader "Mipmaps/Aliasing"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Texture2D _RenderTex;
            Texture2D _SRC;
            SamplerState sampler_SRC;
            float4 _SRC_ST;
            float4 _SRC_TexelSize; // Vector4( 1/width, 1/height, width, height )

            v2f vert (Attributes v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv * _SRC_ST.xy + _SRC_ST.zw;
                o.uv.x += _Time.x * 0.1f;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                const int2 intUV = int2(i.uv * _SRC_TexelSize.z); // integer uvs

                if(intUV.x % 2 == 0 && intUV.y % 2 == 0)
                    return half4(1,1,1,1);

                discard;

                return half4(0,0,0,0); // guard statement for compile errors
            }
            ENDHLSL
        }
    }
}
