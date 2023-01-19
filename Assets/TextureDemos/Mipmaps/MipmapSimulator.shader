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

            // Texture2D _RenderTex;
            Texture2D _MIP;
            Texture2D _DST;
            SamplerState sampler_MIP;
            int _MipEnabled;
            float4 _MIP_ST;
            float4 _MIP_TexelSize; // Vector4( 1/width, 1/height, width, height )

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv * _MIP_ST.xy + _MIP_ST.zw;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // sample the texture
                half4 col = 0;
                ///////////////////////////////////////////////////////
                float tcf = -0.5f; // tc correction factor

                // sample from MIP
                for(int k = 0 ; k < 4 ; k++)
                {
                    const int2 offset = int2(k>>1, k&1);
                    const float4 t = _MIP.Sample(sampler_MIP, i.uv + (tcf + offset) / _MIP_TexelSize.z);
                    // const float4 t = half4(i.uv * lv + (tcf + offset) / _MIP_TexelSize.z, 0, 1);

                    if(dot(half3(1,1,1), t.rgb) > 0) col += half4(t.rgb, 1);
                }

                if(col.a > 0) col /= col.a;

                if(_MipEnabled == 0) col = _MIP.Sample(sampler_MIP, i.uv);

                // sample mipmap
                return col;
                return half4(i.uv, 0, 1);
            }
            ENDHLSL
        }
    }
}
