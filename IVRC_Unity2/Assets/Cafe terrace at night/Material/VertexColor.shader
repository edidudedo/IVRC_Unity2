// テクスチャとして設定した画像を柄として、頂点カラーをそのまま色に反映するテクスチャ

Shader "VertexColorShader"
{
    Properties
    {   
        _Tex ("Texture", 2D) = "white" {}       // 筆の質感を設定するテクスチャ
        [Toggle(GrayScale)] _GrayScale ("GlayScale", Int) = 0
    }
    CGINCLUDE

    sampler2D _Tex; 

    ENDCG
    SubShader
    {
        Cull Off        // CullingをOFFに（裏面を描画）
        AlphaToMask On  // Alpha値をマスクとして利用
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature GrayScale


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;   // 頂点位置
                float2 uv : TEXCOORD0;      // UV
                float4 color : COLOR;       // 頂点カラー

                // single-pass instanced rendering
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;// ピクセル位置
                float2 uv : TEXCOORD0;      // UV
                float4 color : COLOR;       // ピクセルカラー

                // single-pass instanced rendering
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // 頂点シェーダー
            v2f vert (appdata v)
            {
                v2f o;

                // single-pass instanced rendering
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);      // 頂点位置からピクセル位置を算出
                o.color = v.color;  // 頂点カラーをそのまま利用
                o.uv = v.uv;        // UVをそのまま利用
                return o;
            }
            
            // フラグメントシェーダー
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col;
                
                #ifdef GrayScale
                    col = tex2D(_Tex, i.uv);            // テクスチャのみ
                #else
                    col = tex2D(_Tex, i.uv) * i.color;  // カラーを指定したテクスチャでマスク
                #endif
                 
                return col;
            }
            ENDCG
        }
    }
}
