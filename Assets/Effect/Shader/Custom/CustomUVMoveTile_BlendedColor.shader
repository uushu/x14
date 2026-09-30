// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UVMoveTile/Alpha Blended (Color)"
{
	Properties
	{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,1.0)
		_MainTex("Particle Texture", 2D) = "white" {}
		_Additive("Additive", Range(0.01, 5.0)) = 2.0
		_offsetx ("Scrollx Speed", float) = 0.99
		_offsety ("Scrolly Speed", float) = 0.99
		//_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0

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
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		//AlphaTest Greater .01
		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }

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
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile_fog
				#pragma multi_compile_particles
				#include "UnityUI.cginc"
				#include "UnityCG.cginc"
				#pragma multi_compile __ UNITY_UI_CLIP_RECT
				//#pragma multi_compile_particles

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				fixed4 _TintColor;
				float _Additive;
				float4 _ClipRect;

				struct appdata_t
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float4 worldPosition : TEXCOORD1;
					//#ifdef SOFTPARTICLES_ON
					//float4 projPos : TEXCOORD1;
					//#endif
				};

				float4 _MainTex_ST;
				uniform half _offsetx,_offsety;

				v2f vert(appdata_t v)
				{
					v2f o;
					o.worldPosition = v.vertex;
					o.vertex = UnityObjectToClipPos(o.worldPosition);
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.texcoord = half4(o.texcoord.x + _offsetx * _Time.x , o.texcoord.y + _offsety * _Time.x, 0, 1);
					return o;
				}

				fixed4 frag(v2f i) : COLOR
				{
					//#ifdef SOFTPARTICLES_ON
					//float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
					//float partZ = i.projPos.z;
					//float fade = saturate(_InvFade * (sceneZ - partZ));
					//i.color.a *= fade;
					//#endif
					fixed4 col = _Additive * i.color * _TintColor *  tex2D(_MainTex, i.texcoord);

					#ifdef UNITY_UI_CLIP_RECT
					col*= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
					#endif

					#ifdef UNITY_UI_ALPHACLIP
					clip (col.a - 0.001);
					#endif

					return col;
				}
				ENDCG
			}
		}
	}
}
