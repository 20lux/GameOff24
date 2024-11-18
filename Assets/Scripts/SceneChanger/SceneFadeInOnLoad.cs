using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using TheFall.AudioControl;

public class SceneFadeInOnLoad : MonoBehaviour
{
    [Tooltip("Speed of fade-in transition")]
    public float speed = 1f;
    [Tooltip("Animation for fade-in")]
    public Animator fadeAnimation;
    [Tooltip("Audio mixer group to fade audio when transitioning scene")]
    public AudioMixer mixer;
    [Tooltip("Exposed parameter from audio mixer to choose to fade out")]
    public string exposedParameter;
    [Tooltip("Target volume of final fade")]
    public float targetVol = 1;

    void Awake()
    {
        var temp = fadeAnimation.GetCurrentAnimatorClipInfo(0);
        speed = temp[0].clip.length;
        StartCoroutine(WaitUntilFadeIn());
        StartCoroutine(FadeMixerGroup.StartFade(mixer, exposedParameter, speed, targetVol));
    }

    IEnumerator WaitUntilFadeIn()
    {
        fadeAnimation.SetBool("Activate", true);
        yield return new WaitForSeconds(speed);
    }
}
