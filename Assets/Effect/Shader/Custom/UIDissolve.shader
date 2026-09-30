Shader "UI/Dissolve"
{
    //
    Properties
    {
        _BurnMap ("Burn Map", 2D) = "white" { }
        _InnerRingRange ("Inner Ring Range", Range(0, 0.3)) = 0.3
        _OuterRingRange ("Outer Ring Range", Range(0, 0.3)) = 0.1

        _InnerRingColor ("Inner Ring Color", Color) = (1, 1, 1, 1)
        _OuterRingColor ("Outer Ring Color", Color) = (0, 0, 0, 1)

        _RimPower ("Rim Power", Range(1, 10)) = 5

        _OutLightStrength ("Transparency", Float) = 15
        //光晕强度

        /*unity ui*/
        [PerRendererData] _MainTex ("Sprite Texture",2D)="white" { }
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        _Additive ("Additive", Range(0.01, 5)) = 1
        _Color ("Color", Color) = (1, 1, 1, 1)
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        /*unity ui end*/
    }

    SubShader
    {
        /*unity ui*/
        Tags
        {
            "Queue" = "Transparent"
            // UI存在透明图片，所以渲染队列是 Transparent 
            "IgnoreProjector" = "True"
            // 忽略投影器Projector
            "RenderType" = "Transparent"
            // 渲染模式
            "PreviewType" = "Plane"
            // UI预览的正常都是一个平面(Plane) 
            "CanUseSpriteAtlas" = "True"
            // UI经常使用图集,所以要设置图集可以使用
        }

        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] }

        Cull Off
        // 剔除关闭
        Lighting Off
        // 光照关闭
        ZWrite Off
        // 深度写入关闭
        ZTest [unity_GUIZTestMode]
        // 用于UI组件的shader都要包含一句：ZTest [unity_GUIZTestMode]，以确保UI能在前层显示
        Blend SrcAlpha OneMinusSrcAlpha
        // 混合模式是 OneMinusSrcAlpha 正常模式(透明度混合)
        ColorMask [_ColorMask]

        // 将定义的_ColorMask参数导入
        Pass
        {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #include "UnityUI.cginc"
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : Color;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float amount : TEXCOORD1;
                float4 worldPosition : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BurnMap;
            float4 _BurnMap_ST;
            fixed _Amount;
            float _InnerRingRange;
            float _OuterRingRange;
            fixed4 _OuterRingColor;
            fixed4 _InnerRingColor;
            float _OutLightStrength;
            float _RimPower;
            fixed4 _Color;
            float _Additive;

            float4 _ClipRect;
            float4 rectUV;

            fixed2 comuv(fixed2 uv)
            {
                fixed2 r = uv;
                r.x = rectUV.x + (rectUV.z - rectUV.x) * r.x;
                r.y = rectUV.y + (rectUV.w - rectUV.y) * r.y;
                return r;
            }

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                // 实例化处理
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.uv, _BurnMap);
                o.worldPosition = o.vertex;

                o.amount = v.color.a;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {

                fixed4 burnColor = tex2D(_BurnMap, i.uv.zw);
                float progress = (1 + _InnerRingRange + _OuterRingRange) * i.amount;
                float value = burnColor.r - (progress - _InnerRingRange - _OuterRingRange);

                #ifdef UNITY_UI_CLIP_RECT
                value *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif
                clip(value);

                fixed2 r = comuv(i.uv.xy);
                fixed4 col = tex2D(_MainTex, r);
                fixed alpha = col.a;

                if (value < _OuterRingRange)
                {
                    col = _OuterRingColor;
                    col.a *= pow((1 + value), _RimPower);
                    col *= _OutLightStrength * value;
                }else
                 if (value < _InnerRingRange + _OuterRingRange)
                //内圈
                {
                    col = _InnerRingColor;
                    col.a *= pow((1 + value), _RimPower);
                    col *= _OutLightStrength * value;
                }
                col.a *= alpha;
                col.rgb *= _Color.rgb;
                col.rgb *= _Additive;

                return col;
            }
            ENDCG
        }
    }
}