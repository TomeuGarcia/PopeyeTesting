Shader "Unlit/ElectricChain_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            float sin01(float t)
            {
                return (sin(t) + 1.0f) * 0.5f;
            }


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

                float X = i.uv.x;
                float Y = i.uv.y;

                float value = 0.0f;

                /*
                float shake = (Y*0.02f + X) * 3 * sin01(_Time.y * 100);
                shake %= 0.9f;
                value = sin01((Y * 20) + sin01(shake *100));
                value /= 1.0f;
                value = smoothstep(value, 0.1f, 1.0f);
                */

                float wave1 = sin(X*10) * sin(X*10 + _Time.y*30) * 1.5f;
                float wave2 = sin(X*25) * sin(_Time.y*20) * 2.9f;
                float wave3 = sin(X*15);
                value = sin01(Y*10 + wave1 + wave2 + wave3);

                finalColor.x = value;
                return finalColor;
            }
            ENDCG
        }
    }
}
