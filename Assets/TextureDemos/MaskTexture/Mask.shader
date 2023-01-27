Shader "URPTraining/MaskTexture"
{
    Properties
    {
        _MainTex ("Texture 1", 2D) = "white" {}
        _MainTex2 ("Texture 2", 2D) = "white" {}
        
        _MaskTex ("Mask Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        Pass
        {
            Name "Universal Forward"
            Tags {"LightMode" = "UniversalForward" }
            
            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct VertexInput
            {
                float4 vertex: POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            Texture2D _MainTex;
            Texture2D _MainTex2;
            Texture2D _MaskTex;
            SamplerState sampler_MainTex;
            float4 _MainTex_ST;
            float4 _MainTex2_ST;
            float4 _MaskTex_ST;

            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv2 = v.uv * _MainTex2_ST.xy + _MainTex2_ST.zw;
                return o;
            }

            half4 frag(VertexOutput i): SV_Target
            {
                float4 color = _MainTex.Sample(sampler_MainTex, i.uv);
                float4 color2 = _MainTex2.Sample(sampler_MainTex, i.uv2);
                float4 mask = _MaskTex.Sample(sampler_MainTex, i.uv);

                float4 f = lerp(color, color2, mask.g);
                return f;
            }
            ENDHLSL
        }
    }
}
