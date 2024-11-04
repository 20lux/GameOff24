using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public string displayMessage;
    public UnityEvent onInteraction;

    private void Reset()
    {
        if (onInteraction == null)
        {
            onInteraction = new UnityEvent();
        }
    }

    private void Start()
    {
        //for checkbox use
    }

    public void Interact()
    {
        onInteraction?.Invoke();
    }
    
    public void ChangeDisplayMessage(string inputMessage)
    {
        displayMessage = inputMessage;
    }

    public void ClearDisplayMessage()
    {
        displayMessage = string.Empty;
    }
}
