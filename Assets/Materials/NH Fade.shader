// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Nothing Happens/NH Fade"
{
    Properties
    {
        [HideInInspector] _MainTex ("Screen", 2D) = "white" {}
        [HideInInspector] _CubeTex ("Main Texture", CUBE) = "black" {}
        [Space(20)]
        [HideInInspector] _UseTexture ("Use Texture", Float) = 0.0
        _Color ("Color/Tint", Color) = (1,1,1,1)
    }

    SubShader {
        Tags
        {
            "IgnoreProjector" = "True"
            "Queue" = "Overlay"
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

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            ZTest Always

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #pragma target 4.6


            // public properties
            uniform sampler2D _MainTex;
            uniform float _UseTexture;
            uniform samplerCUBE _CubeTex;
            uniform float4 _Color;


            // structs
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            // vertex
            v2f vert (appdata v)
            {
                v2f o;

                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                return o;
            }

            // fragment
            // v2f_img for image effects
            float4 frag(v2f i) : COLOR
            {
                // calculate view direction
                float3 viewDirection;
                viewDirection.x = _WorldSpaceCameraPos.x - i.worldPos.x;
                viewDirection.y = i.worldPos.y - _WorldSpaceCameraPos.y;
                viewDirection.z = i.worldPos.z - _WorldSpaceCameraPos.z;

                // fog cubemap
                float4 cube = texCUBE(_CubeTex, viewDirection);
                cube.rgb = lerp(1, cube.rgb, _UseTexture); // toggle cubemap

                // screen texture
                //float4 screen = tex2D(_MainTex, i.uv);

                float4 c;
                //c.rgb = lerp(screen, _Color.rgb * cube.rgb, _Color.a);
                c.rgb = _Color.rgb * cube.rgb;
                c.a = _Color.a;

                return c;
            }

            ENDCG
        }
    }
}