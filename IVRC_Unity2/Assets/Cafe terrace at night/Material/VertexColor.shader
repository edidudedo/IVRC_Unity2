// テクスチャとして設定した画像を柄として、頂点カラーをそのまま色に反映するテクスチャ

Shader "VertexColorShader"
{
    Properties
    {   
        _Tex ("Texture", 2D) = "white" {}                       // 筆の質感を設定するテクスチャ
        [Normal] _NormalMap ("Normal Map", 2D) = "bump" {}      // ノーマルマップ（凹凸を表現）
        [Toggle(GrayScale)] _GrayScale ("GlayScale", Int) = 0   // ☑白黒に変更
    }
    CGINCLUDE

    ENDCG
    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque"}
        Cull Off        // CullingをOFFに（裏面を描画）
        // AlphaToMask On  // Alpha値をマスクとして利用

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature GrayScale


            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _Tex;
            sampler2D _NormalMap;

            struct appdata
            {
                float4 vertex : POSITION;   // 頂点位置
                float2 uv : TEXCOORD0;      // UV
                float4 color : COLOR;       // 頂点カラー
                float3 normal : NORMAL;     // 法線
                float4 tangent : TANGENT;   // 接線

                // single-pass instanced rendering
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;// ピクセル位置
                float2 uv : TEXCOORD0;      // UV
                float4 color : COLOR;       // ピクセルカラー
                half3 lightDir : TEXCOORD1; // ライトの向き
                half3 viewDir : TEXCOORD2;  // カメラの向き

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

                o.vertex = UnityObjectToClipPos(v.vertex);  // 頂点位置からピクセル位置を算出
                o.color = v.color;                          // 頂点カラーをそのまま利用
                o.uv = v.uv;                                // UVをそのまま利用
                
                // 接空間におけるライト方向とカメラ方向のベクトルを求める
                TANGENT_SPACE_ROTATION;
                o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
                o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));

                return o;
            }
            
            // フラグメントシェーダー
            fixed4 frag (v2f i) : SV_Target
            {   
                // 色とマスクを適応
                fixed4 col, mask;
                mask = tex2D(_Tex, i.uv);  
                #ifdef GrayScale
                    col = mask;
                #else
                    col = i.color;
                    clip(mask.a - 0.01);
                    col *= mask;    // カラーを指定したテクスチャでマスク
                #endif
                
                // ライトとカメラ方向のベクトルを正規化
                i.lightDir = normalize(i.lightDir);
                i.viewDir = normalize(i.viewDir);
                half3 halfDir = normalize(i.lightDir + i.viewDir);

                //ノーマルマップから法線を取得
                half3 normal = UnpackNormal(tex2D(_NormalMap, i.uv));
                normal = normalize(normal);

                // ノーマルマップから得た法線情報をつかってライティング計算をする
                half3 diffuse = max(0, dot(normal, i.lightDir)) * _LightColor0;
                half3 specular = pow(max(0, dot(normal, halfDir)), 128.0) * _LightColor0;

                // diffuseの影響を減らす
                diffuse = diffuse + .2;
                
                // ライティングの結果を反映
                col.rgb *= (diffuse + specular);
            
                return col;
            }
            ENDCG
        }
    }
}
