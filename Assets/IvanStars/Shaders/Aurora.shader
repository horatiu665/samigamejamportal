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
		_RandomWobbleVector ("Random Wobble Vector 1", Vector) = (113.235, 217.61346, 79.1324123, 94.8917)
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
					float4	pos		: POSITION;
					float2  tex0	: TEXCOORD0;
					float4	col		: COLOR;
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
				float4 _RandomWobbleVector;
				
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
					float positionBasedPhase = v.vertex.x * _RandomWobbleVector.x + v.vertex.y * _RandomWobbleVector.y + v.vertex.z * _RandomWobbleVector.z;
					float3 worldPos = mul(unity_ObjectToWorld, v.vertex.xyz);
					float worldPositionBasedPhase = worldPos.x * _RandomWobbleVector.y + worldPos.y * _RandomWobbleVector.z + worldPos.z * _RandomWobbleVector.w; 
					positionBasedPhase += worldPositionBasedPhase;
					float lum = Luminance(v.color.rgb) + getNoise(_FlickerLuminosity.y, positionBasedPhase) * _FlickerLuminosity.x;
					float sat = _ColorSaturation + getNoise(_FlickerSaturation.y, positionBasedPhase) * _FlickerSaturation.x;

					float4 color = lerp(float4(lum, lum, lum, v.color.a), v.color, sat) * _TintColor;
					float luminosity = pow(color.a, _ColorGamma) * _ColorLuminosity;
					output.col = color * luminosity;

					return output;
				}



				// Geometry Shader -----------------------------------------------------
				/*[maxvertexcount(4)]
				void GS_Main(point GS_INPUT p[1], inout TriangleStream<FS_INPUT> triStream)
				{
					float3 up = UNITY_MATRIX_IT_MV[1].xyz;
					float3 right = -UNITY_MATRIX_IT_MV[0].xyz;

					float positionBasedPhase = p[0].pos.x * 113.2343 + p[0].pos.y * 217.6134 + p[0].pos.z * 7.673239;
					float size = _Size + getNoise(_FlickerSize.y, positionBasedPhase) * _FlickerSize.x;

					float halfS = 0.5f * size * p[0].col.a;
							
					float4 v[4];
					v[0] = float4(p[0].pos + halfS * right - halfS * up, 1.0f);
					v[1] = float4(p[0].pos + halfS * right + halfS * up, 1.0f);
					v[2] = float4(p[0].pos - halfS * right - halfS * up, 1.0f);
					v[3] = float4(p[0].pos - halfS * right + halfS * up, 1.0f);

					FS_INPUT pIn;

					pIn.col = p[0].col;

					pIn.pos = UnityObjectToClipPos(v[0]);
					pIn.tex0 = float2(1.0f, 0.0f);
					triStream.Append(pIn);

					pIn.pos = UnityObjectToClipPos(v[1]);
					pIn.tex0 = float2(1.0f, 1.0f);
					triStream.Append(pIn);

					pIn.pos = UnityObjectToClipPos(v[2]);
					pIn.tex0 = float2(0.0f, 0.0f);
					triStream.Append(pIn);

					pIn.pos = UnityObjectToClipPos(v[3]);
					pIn.tex0 = float2(0.0f, 1.0f);
					triStream.Append(pIn);
				}*/



				// Fragment Shader -----------------------------------------------
				float4 FS_Main(FS_INPUT input) : COLOR
				{
					return tex2D(_SpriteTex, input.tex0) * input.col * 2;
				}

			ENDCG
		}
	} 
}
