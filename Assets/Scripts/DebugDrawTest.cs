using UnityEngine;
using jazari.DebugDraw;

public class DebugDrawTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DebugDraw.Box(Matrix4x4.identity, Color.yellow, 10f, false, 1);
    }
}
