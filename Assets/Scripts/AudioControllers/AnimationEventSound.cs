using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AnimationEventSound : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioClip clip;
    [SerializeField] private string exposedParam;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PitchShift()
    {
        mixer.SetFloat(exposedParam, Random.Range(0.5f, 1.0f));
    }

    public void PlayAudio()
    {
        PitchShift();
        audioSource.PlayOneShot(clip);
    }
}
