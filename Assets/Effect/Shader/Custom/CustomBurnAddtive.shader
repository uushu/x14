// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Burn/Additive"
{
	Properties
	{
		_MainTex("Main (RGB)", 2D) = "white" {}
		_BurnTex("Burn (RGB)", 2D) = "white" {}
		_TintColor("Tint Color", Color) = (1,1,1,1)
		_BurnColor("Burn Color",Color) = (0.3, 0.4, 0.7, 1.0)
		_Cutout("Cut Out",Range(0, 1.0)) = .0
	}

		SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Pass
		{
			Lighting Off
			Zwrite Off
			Cull Off
			Fog{ Mode Off }
			Blend SrcAlpha One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoordBurn : TEXCOORD1;
			};

			uniform sampler2D _MainTex;
			uniform sampler2D _BurnTex;
			float4 _MainTex_ST;
			float4 _BurnTex_ST;
			uniform fixed4 _TintColor;
			uniform fixed4 _BurnColor;
			uniform fixed _Cutout;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoordBurn = TRANSFORM_TEX(v.texcoord, _BurnTex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) * _TintColor;
				fixed4 burnValue = tex2D(_BurnTex, i.texcoordBurn);
				fixed value = (burnValue.r + burnValue.g + burnValue.b) * 0.33;
				value = value - _Cutout;
				clip(value);
				fixed4 burn = saturate(_BurnColor * step(value,0.1));
				burn.g += burn.g * -value * 10;
				col = col + burn;
				return col;
			}
			ENDCG
		}
	}
		Fallback Off
}