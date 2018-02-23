Shader "Aurora" 
{
	Properties 
	{
		_TintColor ("Color", Color) = (0,1,0,1)
		_SpriteTex ("Base (RGB)", 2D) = "white" {}
		_Size ("Size", Range(0, 3)) = 0.5
		_ColorLuminosity ("Color: Luminosity", Range(0, 15)) = 1
		_ColorSaturation ("Color: Saturation", Range(0, 2)) = 1
		_ColorGamma ("Color: Gamma", Range(0, 2)) = 1

		// Flicker params are: x = amount, y = rate multiplier, // UNIMPLEMENTED BUT COOL IDEA: z = factor of size, w = unused
		_FlickerLuminosity ("Flicker: Luminosity", Vector) = (0.1, 1, 0, 0)
		_FlickerSaturation ("Flicker: Saturation", Vector) = (0.1, 1, 0, 0)
		_FlickerSize ("Flicker: Size", Vector) = (0.1, 1, 0, 0)
		_RandomWobbleVector1 ("Random Wobble Vector 1", Vector) = (113.235, 217.61346, 79.1324123, 94.8917)
		_RandomWobbleVector2 ("Random Wobble Vector 2", Vector) = (125.543, 96.645, 83.12678, 126.46441)
	
		_Dithering ("Dithering", Vector) = (1000, 1, 0, 0)
	}

	SubShader 
	{

		
		Pass
		{
			Blend One One
			ZWrite Off

			Tags 
			{ 
				"RenderType"="Transparent" 
				"IgnoreProjector"="True"
			}

			LOD 200
		
			CGPROGRAM
				//#pragma target 5.0
				#pragma vertex VS_Main
				#pragma fragment FS_Main
				//#pragma geometry GS_Main
				#include "UnityCG.cginc" 
				#include "ClassicNoise2D.hlsl"

				// **************************************************************
				// Data structures												*
				// **************************************************************
				struct GS_INPUT
				{
					float4	pos		: POSITION;
					float3	normal	: NORMAL;
					float2  tex0	: TEXCOORD0;
					float4	col		: COLOR;
				};

				struct FS_INPUT
				{
					float4	pos 		: POSITION;
					float2  tex0		: TEXCOORD0;
					float4	col			: COLOR;
					float4  tex1		: TEXCOORD1;
				};


				// **************************************************************
				// Vars															*
				// **************************************************************

				float4x4 _VP;
				sampler2D _SpriteTex;
	            float4 _SpriteTex_ST;
				float _Size;
				float _ColorSaturation;
				float _ColorLuminosity;
				float _ColorGamma;
				float4 _FlickerSaturation;
				float4 _FlickerLuminosity;
				float4 _FlickerSize;
				float4 _TintColor;
				float4 _RandomWobbleVector1;
				float4 _RandomWobbleVector2;
				float4 _Dithering;

				// *** 
				// random dithering
				// *** 

				float mod289(float x) {return x - floor(x * (1.0 / 289.0)) * 289.0;}
				float4 perm(float4 x) { return mod289(((x * 34.0) + 1.0) * x); }
				float fract(float x) { return x - floor(x); }

				float noise(float3 p) {
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


				// **************************************************************
				// Shader Funks													*
				// **************************************************************
				
				// random noise w seed that can be based on time
				float getNoise(float timeMultiplier, float phaseSeed)
				{
					return cnoise(float2(_SinTime.x * timeMultiplier, phaseSeed));
				}
				 
				// **************************************************************
				// Shader Programs												*
				// **************************************************************

				// Vertex Shader ------------------------------------------------
				FS_INPUT VS_Main(appdata_full v)
				{
					FS_INPUT output = (FS_INPUT)0;

					output.pos = UnityObjectToClipPos(v.vertex);
					output.tex0 = v.texcoord;

					// lerp between grayscale version of v.color, and v.color, with _ColorSaturation as the lerp factor
					float positionBasedPhase = v.vertex.x * _RandomWobbleVector1.x + v.vertex.y * _RandomWobbleVector1.y + v.vertex.z * _RandomWobbleVector1.z;
					float3 worldPos = mul(unity_ObjectToWorld, v.vertex.xyz);
					float worldPositionBasedPhase = worldPos.x * _RandomWobbleVector2.x + worldPos.y * _RandomWobbleVector2.y + worldPos.z * _RandomWobbleVector2.z; 
					positionBasedPhase += worldPositionBasedPhase;
					float lum = Luminance(v.color.rgb) + getNoise(_FlickerLuminosity.y, positionBasedPhase) * _FlickerLuminosity.x;
					float sat = _ColorSaturation + getNoise(_FlickerSaturation.y, positionBasedPhase) * _FlickerSaturation.x;

					float4 color = lerp(float4(lum, lum, lum, v.color.a), v.color, sat) * _TintColor;
					float luminosity = pow(color.a, _ColorGamma) * _ColorLuminosity;
					output.col = color * luminosity;

					output.tex1 = float4(v.vertex.xyz + worldPos, 0); 

					return output;
				}

				// Fragment Shader -----------------------------------------------
				float4 FS_Main(FS_INPUT input) : COLOR
				{
					float4 final = tex2D(_SpriteTex, input.tex0) * input.col * 2;
					float randomizedColor = noise(float3(input.tex0.xy * _Dithering.x, input.tex0.x)) * _Dithering.y;
					//final = float4(final.xyz, final.w * randomizedColor);
					final *= 1 + randomizedColor;
					//final.a *= randomizedColor;
					return final;

					//return tex2D(_SpriteTex, input.tex0) * input.col * 2;
				}

			ENDCG
		}
	} 
}
