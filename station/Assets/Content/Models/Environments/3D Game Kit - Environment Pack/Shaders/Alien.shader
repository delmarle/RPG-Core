// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/Alien" {
	Properties {
		_MainTex ("Albedo(RGB) Emisssive(A)", 2D) = "white" {}
		_NormalMap("Normal", 2D) = "bump" {}
		_MetallicSmooth ("Metallic(RGB) Smooth(A)", 2D) = "white" {}
		_PackedMap ("_PackedMap", 2D) = "white" {}
		[HDR] _EmissiveColor ("Emissive Color", Color) = (1,1,1,1)
		[HDR] _Color ("Inner Tint", Color) = (1,1,1,1)
		_Ramp("Edge Highlight Ramp", 2D) = "white" {}
		_Ramp2("Inner Structure Ramp", 2D) = "white" {}
		_marchDistance ("Inner Offset", Float) = 0.5
		_numSteps ("Inner Samples", Float) = 4
		_glowMin ("Glow Min", Float) = 0
		_glowMax ("Glow Max", Float) = 1
		_glowFrequency ("Glow Frequency", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque"}
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 4.0

		sampler2D _MainTex;
		sampler2D _PackedMap;
		sampler2D _MetallicSmooth;
		sampler2D _NormalMap;
		sampler2D _Ramp;
		sampler2D _Ramp2;


		struct Input {
			float2 uv_MainTex;
			float2 uv_PackedMap;
			float3 viewDir;
		};


		fixed4 _Color;
		float4 _EmissiveColor;
		float _RimPower;
		float _numSteps;
        float _marchDistance;
        float _glowMin;
		float _glowMax;
		float _glowFrequency; 


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)


		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			o.Normal = UnpackNormal (tex2D (_NormalMap, IN.uv_MainTex));
			fixed4 metallicSmooth = tex2D(_MetallicSmooth, IN.uv_MainTex);
			fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex);

			//Edge hightlight
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			fixed3 edgeHighlight = tex2D(_Ramp, float2(pow(rim, 0.5), 0.5)) * tex2D(_PackedMap, IN.uv_MainTex).b;

			o.Metallic = metallicSmooth;
			o.Smoothness = metallicSmooth.a;
			

			//Inner structure parallax
			float3 InnerStructure = float3(0, 0, 0);
			float2 UV = IN.uv_PackedMap;
			float offset = 1;
			for (float d = 0.0; d < _marchDistance; d += _marchDistance / _numSteps)
			{
				UV -= (IN.viewDir*d)/_numSteps *  tex2D(_PackedMap, IN.uv_PackedMap).r;
				float4 Ldensity = tex2D(_PackedMap, UV);
				InnerStructure += saturate(Ldensity[0])*tex2D(_Ramp2, float2(1/_numSteps * offset, 0.5));
				offset ++;
			}

			float glowPulse = lerp(_glowMin, _glowMax, sin(_Time.y * _glowFrequency))+_glowMax;

			//o.Emission = InnerStructure * glowPulse * _EmissiveColor;
			o.Albedo = albedo + edgeHighlight + (InnerStructure * tex2D(_PackedMap, IN.uv_MainTex).a * _Color * glowPulse);

			
		}
		ENDCG
	}
	FallBack "Diffuse"
}
