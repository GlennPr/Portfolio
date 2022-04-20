Shader "ParticleScreen/ParticleMesh"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "black" {}
	}
	
	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}
		LOD 100

		Blend One OneMinusSrcAlpha
		ZWrite Off
		Cull Back

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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD1;
				float4 screenPosPrev : TEXCOORD2;
				float3 viewDir : TEXCOORD3;
				float3 normal : TEXCOORD4;
			};

			float4x4 _ViewProjection;
			float4x4 _ViewProjectionPrev;
			  
			float4x4 _LocalToWorldMatrix;
			float4x4 _LocalToWorldMatrixPrev;

			float3 _CameraWorldPos;
			float _Strength;

			float _IgnoreMotionVectorScaling;
			float _InvertMotionVectorScaling;

			sampler2D _MainTex;
			float4 _MainTex_ST;


			v2f vert(appdata v)
			{
				v2f o;

				float4 worldPosPrev = mul(_LocalToWorldMatrixPrev, v.vertex);
				float4 worldPos = mul(_LocalToWorldMatrix, v.vertex);

				o.screenPosPrev = mul(_ViewProjectionPrev, worldPosPrev);
				o.screenPos = mul(_ViewProjection, worldPos);

				o.vertex = o.screenPosPrev;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				//o.vertex = UnityObjectToClipPos(v.vertex);

				o.normal = mul((float3x3)_LocalToWorldMatrix, v.normal);
				o.viewDir = worldPos.xyz - _CameraWorldPos;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 n = normalize(i.normal);
				float3 v = normalize(i.viewDir);

				float nDotV = abs(dot(n, v));
				float fresnel = 1 - nDotV;
				fresnel = pow(fresnel, 2.0);

				float brightness = tex2D(_MainTex, i.uv).x;
				brightness += fresnel * 0.2;

				float strength = saturate(brightness) * _Strength;


				float2 screenPosPrev = i.screenPosPrev.xy / i.screenPosPrev.w;
				float2 screenPos = i.screenPos.xy / i.screenPos.w;
				float2 motionVector = screenPos - screenPosPrev;

				motionVector.y = lerp(motionVector.y, -motionVector.y, _InvertMotionVectorScaling);

				//return float4 (1, 0, 0, 1);

				//if (Global_Story1Mode > 0.5)
				return float4 (strength, motionVector * lerp(strength, 1, _IgnoreMotionVectorScaling), 1);// strength);
				//else
				//	return float4 (strength, motionVector * lerp(_Strength, 1, _IgnoreMotionVectorScaling), lerp(_Strength, strength, _BrightnessToAlpha));


			}
			ENDCG
		}
	}
}
