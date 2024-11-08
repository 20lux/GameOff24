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

            materialEditor.ShaderProperty(ambientLightStrength, new GUIContent(ambientLightStrength.displayName, "Interpolates the shade color between black and the ambient color"));
        }

        EditorGUILayout.Space();

        showAdditionalLightingSettings = EditorGUILayout.Foldout(showAdditionalLightingSettings, "Additional Lighting Settings");
        if (showAdditionalLightingSettings)
        {
            MaterialProperty additionalLightHueFalloff = FindProperty("_AdditionalLightHueFalloff", properties);
            MaterialProperty additionalLightSaturationFalloff = FindProperty("_AdditionalLightSaturationFalloff", properties);
            MaterialProperty additionalLightIntensityCurve = FindProperty("_AdditionalLightIntensityCurve", properties);

            materialEditor.ShaderProperty(additionalLightHueFalloff, new GUIContent(additionalLightHueFalloff.displayName, "Controls the hue contrast as the light falls off"));
            materialEditor.ShaderProperty(additionalLightSaturationFalloff, new GUIContent(additionalLightSaturationFalloff.displayName, "Controls the saturation contrast as the light falls off. Mainly used when the light color is desaturated"));
            materialEditor.ShaderProperty(additionalLightIntensityCurve, new GUIContent(additionalLightIntensityCurve.displayName, "Controls hows the light intensity interpolates"));
        }

        EditorGUILayout.Space();

        showHalftoneSettings = EditorGUILayout.Foldout(showHalftoneSettings, "Halftone Settings");
        if (showHalftoneSettings)
        {
            MaterialProperty halftonePattern = FindProperty("_HalftonePattern", properties);
            MaterialProperty halftoneFalloffThreshold = FindProperty("_HalftoneFalloffThreshold", properties);
            MaterialProperty halftoneLightThreshold = FindProperty("_HalftoneLightThreshold", properties);
            MaterialProperty halftoneSoftness = FindProperty("_HalftoneSoftness", properties);

            materialEditor.ShaderProperty(halftonePattern, new GUIContent(halftonePattern.displayName, "Use black and white texture pattern"));
            materialEditor.ShaderProperty(halftoneFalloffThreshold, halftoneFalloffThreshold.displayName);
            materialEditor.ShaderProperty(halftoneLightThreshold, halftoneLightThreshold.displayName);
            materialEditor.ShaderProperty(halftoneSoftness, halftoneSoftness.displayName);
        }
    }
}