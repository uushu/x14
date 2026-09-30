Shader "Sprites/CircleProgress"
{
    Properties
    {
            _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
            _Angle("Angle", range(0,361)) = 360
            _Center("Center", vector) = (.5,.5,0,0)

    }

    SubShader
    {
        LOD 200

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

         Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            Fog { Mode Off }
            Offset -1, -1
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature CLOCK_WISE

            #include "UnityCG.cginc"

            float _Angle;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Center;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };
    
            struct v2f
            {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };
    
            v2f o;

            v2f vert (appdata_t v)
            {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
            }



            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 color=tex2D(_MainTex, IN.texcoord);
                
                float2 pos = IN.texcoord.xy - _Center.xy;
                float ang = degrees(atan2(pos.x,-pos.y)) + 180;
                
                color.a =color.a* step(ang,_Angle);
                
                return color;
            }
            ENDCG
        }
    }
}