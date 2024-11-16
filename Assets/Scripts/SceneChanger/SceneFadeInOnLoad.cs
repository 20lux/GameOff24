using System.Collections;
using UnityEngine;

public class SceneFadeInOnLoad : MonoBehaviour
{
    [Tooltip("Speed of fade-in transition")]
    public float speed = 1f;
    [Tooltip("Animation for fade-in")]
    public Animator fadeAnimation;

    void Awake()
    {
        var temp = fadeAnimation.GetCurrentAnimatorClipInfo(0);
        speed = temp[0].clip.length;
        StartCoroutine(WaitUntilFadeIn());
    }

    IEnumerator WaitUntilFadeIn()
    {
        fadeAnimation.SetBool("Activate", true);
        yield return new WaitForSeconds(speed);
    }
}
