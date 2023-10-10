Shader "Unlit/CircularFill_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        [Header(PHONG SHADING)] 
        [Space(10)]
        _AmbientColor("Ambient Color", Color) = (0.25, 0.25, 0.35, 1.0)
        _AmbientStrength ("Ambient Strength", Range(0.0, 1.0)) = 0.1
        _SpecularStrength ("Specular Strength", Range(0.0, 10.0)) = 1.0
        _SpecularPow("Smoothness", Range(0.0, 1.0)) = 0.1
        [Space(20)]

        _BaseColor("Base Color", Color) = (1,0,0,1)
        _FillColor("Fill Color", Color) = (0,1,0,1)
        _FillDuration("Fill Duration", Range(0, 30)) = 2
        _StartTime("Start Time", Float) = 0
        _CanUnfill("Can Unfill", Range(0,1)) = 1

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
            #include "../../ShaderTesting/MyShaderLibraries/MyUVFunctions.cginc"
            #include "../../ShaderTesting/MyShaderLibraries/MyLighting.cginc"

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
                float3 objectPosition : TEXCOORD1;
                float3 worldPosition : TEXCOORD2;
                float3 worldNormal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _AmbientColor;
            half _AmbientStrength;
            half _SpecularStrength, _SpecularPow;

            float4 _BaseColor, _FillColor;
            float _FillDuration, _StartTime, _CanUnfill;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.objectPosition = v.vertex.xyz;
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 finalColor = fixed4(0,0,0,1);

                float2 positionUv = i.objectPosition.xz;

                positionUv = cartesianToPolar(positionUv, UNITY_TWO_PI);


                float progress = saturate((_Time.y - _StartTime) / _FillDuration);                

                float fillMask = 1 - step(progress, 0.5f+positionUv.x);
                fillMask *= _CanUnfill;

                finalColor = lerp(_FillColor, _BaseColor, fillMask);


                // Apply Lighting    
                // DIRECTIONAL LIGHT
                half3 directionToCamera = normalize(_WorldSpaceCameraPos - i.worldPosition); // Better results if computed in FRAGMENT
                finalColor = applyPhong(finalColor, i.worldPosition, i.worldNormal, 
                                        directionToCamera, _WorldSpaceLightPos0, _WorldSpaceLightPos0.xyz, _AmbientColor,
                                        _AmbientStrength, _SpecularStrength, _SpecularPow);


                return finalColor;
            }
            ENDCG
        }
    }
}
