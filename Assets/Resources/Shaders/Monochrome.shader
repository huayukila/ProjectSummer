Shader "MShaders/PostEffect/Monochrome"
{
  Properties
  {
    _MainTex("Texture", 2D) = "white" {}
    _EffectRate("Effect Rate",Range(0,1)) = 1
  }
  SubShader
  {
    Tags { "RenderType"="Opaque" }
    LOD 100

    Pass
    {
      Cull Off 
      ZWrite Off 
      ZTest Off

      Stencil
      {
        Ref 1
        Comp NotEqual
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
        UNITY_FOG_COORDS(1)
        float4 vertex : SV_POSITION;
      };

      fixed3 getGrayscale(fixed r, fixed g, fixed b)
      {
        fixed gray = r * 0.299 + g * 0.589 + b * 0.114;
        return fixed3(gray, gray, gray);
      }

      sampler2D _MainTex;
      float4 _MainTex_ST;
      float _EffectRate;

      v2f vert (appdata v)
      {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        UNITY_TRANSFER_FOG(o,o.vertex);
        return o;
      }

      fixed4 frag (v2f i) : SV_Target
      {
        // sample the texture
        fixed4 col = tex2D(_MainTex, i.uv);
        fixed3 grayScale = getGrayscale(col.r, col.g, col.b);

        fixed3 c = lerp(col, grayScale, _EffectRate).rgb;
        return fixed4(c, col.w);
      }
      ENDCG
    }
  }
}
