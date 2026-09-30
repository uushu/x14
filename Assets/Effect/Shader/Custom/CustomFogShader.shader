Shader "Unlit/CustomFogShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LightColor("LightColor",Color) = (1,0,0,1)
        _LightRange("LightRange",Float) = 100
        _LightLerp("LightLerp",Float) = 100
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _LightLerp;

            uniform float4 _LightPos[100];
            uniform float _LightPosLength;            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float getRenderColor(float2 texPos,float2 lightPos,float lightRange,float lightLerp)
            {
                float2 pos = texPos - lightPos;
                float d = length(pos);
                float rgb;
                rgb = (d-lightRange)/(lightLerp);

                rgb = 1-clamp(rgb, 0.0, 1.0);//clamp意义为 min(max(a, b), c);将a的大小限制在b,c之间， 1-rgb是将颜色反转
	            return rgb;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col*=0.5;
                float Totrgb = 0;
                for(int index=0;index<_LightPosLength;index++)
                {
                    float rgb = getRenderColor(i.vertex,_LightPos[index],_LightPos[index].w,_LightLerp); 
                    Totrgb+=rgb;
                }
                // if(aaa!=0)
                // Totrgb/=aaa;
                
                // temp = max(temp,Totrgb);                
                fixed4 color = fixed4(Totrgb,Totrgb,Totrgb,1);                   
                color = clamp(color,0,1);
                col/= lerp(1,0.5,color);
                return col;
            }
            ENDCG
        }
    }
}
