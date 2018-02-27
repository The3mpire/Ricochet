Shader "Custom/Sprite Chromatic Aberration"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_Color ("Tint", Color) = (1,1,1,1)
		_XIntensity("X Intensity", Range(-1,1)) = 0
		_YIntensity("Y Intensity", Range(-1,1)) = 0
	}
	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			fixed4 _Color;
			float _AlphaSplitEnabled;
			float _XIntensity;
			float _YIntensity;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);
				#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
					if (_AlphaSplitEnabled)
						color.a = tex2D (_AlphaTex, uv).r;
				#endif
				return color;
			}

			float norm(float value, int min, int max) {
				return (value-min)/(max-min);
			}

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}
			
			fixed4 frag (v2f IN) : SV_Target
			{
				float4 col;
				col.r = SampleSpriteTexture(float2(IN.texcoord.x + _XIntensity, IN.texcoord.y + _YIntensity)).r;
				col.g = SampleSpriteTexture(float2(IN.texcoord.x, IN.texcoord.y)).g;
				col.b = SampleSpriteTexture(float2(IN.texcoord.x - _XIntensity, IN.texcoord.y - _YIntensity)).b;
				col.a = SampleSpriteTexture(IN.texcoord).a;

				col = col * IN.color;
				col.rgb *= col.a;

				return col;
			}
			ENDCG
		}
	}
}
