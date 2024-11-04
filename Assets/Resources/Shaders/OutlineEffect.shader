
Shader "MShaders/OutlineEffect2D"
{
    Properties
    {
        [NoScaleOffset]
        [PreRenderData]_MainTex("Texture", 2D) = "white" {}
         
        // outline properties
        [HDR]
        _OutlineColor("Outline Color",Color) = (1, 1, 1, 1)

        _OutlineWidth("Outline Width",Range(0,10)) = 0

        _OutlineIntense("Outline Intense",Range(0,1)) = 0

        // alpha threshold
        _AlphaThreshold("Alpha Threshold",Range(0,1)) = 0

        // alpha of whole material
        _AlphaAll("Alpha All",Range(0,1)) = 1


    }
    CGINCLUDE

        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile PIXELSNAP_ON

        #include "UnityCG.cginc"

        struct Input
        {
            float4 vertPos : POSITION;
            float2 uv : TEXCOORD0; 
        };

        struct Output
        {
            float4 vertPos : SV_POSITION;
            float2 uv :TEXCOORD0;
        };

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;

        float _OutlineWidth;
        fixed4 _OutlineColor;
        float _OutlineIntense;

        float _AlphaThreshold;
        float _AlphaAll;
        
        float4 outline(float4 vertPos,float outlineWidth)
        {
            float4x4 scale = float4x4
            (
                1 + outlineWidth, 0, 0, 0,
                0, 1 + outlineWidth, 0, 0,
                0, 0, 1 + outlineWidth, 0,
                0, 0, 0, 1 + outlineWidth
            );

            return mul(scale,vertPos);
        }

        Output vertOutline(Input input)
        {
            Output output;
            output.vertPos = UnityObjectToClipPos(input.vertPos);
            #ifdef PIXELSNAP_ON
                output.vertPos = UnityPixelSnap(output.vertPos); 
            #endif
            output.uv = input.uv;

            return output;

        }

    ENDCG

    SubShader
    {       
        Tags 
        { 
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas"="True"
        }
          
        Cull Off
        Lighting Off
        ZWrite Off

        // OUTLINE SHADER
        Pass
        {

            Blend SrcAlpha OneMinusSrcAlpha

            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

            CGPROGRAM


     //        half2 SobelUV[9] = { half2(-1,1),half2(0,1),half2(1,1),
					// half2(-1,0),half2(0,0),half2(1,0),
					// half2(-1,-1),half2(0,-1),half2(1,-1) };
     //        half SobelX[9] = { -1,  0,  1,
					// -2,  0,  2,
					// -1,  0,  1 };
     //        half SobelY[9] = { -1, -2, -1,
					// 0,  0,  0,
					// 1,  2,  1 };

            // fixed Luminance(fixed4 color)
            // {
            //     return color.r * 0.33 + color.g * 0.33 + color.b * 0.34;
            // }
            
            Output vert (Input input)
            {
                return vertOutline(input);
            }

            fixed4 frag (Output output) : SV_Target
            {                
                fixed4 col = tex2D(_MainTex,output.uv);

                float2 up_uv = output.uv + float2(0,1) * _OutlineWidth * _MainTex_TexelSize.xy;
                float2 down_uv = output.uv + float2(0,-1) * _OutlineWidth * _MainTex_TexelSize.xy;
                float2 left_uv = output.uv + float2(-1,0) * _OutlineWidth * _MainTex_TexelSize.xy;
                float2 right_uv = output.uv + float2(1,0) * _OutlineWidth * _MainTex_TexelSize.xy;
                float2 upperRight_uv = output.uv + float2(1,1) * _OutlineWidth * _MainTex_TexelSize.xy;
                float2 lowerRight_uv = output.uv + float2(1,-1) * _OutlineWidth * _MainTex_TexelSize.xy;
                float2 lowerLeft_uv = output.uv + float2(-1,-1) * _OutlineWidth * _MainTex_TexelSize.xy;
                float2 upperLeft_uv = output.uv + float2(-1,1) * _OutlineWidth * _MainTex_TexelSize.xy;

                float w = (tex2D(_MainTex,up_uv).a + tex2D(_MainTex,down_uv).a + tex2D(_MainTex,left_uv).a + tex2D(_MainTex,right_uv).a + tex2D(_MainTex,upperRight_uv).a + tex2D(_MainTex,lowerRight_uv).a + tex2D(_MainTex,lowerLeft_uv).a + tex2D(_MainTex,upperLeft_uv).a);

                float arroundStep = step(0.01,w);

                float pointStep = step(_AlphaThreshold,col.a);

                fixed4 result = lerp(col,_OutlineColor,arroundStep);


                result = lerp(result,col,pointStep);

                if(pointStep < 0.01)
                {
                    result.rbg *= _OutlineIntense;
                }

                clip(result.a - 0.01);

                return result * _AlphaAll;
                
            }
            ENDCG
        }
    }

    
}

