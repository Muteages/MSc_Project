Shader "Hidden/CombinedTextures"
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
	float4 _MainTex_TexelSize;
	Texture2D _darkTexture;
	SamplerState sampler_darkTexture;
	Texture2D _edgeTexture;
	SamplerState sampler_edgeTexture;

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
			// Dark texture
			float4 darkOutput  = _darkTexture.SampleLevel(sampler_darkTexture, input.uv, 0);

			// Edge detection texture
			float4 edgeOutput = _edgeTexture.SampleLevel(sampler_edgeTexture, input.uv, 0);

			return float4((darkOutput + edgeOutput).rgb, 1);

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
