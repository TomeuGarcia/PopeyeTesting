Shader "Unlit/AnchorChain_Mat"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _NormalColor("Normal Color", Color) = (0,0,1,1)
        _ChargedColor("Charged Color", Color) = (1,0,0,1)
        _ChargedPer1("Charged Per 1", Range(0,1)) = 0

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
                float3 objectPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _NormalColor, _ChargedColor;
            float _ChargedPer1;

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
                float t = i.uv.x;
                float chargedThreshold = step(_ChargedPer1, t);

                fixed4 finalColor = lerp(_ChargedColor, _NormalColor, chargedThreshold);

                float fade = 1.0f - (t * 0.1f);
                finalColor *= fade;

                return finalColor;
            }
            ENDCG
        }
    }
}
