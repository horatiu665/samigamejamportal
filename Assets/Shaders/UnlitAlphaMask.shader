// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/AlphaMask" {
Properties {
	_Color ("Main Color", Color) = (0,0,0,0)
    _MainTex ("Base (RGBA)", 2D) = "white" {}
    _AlphaTex ("Alpha mask (A)", 2D) = "white" {}
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100
    
    Lighting Off
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha 
    
    Pass {  
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
                half2 texcoord1 : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            
            float4 _MainTex_ST;
            float4 _AlphaTex_ST;
            
            half4 _Color;
            
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.texcoord1 = TRANSFORM_TEX(v.texcoord, _AlphaTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                fixed4 col2 = tex2D(_AlphaTex, i.texcoord1);
                
                return fixed4(col.r, col.g, col.b, col.a * col2.a) * _Color;
            }
        ENDCG
    }
}

}