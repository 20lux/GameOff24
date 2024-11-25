using System.Collections.Generic;
using UnityEngine;

public class CheckIfCorrect : MonoBehaviour
{
    public List<string> list = new List<string>();

    public void InitiateEnd()
    {
        if (list.Count == 3)
        {
            Debug.Log("Starting the end bit!");
        }
    }
}
