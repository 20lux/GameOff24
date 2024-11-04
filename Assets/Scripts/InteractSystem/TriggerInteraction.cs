using BoomMicCity.PlayerController;
using UnityEngine;
using UnityEngine.Events;

public class TriggerInteraction : MonoBehaviour
{
    public UnityEvent triggerEvent;
    [SerializeField] private bool turnOffAfterTrigger = true;

    private bool deactivated = false;

    private void Start()
    {
        //used for exposing checkbox
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerController justinController = other.GetComponent<PlayerController>();
        if (!deactivated && other.CompareTag("Player") && justinController != null /*&& !justinController.isPlayingTape*/)
        {
            if (turnOffAfterTrigger)
            {
                deactivated = true;
            }
            triggerEvent?.Invoke();
        }
    }

}