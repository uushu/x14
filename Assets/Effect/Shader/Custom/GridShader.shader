Shader "Custom/GridShader"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            

            struct appdata
            {
                float4 vertex : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                half2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float width = 100;
				float height = 100;
				/*


				int stepX=i.uv.x*width;
				stepX = stepX%2;

				int stepY=i.uv.y*height;
				stepY = stepY%2;

				return stepX==stepY?fixed4(0,0,0,0.5):fixed4(0,0,0,0);
				*/

				float cu=0.03;
				int stepX = i.uv.x*width;
				float offsetX=i.uv.x*width-stepX;

				int stepY=i.uv.y*height;
				float offsetY=i.uv.y*height-stepY;

				if(offsetX<cu || offsetY<cu)
					return fixed4(0,0,0,0.1);
				else
					return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}
