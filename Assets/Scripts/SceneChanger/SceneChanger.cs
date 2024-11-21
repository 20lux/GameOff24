using UnityEngine;
using UnityEngine.Audio;
using TheFall.AudioControl;

[RequireComponent(typeof(BoxCollider))]
public class SceneChanger : MonoBehaviour
{
    public enum Scene 
    {
        None,
        Home,
        Forest,
        Cliff,
        Fall,
        End
    }

    [Tooltip("Select scene for the trigger to transition to")]
    public Scene scene;
    [Tooltip("Speed of scene fade transition")]
    public float speed = 1f;
    [Tooltip("Audio mixer group to fade audio when transitioning scene")]
    public AudioMixer mixer;
    [Tooltip("Exposed parameter from audio mixer to choose to fade out")]
    public string exposedParameter;
    [Tooltip("Target volume of final fade")]
    public float targetVol = 0;
    [Tooltip("Colour of fade")]
    public Color color;
    private BoxCollider bc;

    void Start()
    {
        bc = GetComponent<BoxCollider>();
        bc.isTrigger = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            switch (scene)
            {
                case Scene.Home:
                    StartCoroutine(FadeMixerGroup.StartFade(mixer, 
                        exposedParameter, speed, targetVol));
                    Initiate.Fade("02_Home", color, speed);
                    break;
                case Scene.Forest:
                    StartCoroutine(FadeMixerGroup.StartFade(mixer, 
                        exposedParameter, speed, targetVol));
                    Initiate.Fade("03_Forest", color, speed);
                    break;
                case Scene.Cliff:
                    StartCoroutine(FadeMixerGroup.StartFade(mixer, 
                        exposedParameter, speed, targetVol));
                    Initiate.Fade("04_Cliff", color, speed);
                    break;
                case Scene.Fall:
                    StartCoroutine(FadeMixerGroup.StartFade(mixer, 
                        exposedParameter, speed, targetVol));
                    Initiate.Fade("05_Fall", color, speed);
                    break;
                case Scene.End:
                    StartCoroutine(FadeMixerGroup.StartFade(mixer, 
                        exposedParameter, speed, targetVol));
                    Initiate.Fade("06_End", color, speed);
                    break;
                default:
                    StartCoroutine(FadeMixerGroup.StartFade(mixer, 
                        exposedParameter, speed, targetVol));
                    Initiate.Fade("01_Title", color, speed);
                    break;
            }
        }
    }
}
