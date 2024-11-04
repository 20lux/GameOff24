#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif
using BoomMicCity.RuntimeUtilities;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Interactable))]
public class HighlightInteraction : MonoBehaviour
{
    private Renderer _renderer;
    private bool canHighlight;
    private int highlightIndex;
    private Material highlightMaterial;

    private bool editorInitialized;
#if UNITY_EDITOR
    private void Reset()
    {
        if (!editorInitialized)
        {
            // If gameObject has Default layer, set the layer to 'Interactable'
            if (gameObject.layer == 0) gameObject.layer = LayerMask.NameToLayer("Interactable");

            // Initialize the Interactable component
            Interactable interactable = GetComponent<Interactable>();

            Collider collider = GetComponent<Collider>();
            if (collider == null)
                collider = gameObject.AddComponent<BoxCollider>();

            // Add the StopHighlighting method as a persistent listener if not already added
            UnityAction stopHighlightingAction = StopHighlighting;
            if (!BMCEventTools.IsMethodAlreadySubscribed(interactable.onInteraction, stopHighlightingAction))
            {
                UnityEventTools.AddPersistentListener(interactable.onInteraction, stopHighlightingAction);
            }

            // Mark the editor as initialized to prevent this from running multiple times
            editorInitialized = true;
        }
    }
#endif

    private void Start()
    {
        canHighlight = CheckHighlightable();
    }

    public void StartHighlighting()
    {
        if (!CheckHighlightable())
            return;
        if (highlightMaterial == null)
            AssignRuntimeMaterial();

        highlightMaterial.SetInt("_Highlighting", 1);
        _renderer.sharedMaterials[highlightIndex] = highlightMaterial;
    }

    public void StopHighlighting()
    {
        if (!CheckHighlightable())
            return;
        if (highlightMaterial == null)
            AssignRuntimeMaterial();

        highlightMaterial.SetInt("_Highlighting", 0);
        _renderer.sharedMaterials[highlightIndex] = highlightMaterial;
    }

    private void AssignRuntimeMaterial()
    {
        Shader cellShadeShader = Shader.Find("Shader Graphs/s_cellShade");
        if (cellShadeShader == null)
        {
            Debug.LogError("s_cellShade shader not found.");
            return;
        }

        // Create a new instance of the material with the shader
        highlightMaterial = new Material(cellShadeShader);
        highlightMaterial.name = _renderer.sharedMaterials[highlightIndex].name + "_(Copy)";  // Set the name of the new material
        highlightMaterial.CopyPropertiesFromMaterial(_renderer.sharedMaterials[highlightIndex]);

        // Clone the existing materials array
        Material[] materials = _renderer.sharedMaterials;
        materials[highlightIndex] = highlightMaterial;  // Replace the material at the specified index
        _renderer.sharedMaterials = materials;  // Re-assign the modified array back to the renderer
    }

    public bool CheckHighlightable()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
            return false;

        Shader targetShaderOneSided = Shader.Find("Shader Graphs/s_cellShade");
        Shader targetShaderDoubleFace = Shader.Find("Shader Graphs/s_cellShadeDoubleFace");
        if (targetShaderOneSided == null && targetShaderDoubleFace == null)
        {
            Debug.LogError("Target shader not found.");
            return false;
        }

        if (targetShaderOneSided != null)
            for (int i = 0; i < _renderer.sharedMaterials.Length; i++)
            {
                if (_renderer.sharedMaterials[i].shader == targetShaderOneSided)
                {
                    highlightIndex = i;
                    return true;
                }
            }

        if (targetShaderDoubleFace != null)
            for (int i = 0; i < _renderer.sharedMaterials.Length; i++)
            {
                if (_renderer.sharedMaterials[i].shader == targetShaderDoubleFace)
                {
                    highlightIndex = i;
                    return true;
                }
            }

        return false;
    }
}

#if UNITY_EDITOR
    [CustomEditor(typeof(HighlightInteraction))]
    public class HighlightInteractionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            HighlightInteraction script = (HighlightInteraction)target;

            if (GUILayout.Button("Start Highlighting"))
            {
                script.CheckHighlightable();
                script.StartHighlighting();
            }
            if (GUILayout.Button("Stop Highlighting"))
            {
                script.CheckHighlightable();
                script.StopHighlighting();
            }
        }
    }
#endif