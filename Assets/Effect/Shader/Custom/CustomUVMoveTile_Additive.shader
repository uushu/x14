Shader "Custom/UVMoveTile/Additive" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (0,0,0,1)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_offsetx ("Scrollx Speed", float) = 0.99
		_offsety ("Scrolly Speed", float) = 0.99
		_Value ("Value", float) = 2.0

		_StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}

	Category 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True"}
		Blend SrcAlpha One
		// AlphaTest Greater .01
		Cull Off 
		Lighting Off
		ZWrite Off

		Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

		SubShader 
		{
			Pass 
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog
				#pragma multi_compile_particles
				#include "UnityUI.cginc"
				#include "UnityCG.cginc"
				#pragma multi_compile __ UNITY_UI_CLIP_RECT

				struct appdata_t 
				{
					half4 vertex : POSITION;
					fixed4 color : COLOR;
					half2 texcoord : TEXCOORD0;
				};

				struct v2f 
				{
					half4 vertex : POSITION;
					fixed4 color : COLOR;
					half2 texcoord : TEXCOORD0;
					float4 worldPosition : TEXCOORD1;
					// UNITY_FOG_COORDS(1)
				};

				uniform sampler2D _MainTex;
				uniform fixed4 _Color;
				uniform half4 _MainTex_ST;
				uniform half _offsetx,_offsety;
				uniform half _Value;
				float4 _ClipRect;
 
				v2f vert (appdata_t v)
				{
					v2f o;
					o.worldPosition = v.vertex;
					o.vertex = UnityObjectToClipPos(o.worldPosition);
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.texcoord = half4(o.texcoord.x + _offsetx * _Time.x , o.texcoord.y + _offsety * _Time.x, 0, 1);
					// UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}
			
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = _Value * i.color * _Color * tex2D(_MainTex, i.texcoord);

					#ifdef UNITY_UI_CLIP_RECT
					col*= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
					#endif

					// UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		} 
	}
}