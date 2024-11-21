using UnityEngine;
using UnityEditor;
using System.IO;

public class ShaderEditorHelper
{
    [MenuItem("Assets/Create/Shader/Custom Function (HLSL)")]
    static void CreateCustomFunction()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/HLSL Asset/Editor/CFTemplate.hlsl", "New Function.hlsl");
    }
}
