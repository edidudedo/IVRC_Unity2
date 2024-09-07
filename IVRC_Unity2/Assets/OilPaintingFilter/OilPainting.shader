Shader "OilPainting"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CellScale("Cell Scale", float) = 1
        _Radius ("Radius", Range(0, 10)) = 10
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Renderpipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #if SHADER_API_GLES
                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };
            #else
                struct appdata
                {
                    uint vertexID : SV_VertexID;
                };
            #endif

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            int _SampleCountFactor;
            int _Radius;
            float4 _MainTex_TexelSize;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                #if SHADER_API_GLES
                    float4 vertex = v.vertex;
                    float2 uv  = v.uv;
                #else
                    float4 vertex = GetFullScreenTriangleVertexPosition(v.vertexID);
                    float2 uv  = GetFullScreenTriangleTexCoord(v.vertexID);
                #endif

                o.vertex = vertex;
                o.uv = uv;
                return o;
            }

            float frag (v2f i) : SV_Target
            {
                half2 uv = i.uv;

                float3 mean[4] = {
                    {0, 0, 0},
                    {0, 0, 0},
                    {0, 0, 0},
                    {0, 0, 0}
                };

                float3 sigma[4] = {
                    {0, 0, 0},
                    {0, 0, 0},
                    {0, 0, 0},
                    {0, 0, 0}
                };

                float2 start[4] = {{-_Radius, -_Radius}, {-_Radius, 0}, {0, -_Radius}, {0, 0}};

                float2 pos;
                float3 col;
                for (int k = 0; k < 4; k++) {
                    for(int i = 0; i <= _Radius; i++) {
                        for(int j = 0; j <= _Radius; j++) {
                            pos = float2(i, j) + start[k];
                            col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearRepeat, float4(uv + float2(pos.x * _MainTex_TexelSize.x, pos.y * _MainTex_TexelSize.y), 0., 0.)).rgb;
                            mean[k] += col;
                            sigma[k] += col * col;
                        }
                    }
                }

                float sigma2;

                float n = pow(_Radius + 1, 2);
                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearRepeat, float4(i.uv,0.,0.));
                float min = 1;

                for (int l = 0; l < 4; l++) {
                    mean[l] /= n;
                    sigma[l] = abs(sigma[l] / n - mean[l] * mean[l]);
                    sigma2 = sigma[l].r + sigma[l].g + sigma[l].b;

                    if (sigma2 < min) {
                        min = sigma2;
                        color.rgb = mean[l].rgb;
                    }
                }
                return color;
            }
            ENDHLSL
        }
    }
}