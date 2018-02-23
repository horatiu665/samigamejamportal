Shader "Nothing Happens/NH Uber Surface"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _FogOverride ("Fog Override", Range(-1, 1)) = 0.0

        [HideInInspector] _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        [HideInInspector] _FogBackground ("Fog Background", Float) = 0.0

        [HideInInspector] _SrcBlend ("__SrcBlend", Float) = 1.0
        [HideInInspector] _DstBlend ("__DstBlend", Float) = 0.0

        [HideInInspector] _ZWrite ("__ZWrite", Float) = 1.0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4.0

        [HideInInspector] _Cull ("__Cull", Float) = 2.0
        [HideInInspector] _IgnoreProjector ("__Ignore Projector", Float) = 1.0

        [HideInInspector] _Skybox ("Skybox Texture", Cube) = "black" {}

        [HideInInspector] _Transparency ("__Transparency", Float) = 0.0
        [HideInInspector] _EdgeLength ("__Edge length", Float) = 10
        [HideInInspector] _Smoothness ("__Smoothness", Float) = 0.5
    }

    SubShader {
        Tags
        {
            "IgnoreProjector" = "True"
            "Queue" = "Geometry"
            "RenderType" = "Transparent"
            "CanUseSpriteAtlas" = "True"
            "ForceNoShadowCasting" = "True"
        }

        LOD 100

        Cull [_Cull]
        ZWrite [_ZWrite]
        ZTest [_ZTest]

        CGPROGRAM

        #pragma shader_feature INSTANCING
        #pragma shader_feature CUTOUT
        #pragma shader_feature BACKGROUND
        #pragma shader_feature SKYBOX
        #pragma shader_feature TESSELLATE

        #pragma surface surf NoLighting vertex:disp tessellate:tessEdge tessphong:_Smoothness nolightmap

        #include "Tessellation.cginc"
        #include "UnityCG.cginc"
        #include "NHBase.cginc"


        // structs
        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
            float2 texcoord1 : TEXCOORD1;
            float2 texcoord2 : TEXCOORD2;

            #if defined(INSTANCING)
                UNITY_VERTEX_INPUT_INSTANCE_ID
            #endif
        };

        // tessellation
        float _EdgeLength;
        float _Smoothness;

        float4 tessEdge (appdata v0, appdata v1, appdata v2)
        {
            #if defined(TESSELLATE)
                return UnityEdgeLengthBasedTessCull (v0.vertex, v1.vertex, v2.vertex, _EdgeLength, 0.0);
            #else
                return 1;
            #endif
        }

        // public properties
        uniform sampler2D _MainTex;
        uniform float _FogOverride;
        uniform float _AlphaCutoff;
        uniform float _FogBackground;
        uniform samplerCUBE _Skybox;

        // global properties
        uniform samplerCUBE _FOG_CUBE;
        uniform float _FOG_AMOUNT;
        uniform float _FOG_EXP;
        uniform float _FOG_OFFSET;
        uniform float _FOG_LIMIT;


        void disp (inout appdata v)
        {
        }

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        // lighting model
        half4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            half4 c;
            c.rgb = s.Albedo;
            c.a = s.Alpha;
            return c;
        }


        // surface
        void surf(Input i, inout SurfaceOutput o)
        {
            #if defined(INSTANCING)
                UNITY_SETUP_INSTANCE_ID (i);
                UNITY_TRANSFER_INSTANCE_ID (i, o);
            #endif

            // view direction
            float3 viewDirection = -1 * normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

            // normal texture or skybox/cubemap?
            half4 c = 1;
            #if defined(SKYBOX)
                c.rgb = texCUBE(_Skybox, viewDirection);
            #else
                c = tex2D(_MainTex, i.uv_MainTex);
            #endif

            // fog cubemap
            half4 cube = texCUBE(_FOG_CUBE, viewDirection);

            // constant fog on background elements
            #if defined(BACKGROUND)
                float fogAmount = _FogBackground;
            #else
                float fogAmount = fog(i.worldPos.rgb, cube, _FOG_AMOUNT, _FOG_EXP, _FOG_OFFSET, _FOG_LIMIT) * (1 + _FogOverride);
            #endif

            c.rgb = lerp(c.rgb, cube.rgb, saturate(fogAmount));

            #if defined(CUTOUT)
                clip(c.a - _AlphaCutoff);
            #endif

            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }

        ENDCG
    }
    FallBack "Diffuse"
    CustomEditor "NothingHappensShaderEditor"
}