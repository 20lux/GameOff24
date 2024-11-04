#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

float3 AdditionalLights(float3 worldPos, float3 worldNormal, half4 Shadowmask, float lightIntensityCurve, float3 lightHueFalloff, float lightSaturationFalloff)
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
        lightMapColorLerp = lerp(hueShiftLightColorRGB, light.color, fullShadowMap);
        lightTexColoredShadows += lightMapColorLerp * fullShadowMap;
    }

    return lightTexColoredShadows;
}

float3 MainLight(float3 worldPos, float3 worldNormal, half4 Shadowmask)
{
    float3 mainLightTex = (0, 0, 0);
    float3 fullShadowMap = 0;
    
    Light mainLight = GetMainLight();
    float4 shadowCoord = TransformWorldToShadowCoord(worldPos);
    half mainShadow = MainLightRealtimeShadow(shadowCoord);
    
    float direction = saturate(dot(mainLight.direction, worldNormal));
    float finalAtten = clamp(mainLight.distanceAttenuation * mainLight.shadowAttenuation, 0, 0.95);
    
    fullShadowMap = direction * finalAtten;
    mainLightTex = mainLight.color * direction * finalAtten * mainShadow;
    
    return mainLightTex;
}