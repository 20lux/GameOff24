Shader "Unlit/s_bakedTextures"
{
    Properties
    { 
        [NoScaleOffset] _BakedTexture ("Baked Texture", 2D) = "white" {}

        [NoScaleOffset] _NormalTexture ("Normal Texture", 2D) = "blue" {}
        [HideInInspector] _NormalStrength("Normal Strength", Range(0,1)) = 1
        [HideInInspector] _NormalOffset("Normal Offset", Range(0,1)) = 0.1
        
        [HideInInspector] _AdditionalLightHueFalloff("Additional Light Hue Falloff", Range(0,360)) = 180
        [HideInInspector] _AdditionalLightSaturationFalloff("Additional Light Saturation Falloff", Float) = 1 
        [HideInInspector] _AdditionalLightIntensityCurve("Additional Light Intensity Curve", Range(0.01, 2.0)) = 1
        [HideInInspector] _AmbientLightStrength("Ambient Light Strength", Range(0,1)) = 1
    }
    SubShader
    {
        LOD 100
            HLSLINCLUDE
                #include "Assets/Shaders/HLSLSubFiles/HelperShaderFunctions.hlsl"
                #include "Assets/Shaders/HLSLSubFiles/CustomLightingFunctions.hlsl"
            
                CBUFFER_START(UnityPerMaterial)
                    
                    float _NormalStrength;
                    float _NormalOffset;

                    float _AdditionalLightHueFalloff;
                    float _AdditionalLightSaturationFalloff;
                    float _AdditionalLightIntensityCurve;

                    float _AmbientLightStrength;

                CBUFFER_END
            
                TEXTURE2D(_BakedTexture);
                TEXTURE2D(_NormalTexture);

                SAMPLER(sampler_BakedTexture);
                SAMPLER(sampler_NormalTexture);

                struct VertexInput
                {
                    float4 position : POSITION;

                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;

                    float2 uvBakedTexture : TEXCOORD0;
                    float2 uvNormalTexture : TEXCOORD1;
                    float3 bitangent : TEXCOORD4;
                    float3 worldPos : TEXCOORD5;
                };

                struct VertexOutput 
                {
                    float4 position : SV_POSITION;

                    float2 uvBakedTexture : TEXCOORD0;
                    float2 uvNormalTexture : TEXCOORD1;

                    float3 normal : TEXCOORD2;
                    float4 tangent : TEXCOORD3;
                    float3 bitangent : TEXCOORD4;

                    float3 worldPos : TEXCOOR5;
                };

            ENDHLSL

        Pass
        {
            Tags {"Lightmode" = "UniversalForward"}
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE

            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA

            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

                VertexOutput vert (VertexInput v)
                {
                    VertexOutput o;
                    o.position = TransformObjectToHClip(v.position);
                    o.worldPos = mul(unity_ObjectToWorld, float4(v.position.xyz, 1.0)).xyz;
                    o.uvBakedTexture = v.uvBakedTexture;
                    o.uvNormalTexture = v.uvNormalTexture;
                    o.normal = TransformObjectToWorldNormal(v.normal);
                    float3 tangent = normalize(v.tangent.xyz);
                    o.tangent = (tangent,1);
                    o.bitangent = normalize(cross(o.normal,o.tangent) * v.tangent.w);
                    return o;
                }

                float4 frag (VertexOutput IN) : SV_TARGET
                {
                    float3 bakedTextureRGB = SAMPLE_TEXTURE2D(_BakedTexture, sampler_BakedTexture, IN.uvBakedTexture).rgb;
                    float3 tangentNormals = TextureToTangentNormal(
                        _NormalTexture,
                        sampler_NormalTexture,
                        _NormalStrength,
                        _NormalOffset, IN.uvNormalTexture,
                        1);
                    IN.normal = TangentToWorldNormal(tangentNormals, IN.tangent, IN.bitangent, IN.normal);
                    
                    //inputdata related functions
                    InputData inputData = (InputData)0;
                    half4 calculatedShadows = CalculateShadowMask(inputData);
                    half3 ambientLight = SampleSH(IN.normal) * _AmbientLightStrength;

                    float3 additionalLightsMap = AdditionalLights(
                        IN.worldPos,
                        IN.normal,
                        calculatedShadows,
                        _AdditionalLightIntensityCurve,
                        _AdditionalLightHueFalloff,
                        _AdditionalLightSaturationFalloff);

                    float3 mainlightMap = MainLight(IN.worldPos, IN.normal, calculatedShadows);
                    
                    float3 albedo = bakedTextureRGB * (mainlightMap + additionalLightsMap + ambientLight) ;
                    return float4(albedo,1);
                }
            
            ENDHLSL
        }

        Pass
        {
            Tags {"Lightmode" = "ShadowCaster"}
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

                half4 Shadowmask (float2 lightmapUV)
                {
                    OUTPUT_LIGHTMAP_UV(lightmapUV, unity_LightmapST, lightmapUV);
                    return SAMPLE_SHADOWMASK(lightmapUV);
                }


                //nessacary boilerplate 
                VertexOutput vert (VertexInput v) 
                {
                    VertexOutput o; 
                    o.position = TransformObjectToHClip(v.position);
                    return o;
                }
                float4 frag (VertexOutput IN) : SV_TARGET 
                {
                    return float4(0,0,0,0);
                }

            ENDHLSL
        }
    }
        CustomEditor "BakedTextureGUI"
}
