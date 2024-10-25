﻿// Toony Colors Pro+Mobile 2
// (c) 2014-2018 Jean Moreno


Shader "Hidden/Toony Colors Pro 2/User/sand-Base"
{
	Properties
	{
		//TOONY COLORS
		_HColor ("Highlight Color", Color) = (0.6,0.6,0.6,1.0)
		_SColor ("Shadow Color", Color) = (0.3,0.3,0.3,1.0)

		//TOONY COLORS RAMP
	[TCP2Header(RAMP SETTINGS)]
		_RampThreshold ("Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("Ramp Smoothing", Range(0.001,1)) = 0.1

	[TCP2Separator]

	[TCP2HeaderHelp(SPECULAR, Specular)]
		//SPECULAR
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range(0.0,2)) = 0.1
	[TCP2Separator]

		//TERRAIN PROPERTIES
		[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
		[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
		[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
		[HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
		// used in fallback on old cards & base map
		[HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
	}

	CGINCLUDE
		#pragma surface surf ToonyColorsCustom addshadow vertex:SplatmapVert_TCP2 finalcolor:SplatmapFinalColor_TCP2

		#pragma multi_compile_fog

		sampler2D _MainTex;

		fixed _Shininess;

		struct Input
		{
			UNITY_FOG_COORDS(0)
			float2 uv_MainTex;

		};

		struct appdata_tcp2
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
	#ifdef _TERRAIN_NORMAL_MAP
			float4 tangent : TANGENT;
	#endif
	#if UNITY_VERSION >= 550
			UNITY_VERTEX_INPUT_INSTANCE_ID
	#endif
		};

		void SplatmapVert_TCP2(inout appdata_tcp2 v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

		}

		#ifndef TERRAIN_SURFACE_OUTPUT
			#define TERRAIN_SURFACE_OUTPUT SurfaceOutputCustom
		#endif

		//Custom SurfaceOutput
		struct SurfaceOutputCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
			fixed Gloss;
			fixed Alpha;
		};

		void SplatmapFinalColor_TCP2(Input IN, TERRAIN_SURFACE_OUTPUT o, inout fixed4 color)
		{
			color *= o.Alpha;
			#ifdef TERRAIN_SPLAT_ADDPASS
				UNITY_APPLY_FOG_COLOR(IN.fogCoord, color, fixed4(0,0,0,0));
			#else
				UNITY_APPLY_FOG(IN.fogCoord, color);
			#endif

		}

		//================================================================
		// CUSTOM LIGHTING

		//Lighting-related variables
		fixed4 _HColor;
		fixed4 _SColor;
		float _RampThreshold;
		float _RampSmooth;

		inline half4 LightingToonyColorsCustom (inout TERRAIN_SURFACE_OUTPUT s, half3 lightDir, half3 viewDir, half atten)
		{
			s.Normal = normalize(s.Normal);
			fixed ndl = max(0, dot(s.Normal, lightDir));

			fixed3 ramp = smoothstep(_RampThreshold-_RampSmooth*0.5, _RampThreshold+_RampSmooth*0.5, ndl);
		#if !(POINT) && !(SPOT)
			ramp *= atten;
		#endif

			_SColor = lerp(_HColor, _SColor, _SColor.a);	//Shadows intensity through alpha
			ramp = lerp(_SColor.rgb, _HColor.rgb, ramp);

			//Specular
			half3 h = normalize(lightDir + viewDir);
			float ndh = max(0, dot (s.Normal, h));
			float spec = pow(ndh, (s.Specular+1e-4f)*128.0) * s.Gloss * 2.0;
			spec *= atten;
			fixed4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp;
		#if (POINT || SPOT)
			c.rgb *= atten;
		#endif
			c.rgb += _LightColor0.rgb * _SpecColor.rgb * spec;
			c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec;
			return c;
		}


		//================================================================

		void surf(Input IN, inout TERRAIN_SURFACE_OUTPUT o)
		{

			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.rgb;
			o.Alpha = 1.0f;

			//Specular
			o.Gloss = 1;
			o.Specular = _Shininess;
		}
	ENDCG

	Category
	{
		Tags
		{
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}


		SubShader
		{
			CGPROGRAM
			ENDCG
		}
	}


	Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector_SG"
}

