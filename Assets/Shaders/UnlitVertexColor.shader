Shader "Unlit/VertexColor"
{
	Properties 
	{
		
	}
	SubShader 
	{
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM
		#pragma surface surf SimpleLambert 
		#pragma vertex vert
		
		half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) 
		{
			half4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}
		
		struct Input 
		{
			half4 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			o.Albedo = IN.color;
		}

		void vert (inout appdata_full v, out Input IN)
		{
			UNITY_INITIALIZE_OUTPUT(Input,IN);
		}

		void endcolor (Input IN, SurfaceOutput o, inout fixed4 color)
		{
			
		}

        ENDCG
	}
	Fallback "Diffuse"
}
