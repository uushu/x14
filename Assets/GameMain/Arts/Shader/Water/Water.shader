Shader "Unlit/Water"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color3("Color3", Color) = (1,1,1,1)
        _Color2("Color2", Color) = (1,1,1,1)
        _Color1("Color1", Color) = (1,1,1,1)
        _Color0("Color0", Color) = (1,1,1,1)
        _FillAmount("FillAmount", Range(-0.5, 0.5)) = 0
        _SARM("SARM", float) = 4.35
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "DisableBatching" ="True"}
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        LOD 100
        Cull Off
        ZTest LEqual
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma target 2.0
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ws:TEXCOORD1;
                float3 os:TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color3;
            float4 _Color2;
            float4 _Color1;
            float4 _Color0;
            float _FillAmount;
            float _SARM;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.ws = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.os = v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float3 center = unity_ObjectToWorld._14_24_34;

                float y = (i.ws - center).y;

                float l1 = _SARM * -0.25;
                float s1 = step(y, l1);
                float4 lerpClor = lerp(_Color1, _Color0, s1);

                float l2 = _SARM * -0.065;
                float s2 = step(y, l2);
                lerpClor = lerp(_Color2, lerpClor, s2);

                float l3 = _SARM * 0.13;
                float s3 = step(y, l3);
                lerpClor = lerp(_Color3, lerpClor, s3);

                float alpha = _FillAmount * _SARM;
                lerpClor.a = step(y, alpha);

                return lerpClor * col;
            }
            ENDCG
        }
    }
}
