// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CircleSprite"
{
    Properties
    {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
        _CenterX("CenterX", Float) = 0.0
        _CenterY("CenterY", Float) = 0.0
        _Width("Width", Float) = 0.0
        _Height("Height", Float) = 0.0
        _LengthRate("LengthRate", Float) = 0.0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

		[Toggle] _gray("启用变灰", Int) = 0
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }
    
    SubShader
    {
        LOD 200

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
		    "CanUseSpriteAtlas" = "True"
        }

        Stencil
		{
		    Ref [_Stencil]
		    Comp [_StencilComp]
		    Pass [_StencilOp]
		    ReadMask [_StencilReadMask]
		    WriteMask [_StencilWriteMask]
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
            
            #pragma shader_feature _GRAY_ON
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _CenterX;
            float _CenterY;
            float _Width;
            float _Height;
            float _LengthRate;
			float4 _ClipRect;

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
                float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
            };
    
            v2f vert (appdata_t v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
				o.worldPosition = v.vertex;
                o.color = v.color;
                return o;
            }
                
            fixed4 frag (v2f IN) : COLOR
            {
                float2 pt = IN.texcoord-float2(_CenterX,_CenterY);
                float length = (pt.x*pt.x)/(_Width*_Width)+(pt.y*pt.y)/(_Height*_Height);
                fixed alpha= step(length,_LengthRate);
                
                fixed4 color= tex2D(_MainTex, IN.texcoord) * IN.color;

                #ifdef _GRAY_ON
				color.rgb = dot(color.rgb, fixed3(0.222, 0.707, 0.071));  //0.299, 0.587, 0.114
                #endif

                color= fixed4(color.r,color.g,color.b,alpha*color.a);

                #ifdef UNITY_UI_CLIP_RECT
				color.a*= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				#endif

				#ifdef UNITY_UI_ALPHACLIP
				clip(color.a - 0.001);
				#endif

                return color;
            }
            ENDCG
        }
    }

}
