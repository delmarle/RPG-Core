Shader "Custom/MossMask" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        [Toggle] _Use_Mask ("Use mask?", Float) = 0
        _MainTex ("Albedo", 2D) = "black" {}
        [NoScaleOffset]_BumpMap("Normal", 2D) = "bump" {}
        [NoScaleOffset]_DetailBump("Detail Normal", 2D) = "bump" {}
        _DetailScale("Detail Scale", Float) = 1.0
        [NoScaleOffset]_OcclusionMap("Occlusion", 2D) = "white" {}
        _OcclusionStrength("Occlusion Strength", Range(0.0, 1.0)) = 1.0
        [NoScaleOffset]_MetallicRough ("Metallic/Roughness (RGBA)", 2D) = "white" {}
        [Gamma] _Metallic("Metallic", Range(0, 1)) = 0
        _Glossiness("Smoothness", Range(0, 1)) = 0.5

        _TopAlbedo ("Top Albedo", 2D) = "white" {}
        [NoScaleOffset]_TopNormal("Top Normal", 2D) = "bump" {}
        _TopNormal2("Top Normal Detail", 2D) = "bump" {}


        [NoScaleOffset]_TopMetallicRough ("Metallic/Roughness (RGBA)", 2D) = "white" {}
        [Gamma] _TopMetallic("Metallic", Range(0, 1)) = 0
        _TopGlossiness("Smoothness", Range(0, 1)) = 0.5

        _Emission ("Emission", 2D) = "black" {}
        [HDR]_EmissiveColor ("EmissiveColor", Color) = (1,1,1,1)
    }
    SubShader {
         Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 200
        
        CGPROGRAM
        #pragma multi_compile _ LOD_FADE_CROSSFADE
        #pragma surface surf Standard fullforwardshadows
        #pragma target 5.0
        #pragma multi_compile __ _USE_MASK_ON
        #include "UnityStandardUtils.cginc"
        #include "UnityCG.cginc" 
        #include "AutoLight.cginc"
        //#include "Tessellation.cginc"

       
        // Reoriented Normal Mapping
        // http://blog.selfshadow.com/publications/blending-in-detail/
        // Altered to take normals (-1 to 1 ranges) rather than unsigned normal maps (0 to 1 ranges)
        half3 blend_rnm(half3 n1, half3 n2)
        {
            n1.z += 1;
            n2.xy = -n2.xy;

            return n1 * dot(n1, n2) / n1.z - n2;
        }


        sampler2D _MainTex, _TopAlbedo, _BumpMap, _TopNormal, _TopNormal2, _Emission, _OcclusionMap, _MetallicRough, _TopMetallicRough;
        sampler2D _PaintNormal, _DetailBump;
        float4 _Top_ST;
        half _Glossiness, _FresnelAmount, _FresnelPower, _TopNormal2Scale, _TopScale, _NoiseAmount, _NoiseFallOff, _Metallic, _TopMetallic, _TopGlossiness, _OcclusionStrength, _noiseScale, _MaskNormalScale;
        half4 _Color, _EmissiveColor;
        half _MaskNormalAmount, _MossAmount, _DetailScale;


        struct Input {
            float4 screenPos;
            float3 worldPos;
            float3 viewDir;
            float3 worldNormal;
            float2 uv_MainTex;
            float2 uv_TopAlbedo;
            float2 uv_TopNormal2;
            float2 uv_PaintNormal;
            INTERNAL_DATA
        };

        void surf (Input IN, inout SurfaceOutputStandard o) {

            //mask texture
           half mask;

            #if _USE_MASK_ON
                mask = tex2D(_MainTex, IN.uv_MainTex).a; 

            #else
                mask = 0;
            #endif  


            //tangent space normal maps
            half3 tnormalY = UnpackNormal(tex2D(_TopNormal,  IN.uv_TopAlbedo));
            half3 tnormalY2 = UnpackNormal(tex2D(_TopNormal2,  IN.uv_TopNormal2));
            half3 normalMain = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
            half3 detailNormal = UnpackNormal(tex2D(_DetailBump, IN.uv_MainTex * _DetailScale));

            normalMain = blend_rnm(normalMain, detailNormal);

            tnormalY = blend_rnm(tnormalY, normalMain);
            tnormalY = blend_rnm(tnormalY, tnormalY2);

            //Albedo
            float fresnel = (dot(tnormalY, IN.viewDir));
            fresnel = clamp(pow(1-fresnel, _FresnelPower), 0, 6);
            fixed4 colY = tex2D(_TopAlbedo,  IN.uv_TopAlbedo);
            colY += colY*lerp(-1, 1, fresnel)*_FresnelAmount/2;
            fixed4 colMain = tex2D(_MainTex, IN.uv_MainTex);

            //Occlusion
            half occ =  lerp(1, tex2D(_OcclusionMap, IN.uv_MainTex), _OcclusionStrength);
            fixed4 col = lerp(colMain, colY * occ, mask) ;

            //Metallic/Smoothness
            half4 metallicSmoothness = tex2D(_MetallicRough, IN.uv_MainTex);
            half4 TopMetallicSmoothness = tex2D(_TopMetallicRough,  IN.uv_MainTex);
            half m = lerp(metallicSmoothness.r * _Metallic, TopMetallicSmoothness.r * _TopMetallic, mask);
            half s = lerp(metallicSmoothness.a * _Glossiness, TopMetallicSmoothness.a * _TopGlossiness, mask);

             #ifdef LOD_FADE_CROSSFADE
            float2 vpos = IN.screenPos.xy / IN.screenPos.w * _ScreenParams.xy;
            UnityApplyDitherCrossFade(vpos);
            #endif
            //set surface ouput properties

            o.Albedo = col;
            o.Occlusion = occ;
            o.Metallic = m;
            o.Smoothness = s; 
            

            #if _USE_MASK_ON

                o.Normal = lerp(normalMain, tnormalY, mask);

                o.Emission = tex2D(_Emission, IN.uv_MainTex) * _EmissiveColor * (1-mask*0.99);   
            #else
                o.Normal = lerp(normalMain, tnormalY, mask);
                o.Emission = tex2D(_Emission, IN.uv_MainTex) * _EmissiveColor;
            #endif   
                      
        }
        ENDCG

    }
    FallBack "Diffuse"
}
