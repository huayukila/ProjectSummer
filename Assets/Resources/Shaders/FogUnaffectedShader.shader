Shader "MShaders/PostEffect/FogUnaffectedShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlinkColor("Blink Color",Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"= "Transparent" 
            "Queue" = "Transparent" 
            "IgnoreProjector" = "True"
        }

        Lighting Off
        Cull Off 
        ZWrite Off

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            //AlphaToMask On
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace

            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _BlinkColor;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex,i.uv);

                clip(col.a - 0.0001);

                return col + fixed4(_BlinkColor.rbg,0);
            }
            ENDCG
        }
    }
}
