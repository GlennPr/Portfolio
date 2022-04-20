Shader "Unlit/DrawDisc"
{
	Properties
	{
	}
	SubShader
	{
		Tags{ "RenderType" = "Transparent" }
		Blend One OneMinusSrcAlpha
		LOD 100

		Pass
		{
			Cull Off
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#pragma instancing_options procedural:setup

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 colorFill : COLOR;
				float2 uv : TEXCOORD1;
			};

			struct Particle
			{
				float2 Position;
				float Unused;
				float Fill;
				float3 Color;
				float Size;
			};

			float4x4 _LocalToWorld;
			float4x4 _WorldToLocal;
			float _Opacity;

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			StructuredBuffer<Particle> _ParticleBuffer;
			uint _InstanceCount;
#endif

			v2f vert(appdata v)
			{
				v2f o;

		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
				UNITY_SETUP_INSTANCE_ID(v);
				Particle p = _ParticleBuffer[unity_InstanceID];

				//p.Position.x = -1.8;

				float2 world = p.Position + v.vertex.xy * p.Size;

				o.vertex = UnityObjectToClipPos(float4(world, 0, 1));

				o.colorFill = float4(p.Color, p.Fill);

		#else
				o.vertex = v.vertex;
				o.colorFill = float4(1,0,0, 1);
		#endif
				o.uv = v.vertex.xy;
				return o;
			}

			void setup()
			{
				unity_ObjectToWorld = _LocalToWorld;
				unity_WorldToObject = _WorldToLocal;
			}

			float linStep(float edge0, float edge1, float x)
			{
				return saturate((x - edge0) / (edge1 - edge0));
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float l = length(i.uv);
				float x = ddx(l);
				float y = ddy(l);
				float dl = sqrt(x * x + y * y);

				float w = dl * 1.5;
				//float alpha = smoothstep(w, 0, abs(l - (1 - w)) );
				float alpha = linStep(w, 0, l - (1 - w));
				alpha *= linStep(w, 0, (1 - w - i.colorFill.w) - l);
				alpha *= _Opacity;
				//float alpha = linStep(w, 0, abs(l - (1 - w)) );
				return float4(i.colorFill.xyz * alpha, alpha);

			}

			ENDCG
		}
	}
}

