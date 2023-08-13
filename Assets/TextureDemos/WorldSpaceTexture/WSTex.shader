Shader "Custom/Triplanar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        [KeywordEnum(XY, XZ, YZ, Triplanar)] _MappingPlane ("Mapping Plane", Float) = 1
        _BlendSharpness("Blend Sharpness", Range(0, 5)) = 1.0
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

            #pragma shader_feature _MAPPINGPLANE_XY _MAPPINGPLANE_XZ _MAPPINGPLANE_YZ _MAPPINGPLANE_TRIPLANAR
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct VertexInput
            {
                float4 vertex: POSITION;
                float3 normal: NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD2;
                float3 worldNormal: TEXCOORD3;
            };

            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            float _BlendSharpness;

            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                return o;
            }

            half4 frag(VertexOutput i): SV_Target
            {
                // color to return
                half4 color = half4(0,0,0,1);

                #if defined(_MAPPINGPLANE_XY)
                color = _MainTex.Sample(sampler_MainTex, i.worldPos.xy);

                #elif defined(_MAPPINGPLANE_XZ)
                color = _MainTex.Sample(sampler_MainTex, i.worldPos.xz);
                
                #elif defined(_MAPPINGPLANE_YZ)
                color = _MainTex.Sample(sampler_MainTex, i.worldPos.yz);

                #elif defined(_MAPPINGPLANE_TRIPLANAR)
                // sample for each projection plane
			    half3 xAlbedo = _MainTex.Sample(sampler_MainTex, i.worldPos.yz);
                half3 yAlbedo = _MainTex.Sample(sampler_MainTex, i.worldPos.xz);
			    half3 zAlbedo = _MainTex.Sample(sampler_MainTex, i.worldPos.xy);

                // calculate weights and normalize
                half3 weight = pow(abs(i.worldNormal), _BlendSharpness);
                weight /= weight.x + weight.y + weight.z;

                // blend colors
                color.rgb = xAlbedo * weight.x + yAlbedo * weight.y + zAlbedo * weight.z;

                #endif
                
                return color;
            }
            ENDHLSL
        }
    }
}