Shader "Custom/birdtest"
{
     Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Boid
            {
                float3 position;
                float3 direction;
                float noise_offset;
            };

            StructuredBuffer<Boid> boidsBuffer;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : NORMAL;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert (Attributes IN, uint instanceID : SV_InstanceID)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                Boid boid = boidsBuffer[instanceID];

                float4 worldPos = float4(IN.positionOS + boid.position, 1.0);
                OUT.positionCS = TransformWorldToHClip(worldPos.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                return half4(1,1,1,1); // ±âº» Èò»ö
            }
            ENDHLSL
        }
    }
}
