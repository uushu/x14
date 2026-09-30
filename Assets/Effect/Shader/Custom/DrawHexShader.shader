Shader "Custom/DrawHex"
{
    Properties {
        _outerRadius("六边形大小",Float) = 0.5
        _center("六边形位置",Vector) =(0.5,0.5,0,0)
        _color("颜色",Color) = (1,1,1,1)
        _width("画布宽度",Float) = 1
        _height("画布高度",Float) = 1 
    }

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

    Cull Off
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha

    Pass {  
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
			
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            float _outerRadius;
            float4 _center;
            fixed4 _color;
            float _width;
            float _height;

            fixed4 frag (v2f input) : COLOR
            {
                float _innerRadius = _outerRadius * 0.866025404;
                //周围6个六边形中心点
                float2 nearHexCenterOffset[6] = {
                    float2(_innerRadius,_outerRadius*1.5),
                    float2(_innerRadius*2.0,0),
                    float2(_innerRadius,-_outerRadius*1.5),
                    float2(-_innerRadius,-_outerRadius*1.5),
                    float2(-_innerRadius*2.0,0),
                    float2(-_innerRadius,_outerRadius*1.5),
				};
                fixed2 factor = fixed2(_width,_height);
                input.uv.xy*=factor;
                _center.xy*=factor;
                float mindist = 999;
                for(int i=0;i<6;i++)
                {
                    float2 otherHexCenter = _center+ nearHexCenterOffset[i];
                    float temp=distance(input.uv.xy,otherHexCenter);
                    mindist = min(mindist,temp);
				}

                float dist = distance(input.uv.xy,_center);

                //return fixed4(mindist,0,0,mindist);

                fixed e = step(dist,mindist);
                _color.a = _color.a * e;

                return _color;
            }
        ENDCG
    }
}
}