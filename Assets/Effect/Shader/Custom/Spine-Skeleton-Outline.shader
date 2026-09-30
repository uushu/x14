Shader "SpineEx/Outline/Skeleton" {
	Properties {
		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
		[NoScaleOffset] _MainTex ("Main Texture", 2D) = "black" {}
		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		[HideInInspector] _StencilRef("Stencil Reference", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8 // Set to Always as default

		// Outline properties are drawn via custom editor.
		_OutlineWidth("Outline Width", Range(0,100)) = 3.0
		_OutlineColor("Outline Color", Color) = (1,1,0,1)
		_OutlineReferenceTexWidth("Reference Texture Width", Int) = 1024
		_ThresholdEnd("Outline Threshold", Range(0,1)) = 0.25
		_OutlineSmoothness("Outline Smoothness", Range(0,1)) = 1.0
		[MaterialToggle(_USE8NEIGHBOURHOOD_ON)] _Use8Neighbourhood("Sample 8 Neighbours", Float) = 1
		_OutlineOpaqueAlpha("Opaque Alpha", Range(0,1)) = 1.0
		_OutlineMipLevel("Outline Mip Level", Range(0,3)) = 0
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }

		Fog { Mode Off }
		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Lighting Off

		Stencil {
			Ref[_StencilRef]
			Comp[_StencilComp]
			Pass Keep
		}

		Pass {
			Name "Outline"
			CGPROGRAM
			#pragma vertex vertOutline
			#pragma fragment fragOutline
			#pragma shader_feature _ _USE8NEIGHBOURHOOD_ON

			
			#ifndef SPINE_OUTLINE_PASS_INCLUDED
			#define SPINE_OUTLINE_PASS_INCLUDED

			#include "UnityCG.cginc"

			#ifdef SKELETON_GRAPHIC
			#include "UnityUI.cginc"
			#endif

			#include "CGIncludes/Spine-Outline-Common.cginc"

			sampler2D _MainTex;

			float _OutlineWidth;
			float4 _OutlineColor;
			float4 _MainTex_TexelSize;
			float _ThresholdEnd;
			float _OutlineSmoothness;
			float _OutlineOpaqueAlpha;
			float _OutlineMipLevel;
			int _OutlineReferenceTexWidth;

			#ifdef SKELETON_GRAPHIC
			float4 _ClipRect;
			#endif

			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float vertexColorAlpha : COLOR;
			#ifdef SKELETON_GRAPHIC
				float4 worldPosition : TEXCOORD1;
			#endif
				UNITY_VERTEX_OUTPUT_STEREO
			};


			#ifdef SKELETON_GRAPHIC
			VertexOutput vertOutlineGraphic(VertexInput v) {
				VertexOutput o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.worldPosition = v.vertex;
				o.pos = UnityObjectToClipPos(o.worldPosition);
				o.uv = v.uv;

			#ifdef UNITY_HALF_TEXEL_OFFSET
				o.pos.xy += (_ScreenParams.zw - 1.0) * float2(-1, 1);
			#endif

				o.vertexColorAlpha = v.vertexColor.a;
				return o;
			}

			#else // !SKELETON_GRAPHIC

			VertexOutput vertOutline(VertexInput v) {
				VertexOutput o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.vertexColorAlpha = v.vertexColor.a;
				return o;
			}
			#endif

			float4 fragOutline(VertexOutput i) : SV_Target {

				float4 texColor = computeOutlinePixel(_MainTex, _MainTex_TexelSize.xy, i.uv, i.vertexColorAlpha,
					_OutlineWidth, _OutlineReferenceTexWidth, _OutlineMipLevel,
					_OutlineSmoothness, _ThresholdEnd, _OutlineOpaqueAlpha, _OutlineColor);

			#ifdef SKELETON_GRAPHIC
				texColor *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
			#endif

				return texColor;
			}

			#endif // SPINE_OUTLINE_PASS_INCLUDED

			
			ENDCG
		}
		
		Pass {
			CGPROGRAM
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma shader_feature _ _BRIGHTZ_ON
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			fixed _BrightZ;

			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.vertexColor = v.vertexColor;
				return o;
			}

			float4 frag (VertexOutput i) : COLOR {
				float4 texColor = tex2D(_MainTex, i.uv);

				#if defined(_STRAIGHT_ALPHA_INPUT)
				texColor.rgb *= texColor.a;
				#endif
				float4 color = (texColor * i.vertexColor);

				#if defined(_BRIGHTZ_ON)
				color.rgb*=_BrightZ*i.pos.z;
				#endif
				return color;
			}
			ENDCG
		}

		Pass {
			Name "Caster"
			Tags { "LightMode"="ShadowCaster" }
			Offset 1, 1
			ZWrite On
			ZTest LEqual

			Fog { Mode Off }
			Cull Off
			Lighting Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			fixed _Cutoff;

			struct VertexOutput { 
				V2F_SHADOW_CASTER;
				float2 uv : TEXCOORD1;
			};

			VertexOutput vert (appdata_base v) {
				VertexOutput o;
				o.uv = v.texcoord;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}

			float4 frag (VertexOutput i) : COLOR {
				fixed4 texcol = tex2D(_MainTex, i.uv);
				clip(texcol.a - _Cutoff);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
}
