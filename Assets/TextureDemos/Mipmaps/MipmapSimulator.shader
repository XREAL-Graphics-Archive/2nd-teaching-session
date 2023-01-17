Shader "MipmapSimulator"
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Texture2D _RenderTex;
            Texture2D _MIP;
            Texture2D _DST;
            SamplerState sampler_RenderTex;
            float4 _RenderTex_ST;
            float4 _RenderTex_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv * _RenderTex_ST.xy + _RenderTex_ST.zw;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // sample the texture
                half4 col = _RenderTex.Sample(sampler_RenderTex, i.uv);

                // sample mipmap
                
                return col;
            }
            ENDHLSL
        }
    }
}
