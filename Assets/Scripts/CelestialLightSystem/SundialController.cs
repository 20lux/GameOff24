using UnityEngine;

public class SundialController : MonoBehaviour
{
    [SerializeField] private Animator sundialAnimator;
    
    private bool isRaised;

    public void RaiseSundial()
    {
        sundialAnimator.SetBool("Activate", true);
    }

    public void LowerSundial()
    {
        sundialAnimator.SetBool("Activate", false);
    }
}
