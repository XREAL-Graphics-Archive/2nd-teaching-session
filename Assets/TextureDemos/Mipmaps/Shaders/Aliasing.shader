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
            Texture2D _MIP;
            Texture2D _DST;
            SamplerState sampler_MIP;
            int _MipEnabled;
            float4 _MIP_ST;
            float4 _MIP_TexelSize; // Vector4( 1/width, 1/height, width, height )

            v2f vert (Attributes v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv * _MIP_ST.xy + _MIP_ST.zw;
                o.uv.x += _Time.x * 0.1f;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                const int2 intUV = int2(i.uv * _MIP_TexelSize.z); // integer uvs

                if(intUV.x % 2 == 0 && intUV.y % 2 == 0)
                    return half4(1,1,1,1);

                discard;

                return half4(0,0,0,0); // guard statement for compile errors
            }
            ENDHLSL
        }
    }
}
