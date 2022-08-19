Shader "Hidden/EdgeDetectionProcess"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		CGINCLUDE
#include "UnityCG.cginc" 
#include "Utils.cginc"

		Texture2D _MainTex;
	SamplerState my_trilinear_clamp_sampler;

	Texture2D _FoveationLUT;
	SamplerState sampler_FoveationLUT;

	int _useLUT;

	float4 _MainTex_TexelSize;
	float _screenWidth;
	float _screenHeight;
	float _texSize;
	float _strengthLevel;
	float _darkLevel;

	struct Input
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct Varyings
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	Varyings vertex(in Input input)
	{
		Varyings output;
		output.vertex = UnityObjectToClipPos(input.vertex.xyz);
		output.uv = input.uv;

#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			output.uv.y = 1. - input.uv.y;
#endif

		return output;
	}

	float4 fragment(in Varyings input) : SV_Target
	{
			float lod = 0;
			float sx = _texSize / _screenWidth;
			float sy = _texSize / _screenHeight;
			float2 lutuv = float2(input.uv.x * sx, input.uv.y * sy);

			lod = _FoveationLUT.SampleLevel(sampler_FoveationLUT, lutuv, 0).r;

			// Laplacian filter
			float4 a = _MainTex.SampleLevel(my_trilinear_clamp_sampler, input.uv, 0);
			float4 b = _MainTex.SampleLevel(my_trilinear_clamp_sampler, input.uv, 6 /*lod*/);

			// only modified the Y channel
			float output = abs(a.r - b.r);

			if (lod > _darkLevel)
			{
				return float4(3 * output, 0, 0, 1);

			}

			return float4(0, 0, 0, 1);

	}
		ENDCG

		SubShader
	{
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			CGPROGRAM
			#pragma vertex vertex
			#pragma fragment fragment
			ENDCG
		}
	}
}
