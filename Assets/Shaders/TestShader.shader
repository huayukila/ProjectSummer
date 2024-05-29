Shader "Unlit/TestShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _TileSize ("Tile Size", Float) = 1.0
        _TextureSize("TextureSize",Float)=1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

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

            sampler2D _MainTex;
            float _TileSize;
            float _TextureSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float UV_X = (i.worldPos.x % _TextureSize) / _TextureSize;
                float UV_Y = (i.worldPos.z % _TextureSize) / _TextureSize;

                i.uv.x = UV_X;
                i.uv.y = UV_Y;
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}