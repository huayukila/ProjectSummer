Shader "Paint/Paintable"
{
    Properties
    {
        [Head(Render)]
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1) // Define _Color as a Color property
        _BumpMap ("Normal", 2D) = "white" {} 

        [Head(Paint)]
        _MaskTex ("Mask (RGB)", 2D) = "black" {}
        _Emission ("Emission", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            sampler2D _BumpMap;
            sampler2D _MaskTex;

            half4 _Color; // Declare _Color as a variable

            half _Emission;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            struct CustomSurfaceOutput
            {
                half3 Albedo;
                half3 Normal;
                half3 Emission; // Custom emission property
            };

            CustomSurfaceOutput frag (v2f i) : SV_Target
            {
                // Albedo comes from a texture tinted by color
                fixed4 c = tex2D(_MainTex, i.uv) * _Color; // Use _Color
                fixed4 mask = tex2D(_MaskTex, i.uv);

                CustomSurfaceOutput o;
                o.Albedo = lerp(c.rgb, mask.rgb, mask.a);
                o.Normal = UnpackNormal(tex2D(_BumpMap, i.uv));
                o.Emission = _Emission; // Set the custom emission property

                return o;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
