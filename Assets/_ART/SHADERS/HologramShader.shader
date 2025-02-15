Shader "Custom/HologramHDRP"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _DistortionStrength("Distortion Strength", Range(0, 1)) = 0.1
        _NoiseIntensity("Noise Intensity", Range(0, 1)) = 0.2
        _Opacity("Opacity", Range(0, 1)) = 0.5
        _VertexDistortion("Vertex Distortion", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderPipeline"="HDRenderPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            float _DistortionStrength;
            float _NoiseIntensity;
            float _Opacity;
            float _VertexDistortion;
            
            float random(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            float2 distortUV(float2 uv, float time)
            {
                float noise = (random(uv + time) - 0.5) * _NoiseIntensity;
                uv += noise;
                return uv;
            }
            
            float3 distortVertex(float3 position, float time)
            {
                float noise = (random(position.xy + time) - 0.5) * _VertexDistortion;
                position.xyz += float3(noise, noise, noise);
                return position;
            }
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float time = _Time.y;
                float3 distortedPosition = distortVertex(IN.positionOS.xyz, time);
                OUT.positionCS = TransformObjectToHClip(distortedPosition);
                OUT.uv = IN.uv;
                return OUT;
            }
            
            float4 frag(Varyings IN) : SV_Target
            {
                float time = _Time.y;
                float2 distortedUV = distortUV(IN.uv, time);
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);
                
                col.a *= _Opacity;
                return col;
            }
            ENDHLSL
        }
    }
}