// 	Properties
// 	{
// 		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
// 		_Color ("Tint", Color) = (1,1,1,1)
// 		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
// 		_OutLineSpread ("Outline Spread", Range(0, 0.1)) = 0
// 		_Smoothness ("Outline Smoothness", Range(0, 0.5)) = 0
// 	}

// 	SubShader
// 	{
// 		Tags
// 		{ 
// 			"Queue"="Transparent" 
// 			"IgnoreProjector"="True" 
// 			"RenderType"="Transparent" 
// 			"PreviewType"="Plane"
// 			"CanUseSpriteAtlas"="True"
// 		}

// 		Cull Off
// 		Lighting Off
// 		ZWrite Off
// 		Fog { Mode Off }
// 		Blend SrcAlpha OneMinusSrcAlpha

// 		Pass
// 		{
// 		CGPROGRAM
// 			#pragma vertex vert
// 			#pragma fragment frag
// 			#pragma multi_compile DUMMY PIXELSNAP_ON
// 			#include "UnityCG.cginc"
			
// 			struct appdata
// 			{
// 				float4 vertex   : POSITION;
// 				float4 color    : COLOR;
// 				float2 texcoord : TEXCOORD0;
// 			};

// 			struct v2f
// 			{
// 				float4 vertex	: SV_POSITION;
// 				fixed4 color    : COLOR;
// 				float2 texcoord : TEXCOORD0;
// 			};
			
// 			sampler2D _MainTex;
// 			fixed4 _Color;
// 			half _OutLineSpread;
// 			fixed _Smoothness;

// 			v2f vert(appdata IN)
// 			{
// 				fixed scale = 1 + _OutLineSpread * 2;

// 				float2 tex = IN.texcoord * scale;
// 				tex -= (scale - 1) / 2;

// 				v2f OUT;
// 				OUT.vertex = UnityObjectToClipPos(IN.vertex);
// 				OUT.texcoord = tex;
// 				OUT.color = IN.color;
// 				#ifdef PIXELSNAP_ON
// 				OUT.vertex = UnityPixelSnap (OUT.vertex);
// 				#endif

// 				return OUT;
// 			}

// 			sampler2D _AlphaTex;
// 			float _AlphaSplitEnabled;

// 			fixed4 SampleSpriteTexture (float2 uv)
// 			{
// 				fixed4 color = tex2D (_MainTex, uv);

// #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
// 				if (_AlphaSplitEnabled)
// 				{
// 					color.a = tex2D (_AlphaTex, uv).r;
// 				}
// #endif

// 				return color;
// 			}

// 			fixed4 frag(v2f IN) : SV_Target
// 			{
// 				fixed4 base = SampleSpriteTexture(IN.texcoord);
// 				base.rgb *= _Color.rgb;

// 				fixed4 out_col = IN.color;
// 				out_col.a = 1;
// 				half2 line_w = half2(_OutLineSpread, 0);
// 				fixed4 line_col = SampleSpriteTexture(IN.texcoord + line_w.xy)
// 							    + SampleSpriteTexture(IN.texcoord - line_w.xy)
// 								+ SampleSpriteTexture(IN.texcoord + line_w.yx)
// 								+ SampleSpriteTexture(IN.texcoord - line_w.yx);
// 				out_col.a *= line_col.a;
// 				out_col = lerp(base, out_col, max(0, sign(_OutLineSpread)));

// 				fixed4 main_col = base;
// 				main_col = lerp(main_col, out_col, (1 - main_col.a));
// 				main_col.a = IN.color.a * max(0, sign(main_col.a - _Smoothness));
// 				return main_col;
// 			}
// 		ENDCG
// 		}
// 	}


// Properties
//     {
//         [PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
//         _Color ("Tint", Color) = (1,1,1,1)
//         [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

