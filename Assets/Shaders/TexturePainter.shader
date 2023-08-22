Shader "LSQ/Effect/Paint/TexturePainter"
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

			sampler2D _MainTex;

            float3 _Position;
            float4 _Color;
            float _Radius;
            float _Hardness;
            float _Strength;

            int _DebugUV;

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

            float mask(float3 position, float3 center, float radius, float hardness)
            {
                float m = distance(center, position);
                return 1 - smoothstep(radius * hardness, radius, m);    
            }

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
                if (_DebugUV)
                {
                    return 1;
                }

                float4 col = tex2D(_MainTex, i.uv);
                float f = mask(i.worldPos, _Position, _Radius, _Hardness);
                float edge = f * _Strength;
                return lerp(col, _Color, edge);
            }
            ENDCG
        }
    }
}