Shader "Custom/Texture2D"
{
    Properties
    {
		_MaskTex("Mask", 2D) = "white" {}
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 maskTexUv : TEXCOORD0;
				float2 mainTexUv : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MaskTex;
			float4 _MaskTex_ST;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.maskTexUv = TRANSFORM_TEX(v.uv, _MaskTex);
				o.mainTexUv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 maskTexColor_ = tex2D(_MaskTex, i.maskTexUv);
				fixed4 mainTexColor_ = tex2D(_MainTex, i.mainTexUv);
				return fixed4(mainTexColor_.rgb, maskTexColor_.a);
			}
			ENDCG
		}
    }
}
