// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Nothing Happens/NH Constant" {
    Properties {
        _Color ("Color", Color) = (0,0,0,1)
    }

    SubShader {
        Tags
        {
            "IgnoreProjector" = "True"
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
            "CanUseSpriteAtlas" = "True"
        }

        LOD 100

        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"

            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2
            #pragma target 4.6
            uniform float4 _Color;

            struct VertexInput {
                float4 vertex : POSITION;
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }

            float4 frag(VertexOutput i) : COLOR {

                fixed3 finalColor = _Color.rgb;
                return fixed4(finalColor,1);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
