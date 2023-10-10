Shader "Custom/Grayscale" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f {
                    float2 texcoord : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;

                v2f vert(appdata_t v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                    return o;
                }

                half4 frag(v2f i) : SV_Target {
                    half4 col = tex2D(_MainTex, i.texcoord);
                    
                    half gray = dot(col.rgb, half3(0.299, 0.587, 0.114));
                    return half4(gray, gray, gray, col.a);
                }
                ENDCG
            }
        }
 }