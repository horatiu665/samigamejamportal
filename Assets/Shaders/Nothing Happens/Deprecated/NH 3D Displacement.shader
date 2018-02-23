// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:True,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:1,fgcg:1,fgcb:1,fgca:1,fgde:0.01,fgrn:10,fgrf:12,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:5245,x:33510,y:32445,varname:node_5245,prsc:2|custl-2501-OUT,disp-456-OUT,tess-8935-OUT;n:type:ShaderForge.SFN_Tex2d,id:7646,x:32530,y:32503,ptovrint:True,ptlb:Main Tex,ptin:_MainTex,varname:_MainTex,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:2501,x:32789,y:32686,varname:node_2501,prsc:2|A-7646-RGB,B-4389-RGB,T-1327-OUT;n:type:ShaderForge.SFN_Clamp01,id:9717,x:32367,y:32972,varname:node_9717,prsc:2|IN-2448-OUT;n:type:ShaderForge.SFN_Cubemap,id:899,x:32293,y:32705,ptovrint:False,ptlb:Fog Skybox,ptin:_FogSkybox,varname:_FogSkybox,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0|DIR-7975-OUT;n:type:ShaderForge.SFN_Multiply,id:6260,x:32530,y:32856,varname:node_6260,prsc:2|A-4389-A,B-9717-OUT;n:type:ShaderForge.SFN_Negate,id:7975,x:32105,y:32705,varname:node_7975,prsc:2|IN-2497-OUT;n:type:ShaderForge.SFN_ViewVector,id:2497,x:31929,y:32705,varname:node_2497,prsc:2;n:type:ShaderForge.SFN_Clamp,id:1327,x:32922,y:32930,varname:node_1327,prsc:2|IN-6260-OUT,MIN-93-OUT,MAX-2195-OUT;n:type:ShaderForge.SFN_Vector1,id:93,x:32738,y:32952,varname:node_93,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:2195,x:32581,y:33028,ptovrint:False,ptlb:Fog Cutoff,ptin:_FogCutoff,varname:_FogCutoff,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:8935,x:33453,y:33030,ptovrint:False,ptlb:Tesselation,ptin:_Tesselation,varname:_Tesselation,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:32;n:type:ShaderForge.SFN_Tex2d,id:1623,x:32849,y:33206,ptovrint:False,ptlb:Displacement Texture,ptin:_DisplacementTexture,varname:_DisplacementTexture,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_NormalVector,id:3581,x:32849,y:33410,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:456,x:33080,y:33206,varname:node_456,prsc:2|A-1623-RGB,B-3581-OUT,C-9450-OUT;n:type:ShaderForge.SFN_Slider,id:9450,x:32692,y:33580,ptovrint:False,ptlb:Displacement,ptin:_Displacement,varname:_Displacement,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Color,id:4389,x:32459,y:32687,ptovrint:False,ptlb:Fog Color,ptin:_FogColor,varname:_FogColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_FragmentPosition,id:7726,x:30800,y:33155,varname:node_7726,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:8162,x:30800,y:33023,varname:node_8162,prsc:2;n:type:ShaderForge.SFN_Distance,id:6152,x:30981,y:33023,varname:node_6152,prsc:1|A-8162-XYZ,B-7726-XYZ;n:type:ShaderForge.SFN_Multiply,id:3263,x:31340,y:33004,varname:node_3263,prsc:2|A-2578-OUT,B-8325-OUT;n:type:ShaderForge.SFN_Negate,id:8325,x:31156,y:33023,varname:node_8325,prsc:2|IN-6152-OUT;n:type:ShaderForge.SFN_OneMinus,id:2448,x:31896,y:33006,varname:node_2448,prsc:2|IN-5233-OUT;n:type:ShaderForge.SFN_Power,id:5433,x:31524,y:33004,varname:node_5433,prsc:2|VAL-7168-OUT,EXP-3263-OUT;n:type:ShaderForge.SFN_Slider,id:2578,x:31122,y:32904,ptovrint:False,ptlb:Fog Amount,ptin:_FogAmount,varname:_FogAmount,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.3;n:type:ShaderForge.SFN_Slider,id:7168,x:31445,y:32872,ptovrint:False,ptlb:Fog Value,ptin:_FogValue,varname:_FogValue,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Add,id:5233,x:31729,y:33004,varname:node_5233,prsc:2|A-5433-OUT,B-2637-OUT;n:type:ShaderForge.SFN_Slider,id:2637,x:31411,y:33209,ptovrint:False,ptlb:No Fog Area,ptin:_NoFogArea,varname:_NoFogArea,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;proporder:7646-899-2578-7168-2195-8935-1623-9450-4389-2637;pass:END;sub:END;*/

Shader "Nothing Happens/Deprecated/NH 3D Displacement" {
    Properties {
        _MainTex ("Main Tex", 2D) = "white" {}
        _FogSkybox ("Fog Skybox", Cube) = "_Skybox" {}
        _FogAmount ("Fog Amount", Range(0, 0.3)) = 0
        _FogValue ("Fog Value", Range(0, 10)) = 1
        _FogCutoff ("Fog Cutoff", Range(0, 1)) = 1
        _Tesselation ("Tesselation", Range(1, 32)) = 1
        _DisplacementTexture ("Displacement Texture", 2D) = "black" {}
        _Displacement ("Displacement", Range(0, 1)) = 0
        _FogColor ("Fog Color", Color) = (0.5,0.5,0.5,1)
        _NoFogArea ("No Fog Area", Range(0, 2)) = 0
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
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "Tessellation.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers d3d9 opengl gles gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 5.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform half _FogCutoff;
            uniform float _Tesselation;
            uniform sampler2D _DisplacementTexture; uniform float4 _DisplacementTexture_ST;
            uniform float _Displacement;
            uniform float4 _FogColor;
            uniform half _FogAmount;
            uniform half _FogValue;
            uniform half _NoFogArea;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float4 _DisplacementTexture_var = tex2Dlod(_DisplacementTexture,float4(TRANSFORM_TEX(v.texcoord0, _DisplacementTexture),0.0,0));
                    v.vertex.xyz += (_DisplacementTexture_var.rgb*v.normal*_Displacement);
                }
                float Tessellation(TessVertex v){
                    return _Tesselation;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
////// Lighting:
                half4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 finalColor = lerp(_MainTex_var.rgb,_FogColor.rgb,clamp((_FogColor.a*saturate((1.0 - (pow(_FogValue,(_FogAmount*(-1*distance(_WorldSpaceCameraPos,i.posWorld.rgb))))+_NoFogArea)))),0.0,_FogCutoff));
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
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "Tessellation.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers d3d9 opengl gles gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 5.0
            uniform float _Tesselation;
            uniform sampler2D _DisplacementTexture; uniform float4 _DisplacementTexture_ST;
            uniform float _Displacement;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float4 _DisplacementTexture_var = tex2Dlod(_DisplacementTexture,float4(TRANSFORM_TEX(v.texcoord0, _DisplacementTexture),0.0,0));
                    v.vertex.xyz += (_DisplacementTexture_var.rgb*v.normal*_Displacement);
                }
                float Tessellation(TessVertex v){
                    return _Tesselation;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
