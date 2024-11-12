#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

float4 PulsingBloom (float4 textureInput, float frequency, float magnitude, float offset)
{
    float t = _Time.y;
    
    t = magnitude * sin(t * frequency) * 0.5 + offset;
    
    textureInput = float4 (RGBToHSV(textureInput), textureInput.w);
    textureInput = float4 (textureInput.x, textureInput.y, textureInput.z * t, textureInput.w);
    textureInput = float4(HSVToRGB(textureInput), textureInput.w);
    return textureInput;
}

float CalculatePulseIntensity(float frequency, float magnitude, float offset)
{
    float t = _Time.y;
    return magnitude * sin(t * frequency) * 0.5 + offset;
}