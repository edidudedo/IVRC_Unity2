Shader "KuwaharaFilter"
{
    Properties{
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off
        ZTest Always
        ZWrite Off

        Tags{ "RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            CBUFFER_START(UnityPerMaterial)
            sampler2D _MainTex;
            float2 _MainTex_TexelSize;
            int _SampleCountFactor;
            float _CellScale;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 color = float4(0, 0, 0, 1);

                // 解像度が違っても同じ見え方にする
                float2 cellSize = _CellScale / 1000;
                cellSize.y *= _MainTex_TexelSize.y / _MainTex_TexelSize.x;
                half3 color0 = tex2D(_MainTex, i.uv).rgb;

                int count = 0;
                float2 offset = -_SampleCountFactor * cellSize;
                for (int x = 0; x <= _SampleCountFactor; ++x)
                {
                    offset.y = -_SampleCountFactor * cellSize.y;

                    for (int y = -_SampleCountFactor; y <= _SampleCountFactor; ++y)
                    {
                        if (x == 0 && y <= 0) {
                            continue;
                        }

                        half3 color1 = tex2D(_MainTex, i.uv + offset).rgb;
                        half3 color2 = tex2D(_MainTex, i.uv - offset).rgb;
                        float3 diff1 = color1 - color0.rgb;
                        float3 diff2 = color2 - color0.rgb;
                        // 中心の色に近いほうを採用
                        color.rgb += dot(diff1, diff1) < dot(diff2, diff2) ? color1 : color2;
                        count++;

                        // cellSize分だけずらしていく
                        offset.y += cellSize.y;
                    }
                    offset.x += cellSize.x;
                }

                color.rgb /= count;
                return color;
            }
            ENDCG
        }
    }
}