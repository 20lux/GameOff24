using UnityEngine;
using UnityEditor;

public class SeemlessTextureGUI : ShaderGUI
{
    private bool showTextureSettings = true;
    private bool showLightingSettings = true;

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

        showLightingSettings = EditorGUILayout.Foldout(showLightingSettings, "Lighting Settings");
        if (showLightingSettings)
        {
            MaterialProperty additionalLightHueFalloff = FindProperty("_AdditionalLightHueFalloff", properties);
            MaterialProperty additionalLightSaturationFalloff = FindProperty("_AdditionalLightSaturationFalloff", properties);
            MaterialProperty additionalLightIntensityCurve = FindProperty("_AdditionalLightIntensityCurve", properties);
            MaterialProperty ambientLightStrength = FindProperty("_AmbientLightStrength", properties);

            materialEditor.ShaderProperty(additionalLightHueFalloff, additionalLightHueFalloff.displayName);
            materialEditor.ShaderProperty(additionalLightSaturationFalloff, additionalLightSaturationFalloff.displayName);
            materialEditor.ShaderProperty(additionalLightIntensityCurve, additionalLightIntensityCurve.displayName);
            materialEditor.ShaderProperty(ambientLightStrength, ambientLightStrength.displayName);
        }
    }
}