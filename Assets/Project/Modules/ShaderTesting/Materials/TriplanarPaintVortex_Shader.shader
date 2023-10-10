Shader "Unlit/TriplanarPaintVortex_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Space(20)]

        [Header(PHONG SHADING)] 
        [Space(10)]
        _AmbientColor("Ambient Color", Color) = (0.25, 0.25, 0.35, 1.0)
        _AmbientStrength ("Ambient Strength", Range(0.0, 1.0)) = 0.1
        _SpecularStrength ("Specular Strength", Range(0.0, 10.0)) = 1.0
        _SpecularPow("Smoothness", Range(0.0, 1.0)) = 0.1
        [Space(20)]

        [Header(TRIPLANAR)] 
        [Space(10)]
        _PaintTexture("Paint Texture", 2D) = "white" {}
        _TriplanarBlendFalloff("Triplanar Blend FallOff", Range(1.0, 8.0)) = 5.0
        _TriplanarTextureSize("Triplanar Texture Size", Vector) = (1,1,1,0)
        [Space(20)]
        
        [Header(PAINT NORMALS)] 
        [Space(10)]
        _WorldNormalMultiplier("World Normal Multiplier", Range(0, 3)) = 1
        _PaintNormalMultiplier("Paint Normal Multiplier", Range(0, 3)) = 0.3
        _PaintNormalSteps("Paint Normal Steps", Vector) = (4,4,4,0)

        [Header(VORTEX)] 
        [Space(10)]
        _VortexSpeed("Vortex Speed", Float) = 1
        _PolarUVSize("Polar UV Size", Float) = 1
        _VortexPos("Vortex Pos", Vector) = (0,0,0,0)
        _VortexRadius("Vortex Radius", Range(0, 10)) = 5
        _VortexMaskSharpness("Vortex Mask Sharpness", Range(0, 10)) = 1

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"
            #include "../MyShaderLibraries/MyLighting.cginc"
            #include "../MyShaderLibraries/MyTriplanar.cginc"
            #include "../MyShaderLibraries/MyUVFunctions.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

                float3 worldPosition : TEXCOORD1;
                half3 worldNormal : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _AmbientColor;
            half _AmbientStrength;
            half _SpecularStrength, _SpecularPow;

            sampler2D _PaintTexture;
            float _TriplanarBlendFalloff;
            float3 _TriplanarTextureSize;

            float _PaintNormalMultiplier, _WorldNormalMultiplier;
            float3 _PaintNormalSteps;

            float _VortexSpeed;
            float _PolarUVSize;
            float3 _VortexPos;
            float _VortexRadius;
            float _VortexMaskSharpness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, i.uv);

                float3 triplanarWorldPosition = i.worldPosition * _TriplanarTextureSize;



                float3 pos = float3(_VortexPos.x, i.worldPosition.y, _VortexPos.z);
                float3 center = i.worldPosition - pos;
                float gradientToCenter = length(center) / _VortexRadius;
                gradientToCenter = 1-gradientToCenter;

                float mask = step(0.0f, gradientToCenter);

                float gradientMask = saturate(pow(1-gradientToCenter, _VortexMaskSharpness));
                gradientToCenter = saturate(gradientToCenter);

                float radius = _VortexRadius * 0.5f;
                float2 uvUp = center.xz;
                uvUp *= 0.25f;
                uvUp /= radius * 2.0f;
                uvUp.x += 0.5f;
                uvUp.y += 0.5f;
                uvUp -= 0.5f;
                uvUp *= 2.0f;
                

                float2 polarUvUp = cartesianToPolar(uvUp, UNITY_TWO_PI);


                fixed4 colorUp2 = fixed4(frac(polarUvUp), 0, 1);
                colorUp2 = tex2D(_PaintTexture, (polarUvUp * _PolarUVSize) + (_Time.y * _VortexSpeed));


                fixed4 triplanarColor = getSeamlessTriplanarColor(triplanarWorldPosition, i.worldNormal, 
                                        _PaintTexture, _PaintTexture, _PaintTexture, _TriplanarBlendFalloff);


                triplanarColor = lerp(colorUp2, triplanarColor, gradientMask);

//return triplanarColor;


                float3 defaultWorldNormal = i.worldNormal;
                float3 paintNormal = (defaultWorldNormal * _WorldNormalMultiplier) + (triplanarColor.xyz * _PaintNormalMultiplier);
                float3 steppedWorldNormal = normalize((floor(paintNormal * _PaintNormalSteps) / _PaintNormalSteps) );
                
                float3 worldNormal = steppedWorldNormal;

                // Apply Lighting    
                // DIRECTIONAL LIGHT
                half3 directionToCamera = normalize(_WorldSpaceCameraPos - i.worldPosition); // Better results if computed in FRAGMENT
                baseColor = applyPhong(baseColor, i.worldPosition, worldNormal, 
                                        directionToCamera, _WorldSpaceLightPos0, _WorldSpaceLightPos0.xyz, _AmbientColor,
                                        _AmbientStrength, _SpecularStrength, _SpecularPow);
                

                

                return baseColor;
            }
            ENDCG
        }
    }
}
