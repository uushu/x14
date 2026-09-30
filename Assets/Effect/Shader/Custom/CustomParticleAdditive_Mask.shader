// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Particle/Additive Mask" 
{
	Properties 
	{
		_MainTex ("Particle Texture", 2D) = "white" {}
		_Additive("Additive", Range(0.01, 5.0)) = 1.0
		//_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_MinX("Min X", Float) = -100000
		_MaxX("Max X", Float) = 100000
		_MinY("Min Y", Float) = -100000
		_MaxY("Max Y", Float) = 100000
	}

	Category 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha One
		//AlphaTest Greater .01
		Cull Off 
		Lighting Off 
		ZWrite Off
		Fog { Mode Off }
		
		SubShader 
		{
			Pass 
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				//#pragma multi_compile_particles

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float _Additive;
				float _MinX;
				float _MaxX;
				float _MinY;
				float _MaxY;
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
					float3 vpos : TEXCOORD2;
					//#ifdef SOFTPARTICLES_ON
					//float4 projPos : TEXCOORD1;
					//#endif
				};

				float4 _MainTex_ST;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vpos = mul(unity_ObjectToWorld, v.vertex);
					o.vertex = UnityObjectToClipPos(v.vertex);
					//#ifdef SOFTPARTICLES_ON
					//o.projPos = ComputeScreenPos(o.vertex);
					//COMPUTE_EYEDEPTH(o.projPos.z);
					//#endif
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					//#ifdef SOFTPARTICLES_ON
					//float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
					//float partZ = i.projPos.z;
					//float fade = saturate(_InvFade * (sceneZ - partZ));
					//i.color.a *= fade;
					//#endif
			    	fixed4 c = _Additive * i.color  *  tex2D(_MainTex, i.texcoord);
					c.a *= (i.vpos.x >= _MinX);
					c.a *= (i.vpos.x <= _MaxX);
					c.a *= (i.vpos.y >= _MinY);
					c.a *= (i.vpos.y <= _MaxY);
					return c;
				}
				ENDCG 
			}
		}
	}
}
