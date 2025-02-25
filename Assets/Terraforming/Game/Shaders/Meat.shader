Shader "Custom/MeatHDRP"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "ForwardOnly" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _BaseColor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                return texColor * _BaseColor;
            }
            ENDHLSL
        }
    }
}