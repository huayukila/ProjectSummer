Shader "Paint/PolygonPainter"
{   
    SubShader
    {
        Cull Off ZWrite Off ZTest Off

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };
            
            float mask(int max, float4 worldPosArray[100], float4 center)
            {
                float2 extreme=float2(100000,center.y);
                int count=0;

                for(int i=0;i<max;i++){
                    float4 vertex1=worldPosArray[i];
                    float4 vertex2 = worldPosArray[(i + 1) % max];
                    
                        if ((vertex1.z > center.z) != (vertex2.z > center.z) &&
                        center.x < (vertex2.x - vertex1.x) * (center.z - vertex1.z)
                        /(vertex2.z - vertex1.z) + vertex1.x)
                        {
                            count++;
                        }
                }
                return (count%2)==1?1.0f:0.0f;
            }

            sampler2D _MainTex;

            float4 _Color;
            vector _worldPosList[100];
            int _MaxVertNum;


            v2f vert (appdata v)
            {
                v2f o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
				float4 uv = float4(0, 0, 0, 1);
                uv.xy = float2(1, _ProjectionParams.x) * (v.uv.xy * 2 - 1);
				o.vertex = uv; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float f = mask(_MaxVertNum, _worldPosList,i.worldPos);
                return lerp(col, _Color, f);
            }
            ENDCG
        }
    }
}