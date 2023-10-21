Shader "Unlit/ElectricChain_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _BaseColor("Base Color", Color) = (0.0, 0.0, 1.0, 1.0)
        _ElectricityColor("Electricity Color", Color) = (1.0, 1.0, 0.4, 1.0)
        _Scale("Scale", Range(0.0, 1.0)) = 0.3
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
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

            fixed4 _BaseColor;
            fixed4 _ElectricityColor;
            float _Scale;


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


                float time = _Time.y * 0.11f;
                X += time;
                X *= _Scale;
                Y += sin01(time*100) * 10;

                float wave1 = sin(X*10) * sin(X*10 + time*30) * 1.5f;
                float wave2 = sin(X*25) * sin(time*20) * 2.9f;
                float wave3 = sin(X*15);
                float wave4 = sin(X*150) * sin01((X) *380)*2.5f;
                value = sin01(Y*10 + wave1 + wave2 + wave3 + wave4);

                float steppedValue = step(sin01((time+X*0.05f)*100)*0.2f, value);

                finalColor = lerp(_ElectricityColor, _BaseColor,steppedValue);
                return finalColor;
            }
            ENDCG
        }
    }
}
