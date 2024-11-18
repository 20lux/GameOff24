using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace TheFall.FragmentController
{
    [RequireComponent(typeof(AudioSource))]
    public class FragmentCaptured : MonoBehaviour
    {
        [Tooltip("Objects to animate after fragment is captured - usually platforms")]
        [SerializeField] private Animator[] animatedObjects;
        
        [Tooltip("Inactive objects to activate once fragment is captured - usually platforms")]
        [SerializeField] private GameObject[] staticObjects;
        [Tooltip("Audio clip to play when player captures the fragment")]
        [SerializeField] private AudioClip capture;
        [Tooltip("Audio clip to loop for player proximity")]
        [SerializeField] private AudioClip loop;
        [Tooltip("To be used for SFX audio mixer group")]
        public AudioMixer mixer;
        [Tooltip("SFX audio mixer group exposed parameter")]
        public string exposedParam = "sfxVol";
        [Tooltip("Duration of SFX audio fade")]
        public float duration = 1f;
        [Tooltip("Target final volume of SFX - usually 0")]
        public float targetVol = 0;
        private AudioSource audioSource;
        private Renderer rend;
        private Light li;


        void Start()
        {
            if (staticObjects.Length != 0)
            {
                for (int h = 0; h < staticObjects.Length; h++)
                {
                    staticObjects[h].SetActive(false);
                }
            }

            audioSource = GetComponent<AudioSource>();
            audioSource.clip = loop;
            audioSource.loop = true;
            audioSource.playOnAwake = true;

            rend = GetComponent<Renderer>();

            li = GetComponentInChildren<Light>();
        }

        public void OnFragmentCaptured()
        {
            ActivateInactiveObjects();
            AnimateActivatedObjects();
            PlaySoundWhenCaptured();
        }

        public void DestroyFragment()
        {
            StartCoroutine(WaitToFinishAudio());
        }

        public void ActivateInactiveObjects()
        {
            if (staticObjects.Length != 0)
            {
                for (int j = 0; j < staticObjects.Length; j++)
                {
                    staticObjects[j].SetActive(true); // Can also have shader trigger here instead
                }
            }
        }

        public void AnimateActivatedObjects()
        {
            if (animatedObjects.Length != 0)
            {
                for (int i = 0; i < animatedObjects.Length; i++)
                {
                    animatedObjects[i].SetBool("Activate", true);
                }
            }
        }

        public void PlaySoundWhenCaptured()
        {
            audioSource.clip = capture;
            audioSource.loop = false;
            audioSource.Play();
        }

        IEnumerator WaitToFinishAudio()
        {
            var temp = audioSource.clip.length + duration;
            StartCoroutine(DisableObject());
            yield return new WaitForSeconds(temp);
        }

        IEnumerator DisableObject()
        {
            rend.enabled = false;
            li.enabled = false;
            yield return null;
        }
    }
}
