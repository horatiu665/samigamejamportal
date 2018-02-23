// RENAME THIS SHIT AND fix the lerping of top sky - horizon, so horizon starts with the offset instead of at zero height
Shader "Skybox Blend 2" {
Properties {
	_SkyColorTop ("SkyColorTop", Color) = (0.01568628,0.454902,0.9725491,1)
	_SkyColorBottom ("SkyColorBottom", Color) = (0.01568628,0.454902,0.9725491,1)
	_RandomizeColor ("Randomize Color", Vector) = (0.5,0.5,0.5,0.5)
	_Gamma ("Gamma", Float) = 1
	_RandomAmount ("RandomAmount", Float) = 1
}

SubShader {
	Tags { 
		"Queue"="Background" 
		"RenderType"="Background" 
		"PreviewType"="Skybox"
	}
	Cull Off ZWrite Off

	Pass {

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		uniform float4 _SkyColorTop;
		uniform float4 _SkyColorBottom;
		uniform float4 _RandomizeColor;
		uniform float _Gamma;
		uniform float _RandomAmount;

		struct appdata_t
		{
			float4 vertex : POSITION;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f
		{
			float4	pos				: SV_POSITION;
			float4  posWorld		: TEXCOORD0;

			UNITY_VERTEX_OUTPUT_STEREO
		};

		float mod289(float x){return x - floor(x * (1.0 / 289.0)) * 289.0;}
		float4 perm(float4 x){ return mod289(((x * 34.0) + 1.0) * x); }
		float fract(float x) { return x - floor(x); }

		float noise(float3 p){
		    float3 a = floor(p);
		    float3 d = p - a;
		    d = d * d * (3.0 - 2.0 * d);

		    float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
		    float4 k1 = perm(b.xyxy);
		    float4 k2 = perm(k1.xyxy + b.zzww);

		    float4 c = k2 + a.zzzz;
		    float4 k3 = perm(c);
		    float4 k4 = perm(c + 1.0);

		    float4 o1 = fract(k3 * (1.0 / 41.0));
		    float4 o2 = fract(k4 * (1.0 / 41.0));

		    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
		    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);

		    return o4.y * d.y + o4.x * (1.0 - d.y);
		}

		v2f vert (appdata_t v) 
		{
			v2f OUT;
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
			OUT.posWorld = mul(unity_ObjectToWorld, v.vertex);
			OUT.pos = UnityObjectToClipPos(v.vertex);

			return OUT;
		}

		half4 frag (v2f IN) : SV_Target
		{
			float3 viewDirection = normalize(-IN.posWorld.xyz);

			float upDownLerp = (dot(viewDirection, float3(0, 1, 0)) + 1) * 0.5;

			float3 randomizedColor = noise(viewDirection * _RandomizeColor.x) * _RandomAmount;

			upDownLerp = pow(upDownLerp, _Gamma);

			// float3 emissive = lerp(
			// 	lerp(
			// 		_SkyColorTop.rgb,
			// 		_SkyColorHorizon.rgb,
			// 		min(1,
			// 			max(0,
			// 				dot(
			// 					(viewDirection + float3(0, 1, 0)),
			// 					float3(0, 1, 0))
			// 				)
			// 			)
			// 		),
			// 	_GroundColorh.rgb,
			// 	saturate(
			// 		pow(max(0, dot((float3(0, _HorizonHeight + 1.0, 0) + viewDirection), float3(0, 1, 0))),
			// 			pow(_HorizonBlur, 2.718281828459)))
			// 	);

			float3 finalColor = lerp(_SkyColorTop, _SkyColorBottom, upDownLerp) + randomizedColor;

			return fixed4(finalColor, 1);
		}
		ENDCG
	}
}


Fallback Off

}
