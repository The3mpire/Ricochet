Shader "Hidden/ChromaticAberration Redux"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Intensity ("Intensity", float) = .025
		_Cycles ("Cycles", float) = 1
		_Progress ("Progress", Range(0,1)) = 0
		_DefaultValue ("Default Value", float) = .002
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
			float _Cycles;
			float _Progress;
			float _DefaultValue;

			fixed Cycle(){
				float min = 0;
				float max = 180;
				float amount = sin(radians(lerp(min, max, _Progress))/(1/_Cycles));
				return amount;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed xAmnt = _Intensity * Cycle();
				xAmnt += _DefaultValue;
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
