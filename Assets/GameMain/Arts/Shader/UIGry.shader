// UI 变为灰色
Shader "UI/Custom/UIGry"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector" = "True" "RenderType"="Transparent"}
 
        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            ZWrite Off
            
            Blend SrcAlpha OneMinusSrcAlpha // 设置混合模式
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                // fixed4 color : COLOR;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                // fixed4 color : COLOR;
            };
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // o.uv = v.uv;
                // o.color = v.color;
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // 转换为灰度
                // fixed gray = dot(col.rgb, fixed3(0.299, 0.587, 0.114));

                fixed grayValue = (col.r + col.g + col.b) / 3;

                // if (col.a > 0)
                // {
                //     col.a = 0.5;
                // }
                
                return fixed4(grayValue, grayValue, grayValue, col.a);
                // return fixed4(1, 1, 1, 0.5);
                // return col;
            }
            ENDCG
        }
    }
}