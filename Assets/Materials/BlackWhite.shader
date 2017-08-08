Shader "Custom/BlackWhite"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_alpha("alpha", float) = 1
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		// No culling or depth
		Cull Off ZWrite Off ZTest Off

		Pass
		{
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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			uniform float _alpha;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float luminance = dot(col, fixed3(0.299, 0.587, 0.114));
				return step(_alpha, 0.5) * fixed4(luminance, luminance, luminance, col.a) + (1 - step(_alpha, 0.5)) * col;
			}
			ENDCG
		}
	}
}
