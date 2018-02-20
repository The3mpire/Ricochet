/*
Base shader courtesy of https://github.com/staffantan/unity-vhsglitch 
Customizations and efficiency changes made by Chris Carlson
*/

Shader "Hidden/VHSPostProcessEffect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_VHSTex ("VHS Glitch Texture", 2D) = "white" {}
		_BleedColor ("Bleed Color", Color) = (0,0,0,0)
		_BleedIntensity ("Inverse Scaling of bleed Intensity", float) = 15
	}

	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
					
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _VHSTex;
			float4 _BleedColor;
			float _BleedIntensity;


			fixed4 frag (v2f_img i) : COLOR{

				fixed4 vhs = tex2D (_VHSTex, i.uv);
				fixed4 c = tex2D (_MainTex, i.uv);

				float4 bleed = tex2D(_MainTex, i.uv + float2(0.01, 0));
				bleed += tex2D(_MainTex, i.uv + float2(0.02, 0));
				bleed += tex2D(_MainTex, i.uv + float2(0.01, 0.01));
				bleed += tex2D(_MainTex, i.uv + float2(0.02, 0.02));
				bleed /= _BleedIntensity;

				bleed *= _BleedColor;

				vhs += bleed;

				return c + vhs;
			}
			ENDCG
		}
	}
Fallback off
}