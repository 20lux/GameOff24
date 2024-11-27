using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool hasTriggered = false;

    // Closes door behind player when they enter
    // home level for the first time
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !hasTriggered)
        {
            animator.SetBool("Activate", false);
            hasTriggered = true;
        }
    }
}
