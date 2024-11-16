#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

float HalfTone (bool isOn, float totalAtten, float halftoneTexture, float falloffThreshold, float lightThreshold,  float softness)
{
    if (isOn > 0.5)
    {
    totalAtten *= -1;
    float halftoneEdge1 = Remap(totalAtten, float2(-1, 1), float2(lightThreshold - falloffThreshold, lightThreshold));
    float halftoneEdge2 = halftoneEdge1 + softness;
    totalAtten = smoothstep(halftoneEdge1, halftoneEdge2, halftoneTexture);   
    }
    
    return totalAtten;

}

float3 AdditionalLights(
float3 worldPos,
float3 worldNormal,
half4 Shadowmask,
float lightIntensityCurve,
float3 lightHueFalloff,
float lightSaturationFalloff,
bool useHalftone,
float halftoneTexture,
float halftoneFalloffThreshold,
float halftoneLightThreshold,
float halftoneSoftness
)
{
    int pixelLightCount = GetAdditionalLightsCount();
                    
    float3 lightMapColorLerp = (0, 0, 0);
    float fullShadowMap = 0;
    float3 lightTexColoredShadows = (0, 0, 0);

    for (uint i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, worldPos, Shadowmask);

        float lightDirection = saturate(dot(light.direction, worldNormal));
        float totalAtten = clamp(light.distanceAttenuation * light.shadowAttenuation, 0, 0.95);
        float adjustAtten = pow(totalAtten, lightIntensityCurve);

        float3 hueShiftLightColor = saturate(HueDegrees(light.color, lightHueFalloff) * 0.1);
        
        float3 hueShiftLightColorHSV = RGBToHSV(hueShiftLightColor);
        float3 saturationShift = float3(hueShiftLightColorHSV.r, hueShiftLightColorHSV.g * lightSaturationFalloff, hueShiftLightColorHSV.b);
        float3 hueShiftLightColorRGB = HSVToRGB(saturationShift);
        
        
        fullShadowMap = lightDirection * adjustAtten;
        float halftoneShadowMap = HalfTone(useHalftone, fullShadowMap, halftoneTexture, halftoneFalloffThreshold, halftoneLightThreshold, halftoneSoftness);
        
        
        lightMapColorLerp = lerp(hueShiftLightColorRGB, light.color, fullShadowMap);
        lightTexColoredShadows += lightMapColorLerp * halftoneShadowMap;
    }

    return lightTexColoredShadows;
}

float3 MainLight(
float3 worldPos,
float3 worldNormal,
bool useHalftone,
float halftoneTexture,
float halftoneFalloffThreshold,
float halftoneLightThreshold,
float halftoneSoftness
)
{
    float3 mainLightTex = (0, 0, 0);
    
    Light mainLight = GetMainLight();
    float4 shadowCoord = TransformWorldToShadowCoord(worldPos);
    half mainShadow = MainLightRealtimeShadow(shadowCoord);
    
    float direction = saturate(dot(mainLight.direction, worldNormal));
    float atten = mainLight.distanceAttenuation * mainLight.shadowAttenuation;
    float totalAtten = atten * direction * mainShadow;
    
    float halftoneShadowMap = HalfTone(useHalftone, totalAtten, halftoneTexture, halftoneFalloffThreshold, halftoneLightThreshold, halftoneSoftness);
    mainLightTex = mainLight.color * halftoneShadowMap;
    
    return mainLightTex;
}