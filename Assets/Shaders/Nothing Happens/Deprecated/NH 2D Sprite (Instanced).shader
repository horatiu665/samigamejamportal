// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:True,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:5245,x:33300,y:32435,varname:node_5245,prsc:2|custl-2501-OUT,alpha-5979-OUT;n:type:ShaderForge.SFN_Tex2d,id:7646,x:32104,y:32046,ptovrint:True,ptlb:Main Tex,ptin:_MainTex,varname:_MainTex,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:2501,x:32924,y:32675,varname:node_2501,prsc:1|A-6469-OUT,B-9470-RGB,T-9971-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:4874,x:30933,y:33123,varname:node_4874,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:7771,x:30933,y:32991,varname:node_7771,prsc:2;n:type:ShaderForge.SFN_Distance,id:9690,x:31114,y:32991,varname:node_9690,prsc:1|A-7771-XYZ,B-4874-XYZ;n:type:ShaderForge.SFN_Multiply,id:7785,x:31473,y:32972,varname:node_7785,prsc:2|A-2314-OUT,B-618-OUT;n:type:ShaderForge.SFN_Negate,id:618,x:31289,y:32991,varname:node_618,prsc:2|IN-9690-OUT;n:type:ShaderForge.SFN_OneMinus,id:4260,x:31997,y:32972,varname:node_4260,prsc:2|IN-7325-OUT;n:type:ShaderForge.SFN_Clamp01,id:9717,x:32199,y:32972,varname:node_9717,prsc:2|IN-4260-OUT;n:type:ShaderForge.SFN_Cubemap,id:9084,x:32090,y:32635,ptovrint:False,ptlb:Fog Skybox A,ptin:_FogSkyboxA,varname:_FogSkyboxA,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0|DIR-4920-OUT;n:type:ShaderForge.SFN_Multiply,id:4161,x:32545,y:32891,varname:node_4161,prsc:1|A-9470-A,B-9717-OUT;n:type:ShaderForge.SFN_ViewVector,id:1023,x:31744,y:32635,varname:node_1023,prsc:2;n:type:ShaderForge.SFN_Negate,id:4920,x:31923,y:32635,varname:node_4920,prsc:2|IN-1023-OUT;n:type:ShaderForge.SFN_Lerp,id:6469,x:32676,y:32239,varname:node_6469,prsc:1|A-7646-RGB,B-3259-OUT,T-4423-OUT;n:type:ShaderForge.SFN_Vector3,id:3259,x:32104,y:32258,varname:node_3259,prsc:1,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_Set,id:5639,x:32260,y:32126,varname:__textureAlpha,prsc:1|IN-7646-A;n:type:ShaderForge.SFN_Get,id:5644,x:32903,y:32835,varname:_textureAlpha,prsc:1|IN-5639-OUT;n:type:ShaderForge.SFN_Multiply,id:1073,x:32329,y:32421,varname:node_1073,prsc:2|A-426-OUT,B-4236-OUT;n:type:ShaderForge.SFN_Clamp01,id:4423,x:32492,y:32421,varname:node_4423,prsc:2|IN-1073-OUT;n:type:ShaderForge.SFN_ValueProperty,id:426,x:32111,y:32421,ptovrint:False,ptlb:Darken Amount,ptin:_DarkenAmount,varname:_DarkenAmount,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Set,id:5313,x:33049,y:33147,varname:__fogAmount,prsc:1|IN-2793-OUT;n:type:ShaderForge.SFN_Get,id:9971,x:32712,y:32745,varname:node_9971,prsc:2|IN-5313-OUT;n:type:ShaderForge.SFN_Get,id:4236,x:32090,y:32542,varname:node_4236,prsc:2|IN-5313-OUT;n:type:ShaderForge.SFN_Clamp,id:2793,x:32842,y:33147,varname:node_2793,prsc:2|IN-4161-OUT,MIN-2459-OUT,MAX-1066-OUT;n:type:ShaderForge.SFN_Vector1,id:2459,x:32658,y:33169,varname:node_2459,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:1066,x:32501,y:33245,ptovrint:False,ptlb:Fog Cutoff,ptin:_FogCutoff,varname:_FogCutoff,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Power,id:8435,x:31645,y:32972,varname:fogPow,prsc:1|VAL-7112-OUT,EXP-7785-OUT;n:type:ShaderForge.SFN_Color,id:9470,x:32329,y:32581,ptovrint:False,ptlb:Fog Color,ptin:_FogColor,varname:_FogColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Slider,id:2314,x:31008,y:32872,ptovrint:False,ptlb:Fog Amount,ptin:_FogAmount,varname:_FogAmount,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.3;n:type:ShaderForge.SFN_Slider,id:7112,x:31140,y:32743,ptovrint:False,ptlb:Fog Value,ptin:_FogValue,varname:_FogValue,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Multiply,id:5979,x:33087,y:32854,varname:node_5979,prsc:2|A-5644-OUT,B-6018-OUT;n:type:ShaderForge.SFN_Slider,id:6018,x:32746,y:32925,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:_Alpha,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Add,id:7325,x:31822,y:32972,varname:node_7325,prsc:2|A-8435-OUT,B-2275-OUT;n:type:ShaderForge.SFN_Slider,id:2275,x:31406,y:33174,ptovrint:False,ptlb:No Fog Area,ptin:_NoFogArea,varname:_NoFogArea,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_ViewVector,id:2881,x:31577,y:32784,varname:node_2881,prsc:2;n:type:ShaderForge.SFN_Negate,id:9271,x:31756,y:32784,varname:node_9271,prsc:2|IN-2881-OUT;n:type:ShaderForge.SFN_Cubemap,id:9095,x:31923,y:32784,ptovrint:False,ptlb:Fog Skybox B,ptin:_FogSkyboxB,varname:_FogSkyboxB,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0|DIR-9271-OUT;proporder:7646-9084-2314-7112-1066-426-9470-6018-2275;pass:END;sub:END;*/

