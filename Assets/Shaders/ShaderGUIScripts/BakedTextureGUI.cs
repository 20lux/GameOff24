using UnityEngine;
using UnityEditor;

public class BakedTextureGUI : ShaderGUI
{
    private bool showTextureSettings = true;

    private bool showTotalLightingSettings = true;
    private bool showAdditionalLightingSettings = true;

    private bool showHalftoneSettings = true;

    private bool showHighlightingSettings = true;

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
            MaterialProperty useHalftone = FindProperty("_UseHalftone", properties);

            MaterialProperty halftonePattern = FindProperty("_HalftonePattern", properties);
            MaterialProperty halftonePatternScale = FindProperty("_HalftonePatternScale", properties);
            MaterialProperty halftoneFalloffThreshold = FindProperty("_HalftoneFalloffThreshold", properties);
            MaterialProperty halftoneLightThreshold = FindProperty("_HalftoneLightThreshold", properties);
            MaterialProperty halftoneSoftness = FindProperty("_HalftoneSoftness", properties);

            bool useHalftoneBool = useHalftone.floatValue > 0.5f;
            useHalftoneBool = EditorGUILayout.Toggle("Use Halftone", useHalftoneBool);
            useHalftone.floatValue = useHalftoneBool ? 1.0f : 0.0f;

            materialEditor.ShaderProperty(halftonePattern, new GUIContent(halftonePattern.displayName, "Use black and white texture pattern"));
            materialEditor.ShaderProperty(halftonePatternScale, halftonePatternScale.displayName);
            materialEditor.ShaderProperty(halftoneFalloffThreshold, halftoneFalloffThreshold.displayName);
            materialEditor.ShaderProperty(halftoneLightThreshold, halftoneLightThreshold.displayName);
            materialEditor.ShaderProperty(halftoneSoftness, halftoneSoftness.displayName);
        }

        EditorGUILayout.Space();

        showHighlightingSettings = EditorGUILayout.Foldout(showHighlightingSettings, "Highlighting Settings");
        if (showHighlightingSettings)
        {
            MaterialProperty isHighlighting = FindProperty("_IsHighlighting", properties);


            MaterialProperty highlightSinOffset = FindProperty("_HighlightSinOffset", properties);

            MaterialProperty highlightColFreq = FindProperty("_HighlightColFreq", properties);
            MaterialProperty highlightColMag = FindProperty("_HighlightColMag", properties);
            MaterialProperty highlightPosFreq = FindProperty("_HighlightPosFreq", properties);
            MaterialProperty highlightPosMag = FindProperty("_HighlightPosMag", properties);

            bool isHighlightingBool = isHighlighting.floatValue > 0.5f;
            isHighlightingBool = EditorGUILayout.Toggle("Is Highlighting", isHighlightingBool);
            isHighlighting.floatValue = isHighlightingBool ? 1.0f : 0.0f;

            materialEditor.ShaderProperty(highlightSinOffset, highlightSinOffset.displayName);

            materialEditor.ShaderProperty(highlightColFreq, highlightColFreq.displayName);
            materialEditor.ShaderProperty(highlightColMag, highlightColMag.displayName);
            materialEditor.ShaderProperty(highlightPosFreq, highlightPosFreq.displayName);
            materialEditor.ShaderProperty(highlightPosMag, highlightPosMag.displayName);
        }
    }
}