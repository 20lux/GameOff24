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

            materialEditor.ShaderProperty(seemlessPattern, seemlessPattern.displayName);
            materialEditor.ShaderProperty(texturePattern, texturePattern.displayName);
            materialEditor.ShaderProperty(seemlessPatternScale, seemlessPatternScale.displayName);
            materialEditor.ShaderProperty(seemlessTextureScale, seemlessTextureScale.displayName);
            materialEditor.ShaderProperty(seemlessPatternNormalOffset, seemlessPatternNormalOffset.displayName);
            materialEditor.ShaderProperty(seemlessPatternNormalStrength, seemlessPatternNormalStrength.displayName);
            materialEditor.ShaderProperty(textureColor, textureColor.displayName);
            materialEditor.ShaderProperty(colorHueOffset, colorHueOffset.displayName);
            materialEditor.ShaderProperty(colorValueOffset, colorValueOffset.displayName);
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
            MaterialProperty halftoneFalloffThreshold = FindProperty("_HalftoneFalloffThreshold", properties);
            MaterialProperty halftoneLightThreshold = FindProperty("_HalftoneLightThreshold", properties);
            MaterialProperty halftoneSoftness = FindProperty("_HalftoneSoftness", properties);

            materialEditor.ShaderProperty(halftonePattern, halftonePattern.displayName);
            materialEditor.ShaderProperty(halftoneFalloffThreshold, halftoneFalloffThreshold.displayName);
            materialEditor.ShaderProperty(halftoneLightThreshold, halftoneLightThreshold.displayName);
            materialEditor.ShaderProperty(halftoneSoftness, halftoneSoftness.displayName);
        }
    }
}