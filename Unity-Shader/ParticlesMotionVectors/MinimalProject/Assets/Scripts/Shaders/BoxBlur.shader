Shader "Unlit/BoxBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off
		ZWrite Off

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 uSourceTexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float3 ts(float2 uv)
            {
	            return tex2D(_MainTex, uv).xyz;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 o = uSourceTexelSize.xyxy * float2(-1, 1).xxyy;
	            float3 s = ts(i.uv + o.xy) + ts(i.uv + o.zy) + ts(i.uv + o.xw) + ts(i.uv + o.zw) ;
                return float4(s * 0.25, 1.);
            }
            ENDCG
        }
    }
}
