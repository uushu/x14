Shader "TaLs/Effectv0.3_BulidIn"
{
    Properties
    { 
        [Header(BlendMode)]
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("CullMode",int)=2
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend",int)=1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DstBlend",int)=1
        [Enum(Off,0,On,1)] _Zwrite("Zwrite",Int)=0
        [Space(5)]
        [Header(Custom)]
        [Toggle] _CustomEnable("CustomEnable",Int)=0
        _Custom("使用说明：使用自定义数据控制，需要在自定义粒子流添加数据到数据显示TEXCOORD1（以数据后面括号内为准）(XY)控制主贴图的UV（Z控制溶解速度）此数值没有任何意义",Int)=0
        [Space(5)]
        [Header(Base)]
        // [Toggle] _BaseEnable("BaseEnable",Int)=0
        [Enum(Repeat,0,Clamp,1)] _WrapMode("WrapMode",Int)=0
        [HDR] _MainColor("MainColor",Color)=(1,1,1,1)
        [HDR] _BackColor("BackColor",Color)=(1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _MaintexUV("MaintexUV(XY)Intersity(Z)",Vector)=(0,0,1,1)
        [Space(5)]
        [Header(Ramp)]
        [Toggle] _RampEnable("MaskEnable",Int)=0
        _RampTex ("RampTex", 2D) = "white" {}
        [Space(5)]
        [Header(Mask)]
        [Toggle] _MaskEnable("MaskEnable",Int)=0
        _MaskTex ("MaskTex", 2D) = "white" {}
        _MasktexUV("MasktexUV(XY)Intersity(Z)",Vector)=(0,0,1,1)
        [Space(5)]
        [Header(Distort)]
        [Toggle] _DistortEnable("DistortEnable",Int)=0
        _DistortTex ("DistortTex", 2D) = "white" {}
        _DistortTexUV("DistortTexUV(XY)Distort(Z)",Vector)=(0,0,1,1)
        [Space(5)]
        [Header(Disslove)]
        [Toggle] _DissloveEnable("DissloveEnable",Int)=0
        _DissloveTex("DissloveTex",2D)="white"{}
        [HDR]_DissloveWidthColor("DissloveWidthColor",Color)=(1,1,1,1)
        _DissloveEdgeWidth("DissloveEdgeWidth",range(0.0001,0.5))=0.05
        _DissloveTexUV("DissslovetTexUV(XY)Disslove(Z)",Vector)=(0,0,0,1)
        [Space(5)]
        [Header(Fresnel)]
        [Toggle] _FresnelEnable("FresnelEnable",Int)=0
        [Toggle] _FresnelMul("Fresnel乘法",Int)=0
        [Enum(Fresnel,1,InvertFresnel,0)] _InvertFresnel("InvertFresnel",Int)=1
        [HDR]_FresnelOutColor("FresnelOutColor",Color)=(1,1,1,1)
        [HDR]_FresnelInsColor("FresnelInsColor",Color)=(1,1,1,1)
        _FresnelWidth("FresnelWidth",Range(0,50))=5
        _FresnelPow("FresnelPow",Range(0,10))=2
        [Space(5)]
        [Header(VertexMove)]
        [Toggle] _VertexEnable("VertexEnable",Int)=0
        _VertexTex ("_VertexTex", 2D) = "white" {}
        _VertexTexUV("_VertexTexUV(XY)MoveValue(Z)",Vector)=(0,0,0.01,1)

        
    }
    SubShader
    { 
        Tags {  "Queue" = "Transparent" "IgnoreProjector" = "True"  }
        LOD 100
        Blend [_SrcBlend] [_DstBlend]
        Cull [_CullMode]
        ZWrite[_Zwrite]


        Pass
        { 
            //Tags{}
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _CUSTOMENABLE_ON
            // #pragma shader_feature _BASEENABLE_ON
            #pragma shader_feature _MASKENABLE_ON
            #pragma shader_feature _DISTORTENABLE_ON
            #pragma shader_feature _FRESNELENABLE_ON
            #pragma shader_feature _FRESNELMUL_ON
            #pragma shader_feature _VERTEXENABLE_ON  
            #pragma shader_feature _DISSLOVEENABLE_ON
            #pragma shader_feature _RAMPENABLE_ON
            #include "UnityCG.cginc"
            float4 _MainTex_ST,_MaskTex_ST,_DistortTex_ST,_MainColor,_MaintexUV,_MasktexUV,_DistortTexUV,_FresnelOutColor,_FresnelInsColor,_VertexTex_ST,_VertexTexUV,_BackColor;
            float _FresnelWidth,_FresnelPow,_WrapMode,_InvertFresnel,_DissloveEdgeWidth;
            float4 _DissloveTex_ST,_DissloveTexUV,_DissloveWidthColor,_RampTex_ST;

            sampler2D _MainTex;sampler2D _MaskTex;sampler2D _DistortTex;sampler2D _VertexTex;sampler2D _DissloveTex;sampler2D _RampTex;
            

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                half3  normal :  NORMAL;
                float4 Color  :  COLOR;
                float4 custom1 :TEXCOORD1;
            };

            struct v2f
            {
                float4 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 uv2    : TEXCOORD1; 
                half3 normalWS: TEXCOORD2;
                float3 positionVS:TEXCOORD3;
                float4 uv3   : TEXCOORD4; 
                float4 Color  :TEXCOORD5;
                float4 custom1 :TEXCOORD6;
            };

            

            v2f vert (appdata v)
            {
                v2f o=(v2f)0;
                #if _CUSTOMENABLE_ON
                    o.custom1= v.custom1;
                #endif
                #if _VERTEXENABLE_ON
                    o.uv2.zw = TRANSFORM_TEX(v.uv, _VertexTex)+_VertexTexUV.xy*_Time.y;
                    v.vertex.xyz+=normalize(v.normal.xyz)*(tex2Dlod ( _VertexTex, float4(o.uv2.zw ,0 ,1)).r)*_VertexTexUV.z;
                    //  v.vertex.xz+=( tex2Dlod( _VertexTex, float4(o.uv2.zw , 0, 0) ).gb)*_VertexTexUV.z;
                #endif
                o.vertex =UnityObjectToClipPos (v.vertex.xyz);
                // #if _BASEENABLE_ON
                
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex)+_MaintexUV.xy*_Time.y;
                // #endif
                #if _MASKENABLE_ON
                    o.uv.zw = TRANSFORM_TEX(v.uv, _MaskTex)+_MasktexUV.xy*_Time.y;
                #endif
                #if _DISTORTENABLE_ON
                    o.uv2.xy = TRANSFORM_TEX(v.uv, _DistortTex)+_DistortTexUV.xy*_Time.y;
                #endif
                #if _FRESNELENABLE_ON
                    o.normalWS=UnityObjectToWorldNormal(v.normal);
                    float3 positionWS= mul(unity_ObjectToWorld,v.vertex.xyz);
                    o.positionVS=normalize(_WorldSpaceCameraPos-positionWS.xyz);
                #endif
                #if _DISSLOVEENABLE_ON
                    o.uv3.xy =TRANSFORM_TEX(v.uv, _DissloveTex)+_DissloveTexUV.xy*_Time.y;
                #endif  
                #if _RAMPENABLE_ON 
                    o.uv3.zw =TRANSFORM_TEX(v.uv, _RampTex);
                #endif
                o.Color =v.Color;
                
                return o;
            }

            float4 frag (v2f i, float face:VFACE) : SV_Target
            {
                
                float4 col=0;
                float2 uv =i.uv.xy;
                
                
                #if _CUSTOMENABLE_ON
                    uv+=i.custom1.xy;
                #endif
                // #if _BASEENABLE_ON
                uv=lerp( uv,saturate(uv),_WrapMode);
                // #endif
                //扭曲贴图
                #if _DISTORTENABLE_ON
                    float4 distorttex = tex2D(_DistortTex,i.uv2.xy);
                    uv =lerp(uv,uv+distorttex.rg,_DistortTexUV.z);
                #endif
                //主贴图
                float4 basecolor=1;
                // #if _BASEENABLE_ON
                
                basecolor = tex2D(_MainTex,uv)*_MaintexUV.z*_MainColor;
                float4  basebackcolor = tex2D(_MainTex,uv)*_MaintexUV.z*_BackColor;
                col=face>0?basecolor:basebackcolor;
                
                
                // #endif
                
                //遮罩贴图
                #if _MASKENABLE_ON
                    float4 mask = tex2D(_MaskTex, i.uv.zw)*_MasktexUV.z;
                    col*=mask;
                    col.a*=mask.r;
                #endif
                //菲涅尔
                #if _FRESNELENABLE_ON
                    half3 N = normalize(i.normalWS);
                    float3 V =normalize(i.positionVS);
                    float vdotn=max(0,dot(V,N)) ;
                    float fresnel= pow(abs(_InvertFresnel-vdotn),_FresnelWidth);
                    float4 fresnelColor =lerp(_FresnelInsColor,_FresnelOutColor,fresnel)*_FresnelPow;
                    // col =lerp(col,fresnelColor,fresnel);
                    col+=fresnelColor;
                    #if _FRESNELMUL_ON   
                        col*=fresnelColor;
                    #endif
                    
                #endif
                //溶解
                #if _DISSLOVEENABLE_ON
                    float disslovevalue =_DissloveTexUV.z;
                    #if _CUSTOMENABLE_ON
                        disslovevalue+=i.custom1.z;
                    #endif
                    float4 distex=tex2D(_DissloveTex,i.uv3.xy);
                    
                    float disslove= smoothstep(distex.r+_DissloveEdgeWidth,distex.r,disslovevalue); 
                    col= lerp(_DissloveWidthColor,col,disslove);
                    col =lerp (0,col,disslove);
                    
                    
                    
                #endif
                #if _RAMPENABLE_ON
                    float4 ramptex = tex2D(_RampTex, i.uv3.zw);
                    col*=ramptex;
                    // return 1;
                #endif   

                //粒子alpha color
                
                col*= i.Color*i.Color.a*basecolor.a;
                col *= face > 0 ?col :_BackColor;
                return col;
            }
            ENDCG
        }

    }
    


}
