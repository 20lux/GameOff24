using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float playerReach = 3f;
    Interactable currentInteractable;
    [HideInInspector] public bool hit = false;
    [HideInInspector] public GameObject currentObject;
    [SerializeField] LayerMask interactionLayerMask;
    [SerializeField] TextMeshProUGUI interactionTMP;

    private void Start()
    {
        interactionTMP.text = string.Empty;
    }

    void Update()
    {
        CheckInteraction();
        if (InputManager.GetInteractDown && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    void CheckInteraction()
    {
        // Raycasts from the camera 
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        // Use the interactionLayerMask to only hit objects in the specified layers
        if (Physics.Raycast(ray, out hit, playerReach, interactionLayerMask))
        {
            Interactable newInteractable = hit.collider.GetComponent<Interactable>();
            if (newInteractable == null) // if the object does not have an Interactable script attached
            {
                DisableCurrentInteractable();
                return;
            }
            currentObject = hit.transform.gameObject;

            if (newInteractable.enabled)
            {
                SetNewCurrentInteractable(newInteractable);
            } else //if new interactable is not enabled
            {
                DisableCurrentInteractable();
            }
        } else //if nothing is in reach
        {
            DisableCurrentInteractable();
        }
    }

    void SetNewCurrentInteractable(Interactable newInteractable)
    {
        hit = true;
        currentInteractable = newInteractable;
        interactionTMP.text = currentInteractable.displayMessage; // Display interaction text
    }

    void DisableCurrentInteractable()
    {
        hit = false;
        interactionTMP.text = string.Empty; // Remove interaction text
        if (currentInteractable)
        {
            currentInteractable = null;
        }
    }
}
