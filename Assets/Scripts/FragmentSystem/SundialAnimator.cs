using UnityEngine;

public class SundialAnimator : MonoBehaviour
{
    [Tooltip("Holding game object with sundial arm")]
    [SerializeField] private Animator animator;

    public void TriggerArmRaise()
    {
        animator.SetTrigger("Activate");
    }
}
