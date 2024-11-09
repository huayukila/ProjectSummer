Shader "MShaders/PostEffect/FadeInOut"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Fade ("Fade",Range(-2,1)) = 0
        _FadePos ("Fade Pos",Vector) = (0, 0, 0, 0)
        _FadeColor("Fade Color",Color) = (1, 1, 1, 1)

        _FadeTargetX("Fade Target X",float) = 1920
        _FadeTargetY("Fade Target Y",float) = 1080
    }
    SubShader
    {
        Tags 
        {  
            "RenderType"="Transparent"
            "Queue" = "Transparent" 
            "IgnoreProjector" = "True" 
        }

        LOD 100

        Pass
        {
            // Alpha
            Blend SrcAlpha OneMinusSrcAlpha

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

            struct vertexInput
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Fade;
            fixed4 _FadeColor;
            fixed4 _FadePos;

            float _FadeTargetX;
            float _FadeTargetY;

            float insideCircle(float2 uv, float radius)
            {
                return length(uv) - abs(radius);
            }

            float sqrDist(float2 vec1, float2 vec2)
            {
                float2 diff = vec1 - vec2;

                // diff.x * diff.x + diff.y * diff.yと等しい
                return dot(diff,diff);
            }

            vertexInput vert (appdata v)
            {
                vertexInput o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                v.uv += _FadePos.xy * -0.5;

                float2 size = float2(_FadeTargetX,_FadeTargetY);
                // uvを画面サイズに拡張
                v.uv *= size;
                
                // 画面アスペクト比でuvを調整して、横とたてを1：1にする
                // uv範囲[0,1]を[-1,1]にする
                v.uv = (v.uv * 2 - size.xy) / min(_FadeTargetX,_FadeTargetY);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (vertexInput i) : SV_Target
            {
                float aspect = _FadeTargetX / _FadeTargetY;
                
                // 一番遠い隅を計算する
                float2 fadeStartPos = float2(_FadePos.x * aspect, _FadePos.y);
                float maxDist = 0;               
                // 左下
                maxDist = sqrDist(fadeStartPos, float2(-1 * aspect , -1));
                // 左上
                maxDist = max(maxDist , sqrDist(fadeStartPos, float2(-1 * aspect , 1)));
                // 右下
                maxDist = max(maxDist , sqrDist(fadeStartPos, float2(1 * aspect , -1)));
                // 右上
                maxDist = max(maxDist , sqrDist(fadeStartPos, float2(1 * aspect , 1)));

                // 二乗した距離の平方根を計算
                maxDist = sqrt(maxDist);

                // float minVal = min(0,_Fade);
                // float maxVal = max(0,_Fade);

                float alpha = smoothstep(0 , _Fade, insideCircle(i.uv, _Fade * maxDist));
                
                return fixed4(_FadeColor.rgb,alpha);
            }
            ENDCG
        }
    }
}
