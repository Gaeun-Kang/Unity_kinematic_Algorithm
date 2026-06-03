Shader "Custom/ColorPencilTrail_Stroke"
{
  Properties
    {
        
        [Header(Texture Setting)]
        _MainTex("Pencil Texture (Grayscale/Alpha)", 2D) = "white" {}
        _TextureScale("Texture World Scale", Float) = 0.5 // 월드 단위 대비 텍스처 크기 조절

        [Header(Color Settings)]
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
        _ColorMix("Color Mix Intensity", Range(0,1)) = 1.0

        [Header(Alpha Settings)]
        _MainTexVFade("MainTex V Fade", Range(0, 1)) = 0
        _MainTexVFadePow("MainTex V Fade Pow", Float) = 2
        _MainTexMultiplier("Pencil Density", Float) = 1
        _Multiplier("Overall Multiplier", Float) = 1
        _AlphaClip("Alpha Clip Threshold", Range(0, 1)) = 0.1 
    }
    SubShader
    {
        Tags { 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "Transparent"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha 
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float2 uvOrigin : TEXCOORD0;
                float3 worldPos : TEXCOORD1; // 월드 좌표 전달
                float4 positionHCS : SV_POSITION;
                half4 color : COLOR;
            };

            sampler2D _MainTex;

            CBUFFER_START(UnityPerMaterial)
                half4 _MainTex_ST;
                half4 _BaseColor;
                half _ColorMix;
                half _TextureScale;
                half _MainTexVFade;
                half _MainTexVFadePow;
                half _MainTexMultiplier;
                half _Multiplier;
                half _AlphaClip;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                
                // 1. 버텍스의 월드 공간 좌표 계산
                o.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                
                o.uvOrigin = IN.uv; 
                o.color = IN.color;
                
                return o;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                /* 2. World Space UV 생성
                   단순히 X, Y, Z 좌표를 조합하여 평면 투영을 수행합니다.
                   VR에서는 이 방식이 헤드 트래킹에 상관없이 텍스처를 공간에 고정시킵니다. */
                float2 worldUV;
                
                // 트레일이 주로 수평으로 움직인다면 x, z를 사용하고, 수직이라면 y를 섞습니다.
                // 아래는 가장 범용적인 X+Y 조합 방식입니다.
                worldUV.x = (IN.worldPos.x + IN.worldPos.z) * _TextureScale;
                worldUV.y = (IN.worldPos.y + (IN.worldPos.x * 0.5)) * _TextureScale;

                half4 mainTex = tex2D(_MainTex, worldUV);

                // 3. 색상 계산 
                half3 finalRGB = lerp(IN.color.rgb, _BaseColor.rgb, _ColorMix);

                // 4. 세로 페이드 (기존 Trail UV 사용)
                half vFade = 1 - abs(IN.uvOrigin.y - 0.5) * 2; 
                vFade = pow(abs(vFade), _MainTexVFadePow); 
                vFade = lerp(1, vFade, _MainTexVFade);

                // 5. 최종 알파값 및 클리핑
                half finalAlpha = mainTex.r * vFade * IN.color.a * _Multiplier * _MainTexMultiplier;
                clip(finalAlpha - _AlphaClip);

                half4 col;
                col.rgb = finalRGB;
                col.a = saturate(finalAlpha);

                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Unlit"
}