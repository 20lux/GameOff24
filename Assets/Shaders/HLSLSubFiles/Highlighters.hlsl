#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

float3 PulsingBloomVert (bool isOn, float3 In, float frequency, float magnitude, float offset)
{
    if (isOn > 0.5)
    {
        float t = _Time.y;
    
        t = sin(t * frequency) * 0.5 + offset;
    
        In *= t;
    }
    return In;
}

float3 PulsingBloomFrag (bool isOn, float3 In, float frequency, float magnitude, float offset)
{
    if (isOn > 0.5)
    {
        float t = _Time.y;
    
        t = sin(t * frequency) * 0.5 + offset;
    
        In = RGBToHSV(In);
        In.z *= t;
        In = HSVToRGB(In);
    }
    return In;
}