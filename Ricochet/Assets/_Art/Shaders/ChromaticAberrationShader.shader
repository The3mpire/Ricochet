Shader "Hidden/ChromaticAberration"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Intensity ("Intensity", float) = .025
		_Frequency ("Frequency", float) = 10.0
		_TravelRate ("Travel Rate", float) = 0.0
		_MaskFrequency ("Frequency Mask", float) = 10.0
		_MaskTravelRate ("Mask Travel Rate", float) = 2.0
		_SimulationSpeed ("Simulation speed of the effect", Range(1.0, 20.0)) = 1.0
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
			float _Intensity;
			float _Frequency;
			float _TravelRate;
			float _MaskFrequency;
			float _MaskTravelRate;
			float _SimulationSpeed;

			float rand(float2 co){
				return frac(sin(dot(co.xy ,fixed2(12.9898,78.233))) * 43758.5453);
			}

			fixed ToyAberration(){
				//Credit for this block of math math: https://www.shadertoy.com/view/Mds3zn
				float xAmnt = 0;
				xAmnt = (1.0 + sin(_Time.y*6.0)) * 0.5;
				xAmnt *= 1.0 + sin(_Time.y*16.0) * 0.5;
				xAmnt *= 1.0 + sin(_Time.y*19.0) * 0.5;
				xAmnt *= 1.0 + sin(_Time.y*27.0) * 0.5;
				xAmnt = pow(xAmnt, 3.0);
				xAmnt *= 0.05;
				xAmnt = min(xAmnt, _Intensity);
				return xAmnt;
			}

			fixed MaskedWave(){
				fixed transTime = _Time.y * _SimulationSpeed;
				fixed waveOffset = _Intensity * sin(transTime);
				fixed xAmnt = max(0.0, sin((transTime - transTime * _TravelRate)/_Frequency)) * max(0.0, _Intensity * sin((transTime - transTime * _MaskTravelRate)/_MaskFrequency) + waveOffset);
				return xAmnt;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed xAmnt = MaskedWave();

				fixed yAmnt = sin(xAmnt);

				fixed3 col;
				col.r = tex2D(_MainTex, fixed2(i.uv.x + xAmnt, i.uv.y + yAmnt)).r;
				col.g = tex2D(_MainTex, fixed2(i.uv.x, i.uv.y)).g;
				col.b = tex2D(_MainTex, fixed2(i.uv.x - xAmnt, i.uv.y - yAmnt)).b;
				
				return fixed4(col, 1.0);
			}
			ENDCG
		}
	}
}