Shader "Nothing Happens/Deprecated/NH 2D Sprite (Instanced)" {
    Properties {
        _MainTex ("Main Tex", 2D) = "white" {}
        _FogSkyboxA ("Fog Skybox A", Cube) = "_Skybox" {}
        _FogAmount ("Fog Amount", Range(0, 0.3)) = 0
        _FogValue ("Fog Value", Range(0, 10)) = 1
        _FogCutoff ("Fog Cutoff", Range(0, 1)) = 1
        _DarkenAmount ("Darken Amount", Float ) = 2
        _FogColor ("Fog Color", Color) = (1,1,1,1)
        _Alpha ("Alpha", Range(0, 1)) = 1
        _NoFogArea ("No Fog Area", Range(0, 2)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0

            // Enable instancing for this shader
            #pragma multi_compile_instancing

            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform half _DarkenAmount;
            uniform half _FogCutoff;
            uniform float4 _FogColor;
            uniform half _FogAmount;
            uniform half _FogValue;
            uniform half _Alpha;
            uniform half _NoFogArea;

            // appdata
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // v2f
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o;

                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                //VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos (v.vertex);
                return o;
            }

            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                UNITY_SETUP_INSTANCE_ID (i);

                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
                half4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                half __fogAmount = clamp((_FogColor.a*saturate((1.0 - (pow(_FogValue,(_FogAmount*(-1*distance(_WorldSpaceCameraPos,i.posWorld.rgb))))+_NoFogArea)))),0.0,_FogCutoff);
                float3 finalColor = lerp(lerp(_MainTex_var.rgb,half3(0,0,0),saturate((_DarkenAmount*__fogAmount))),_FogColor.rgb,__fogAmount);
                half __textureAlpha = _MainTex_var.a;
                return fixed4(finalColor,(__textureAlpha*_Alpha));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
