using UnityEngine;
using UnityEditor;

public class BakedTextureGUI : ShaderGUI
{
    private bool showTextureSettings = true;

    private bool showTotalLightingSettings = true;
    private bool showAdditionalLightingSettings = true;

    private bool showHalftoneSettings = true;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        showTextureSettings = EditorGUILayout.Foldout(showTextureSettings, "Texture Settings");
        if (showTextureSettings)
        {
            MaterialProperty bakedTexture = FindProperty("_BakedTexture", properties);
            MaterialProperty normalTexture = FindProperty("_NormalTexture", properties);
            MaterialProperty emissionTexture = FindProperty("_EmissionTexture", properties);

            materialEditor.ShaderProperty(bakedTexture, bakedTexture.displayName);
            materialEditor.ShaderProperty(normalTexture, normalTexture.displayName);
            materialEditor.ShaderProperty(emissionTexture, emissionTexture.displayName);
        }

        EditorGUILayout.Space();

        showTotalLightingSettings = EditorGUILayout.Foldout(showTotalLightingSettings, "Total Lighting Settings");
        if (showTotalLightingSettings)
        {
            MaterialProperty ambientLightStrength = FindProperty("_AmbientLightStrength", properties);

            materialEditor.ShaderProperty(ambientLightStrength, ambientLightStrength.displayName);
        }

        EditorGUILayout.Space();

        showAdditionalLightingSettings = EditorGUILayout.Foldout(showAdditionalLightingSettings, "Additional Lighting Settings");
        if (showAdditionalLightingSettings)
        {
            MaterialProperty additionalLightHueFalloff = FindProperty("_AdditionalLightHueFalloff", properties);
            MaterialProperty additionalLightSaturationFalloff = FindProperty("_AdditionalLightSaturationFalloff", properties);
            MaterialProperty additionalLightIntensityCurve = FindProperty("_AdditionalLightIntensityCurve", properties);

            materialEditor.ShaderProperty(additionalLightHueFalloff, additionalLightHueFalloff.displayName);
            materialEditor.ShaderProperty(additionalLightSaturationFalloff, additionalLightSaturationFalloff.displayName);
            materialEditor.ShaderProperty(additionalLightIntensityCurve, additionalLightIntensityCurve.displayName);
        }

        EditorGUILayout.Space();

        showHalftoneSettings = EditorGUILayout.Foldout(showHalftoneSettings, "Halftone Settings");
        if (showHalftoneSettings)
        {
            MaterialProperty halftonePattern = FindProperty("_HalftonePattern", properties);
            MaterialProperty halftoneLightOffset = FindProperty("_HalftoneLightOffset", properties);
            MaterialProperty halftoneSoftness = FindProperty("_HalftoneSoftness", properties);

            materialEditor.ShaderProperty(halftonePattern, halftonePattern.displayName);
            materialEditor.ShaderProperty(halftoneLightOffset, halftoneLightOffset.displayName);
            materialEditor.ShaderProperty(halftoneSoftness, halftoneSoftness.displayName);
        }
    }
}