Shader "Custom/2DInline"
{
    Properties
    {
        _MainTex("MainTex",2D) = "white"{}
        _InlineColor("InlineColor",Color) = (0,0,0,1)
        _InlineWidth("InlineWidth",Range(0,10)) = 1
    }

    SubShader{
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha

        Pass{
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"                                                                                                                        

            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f{
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _InlineColor;
            float _InlineWidth;
            
            v2f vert(appdata a){
                v2f v;
                v.pos = UnityObjectToClipPos(a.vertex);
                v.uv = TRANSFORM_TEX(a.uv,_MainTex);
                return v;
            }

            fixed4 frag(v2f v) : SV_TARGET{
                 
                 float4 col = tex2D(_MainTex,v.uv);

				 //获取周围上下左右4个点的uv
                 float2 up_uv = v.uv + float2(0,_InlineWidth * _MainTex_TexelSize.y);
                 float2 down_uv = v.uv + float2(0,-_InlineWidth * _MainTex_TexelSize.y);
                 float2 left_uv = v.uv + float2(-_InlineWidth * _MainTex_TexelSize.x,0);
                 float2 right_uv = v.uv + float2(_InlineWidth * _MainTex_TexelSize.x,0);
                 
                 //根据uv,获取周围上下左右4个点的alpha乘积
                 float arroundAlpha = tex2D(_MainTex,up_uv).a *
                 tex2D(_MainTex,down_uv).a *
                 tex2D(_MainTex,left_uv).a *
                 tex2D(_MainTex,right_uv).a;
                 
                 //让描边变色
                 float4 result = lerp(_InlineColor,col,arroundAlpha);
                 //使用原来的透明度
                 result.a = col.a;
                                 
                 return result;               
            }

            ENDCG
        }

    }
    FallBack "Sprites/Default"
}