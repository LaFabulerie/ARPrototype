﻿Shader "Custom/Transparent"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

		Pass
		{
			ColorMask 0
		}

		Cull Off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade fullforwardshadows interpolateview halfasview
        #pragma target 3.0

        sampler2D _MainTex;
		fixed4 _Color;
		half _Metallic;
		half _Glossiness;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 texColor = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = texColor.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
			o.Alpha = texColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
