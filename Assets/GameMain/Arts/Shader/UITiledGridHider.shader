// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UI/TiledGridHider" 
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _GridCols ("Grid Columns", Int) = 16
        _GridRows ("Grid Rows", Int) = 16
        _HideProgress ("Hide Progress", Range(0,1)) = 0
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil Value", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="UI"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
            #pragma target 3.0
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEX_POSITION1;
                float4 mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            int _GridCols;
            int _GridRows;
            float _HideProgress;
            fixed4 _ClipRect;
            fixed4 _TextureSampleAdd;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                OUT.worldPosition = IN.vertex;
                
                #ifdef UNITY_UI_CLIP_RECT
                OUT.mask = IN.vertex;
                #endif
                
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 color = IN.color;
                
                // 应用RectMask2D裁剪
                #ifdef UNITY_UI_CLIP_RECT
                float2 pixelUV = IN.worldPosition.xy;
                half2 maskUV = saturate((pixelUV - _ClipRect.xy) / (_ClipRect.zw - _ClipRect.xy));
                
                // 如果在裁剪区域外，完全透明
                if (maskUV.x < 0.0 || maskUV.x > 1.0 || maskUV.y < 0.0 || maskUV.y > 1.0)
                {
                    discard;
                }
                
                // 应用Alpha裁剪
                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif
                #endif

                // 计算网格坐标
                float2 gridUV = frac(IN.texcoord * float2(_GridCols, _GridRows));
                float2 cellCoord = floor(IN.texcoord * float2(_GridCols, _GridRows));
                
                // 计算总格子数和当前隐藏进度
                float totalCells = _GridCols * _GridRows;
                float hideCount = _HideProgress * totalCells;
                
                // 计算当前格子索引
                float currentCellIndex = cellCoord.y * _GridCols + cellCoord.x;
                
                // 如果当前格子索引大于等于隐藏数量，则隐藏
                if (currentCellIndex >= hideCount)
                {
                    return fixed4(0, 0, 0, 0);
                }
                
                // 采样纹理，使用网格UV坐标来显示瓦片效果
                fixed4 texColor = tex2D(_MainTex, gridUV) * color;
                
                #ifdef UNITY_UI_CLIP_RECT
                // 应用遮罩
                texColor.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
                
                return texColor;
            }
            ENDCG
        }
    }
}
