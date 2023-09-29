Shader "Unlit/AreaHit_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaveColor ("Wave Color", Color) = (1,0,0,1)
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 1
        _WaveFrequency ("Wave Frequency", Range(0, 10)) = 1
        _WaveSharpness("Wave Sharpness", Range(0, 10)) = 1
        _StartTime("Start Time", Range(0, 10)) = 1
        _WaveDuration("Wave Duration", Range(0, 10)) = 1
        _TimeOverwrite("Time Overwrite", Range(-1, 10)) = 1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

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
                float3 objectPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _WaveColor;
            float _WaveSpeed, _WaveFrequency, _WaveSharpness, _WaveDuration;
            float _StartTime, _TimeOverwrite;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.objectPosition = v.vertex.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 endColor = _WaveColor;

                float time = _Time.y - _StartTime + (0.4f * _WaveDuration);
                time = lerp(_TimeOverwrite, time, step(_TimeOverwrite, -0.1));

                float x = ((i.objectPosition.y * _WaveDuration * _WaveFrequency + time * _WaveSpeed) % _WaveDuration) / _WaveDuration;
                endColor.w = pow(abs(1.0f - x), _WaveSharpness);

                return endColor;
            }
            ENDCG
        }
    }
}