//         Add values to determine if outlining is enabled and outline color.
//          _OutlineSize ("Outline", Float) = 0
//          _OutlineColor("Outline Color", Color) = (1,1,1,1)
//     }

//     CGINCLUDE
//     #include "UnityCG.cginc"

//     struct appdata_t
//     {
//         float4 vertex   : POSITION;
//         float4 color    : COLOR;
//         float2 texcoord : TEXCOORD0;
//     };

//     struct v2f
//     {
//         float4 vertex   : SV_POSITION;
//         fixed4 color    : COLOR;
//         float2 texcoord  : TEXCOORD0;
//     };
//     fixed4 _Color;

//     v2f vert_outline(appdata_t IN)
//     {
//         v2f OUT;
//         OUT.vertex = UnityObjectToClipPos(IN.vertex);
//         OUT.texcoord = IN.texcoord;
//         OUT.color = IN.color * _Color;
//         #ifdef PIXELSNAP_ON
//         OUT.vertex = UnityPixelSnap (OUT.vertex);
//         #endif

//         return OUT;
//     }

//    	sampler2D _MainTex;
//     sampler2D _AlphaTex;

//     float4 _MainTex_TexelSize; magic var
//     float _OutlineSize; outline size
//     fixed4 _OutlineColor; outlie color

//     fixed4 SampleSpriteTexture (float2 uv)
//     {
//         fixed4 color = tex2D (_MainTex, uv);
//         return color;
//     }

// 	ENDCG

//     SubShader
//     {
//         Tags
//         {
//             "Queue"="Transparent"
//             "IgnoreProjector"="True"
//             "RenderType"="Transparent"
//             "PreviewType"="Plane"
//             "CanUseSpriteAtlas"="True"
//         }

//         Cull Off
//         Lighting Off
//         ZWrite Off
//         Blend One OneMinusSrcAlpha

//         outline down
//         Pass
//         {
//        		Offset 1, 1

//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #pragma multi_compile _ PIXELSNAP_ON
//             #pragma shader_feature ETC1_EXTERNAL_ALPHA

//             v2f vert(appdata_t IN)
//             {
//             	return vert_outline(IN);
//             }

//             fixed4 frag(v2f IN) : SV_Target
//             {
//                	fixed4 texColor = SampleSpriteTexture (IN.texcoord + fixed2(0,_MainTex_TexelSize.y*_OutlineSize)) * IN.color;
//                	fixed4 c = _OutlineColor * texColor.a;
//                 return c;
//             }
//             ENDCG
//         }

//         outline up
//         Pass
//         {
//        		Offset 1, 1

//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #pragma multi_compile _ PIXELSNAP_ON
//             #pragma shader_feature ETC1_EXTERNAL_ALPHA

//             v2f vert(appdata_t IN)
//             {
//             	return vert_outline(IN);
//             }

//             fixed4 frag(v2f IN) : SV_Target
//             {

//                	fixed4 texColor = SampleSpriteTexture (IN.texcoord + fixed2(0,- _MainTex_TexelSize.y* _OutlineSize)) * IN.color;
//                	fixed4 c = _OutlineColor * texColor.a;
//                 return c;
//             }
//             ENDCG
//         }

//         outline left
//         Pass
//         {
//        		Offset 1, 1

//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #pragma multi_compile _ PIXELSNAP_ON
//             #pragma shader_feature ETC1_EXTERNAL_ALPHA

//             v2f vert(appdata_t IN)
//             {
//             	return vert_outline(IN);
//             }

//             fixed4 frag(v2f IN) : SV_Target
//             {

//                	fixed4 texColor = SampleSpriteTexture (IN.texcoord + fixed2(_MainTex_TexelSize.x* _OutlineSize,0)) * IN.color;
//                	fixed4 c = _OutlineColor * texColor.a;
//                 return c;
//             }
//             ENDCG
//         }

//         outline right
//         Pass
//         {
//        		Offset 1, 1

//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #pragma multi_compile _ PIXELSNAP_ON
//             #pragma shader_feature ETC1_EXTERNAL_ALPHA

//             v2f vert(appdata_t IN)
//             {
//             	return vert_outline(IN);
//             }

//             fixed4 frag(v2f IN) : SV_Target
//             {

//                	fixed4 texColor = SampleSpriteTexture (IN.texcoord + fixed2(-_MainTex_TexelSize.x* _OutlineSize,0)) * IN.color;
//                	fixed4 c = _OutlineColor * texColor.a;
//                 return c;
//             }
//             ENDCG
//         }

