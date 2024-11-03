using UnityEngine;
using UnityEditor;

public class BakedTextureGUI : ShaderGUI
{
    private bool showTextureSettings = true;
    private bool showLightingSettings = true;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        showTextureSettings = EditorGUILayout.Foldout(showTextureSettings, "Texture Settings");
        if (showTextureSettings)
        {
            MaterialProperty bakedTexture = FindProperty("_BakedTexture", properties);
            MaterialProperty normalTexture = FindProperty("_NormalTexture", properties);

            materialEditor.ShaderProperty(bakedTexture, bakedTexture.displayName);
            materialEditor.ShaderProperty(normalTexture, normalTexture.displayName);
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