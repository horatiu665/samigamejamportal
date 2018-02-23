// RENAME THIS SHIT AND fix the lerping of top sky - horizon, so horizon starts with the offset instead of at zero height
Shader "Skybox Blend 2 TRIPPY AS FUCK 2" {
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

			float3 randomizedColor = float3(
				0.5 + 0.5 * sin(viewDirection.x *_RandomizeColor.w) * _RandomizeColor.x,
				0.5 + 0.5 * sin(viewDirection.y *_RandomizeColor.w) * _RandomizeColor.y,
				0.5 + 0.5 * sin(viewDirection.z *_RandomizeColor.w) * _RandomizeColor.z) * _RandomAmount;

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
