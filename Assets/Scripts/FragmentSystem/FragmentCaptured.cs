using UnityEngine;

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
        private AudioSource audioSource;


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
        }

        public void OnFragmentCaptured()
        {
            ActivateInactiveObjects();
            AnimateActivatedObjects();
            PlaySoundWhenCaptured();
        }

        public void DestroyFragment()
        {
            Destroy(gameObject);
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
    }
}
