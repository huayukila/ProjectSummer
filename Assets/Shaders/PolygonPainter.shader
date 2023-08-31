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

            //float mask(int max, float4 worldPosArray[100], float testx, float testy)
            //{
	           // int i, j;
	           // float c=0.0f;
	           // float vertx[100];
	           // float verty[100];

	           // for(int n=0;n<max;n++)
	           // {
	           //     vertx[n] = worldPosArray[n].x;
	           //     verty[n] = worldPosArray[n].y;
	           // }
	           // for (i = 0, j = max-1; i < max; j = i++) {
	           // if ( ((verty[i]>testy) != (verty[j]>testy)) && (testx < (vertx[j]-vertx[i]) * (testy-verty[i]) / (verty[j]-verty[i]) + vertx[i]) )
	           //    c = 1.0f;
	           // }
	           // return c;
            //}
            
            float mask(int max, float4 worldPosArray[100], float4 center)
            {
                float2 extreme=float2(100000,center.y);
                int count=0;

                for(int i=0;i<max;i++){
                    float2 vertex1 = worldPosArray[i];
                    float2 vertex2 = worldPosArray[(i + 1) % max];
                    if ((vertex1.y > center.y) != (vertex2.y > center.y) && center.x < (vertex2.x - vertex1.x) * (center.y - vertex1.y) / (vertex2.y - vertex1.y) + vertex1.x)
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
                //float f = mask(_MaxVertNum, _worldPosList,i.worldPos.x,i.worldPos.y);
                float f = mask(_MaxVertNum, _worldPosList,i.worldPos);
                return lerp(col, _Color, f);
            }
            ENDCG
        }
    }
}