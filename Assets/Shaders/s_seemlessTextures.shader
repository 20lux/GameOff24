Shader "Unlit/s_seemlessTextures"
{
    Properties
    { 
        //seemless texture
        [NoScaleOffset] _SeemlessPattern ("Seemless Mask", 2D) = "white" {}
        [NoScaleOffset] _TexturePattern("Texture Pattern", 2D) = "white" {}
        [HideInInspector] _SeemlessPatternScale ("Seemless Mask Scale", Float) = 1
        [HideInInspector] _SeemlessTextureScale ("Texture Scale", Float) = 1
        [HideInInspector] _SeemlessPatternNormalOffset("Seemless Mask Normal Offset", Range(0,1)) = 0
        [HideInInspector] _SeemlessPatternNormalStrength("Seemless Mask Normal Strength", Range(0,1)) = 0
        [HideInInspector] _TextureColor("Texture Color", Color) = (0.5,0.5,0.5,1)
        [HideInInspector] _ColorHueOffset("Color Hue Offset", Range(0,360)) = 13
        [HideInInspector] _ColorValueOffset("Color Value Offset", Range(0,1)) = 0.5

        //total light
        [HideInInspector] _AmbientLightStrength("Ambient Light Strength", Range(0,1)) = 1

        //additional light
        [HideInInspector] _AdditionalLightHueFalloff("Additional Light Hue Falloff", Range(0,360)) = 180
        [HideInInspector] _AdditionalLightSaturationFalloff("Additional Light Saturation Falloff", Float) = 1 
        [HideInInspector] _AdditionalLightIntensityCurve("Additional Light Intensity Curve", Range(0.01, 2.0)) = 1


        //halftone
        [NoScaleOffset] _HalftonePattern("Halftone Pattern", 2D) = "white" {}
        [HideInInspector] _HalftoneFalloffThreshold("Halftone Falloff Threshold", Float) = 8
        [HideInInspector] _HalftoneLightThreshold("Halftone Light Threshold", Float) = 5
        [HideInInspector] _HalftoneSoftness("Halftone Softness", Range(0.01,5)) = 1
    }
    SubShader
    {
        LOD 100
            HLSLINCLUDE
                #include "Assets/Shaders/HLSLSubFiles/HelperShaderFunctions.hlsl"
                #include "Assets/Shaders/HLSLSubFiles/CustomLightingFunctions.hlsl"
            
                CBUFFER_START(UnityPerMaterial)

                    //seemless texture
                    float _SeemlessPatternScale;
                    float _SeemlessTextureScale;
                    float _SeemlessPatternNormalOffset;
                    float _SeemlessPatternNormalStrength;
                    float4 _TextureColor;
                    float _ColorHueOffset;
                    float _ColorValueOffset;

                    //ambient light
                    float _AmbientLightStrength;

                    // additional lights
                    float _AdditionalLightHueFalloff;
                    float _AdditionalLightSaturationFalloff;
                    float _AdditionalLightIntensityCurve;

                    //halftone
                    float _HalftoneFalloffThreshold;
                    float _HalftoneLightThreshold;
                    float _HalftoneSoftness;


                CBUFFER_END
            
                TEXTURE2D(_SeemlessPattern);
                TEXTURE2D(_TexturePattern);

                TEXTURE2D(_HalftonePattern);

                SAMPLER(sampler_SeemlessPattern);
                SAMPLER(sampler_TexturePattern);

                SAMPLER(sampler_HalftonePattern);


                struct VertexInput
                {
                    float4 position : POSITION;

                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;

                    float2 uvSeemlessPattern : TEXCOORD0;
                    float2 uvTexturePattern : TEXCOORD1;

                    float3 bitangent : TEXCOORD4;
                    float3 worldPos : TEXCOORD5;

                    float2 uvHalftonePattern : TEXCOORD6;
                };

                struct VertexOutput 
                {
                    float4 position : SV_POSITION;

                    float2 uvSeemlessPattern : TEXCOORD0;
                    float2 uvTexturePattern : TEXCOORD1;

                    float3 normal : TEXCOORD2;
                    float4 tangent : TEXCOORD3;
                    float3 bitangent : TEXCOORD4;

                    float3 worldPos : TEXCOOR5;

                    float2 uvHalftonePattern : TEXCOORD6;
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

                    o.uvSeemlessPattern = v.uvSeemlessPattern;
                    o.uvTexturePattern = v.uvTexturePattern;

                    o.uvHalftonePattern = v.uvHalftonePattern;

                    o.normal = TransformObjectToWorldNormal(v.normal);
                    float3 tangent = normalize(v.tangent.xyz);
                    o.tangent = (tangent,1);
                    o.bitangent = normalize(cross(o.normal,o.tangent) * v.tangent.w);
                    return o;
                }

                float3 TextureSampling(float2 uvSeemlessPattern, float2 uvTexturePattern, half3 ambientLight)
                {
                    float2 scaledSeemlessPattern = frac(uvSeemlessPattern * _SeemlessPatternScale);
                    float2 scaledTexturePattern = frac(uvTexturePattern * _SeemlessTextureScale);


                    float3 seemlessPattern = SAMPLE_TEXTURE2D(_SeemlessPattern, sampler_SeemlessPattern, scaledSeemlessPattern).rgb;
                    float3 texturePattern = SAMPLE_TEXTURE2D(_TexturePattern, sampler_TexturePattern, scaledTexturePattern).rgb;

                    float3 colorA = HueDegrees(_TextureColor, _ColorHueOffset);
                    float3 colorB = HueDegrees(_TextureColor, 1 - _ColorHueOffset);
                    float3 lerpA = lerp(colorA, colorB, texturePattern);

                    float3 darkerTextureColor = _TextureColor * lerp(ambientLight, 1, _ColorValueOffset);
                    float3 colorC = HueDegrees(darkerTextureColor, _ColorHueOffset);
                    float3 colorD = HueDegrees(darkerTextureColor, 1 - _ColorHueOffset);
                    float3 lerpB = lerp(colorC, colorD, texturePattern);

                    float3 lerpC = lerp(lerpA, lerpB, seemlessPattern);
                    return lerpC;
                }

                float4 frag (VertexOutput IN) : SV_TARGET
                {
                    float3 tangentNormals = TextureToTangentNormal(
                        _SeemlessPattern,
                        sampler_SeemlessPattern,
                        _SeemlessPatternNormalStrength,
                        _SeemlessPatternNormalOffset, IN.uvSeemlessPattern,
                        _SeemlessPatternScale);
                    IN.normal = TangentToWorldNormal(tangentNormals, IN.tangent, IN.bitangent, IN.normal);

                    //sample textures
                    
                    //inputdata related functions
                    InputData inputData = (InputData)0;
                    half4 calculatedShadows = CalculateShadowMask(inputData);
                    half3 ambientLight = SampleSH(IN.normal) * _AmbientLightStrength;

                    //texture Sampling
                    float3 textureSampling = TextureSampling(IN.uvSeemlessPattern, IN.uvTexturePattern, ambientLight);

                    //halftone
                    float halftoneTexture = SAMPLE_TEXTURE2D(_HalftonePattern, sampler_HalftonePattern, IN.uvHalftonePattern).r;

                    float3 additionalLightsMap = AdditionalLights(
                        IN.worldPos,
                        IN.normal,
                        calculatedShadows,
                        _AdditionalLightIntensityCurve,
                        _AdditionalLightHueFalloff,
                        _AdditionalLightSaturationFalloff,
                        halftoneTexture,
                        _HalftoneFalloffThreshold,
                        _HalftoneLightThreshold,
                        _HalftoneSoftness);

                    float3 mainlightMap = MainLight(
                        IN.worldPos,
                        IN.normal,
                        calculatedShadows,
                        halftoneTexture,
                        _HalftoneFalloffThreshold,
                        _HalftoneLightThreshold,
                        _HalftoneSoftness);
                    
                    float3 totalLightMap = mainlightMap + additionalLightsMap + ambientLight;
                    float3 albedo = textureSampling * totalLightMap;
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
        CustomEditor "SeemlessTextureGUI"
}
