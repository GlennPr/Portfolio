Shader "GK/Effect/IntersectionVisualiser"
{
    Properties
    {
        [Toggle(_KEEP_SIZE_CONSISTENT)] _Keep_Size_Consistent("Keep size 3D consistent", Float) = 1
        [Space]
        _Color("Color", Color) = (1,1,1,1)
        [Space]
        _RotationAngle("RotationAngle", Range(0, 360)) = 45
        _Scale("Scale", Float) = 20
        _BarSize("BarSize", Range(0, 1)) = 0.333
        _BorderPixelThickness("BorderPixelThickness", Range(1, 10)) = 2
        _ScrollSpeed("ScrollSpeed", Range(-5, 5)) = 0
    }
        SubShader
        {
            Tags
            {
                "DisableBatching" = "True"
                "RenderType" = "Opaque"
                "Queue" = "Transparent-100"
            }
            LOD 100


            Pass // invisible pass to Fill in stencil values there where ZFails
            {
                Stencil
                {
                    Ref 0
                    Comp Always
                    ZFail IncrSat
                }

                Blend One One
                Cull Front
                ZWrite Off
                Ztest On

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

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    return fixed4(0, 0, 0, 0); // be invisible
                }
                ENDCG
            }


            Pass // pass to draw lines if the previous pass succedeed filling in stencil values
            {
                Stencil
                {
                    Ref 0
                    Comp NotEqual
                }

                Blend SrcAlpha OneMinusSrcAlpha
                Cull Back
                ZWrite Off
                Ztest On

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;

                    /* Use this if using in VR
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                    */
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float4 screenPos : TEXCOORD1;
                    float CameraDistance : TEXCOORD2;

                    /* Use this if using in VR
                    UNITY_VERTEX_OUTPUT_STEREO
                    */
                };

                float _Keep_Size_Consistent;

                half4 _Color;
                float _RotationAngle;
                float _Scale;
                float _BarSize;
                float _BorderPixelThickness;

                float _ScrollSpeed;

                v2f vert(appdata v)
                {
                    v2f o;

                    /* Use this if using in VR
                    // https://docs.unity3d.com/2018.1/Documentation/Manual/SinglePassInstancing.html
                    UNITY_SETUP_INSTANCE_ID(v); //Insert
                    UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert
                    */

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;

                    o.screenPos = ComputeScreenPos(o.vertex);
                    float3 pivotWorldPosition = unity_ObjectToWorld._m03_m13_m23;
                    o.CameraDistance = distance(pivotWorldPosition, _WorldSpaceCameraPos);

                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    const float PI = 3.14159;

                    float2 screenPosition = (i.screenPos.xy / i.screenPos.w);
                    float2 screenPosFromCenter = screenPosition * 2 - 1;
                    screenPosFromCenter.x *= (_ScreenParams.x / _ScreenParams.y);
                    screenPosFromCenter.xy *= _Scale * lerp(1, i.CameraDistance, _Keep_Size_Consistent);

                    float rotation = _RotationAngle * (PI / 180);
                    float sinX = sin(rotation);
                    float cosX = cos(rotation);
                    float sinY = sin(rotation);
                    float2x2 rotationMatrix = float2x2(cosX, -sinX, sinY, cosX);
                    screenPosFromCenter.xy = mul(screenPosFromCenter.xy, rotationMatrix);

                    float size = (_BarSize * 0.5);
                    float bar = frac(screenPosFromCenter.x + _Time.y * _ScrollSpeed);
                    float barOriginal = bar;
                    bar -= 0.5;
                    bar = (abs(bar) - size) / (fwidth(bar) * floor(_BorderPixelThickness));
                    bar = saturate(bar);
                   
                    float alpha = 1 - bar;
                    alpha -= fwidth(barOriginal); // cancel out artifacts where the bars meet
                    alpha = saturate(alpha);

                    alpha = lerp(alpha, 1, pow(_BarSize, 25)); // Where the bars meet there will be a 1 pixel gap at maximum size, so at the last moment we fade in completetly

                    alpha = lerp(alpha, 1, smoothstep(10, 15, i.CameraDistance)); // to use if lines become 'too thin' on screen and instead use a solid color

                    return fixed4(_Color.r, _Color.g, _Color.b, alpha * _Color.a);
                }
                ENDCG
            }

            Pass // invisible pass revert the Fill in stencil values there where ZFails
            {
                Stencil
                {
                    Ref 0
                    Comp Always
                    ZFail DecrSat
                }

                Blend One One
                Cull Front
                ZWrite Off
                Ztest On

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

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    return fixed4(0, 0, 0, 0); // be invisible
                }
                ENDCG
            }
        }
}