// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:True,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:5245,x:33154,y:32447,varname:node_5245,prsc:2|custl-2501-OUT;n:type:ShaderForge.SFN_Lerp,id:2501,x:32789,y:32686,varname:node_2501,prsc:2|A-6456-RGB,B-578-RGB,T-2044-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:4874,x:30933,y:33123,varname:node_4874,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:7771,x:30933,y:32991,varname:node_7771,prsc:2;n:type:ShaderForge.SFN_Distance,id:9690,x:31114,y:32991,varname:node_9690,prsc:1|A-7771-XYZ,B-4874-XYZ;n:type:ShaderForge.SFN_Multiply,id:7785,x:31473,y:32972,varname:node_7785,prsc:2|A-6286-OUT,B-618-OUT;n:type:ShaderForge.SFN_Negate,id:618,x:31289,y:32991,varname:node_618,prsc:2|IN-9690-OUT;n:type:ShaderForge.SFN_OneMinus,id:4260,x:31825,y:32972,varname:node_4260,prsc:2|IN-3435-OUT;n:type:ShaderForge.SFN_Clamp01,id:9717,x:32367,y:32972,varname:node_9717,prsc:1|IN-4260-OUT;n:type:ShaderForge.SFN_Cubemap,id:243,x:32297,y:32753,ptovrint:False,ptlb:Fog Skybox,ptin:_FogSkybox,varname:_FogSkybox,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0|DIR-6863-OUT;n:type:ShaderForge.SFN_Multiply,id:78,x:32543,y:32874,varname:node_78,prsc:2|A-578-A,B-9717-OUT;n:type:ShaderForge.SFN_Negate,id:6863,x:32040,y:32654,varname:node_6863,prsc:1|IN-751-OUT;n:type:ShaderForge.SFN_ViewVector,id:751,x:31856,y:32654,varname:node_751,prsc:2;n:type:ShaderForge.SFN_Clamp,id:2044,x:32955,y:32931,varname:node_2044,prsc:2|IN-78-OUT,MIN-5830-OUT,MAX-158-OUT;n:type:ShaderForge.SFN_Vector1,id:5830,x:32771,y:32953,varname:node_5830,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:158,x:32614,y:33029,ptovrint:False,ptlb:Fog Cutoff,ptin:_FogCutoff,varname:_FogCutoff,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Tex2d,id:6456,x:32591,y:32440,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False|UVIN-5338-OUT;n:type:ShaderForge.SFN_ObjectPosition,id:6798,x:30783,y:32286,varname:node_6798,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:8599,x:30783,y:32413,varname:node_8599,prsc:2;n:type:ShaderForge.SFN_Subtract,id:6262,x:31007,y:32286,varname:node_6262,prsc:2|A-6798-XYZ,B-8599-XYZ;n:type:ShaderForge.SFN_Transform,id:4974,x:31192,y:32286,varname:node_4974,prsc:1,tffrom:0,tfto:3|IN-6262-OUT;n:type:ShaderForge.SFN_ComponentMask,id:604,x:31380,y:32286,varname:node_604,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-4974-XYZ;n:type:ShaderForge.SFN_ComponentMask,id:9464,x:31380,y:32443,varname:node_9464,prsc:2,cc1:2,cc2:-1,cc3:-1,cc4:-1|IN-4974-XYZ;n:type:ShaderForge.SFN_Divide,id:1859,x:31570,y:32286,varname:node_1859,prsc:2|A-604-OUT,B-9464-OUT;n:type:ShaderForge.SFN_ComponentMask,id:750,x:31742,y:32286,varname:node_750,prsc:1,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-1859-OUT;n:type:ShaderForge.SFN_ScreenPos,id:4083,x:31802,y:31955,varname:node_4083,prsc:2,sctp:1;n:type:ShaderForge.SFN_RemapRange,id:9641,x:32160,y:32041,varname:node_9641,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-4083-UVOUT;n:type:ShaderForge.SFN_ScreenParameters,id:5835,x:31380,y:32130,varname:node_5835,prsc:2;n:type:ShaderForge.SFN_Divide,id:5250,x:31570,y:32130,varname:node_5250,prsc:2|A-5835-PXW,B-5835-PXH;n:type:ShaderForge.SFN_Multiply,id:4787,x:31919,y:32201,varname:node_4787,prsc:2|A-5250-OUT,B-750-R;n:type:ShaderForge.SFN_Append,id:7658,x:32081,y:32307,varname:node_7658,prsc:2|A-4787-OUT,B-750-G;n:type:ShaderForge.SFN_RemapRange,id:9587,x:32252,y:32307,varname:node_9587,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-7658-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:5338,x:32472,y:32132,ptovrint:False,ptlb:Use Object Space,ptin:_UseObjectSpace,varname:_UseObjectSpace,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-9641-OUT,B-9587-OUT;n:type:ShaderForge.SFN_Power,id:3435,x:31651,y:32972,varname:node_3435,prsc:2|VAL-9407-OUT,EXP-7785-OUT;n:type:ShaderForge.SFN_Color,id:578,x:32470,y:32661,ptovrint:False,ptlb:Fog Color,ptin:_FogColor,varname:_FogColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:6286,x:31246,y:32836,ptovrint:False,ptlb:Fog Amount,ptin:_FogAmount,varname:_FogAmount,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.01,max:0.1;n:type:ShaderForge.SFN_Slider,id:9407,x:31528,y:32866,ptovrint:False,ptlb:Fog Value,ptin:_FogValue,varname:_FogValue,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:5;proporder:243-6286-9407-158-6456-5338-578;pass:END;sub:END;*/

Shader "Nothing Happens/Deprecated/NH 3D Projection" {
    Properties {
        _FogSkybox ("Fog Skybox", Cube) = "_Skybox" {}
        _FogAmount ("Fog Amount", Range(0, 0.1)) = 0.01
        _FogValue ("Fog Value", Range(0, 5)) = 2
        _FogCutoff ("Fog Cutoff", Range(0, 1)) = 1
        _MainTex ("MainTex", 2D) = "black" {}
        [MaterialToggle] _UseObjectSpace ("Use Object Space", Float ) = 0.5
        _FogColor ("Fog Color", Color) = (0.5,0.5,0.5,1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
            "CanUseSpriteAtlas"="True"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform half _FogCutoff;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform fixed _UseObjectSpace;
            uniform float4 _FogColor;
            uniform float _FogAmount;
            uniform float _FogValue;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
////// Lighting:
                half3 node_4974 = mul( UNITY_MATRIX_V, float4((objPos.rgb-_WorldSpaceCameraPos),0) ).xyz;
                half2 node_750 = (node_4974.rgb.rg/node_4974.rgb.b).rg;
                float2 _UseObjectSpace_var = lerp( (float2(i.screenPos.x*(_ScreenParams.r/_ScreenParams.g), i.screenPos.y).rg*0.5+0.5), (float2(((_ScreenParams.r/_ScreenParams.g)*node_750.r),node_750.g)*0.5+0.5), _UseObjectSpace );
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(_UseObjectSpace_var, _MainTex));
                float3 finalColor = lerp(_MainTex_var.rgb,_FogColor.rgb,clamp((_FogColor.a*saturate((1.0 - pow(_FogValue,(_FogAmount*(-1*distance(_WorldSpaceCameraPos,i.posWorld.rgb))))))),0.0,_FogCutoff));
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
