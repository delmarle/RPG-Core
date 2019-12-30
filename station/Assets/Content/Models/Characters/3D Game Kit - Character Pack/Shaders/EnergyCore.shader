// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/EnergyCore" {
	Properties {
		[hdr]_Color ("Color", Color) = (1,1,1,1)
		[hdr]_Color2 ("Color2", Color) = (1,1,1,1)
		[NoScaleOffset]_noiseTex ("Noise tex (RGB)", 2D) = "white" {}
		[NoScaleOffset]_Ramp ("Ramp (RGB)", 2D) = "white" {}
		_Scale ("Scale1", Float) = 0.5

		_speed ("Effect Speed", Float) = 1
		_vertOffset ("Vertex Offset", Float) = 1

	}
	SubShader {
		Tags { "RenderType"="Opaque"  "DisableBatching" = "True"}
		LOD 200
		//Blend One One
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _Ramp, _noiseTex;
		//sampler3D _3dTex;
		fixed4 _Color;
		fixed4 _Color2;
		float _Scale, _speed;
		half _vertOffset, _vertScale;
		fixed4 _vertSpeed;

		struct Input {
			float2 uv_noiseTex;
			float3 viewDir;
			float3 worldPos;
			INTERNAL_DATA		
		};


		void vert (inout appdata_full v, out Input o) {
        	UNITY_INITIALIZE_OUTPUT(Input,o);


          	float4 uvs1 =  v.texcoord;

          	uvs1.y -= _Time*30*_speed;
			uvs1.x -= _Time*5*_speed;
        	float4 tex = tex2Dlod(_noiseTex, uvs1 * _Scale);
			v.vertex.xyz += lerp(-_vertOffset * v.normal, _vertOffset * v.normal, tex.x);
    	  }

		

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			float3 modelPos = mul (unity_ObjectToWorld, float4(0,0,0,1));

			float2 uvs1 = IN.uv_noiseTex;
			uvs1.y -= _Time*30*_speed;
			uvs1.x -= _Time*5*_speed;
			float distortion = tex2D(_noiseTex, uvs1*_Scale).x;

			float2 uvs2 = float2(rim + (distortion*0.5) + ((distortion)*pow(rim,2)), 0);
			float ramp = tex2D(_Ramp, uvs2).r;
			float3 rim2 =  pow(rim, 4) * _Color;
			float3 col = lerp(_Color2, _Color*ramp, ramp);

			o.Emission = col + rim2*0.05;
		}
		ENDCG
	}
	FallBack "Diffuse"  

		
}
