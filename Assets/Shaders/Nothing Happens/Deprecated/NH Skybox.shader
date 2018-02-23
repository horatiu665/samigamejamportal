// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:True,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:1,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:0,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:5245,x:33154,y:32447,varname:node_5245,prsc:2|custl-2501-OUT;n:type:ShaderForge.SFN_Lerp,id:2501,x:32789,y:32686,varname:node_2501,prsc:2|A-2592-RGB,B-9231-RGB,T-2044-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:4874,x:30933,y:33123,varname:node_4874,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:7771,x:30933,y:32991,varname:node_7771,prsc:2;n:type:ShaderForge.SFN_Distance,id:9690,x:31114,y:32991,varname:node_9690,prsc:1|A-7771-XYZ,B-4874-XYZ;n:type:ShaderForge.SFN_Multiply,id:7785,x:31473,y:32972,varname:node_7785,prsc:2|A-1660-OUT,B-618-OUT;n:type:ShaderForge.SFN_Negate,id:618,x:31289,y:32991,varname:node_618,prsc:2|IN-9690-OUT;n:type:ShaderForge.SFN_OneMinus,id:4260,x:32013,y:32974,varname:node_4260,prsc:2|IN-811-OUT;n:type:ShaderForge.SFN_Clamp01,id:9717,x:32367,y:32972,varname:node_9717,prsc:1|IN-4260-OUT;n:type:ShaderForge.SFN_Cubemap,id:2592,x:32462,y:32434,ptovrint:False,ptlb:Main Tex,ptin:_MainTex,varname:_MainTex,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0|DIR-6863-OUT;n:type:ShaderForge.SFN_Cubemap,id:243,x:32297,y:32753,ptovrint:False,ptlb:Fog Skybox,ptin:_FogSkybox,varname:_FogSkybox,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0|DIR-6863-OUT;n:type:ShaderForge.SFN_Multiply,id:78,x:32543,y:32874,varname:node_78,prsc:2|A-9231-A,B-9717-OUT;n:type:ShaderForge.SFN_Negate,id:6863,x:32040,y:32654,varname:node_6863,prsc:1|IN-751-OUT;n:type:ShaderForge.SFN_ViewVector,id:751,x:31799,y:32654,varname:node_751,prsc:2;n:type:ShaderForge.SFN_Subtract,id:244,x:32174,y:32279,varname:node_244,prsc:1|A-6160-XYZ,B-7996-OUT;n:type:ShaderForge.SFN_ViewPosition,id:1421,x:31517,y:32368,varname:node_1421,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:6160,x:31678,y:32188,varname:node_6160,prsc:2;n:type:ShaderForge.SFN_Clamp,id:2044,x:32955,y:32931,varname:node_2044,prsc:2|IN-78-OUT,MIN-5830-OUT,MAX-158-OUT;n:type:ShaderForge.SFN_Vector1,id:5830,x:32771,y:32953,varname:node_5830,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:158,x:32614,y:33029,ptovrint:False,ptlb:Fog Cutoff,ptin:_FogCutoff,varname:_FogCutoff,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Negate,id:9025,x:31740,y:32458,varname:node_9025,prsc:2|IN-1421-Y;n:type:ShaderForge.SFN_Append,id:7996,x:31915,y:32414,varname:node_7996,prsc:2|A-1421-X,B-5939-OUT,C-1421-Z;n:type:ShaderForge.SFN_Vector1,id:5939,x:31504,y:32514,varname:node_5939,prsc:2,v1:0;n:type:ShaderForge.SFN_Power,id:7045,x:31659,y:32972,varname:node_7045,prsc:2|VAL-9658-OUT,EXP-7785-OUT;n:type:ShaderForge.SFN_Color,id:9231,x:32468,y:32696,ptovrint:False,ptlb:Fog Color,ptin:_FogColor,varname:_FogColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:1660,x:31224,y:32833,ptovrint:False,ptlb:Fog Amount,ptin:_FogAmount,varname:_FogAmount,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.3;n:type:ShaderForge.SFN_Slider,id:9658,x:31548,y:32839,ptovrint:False,ptlb:Fog Value,ptin:_FogValue,varname:_FogValue,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Add,id:811,x:31843,y:32972,varname:node_811,prsc:2|A-7045-OUT,B-7761-OUT;n:type:ShaderForge.SFN_Slider,id:7761,x:31502,y:33155,ptovrint:False,ptlb:No Fog Area,ptin:_NoFogArea,varname:_NoFogArea,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;proporder:2592-243-1660-9658-158-9231-7761;pass:END;sub:END;*/

Shader "Nothing Happens/Deprecated/NH Skybox" {
    Properties {
        _MainTex ("Main Tex", Cube) = "_Skybox" {}
        _FogSkybox ("Fog Skybox", Cube) = "_Skybox" {}
        _FogAmount ("Fog Amount", Range(0, 0.3)) = 0
        _FogValue ("Fog Value", Range(0, 10)) = 1
        _FogCutoff ("Fog Cutoff", Range(0, 1)) = 1
        _FogColor ("Fog Color", Color) = (0.5,0.5,0.5,1)
        _NoFogArea ("No Fog Area", Range(0, 2)) = 0
    }
    SubShader {
        Tags {
            "Queue"="Background"
            "RenderType"="Opaque"
            "CanUseSpriteAtlas"="True"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Front
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform samplerCUBE _MainTex;
            uniform half _FogCutoff;
            uniform float4 _FogColor;
            uniform half _FogAmount;
            uniform half _FogValue;
            uniform half _NoFogArea;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
////// Lighting:
                half3 node_6863 = (-1*viewDirection);
                float3 finalColor = lerp(texCUBE(_MainTex,node_6863).rgb,_FogColor.rgb,clamp((_FogColor.a*saturate((1.0 - (pow(_FogValue,(_FogAmount*(-1*distance(_WorldSpaceCameraPos,i.posWorld.rgb))))+_NoFogArea)))),0.0,_FogCutoff));
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
