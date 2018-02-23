// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Nothing Happens/NH Uber"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _FogOverride ("Fog Override", Range(-1, 1)) = 0.0

        [HideInInspector] _Alpha ("Alpha", Float) = 1.0
        [HideInInspector] _AlphaCutoff ("Alpha Cutoff", Float) = 0.5
        [HideInInspector] _FogBackground ("Fog Background", Float) = 0.0
        [HideInInspector] _SrcBlend ("__SrcBlend", Float) = 1.0
        [HideInInspector] _DstBlend ("__DstBlend", Float) = 0.0

        [HideInInspector] _ZWrite ("__ZWrite", Float) = 1.0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4.0

        [HideInInspector] _Cull ("__Cull", Float) = 2.0
        [HideInInspector] _IgnoreProjector ("__Ignore Projector", Float) = 1.0
        [HideInInspector] _Transparency ("__Transparency", Float) = 0.0

        [HideInInspector] _Skybox ("Skybox Texture", Cube) = "black" {}

        [HideInInspector] _AnimTex ("__Animation Texture", 2D) = "white" {}
        [HideInInspector] _AnimRepeat ("__Repeat Animation Texture", Float) = 1.0
        [HideInInspector] _SpeedX ("__Animation Speed X", Float) = 0.0
        [HideInInspector] _SpeedY ("__Animation Speed Y", Float) = 0.0
        [HideInInspector] _Distortion ("__Animation Distortion", Float) = 0.0
    }

    SubShader {
        Tags
        {
            "IgnoreProjector" = "True"
            "Queue" = "Geometry"
            "CanUseSpriteAtlas" = "True"
            "ForceNoShadowCasting" = "True"
        }

        LOD 100

        Pass
        {
            Name "FORWARD"
            Tags {
                "LightMode" = "ForwardBase"
            }

            Blend [_SrcBlend] [_DstBlend]
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]


            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "NHBase.cginc"

            #pragma target 4.6
            #pragma shader_feature INSTANCING
            #pragma shader_feature CUTOUT
            #pragma shader_feature BACKGROUND
            #pragma shader_feature SKYBOX
            #pragma shader_feature ANIMATE

            #if defined(INSTANCING)
                #pragma multi_compile_instancing
            #endif

            // public properties
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform float _FogOverride;
            uniform float _Alpha;
            uniform float _AlphaCutoff;
            uniform float _FogBackground;
            uniform samplerCUBE _Skybox;

            #if defined(ANIMATE)
                uniform sampler2D _AnimTex;
                uniform float4 _AnimTex_ST;
                uniform float _AnimRepeat;
                uniform float _SpeedX;
                uniform float _SpeedY;
                uniform float _Distortion;
            #endif

            // global properties
            uniform samplerCUBE _FOG_CUBE;
            uniform float _FOG_AMOUNT;
            uniform float _FOG_EXP;
            uniform float _FOG_OFFSET;
            uniform float _FOG_LIMIT;

            // structs
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                #if defined(INSTANCING)
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                #endif
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 vertexWorld : TEXCOORD1;

                #if defined(INSTANCING)
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                #endif
            };

            // vertex
            v2f vert (appdata v)
            {
                v2f o;

                #if defined(INSTANCING)
                    UNITY_SETUP_INSTANCE_ID (v);
                    UNITY_TRANSFER_INSTANCE_ID (v, o);
                #endif

                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertexWorld = mul(unity_ObjectToWorld, v.vertex);

                return o;
            }

            // fragment
            half4 frag(v2f i) : SV_TARGET
            {
                #if defined(INSTANCING)
                    UNITY_SETUP_INSTANCE_ID (i);
                #endif

                // calculate view direction
                float3 viewDirection;
                viewDirection.x = _WorldSpaceCameraPos.x - i.vertexWorld.x;
                viewDirection.y = i.vertexWorld.y - _WorldSpaceCameraPos.y;
                viewDirection.z = i.vertexWorld.z - _WorldSpaceCameraPos.z;


                // fog cubemap
                half4 cube = texCUBE(_FOG_CUBE, viewDirection);

                float fogAmount = 0;

                // constant fog on background elements
                #if defined(BACKGROUND)
                    fogAmount = _FogBackground;
                #else
                    fogAmount = fog(i.vertexWorld.rgb, cube, _FOG_AMOUNT, _FOG_EXP, _FOG_OFFSET, _FOG_LIMIT) * (1 + _FogOverride);
                #endif

                // animate texture?
                #if defined(ANIMATE)
                    float2 multiplyAnim = float2((_AnimRepeat * i.uv.x + _Time.x * _SpeedX), (_AnimRepeat * i.uv.y + _Time.x * _SpeedY));
                    float4 animTex = tex2D(_AnimTex, TRANSFORM_TEX(multiplyAnim, _AnimTex));
                    float distortionCenter = 1 - _Distortion / 2;
                    float mask = (animTex.rgb * _Distortion + distortionCenter).r;
                    i.uv = i.uv * mask;
                #endif

                // normal texture or skybox/cubemap?
                half4 c = 1;
                #if defined(SKYBOX)
                    c.rgb = texCUBE(_Skybox, viewDirection);
                    c.a = 1;
                #else
                    c = tex2D(_MainTex, i.uv);
                #endif


                // lerp between
                c.rgb = lerp(c.rgb, cube.rgb, saturate(fogAmount));

                // cutout subtraction
                #if defined(CUTOUT)
                    clip(c.a - _AlphaCutoff);

                    /*
                    // screen-door transparency
                    float4x4 thresholdMatrix = {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };

                    float4x4 _RowAccess = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };

                    float2 pos = (i.vertexWorld.xy / i.vertexWorld.w) * _ScreenParams.xy;
                    clip(c.a - thresholdMatrix[fmod(pos.x, 4)] * _RowAccess[fmod(pos.y, 4)]);
                    */
                #else
                    c.a = c.a * _Alpha;
                #endif

                return c;
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "NothingHappensShaderEditor"
}