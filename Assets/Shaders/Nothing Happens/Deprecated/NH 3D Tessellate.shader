// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:True,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:1,fgcg:1,fgcb:1,fgca:1,fgde:0.01,fgrn:10,fgrf:12,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:5245,x:33154,y:32447,varname:node_5245,prsc:2|custl-2501-OUT,tess-271-OUT;n:type:ShaderForge.SFN_Tex2d,id:7646,x:32530,y:32503,ptovrint:True,ptlb:Main Tex,ptin:_MainTex,varname:_MainTex,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:2501,x:32789,y:32686,varname:node_2501,prsc:2|A-7646-RGB,B-1213-RGB,T-1327-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:4874,x:30933,y:33123,varname:node_4874,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:7771,x:30933,y:32991,varname:node_7771,prsc:2;n:type:ShaderForge.SFN_Distance,id:9690,x:31114,y:32991,varname:node_9690,prsc:1|A-7771-XYZ,B-4874-XYZ;n:type:ShaderForge.SFN_Multiply,id:7785,x:31473,y:32972,varname:node_7785,prsc:2|A-4866-OUT,B-618-OUT;n:type:ShaderForge.SFN_Negate,id:618,x:31289,y:32991,varname:node_618,prsc:2|IN-9690-OUT;n:type:ShaderForge.SFN_OneMinus,id:4260,x:31825,y:32972,varname:node_4260,prsc:2|IN-6852-OUT;n:type:ShaderForge.SFN_Clamp01,id:9717,x:32367,y:32972,varname:node_9717,prsc:2|IN-4260-OUT;n:type:ShaderForge.SFN_Cubemap,id:899,x:32282,y:32705,ptovrint:False,ptlb:Fog Skybox,ptin:_FogSkybox,varname:_FogSkybox,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0|DIR-7975-OUT;n:type:ShaderForge.SFN_Multiply,id:6260,x:32530,y:32856,varname:node_6260,prsc:2|A-1213-A,B-9717-OUT;n:type:ShaderForge.SFN_Negate,id:7975,x:32105,y:32705,varname:node_7975,prsc:2|IN-2497-OUT;n:type:ShaderForge.SFN_ViewVector,id:2497,x:31929,y:32705,varname:node_2497,prsc:2;n:type:ShaderForge.SFN_Clamp,id:1327,x:32922,y:32930,varname:node_1327,prsc:2|IN-6260-OUT,MIN-93-OUT,MAX-2195-OUT;n:type:ShaderForge.SFN_Vector1,id:93,x:32738,y:32952,varname:node_93,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:2195,x:32581,y:33028,ptovrint:False,ptlb:Fog Cutoff,ptin:_FogCutoff,varname:_FogCutoff,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Power,id:6852,x:31657,y:32972,varname:node_6852,prsc:2|VAL-9214-OUT,EXP-7785-OUT;n:type:ShaderForge.SFN_Color,id:1213,x:32470,y:32691,ptovrint:False,ptlb:Fog Color,ptin:_FogColor,varname:_FogColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:4866,x:31255,y:32872,ptovrint:False,ptlb:Fog Amount,ptin:_FogAmount,varname:_FogAmount,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.01,max:0.1;n:type:ShaderForge.SFN_Slider,id:9214,x:31578,y:32840,ptovrint:False,ptlb:Fog Value,ptin:_FogValue,varname:_FogValue,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:5;n:type:ShaderForge.SFN_Slider,id:271,x:33039,y:32994,ptovrint:False,ptlb:Tessellation,ptin:_Tessellation,varname:_Tessellation,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:4;proporder:7646-899-4866-9214-2195-1213-271;pass:END;sub:END;*/

Shader "Nothing Happens/Deprecated/NH 3D Tessellate" {
    Properties
    {
        _MainTex ("Main Texture", 2D) = "black" {}

        _FogSkybox ("Fog Skybox", Cube) = "_Skybox" {}
        _FogAmount ("Fog Amount", Range(0, 0.3)) = 0.01
        _FogValue ("Fog Value", Range(0, 10)) = 2
        _FogCutoff ("Fog Cutoff", Range(0, 1)) = 1
        _FogColor ("Fog Color", Color) = (0.5,0.5,0.5,1)
        _NoFogArea ("No Fog Area", Range(0, 2)) = 0

        _EdgeLength ("Edge length", Range(2,50)) = 5
        _Phong ("Phong Strengh", Range(0,1)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

        CGPROGRAM
        #pragma surface surf NoLighting vertex:dispNone tessellate:tessEdge tessphong:_Phong nolightmap
        #include "Tessellation.cginc"

        // Vertex input
        struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
        };

        struct VertexOutput {
            float4 pos : SV_POSITION;
            float2 uv0 : TEXCOORD0;
            float4 posWorld : TEXCOORD1;
        };

        void dispNone (inout appdata v) { }

        // Tessellation
        float _Phong;
        float _EdgeLength;

        float4 tessEdge (appdata v0, appdata v1, appdata v2)
        {
            return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
        }

        // Texture UV's
        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        // Properties
        sampler2D _MainTex;

        uniform half _FogCutoff;
        uniform float4 _FogColor;
        uniform float _FogAmount;
        uniform float _FogValue;
        uniform half _NoFogArea;

        // Unlit
        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed4 c;
            c.rgb = s.Albedo;
            c.a = s.Alpha;
            return c;
        }

        // Set surface color from texture
        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            half3 finalColor = lerp(
                c.rgb,
                _FogColor.rgb,
                clamp((_FogColor.a*saturate((1.0 - (pow(_FogValue,(_FogAmount*(-1*(distance(_WorldSpaceCameraPos,IN.worldPos.rgb)-_NoFogArea)))))))),0.0,_FogCutoff)
                );


            o.Albedo = finalColor.rgb;
            //o.Alpha = 1;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
