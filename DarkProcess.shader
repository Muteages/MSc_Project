Shader "Hidden/DarkProcess"
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
	float _darkScale;
	float _transitionSize;

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
		//if (outOfBounds(input.uv,_screenWidth,_screenHeight,_texSize) == 1)
		//		return float4(0, 0, 0, 1);

			float lod = 0;
			float sx = _texSize / _screenWidth;
			float sy = _texSize / _screenHeight;
			float2 lutuv = float2(input.uv.x * sx, input.uv.y * sy);

			lod = _FoveationLUT.SampleLevel(sampler_FoveationLUT, lutuv, 0).r;

			//  Y Cr Cb components
			float4 YCrCb = _MainTex.SampleLevel(my_trilinear_clamp_sampler, input.uv, 0);

			float Y  = YCrCb.x;
			float Cr = YCrCb.y;
			float Cb = YCrCb.z;

			Y = ((lod > (_darkScale - _transitionSize)) && (lod < _darkScale)) ? (1 - (lod - (_darkScale - _transitionSize)) / _transitionSize) * Y : Y;

			Y = lod >= _darkScale ? 0 : Y;

			return float4( Y, Cr, Cb, 1);


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
