// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:True,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:5245,x:33154,y:32447,varname:node_5245,prsc:2|custl-2501-OUT,clip-9652-OUT;n:type:ShaderForge.SFN_Tex2d,id:7646,x:32005,y:32020,ptovrint:True,ptlb:Main Tex,ptin:_MainTex,varname:_MainTex,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:2501,x:32789,y:32686,varname:node_2501,prsc:2|A-7738-OUT,B-745-RGB,T-5330-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:4874,x:30933,y:33123,varname:node_4874,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:7771,x:30933,y:32991,varname:node_7771,prsc:2;n:type:ShaderForge.SFN_Distance,id:9690,x:31114,y:32991,varname:node_9690,prsc:1|A-7771-XYZ,B-4874-XYZ;n:type:ShaderForge.SFN_Multiply,id:7785,x:31473,y:32972,varname:node_7785,prsc:2|A-1773-OUT,B-618-OUT;n:type:ShaderForge.SFN_Negate,id:618,x:31289,y:32991,varname:node_618,prsc:2|IN-9690-OUT;n:type:ShaderForge.SFN_OneMinus,id:4260,x:31977,y:32968,varname:node_4260,prsc:2|IN-2324-OUT;n:type:ShaderForge.SFN_Clamp01,id:9717,x:32367,y:32972,varname:node_9717,prsc:2|IN-4260-OUT;n:type:ShaderForge.SFN_Cubemap,id:899,x:32293,y:32705,ptovrint:False,ptlb:Fog Skybox,ptin:_FogSkybox,varname:_FogSkybox,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0|DIR-7975-OUT;n:type:ShaderForge.SFN_Multiply,id:6260,x:32553,y:32799,varname:node_6260,prsc:2|A-745-A,B-9717-OUT;n:type:ShaderForge.SFN_Negate,id:7975,x:32105,y:32705,varname:node_7975,prsc:2|IN-2497-OUT;n:type:ShaderForge.SFN_ViewVector,id:2497,x:31929,y:32705,varname:node_2497,prsc:2;n:type:ShaderForge.SFN_Clamp,id:6757,x:32891,y:32941,varname:node_6757,prsc:1|IN-6260-OUT,MIN-2714-OUT,MAX-8957-OUT;n:type:ShaderForge.SFN_Vector1,id:2714,x:32707,y:32963,varname:node_2714,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:8957,x:32550,y:33039,ptovrint:False,ptlb:Fog Cutoff,ptin:_FogCutoff,varname:_FogCutoff,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Lerp,id:7738,x:32577,y:32239,varname:node_7738,prsc:1|A-7646-RGB,B-12-OUT,T-4551-OUT;n:type:ShaderForge.SFN_Vector3,id:12,x:32005,y:32258,varname:node_12,prsc:1,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_Set,id:9087,x:32173,y:32133,varname:__textureAlpha,prsc:1|IN-7646-A;n:type:ShaderForge.SFN_Multiply,id:1286,x:32230,y:32421,varname:node_1286,prsc:2|A-3799-OUT,B-8683-OUT;n:type:ShaderForge.SFN_Clamp01,id:4551,x:32393,y:32421,varname:node_4551,prsc:2|IN-1286-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3799,x:32012,y:32421,ptovrint:False,ptlb:Darken Amount,ptin:_DarkenAmount,varname:_DarkenAmount,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Get,id:8683,x:31991,y:32542,varname:node_8683,prsc:2|IN-1645-OUT;n:type:ShaderForge.SFN_Get,id:9652,x:32952,y:32734,varname:node_9652,prsc:2|IN-9087-OUT;n:type:ShaderForge.SFN_Set,id:1645,x:33066,y:32941,varname:__fogAmount,prsc:1|IN-6757-OUT;n:type:ShaderForge.SFN_Get,id:5330,x:32602,y:32744,varname:node_5330,prsc:2|IN-1645-OUT;n:type:ShaderForge.SFN_Power,id:694,x:31653,y:32972,varname:node_694,prsc:2|VAL-5384-OUT,EXP-7785-OUT;n:type:ShaderForge.SFN_Color,id:745,x:32434,y:32627,ptovrint:False,ptlb:Fog Color,ptin:_FogColor,varname:_FogColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:1773,x:31177,y:32853,ptovrint:False,ptlb:Fog Amount,ptin:_FogAmount,varname:_FogAmount,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.3;n:type:ShaderForge.SFN_Slider,id:5384,x:31473,y:32792,ptovrint:False,ptlb:Fog Value,ptin:_FogValue,varname:_FogValue,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Add,id:2324,x:31833,y:32972,varname:node_2324,prsc:2|A-694-OUT,B-3871-OUT;n:type:ShaderForge.SFN_Slider,id:3871,x:31514,y:33141,ptovrint:False,ptlb:No Fog Area,ptin:_NoFogArea,varname:_NoFogArea,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;proporder:7646-899-1773-5384-8957-3799-745-3871;pass:END;sub:END;*/

Shader "Nothing Happens/Deprecated/NH 2D Sprite Cutout (Instanced)" {
    Properties {
        _MainTex ("Main Tex", 2D) = "white" {}
        _FogSkybox ("Fog Skybox", Cube) = "_Skybox" {}
        _FogAmount ("Fog Amount", Range(0, 0.3)) = 0
        _FogValue ("Fog Value", Range(0, 10)) = 1
        _FogCutoff ("Fog Cutoff", Range(0, 1)) = 1
        _DarkenAmount ("Darken Amount", Float ) = 2
        _FogColor ("Fog Color", Color) = (0.5,0.5,0.5,1)
        _NoFogArea ("No Fog Area", Range(0, 2)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="TransparentCutout"
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
            uniform half _FogCutoff;
            uniform half _DarkenAmount;
            uniform float4 _FogColor;
            uniform half _FogAmount;
            uniform half _FogValue;
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
                half4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                half __textureAlpha = _MainTex_var.a;
                clip(__textureAlpha - 0.5);
////// Lighting:
                half __fogAmount = clamp((_FogColor.a*saturate((1.0 - (pow(_FogValue,(_FogAmount*(-1*distance(_WorldSpaceCameraPos,i.posWorld.rgb))))+_NoFogArea)))),0.0,_FogCutoff);
                float3 finalColor = lerp(lerp(_MainTex_var.rgb,half3(0,0,0),saturate((_DarkenAmount*__fogAmount))),_FogColor.rgb,__fogAmount);
                return fixed4(finalColor,1);
            }

            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                half4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                half __textureAlpha = _MainTex_var.a;
                clip(__textureAlpha - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
