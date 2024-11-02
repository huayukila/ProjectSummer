Shader "Unlit/TestShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 modelSpacePos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float _TileSize;
            float _TextureSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.modelSpacePos = v.vertex.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 scale;
                scale.x = length(UNITY_MATRIX_M[0].xyz);
                scale.y = length(UNITY_MATRIX_M[1].xyz);
                scale.z = length(UNITY_MATRIX_M[2].xyz);

                float UV_X = ((i.modelSpacePos.x + 5) * scale.x % _TextureSize) / _TextureSize;
                float UV_Y = ((i.modelSpacePos.z + 5) * scale.z % _TextureSize) / _TextureSize;

                float2 newUV = float2(UV_X, UV_Y);
                fixed4 col = tex2D(_MainTex, newUV);
                return col;
            }
            ENDCG
        }
    }
}