//         Pass
//         {
//         	Offset 0, 0

//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #pragma multi_compile _ PIXELSNAP_ON
//             #pragma shader_feature ETC1_EXTERNAL_ALPHA

//             v2f vert(appdata_t IN)
//             {
//                 v2f OUT;
//                 OUT.vertex = UnityObjectToClipPos(IN.vertex);
//                 OUT.texcoord = IN.texcoord;
//                 OUT.color = IN.color * _Color;
//                 #ifdef PIXELSNAP_ON
//                 OUT.vertex = UnityPixelSnap (OUT.vertex);
//                 #endif

//                 return OUT;
//             }


//             fixed4 frag(v2f IN) : SV_Target
//             {
//                 fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
//                 c.rgb *= c.a;
//                 return c;
//             }
//             ENDCG
//         }
//     }


    //  Properties
    // {
    //     _MainTex("Texture", 2D) = "white" {}
    //     [HDR]
    //     _Color("Color", Color) = (1,1,1,1)
    //     _Factor("Factor", Range(0, 10)) = 1
    //     _SampleRange("Sample Range", Range(0, 10)) = 7
    //     _SampleInterval("Sample Interval", vector) = (1,1,0,0)
    //     _TexSize("Texture Size", vector) = (256,256,0,0)
    // }

    // SubShader
    // {
    //     LOD 200

    //     Tags
    //     {
    //         "Queue" = "Transparent"
    //         "IgnoreProjector" = "True"
    //         "RenderType" = "Transparent"
    //     }

    //     Pass
    //     {
    //         Cull back
    //         Lighting Off
    //         ZWrite Off
    //         Offset -1, -1
    //         Fog { Mode Off }
    //         //ColorMask RGBA
    //         //Blend one zero
    //         Blend SrcAlpha OneMinusSrcAlpha
            
    //         CGPROGRAM
    //         #pragma vertex vert
    //         #pragma fragment frag
    //         #include "UnityCG.cginc"

    //         sampler2D _MainTex;
    //         float4 _Color;
    //         float _Factor;
    //         float _SampleRange;
    //         float2 _TexSize;
    //         float2 _SampleInterval;
    //         float4 _MainTex_TexelSize;
             
    //         struct appdata_t
    //         {
    //             float4 vertex : POSITION;
    //             float2 texcoord : TEXCOORD0;
    //             fixed4 color : COLOR;
    //         };
    
    //         struct v2f
    //         {
    //             float4 vertex : SV_POSITION;
    //             half2 uv : TEXCOORD0;
    //             fixed4 color : COLOR;
    //         };

    //         v2f vert(appdata_t v)
    //         {
    //             v2f o;
    //             o.vertex = UnityObjectToClipPos(v.vertex);
    //             o.uv = v.texcoord;
    //             o.color = v.color;
    //             return o;
    //         }

    //         half4 frag(v2f i) : COLOR
    //         {
    //             int range = (int)_SampleRange;
    //             float radiusX = _SampleInterval.x / _TexSize.x;
    //             float radiusY = _SampleInterval.y / _TexSize.y;
    //             float inner = 0;
    //             float outter = 0;
    //             int count = 0;
    //             //[unroll(15)]
    //             for (int k = -range; k <= range; ++k)
    //             {
    //                 for (int j = -range; j <= range; ++j)
    //                 {
    //                     float4 m = tex2D(_MainTex, float2(i.uv.x + k*radiusX , i.uv.y + j*radiusY));
    //                     outter += 1 - m.a;
    //                     inner += m.a;
    //                     count += 1;
    //                 }
    //             }
    //             inner /= count;
    //             outter /= count;
                
    //             float4 col = tex2D(_MainTex, i.uv) * i.color;
    //             float out_alpha = max(col.a, inner);
    //             //float in_alpha = min(out_alpha, outter);
    //             //col.rgb = _OutColor.rgb * _OutColor.a*(1-col.a) + _InnerColor.rgb*col.a*_InnerColor.a;
    //             //col.a = in_alpha;
    //             //col.rgb = col.rgb + in_alpha * _Factor * _Color.a * _Color.rgb;
    //             col.rgb = col.rgb + (1-col.a) * _Factor * _Color.a * _Color.rgb;
    //             col.a = out_alpha;
    //             return col;
    //         }
    //         ENDCG
    //     }
