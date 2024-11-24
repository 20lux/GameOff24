using UnityEngine;
using UnityEditor;

public class SeemlessTextureGUI : ShaderGUI
{
    private bool showTextureSettings = true;

    private bool showTotalLightingSettings  = true;
    private bool showAdditionalLightingSettings = true;

    private bool showHalftoneSettings = true;
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        showTextureSettings = EditorGUILayout.Foldout(showTextureSettings, "Texture Settings");
        if (showTextureSettings)
        {
            MaterialProperty seemlessPattern = FindProperty("_SeemlessPattern", properties);
            MaterialProperty texturePattern = FindProperty("_TexturePattern", properties);
            MaterialProperty seemlessPatternScale = FindProperty("_SeemlessPatternScale", properties);
            MaterialProperty seemlessTextureScale = FindProperty("_SeemlessTextureScale", properties);
            MaterialProperty seemlessPatternNormalOffset = FindProperty("_SeemlessPatternNormalOffset", properties);
            MaterialProperty seemlessPatternNormalStrength = FindProperty("_SeemlessPatternNormalStrength", properties);
            MaterialProperty textureColor = FindProperty("_TextureColor", properties);
            MaterialProperty colorHueOffset = FindProperty("_ColorHueOffset", properties);
            MaterialProperty colorValueOffset = FindProperty("_ColorValueOffset", properties);

            materialEditor.ShaderProperty(seemlessPattern, new GUIContent(seemlessPattern.displayName, "Use black & white seemless mask."));
            materialEditor.ShaderProperty(texturePattern, new GUIContent(texturePattern.displayName, "Use greyscale mask"));
            materialEditor.ShaderProperty(seemlessPatternScale, seemlessPatternScale.displayName);
            materialEditor.ShaderProperty(seemlessTextureScale, seemlessTextureScale.displayName);
            materialEditor.ShaderProperty(seemlessPatternNormalOffset, seemlessPatternNormalOffset.displayName);
            materialEditor.ShaderProperty(seemlessPatternNormalStrength, seemlessPatternNormalStrength.displayName);
            materialEditor.ShaderProperty(textureColor, new GUIContent(textureColor.displayName, "Pick the average color"));
            materialEditor.ShaderProperty(colorHueOffset, new GUIContent(colorHueOffset.displayName, "Uses the Texture Color as the centre to pick two new colors"));
            materialEditor.ShaderProperty(colorValueOffset, new GUIContent(colorValueOffset.displayName, "Increases/Decreases contrast of the Seemless Mask"));
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
    }
}