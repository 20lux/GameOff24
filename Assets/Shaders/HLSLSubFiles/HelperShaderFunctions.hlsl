#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
float3 HueDegrees(float3 input, float offset)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 P = lerp(float4(input.bg, K.wz), float4(input.gb, K.xy), step(input.b, input.g));
    float4 Q = lerp(float4(P.xyw, input.r), float4(input.r, P.yzx), step(P.x, input.r));
    float D = Q.x - min(Q.w, Q.y);
    float E = 1e-10;
    float3 hsv = float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);

    float hue = hsv.x + offset/360;
    hsv.x = (hue < 0)
            ? hue + 1
            : (hue > 1)
                ? hue - 1
                : hue;

    // HSV to RGB
    float4 K2 = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
    return float3(hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y));
}

float3 RGBToHSV(float3 In)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 P = lerp(float4(In.bg, K.wz), float4(In.gb, K.xy), step(In.b, In.g));
    float4 Q = lerp(float4(P.xyw, In.r), float4(In.r, P.yzx), step(P.x, In.r));
    float D = Q.x - min(Q.w, Q.y);
    float  E = 1e-10;
    return float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
}

float3 HSVToRGB(float3 In)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
    return In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
}

float3 TangentToWorldNormal(float3 tangentNormal, float3 tangent, float3 bitangent, float3 normal)
{
    float3x3 tangentSpace = transpose(float3x3(tangent, bitangent, normal));
    float3 worldNormal = mul(tangentSpace, tangentNormal);
    return worldNormal;
}

float3 TextureToTangentNormal(Texture2D seemlessTexture, SamplerState Sampler, float strength, float offset, float2 UV, float UVscale)
{
    UV = frac(UV * UVscale);

    offset = pow(offset, 3) * 0.1;
    float2 offsetU = float2(UV.x + offset, UV.y);
    float2 offsetV = float2(UV.x, UV.y + offset);
    float normalSample = seemlessTexture.Sample(Sampler, UV);
    float uSample = seemlessTexture.Sample(Sampler, offsetU);
    float vSample = seemlessTexture.Sample(Sampler, offsetV);
    float3 va = float3(1, 0, (uSample - normalSample) * strength);
    float3 vb = float3(0, 1, (vSample - normalSample) * strength);
    return normalize(cross(va, vb));
}

float3 Posterize(float3 inTexture, float steps)
{
    float3 outTexture = floor(inTexture / (1 / steps)) * (1 / steps);
    return outTexture;
}

float3 Remap(float3 In, float2 InMinMax, float2 OutMinMax)
{
    In = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
    
    return In;
}