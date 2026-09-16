Shader "Custom/Wall Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        [Header(Face Colors)]
        _FrontColor ("Front Color", Color) = (1,1,1,1)
        _TopColor ("Top Color", Color) = (0.65,0.65,0.65,1)
        _SideColor ("Side Color", Color) = (0.4,0.4,0.4,1)
        _BottomColor ("Bottom Color", Color) = (0.2,0.2,0.2,1)
        
        [Header(Outline)]
        _OutlineColor ("Outline Color", Color) = (0,0,0,0.8)
        _OutlineWidth ("Outline Width (pixels)", Range(0.1, 20)) = 2
        _OutlineSoftness ("Outline Softness", Range(0.01, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            
            float4 _FrontColor;
            float4 _TopColor;
            float4 _SideColor;
            float4 _BottomColor;
            
            float4 _OutlineColor;
            float _OutlineWidth;
            float _OutlineSoftness;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float3 n = normalize(IN.normalWS);

                // Определяем грань по нормали
                float3 faceColor;
                if (n.z < -0.5)        faceColor = _FrontColor.rgb;
                else if (n.z > 0.5)    faceColor = _FrontColor.rgb;
                else if (n.y > 0.5)    faceColor = _TopColor.rgb;
                else if (n.y < -0.5)   faceColor = _BottomColor.rgb;
                else                   faceColor = _SideColor.rgb;

                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half3 baseColor = tex.rgb * faceColor;

                // === ИСПРАВЛЕННАЯ ОБВОДКА ===
                float2 distToEdge = min(IN.uv, 1.0 - IN.uv);
                float2 pixelDist = distToEdge / max(fwidth(IN.uv), 1e-6); // защита от деления на 0
                float edgeDistPixels = min(pixelDist.x, pixelDist.y);

                float outlineMask = 1.0 - smoothstep(
                    _OutlineWidth * (1.0 - _OutlineSoftness),
                    _OutlineWidth,
                    edgeDistPixels
                );

                half3 finalColor = lerp(baseColor, _OutlineColor.rgb, outlineMask * _OutlineColor.a);
                return half4(finalColor, 1);
            }
            ENDHLSL
        }
    }
}