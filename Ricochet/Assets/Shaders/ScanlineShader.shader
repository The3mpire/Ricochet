Shader "Hidden/Scanline"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ScanTex("Scanline Texture", 2D) = "white" {}
		_IntensityMultiplier ("Intensity Multiplier", float) = .025
		_Frequency ("Frequency", float) = 3.0
		_Speed ("Speed", float) = 8.0
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

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
			sampler2D _ScanTex;
			float _IntensityMultiplier;
			float _Frequency;
			float _Speed;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 scan = tex2D(_ScanTex, float2(i.uv.x, i.uv.y + tan( _Time.y / _Frequency) * _Speed));
				fixed4 col = tex2D(_MainTex, float2(i.uv.x - _IntensityMultiplier * ((scan.r + scan.g + scan.b)/3.0), i.uv.y));
				return col;
			}

			ENDCG
		}
	}
}
