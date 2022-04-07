Shader "GK/UI/ChevronArrow"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
        [Space]
        [KeywordEnum(Horizontal, Vertical)] _Layout("Layout Mode", Float) = 0

        _Progress("Progress", Range(0, 1)) = 0
        
        [Header(Settings)]
        _MaxAngle("MaxAngle", Range(0, 45)) = 30
        _LineDimensions_XY("Line Dimensions XY", Vector) = (4, 1, 0, 0)

        [Space]
        _RoundedCornerPercentange("RoundedCorner", Range(0, 1)) = 0.5
        _FadeRange("FadeRange", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Tags
        {   
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile _LAYOUT_HORIZONTAL _LAYOUT_VERTICAL

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            static const float PI = 3.14159;

            float _Progress;
            float _MaxAngle;
            float2 _LineDimensions_XY;

            float _RoundedCornerPercentange;
            float _FadeRange;

            sampler2D _MainTex;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            float SDFBox(float2 samplePosition, float2 halfSize)
            {
                float2 componentWiseEdgeDistance = abs(samplePosition) - halfSize;
                float outsideDistance = length(max(componentWiseEdgeDistance, 0));
                float insideDistance = min(max(componentWiseEdgeDistance.x, componentWiseEdgeDistance.y), 0);
                return outsideDistance + insideDistance;
            }

            float2 Rotate(float2 samplePosition, float rotation)
            {
                float angle = rotation * PI * 2 * -1;
                float sine, cosine;
                sincos(angle, sine, cosine);
                return float2(cosine * samplePosition.x + sine * samplePosition.y, cosine * samplePosition.y - sine * samplePosition.x);
            }

            v2f vert(appdata v)
            {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color;
                return o;
            }

            float LineHorizontalLayout(float2 position, float left)
            {
                float pulse = 1-  _Progress;
                float tiltDegrees = lerp(-_MaxAngle, _MaxAngle, pulse);
                float upDownScaler = step(0, tiltDegrees) * 2 - 1;

                tiltDegrees *= left;
                tiltDegrees /= 360;

                float2 rectSize = _LineDimensions_XY;
                rectSize.xy -= _RoundedCornerPercentange;

                float xOffset = cos(tiltDegrees * PI * 2) * -rectSize.x;
                xOffset += sin(tiltDegrees * PI * 2) * rectSize.y * left * upDownScaler;
                
                float2 offset = float2(xOffset * left, 0);
                position -= offset;
                position = Rotate(position, tiltDegrees);
                
                float dist = SDFBox(position, rectSize);
                return dist;
            }

            float LineVerticalLayout(float2 position, float up)
            {
                float pulse = 1 - _Progress;

                float tiltDegrees = lerp(-_MaxAngle, _MaxAngle, pulse);
                float upDownScaler = step(0, tiltDegrees) * 2 - 1;

                tiltDegrees *= up;
                tiltDegrees /= 360;
                
                float2 rectSize = float2(_LineDimensions_XY.y, _LineDimensions_XY.x);
                rectSize.xy -= _RoundedCornerPercentange;

                float yOffset = cos(tiltDegrees * PI * 2) * rectSize.y;
                yOffset += sin(tiltDegrees * PI * 2) * -rectSize.x * up * upDownScaler;
                
                float2 offset = float2(0, yOffset * up);
                position -= offset;
                position = Rotate(position, tiltDegrees);

                float dist = SDFBox(position, rectSize);
                return dist;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                float2 pos = i.texcoord * 2 - 1;
                pos *= 10; // just felt like an easier scale to work with in editor
                
                float leftLineDist = 100;
                float rightLineDist = 100;

                #ifdef _LAYOUT_HORIZONTAL
                    leftLineDist = LineHorizontalLayout(pos, 1);
                    rightLineDist = LineHorizontalLayout(pos, -1);
                #else
                    leftLineDist = LineVerticalLayout(pos, 1);
                    rightLineDist = LineVerticalLayout(pos, -1);
                #endif

                float dist = min(leftLineDist, rightLineDist);
                float alpha = 1 - smoothstep(_RoundedCornerPercentange, _RoundedCornerPercentange + fwidth(dist) + _FadeRange, dist);

                fixed4 col = fixed4(1, 1, 1, alpha);
                col.rgba *= i.color.rgba;
                return col;
            }
            ENDCG
        }
    }
}