Shader "Unlit/s_bakedTextures"
{
    Properties
    { 
        //baked texture
        [NoScaleOffset] _BakedTexture ("Baked Texture", 2D) = "white" {}

        //normals
        [NoScaleOffset] _NormalTexture ("Normal Texture", 2D) = "blue" {}
        [HideInInspector] _NormalStrength("Normal Strength", Range(0,1)) = 1
        [HideInInspector] _NormalOffset("Normal Offset", Range(0,1)) = 0.1

        //emission
        [NoScaleOffset] _EmissionTexture("Emission Texture", 2D) = "black" {}

        //total light
        [HideInInspector] _AmbientLightStrength("Ambient Light Strength", Range(0,1)) = 1
        
        //additional light
        [HideInInspector] _AdditionalLightHueFalloff("Additional Light Hue Falloff", Range(0,360)) = 180
        [HideInInspector] _AdditionalLightSaturationFalloff("Additional Light Saturation Falloff", Float) = 1 
        [HideInInspector] _AdditionalLightIntensityCurve("Additional Light Intensity Curve", Range(0.01, 2.0)) = 1

        //halftone
        [NoScaleOffset] _HalftonePattern("Halftone Pattern", 2D) = "white" {}
        [HideInInspector] _HalftoneFalloffThreshold("Halftone Falloff Threshold", Float) = 0
        [HideInInspector] _HalftoneLightThreshold("Halftone Light Threshold", Float) = 0
        [HideInInspector] _HalftoneSoftness("Halftone Softness", Range(0.01,5)) = 1

        //highlighting
        [HideInInspector] _IsHighlighting("Is Highlighting", Float) = 0

        [HideInInspector] _HighlightSinOffset("Sin Offset", Float) = 0.5

        [HideInInspector] _HighlightColFreq("Color Frequency", Float) = 1
        [HideInInspector] _HighlightColMag("Color Magnitude", Float) = 1

        [HideInInspector] _HighlightPosFreq("Position Frequency", Float) = 1
        [HideInInspector] _HighlightPosMag("Position Magnitude", Float) = 1

    }
    SubShader
    {
        LOD 100
            HLSLINCLUDE
                #include "Assets/Shaders/HLSLSubFiles/HelperShaderFunctions.hlsl"
                #include "Assets/Shaders/HLSLSubFiles/CustomLightingFunctions.hlsl"
                #include "Assets/Shaders/HLSLSubFiles/Highlighters.hlsl"
            
                CBUFFER_START(UnityPerMaterial)
                    
                //normals
                    float _NormalStrength;
                    float _NormalOffset;

                    //total light
                    float _AmbientLightStrength;

                    //additional light
                    float _AdditionalLightHueFalloff;
                    float _AdditionalLightSaturationFalloff;
                    float _AdditionalLightIntensityCurve;

                    //halftone
                    float _HalftoneFalloffThreshold;
                    float _HalftoneLightThreshold;
                    float _HalftoneSoftness;

                    //highlighting
                    bool _IsHighlighting;

                    float _HighlightSinOffset;

                    float _HighlightColFreq;
                    float _HighlightColMag;

                    float _HighlightPosFreq;
                    float _HighlightPosMag;

                CBUFFER_END
            
                //textures
                TEXTURE2D(_BakedTexture);
                TEXTURE2D(_NormalTexture);
                TEXTURE2D(_EmissionTexture);

                TEXTURE2D(_HalftonePattern);

                //samplers
                SAMPLER(sampler_BakedTexture);
                SAMPLER(sampler_NormalTexture);
                SAMPLER(sampler_EmissionTexture);

                SAMPLER(sampler_HalftonePattern);

                struct appdata_full
                {
                    float4 position : POSITION;

                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;

                    float2 uvBakedTexture : TEXCOORD0;
                    float2 uvNormalTexture : TEXCOORD1;

                    float3 bitangent : TEXCOORD4;
                    float3 worldPos : TEXCOORD5;

                    float2 uvEmissionTexture : TEXCOORD6;

                    float2 uvHalftonePattern : TEXCOORD7;
                };

                struct appdata_current
                {
                };

                struct v2f 
                {
                    float4 position : SV_POSITION;

                    float2 uvBakedTexture : TEXCOORD0;
                    float2 uvNormalTexture : TEXCOORD1;

                    float3 normal : TEXCOORD2;
                    float4 tangent : TEXCOORD3;
                    float3 bitangent : TEXCOORD4;

                    float3 worldPos : TEXCOOR5;

                    float2 uvEmissionTexture : TEXCOORD6;

                    float2 uvHalftonePattern : TEXCOORD7;
                };



            ENDHLSL

        Pass
        {
            Tags {"Lightmode" = "UniversalForward"}
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            //enable additional light shadow maps
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

            //enable main light shadow maps
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE

            //enable ambient light generation
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA

            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

                v2f vert (appdata_full v)
                {
                    v2f o;

                    v.position = PulsingBloom(_IsHighlighting, v.position, _HighlightPosFreq, _HighlightPosMag, _HighlightSinOffset);
                    o.worldPos = mul(unity_ObjectToWorld, v.position).xyz;
                    o.position = TransformObjectToHClip(v.position);


                    o.uvBakedTexture = v.uvBakedTexture;
                    o.uvNormalTexture = v.uvNormalTexture;
                    o.uvEmissionTexture = v.uvEmissionTexture;

                    o.normal = TransformObjectToWorldNormal(v.normal);
                    float3 tangent = normalize(v.tangent.xyz);
                    o.tangent = (tangent,1);
                    o.bitangent = normalize(cross(o.normal,o.tangent) * v.tangent.w);

                    o.uvHalftonePattern = v.uvHalftonePattern;
                    return o;
                }

                float4 frag (v2f i) : SV_TARGET
                {
                    float3 bakedTextureRGB = SAMPLE_TEXTURE2D(_BakedTexture, sampler_BakedTexture, i.uvBakedTexture).rgb;

                    float3 emissionTextureRGB = SAMPLE_TEXTURE2D(_EmissionTexture, sampler_EmissionTexture, i.uvEmissionTexture).rgb;

                    float3 tangentNormals = TextureToTangentNormal(
                        _NormalTexture,
                        sampler_NormalTexture,
                        _NormalStrength,
                        _NormalOffset, i.uvNormalTexture,
                        1);
                    i.normal = TangentToWorldNormal(tangentNormals, i.tangent, i.bitangent, i.normal);
                    
                    
                    InputData inputData = (InputData)0;

                    half4 calculatedShadows = CalculateShadowMask(inputData);

                    half3 ambientLight = SampleSH(i.normal) * _AmbientLightStrength;

                    //halftone
                    float halftoneTexture = SAMPLE_TEXTURE2D(_HalftonePattern, sampler_HalftonePattern, i.uvHalftonePattern).r;

                    float3 additionalLightsMap = AdditionalLights(
                        i.worldPos,
                        i.normal,
                        calculatedShadows,
                        _AdditionalLightIntensityCurve,
                        _AdditionalLightHueFalloff,
                        _AdditionalLightSaturationFalloff,
                        halftoneTexture,
                        _HalftoneFalloffThreshold,
                        _HalftoneLightThreshold,
                        _HalftoneSoftness);

                    float3 mainlightMap = MainLight(
                        i.worldPos,
                        i.normal,
                        halftoneTexture,
                        _HalftoneFalloffThreshold,
                        _HalftoneLightThreshold,
                        _HalftoneSoftness);
                    
                    float3 totalLightMap = mainlightMap + additionalLightsMap + ambientLight;
                    float4 albedo = float4((bakedTextureRGB * totalLightMap) + emissionTextureRGB, albedo.a);

                    albedo = PulsingBloom(_IsHighlighting, albedo, _HighlightColFreq, _HighlightColMag, _HighlightSinOffset);
                    return albedo;
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


                v2f vert (appdata_full v)
                {
                    v2f o;
                    v.position = PulsingBloom(_IsHighlighting, v.position, _HighlightPosFreq, _HighlightPosMag, _HighlightSinOffset);
                    o.position = TransformObjectToHClip(v.position);
                    return o;
                }
                float4 frag (v2f i) : SV_TARGET 
                {
                    return float4(0,0,0,0);
                }

            ENDHLSL
        }
    }
        CustomEditor "BakedTextureGUI"
}
