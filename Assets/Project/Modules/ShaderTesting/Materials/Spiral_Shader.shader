Shader "Unlit/Spiral_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _NumberOfSpikes("Number of Spikes", Range(0, 20)) = 3
        _SpikeFade("Spike Fade", Range(0, 1)) = 0.1
        _SpikeSize("Spike Size", Range(0, 10)) = 0.3
        _SpikeOriginOffset("Spike Origin Offset", Range(0, 10)) = 0.3
        _RotationSpeed("Rotation Speed", Range(-10, 10)) = 1.0
        _TwistStraightness("Twist Straightness", Range(1, 10)) = 1

        _SpiralOffset("SpiralOffset", Range(0, 13.3)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "../MyShaderLibraries/MySpiral.cginc"

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

            float _NumberOfSpikes;
            float _SpikeFade;
            float _SpikeSize;
            float _SpikeOriginOffset;
            float _RotationSpeed;
            float _TwistStraightness;

            float _SpiralOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 finalColor = fixed4(0,0,0,1);

                finalColor.x = makeSpiral(i.uv, _NumberOfSpikes, _SpikeSize, _SpikeFade, _SpikeOriginOffset, _TwistStraightness, _RotationSpeed, _Time.y);
                finalColor.y = makeSpiral(i.uv, _NumberOfSpikes, _SpikeSize, _SpikeFade, _SpikeOriginOffset, _TwistStraightness, _RotationSpeed, _Time.y + _SpiralOffset/2.0f);
                finalColor.z = makeSpiral(i.uv, _NumberOfSpikes, _SpikeSize, _SpikeFade, _SpikeOriginOffset, _TwistStraightness, _RotationSpeed, _Time.y + _SpiralOffset);

                return finalColor;
            }
            ENDCG
        }
    }
}
