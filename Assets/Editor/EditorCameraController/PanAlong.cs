using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PanAlong
{
    private static Vector3 previousPosition;
    private static bool isDragging = false;

    static PanAlong()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (Selection.activeTransform == null)
            return;

        Transform selectedTransform = Selection.activeTransform;
        Event currentEvent = Event.current;

        // Check if the scene view is in move tool mode and Left-Shift is held
        if (Tools.current == Tool.Move && currentEvent.shift)
        {
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
            {
                // Start tracking the object's position
                previousPosition = selectedTransform.position;
                isDragging = true;
            } else if (currentEvent.type == EventType.MouseDrag)
            {
                isDragging = true;

                // Calculate the delta movement
                Vector3 delta = selectedTransform.position - previousPosition;

                // Move the scene view pivot by the same delta
                sceneView.pivot += delta;
                previousPosition = selectedTransform.position;

                // Repaint the scene view to reflect changes
                sceneView.Repaint();
            } else if (currentEvent.type == EventType.MouseUp && isDragging)
            {
                // Stop tracking when the mouse button is released
                isDragging = false;
            }
        } else
        {
            // Stop tracking if Left-Shift is released or tool changes
            isDragging = false;
        }

        // Ensure dragging stops if shift is released mid-drag
        if (isDragging && !currentEvent.shift)
        {
            isDragging = false;
        }
    }
}
