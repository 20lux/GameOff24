using UnityEngine;

namespace TheFall.MovingPlatforms
{
    [RequireComponent(typeof(AudioSource))]
    public class MovingPlatforms : MonoBehaviour
    {
        private AudioSource audioSource;
        private Animator animator;
        private bool hasActivated = false;

        void Start()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        public void PlaySoundOnActivate()
        {
            audioSource.loop = false;
            audioSource.Play();
        }

        void Update()
        {
            if (animator.GetBool("Activate") && !hasActivated)
            {
                PlaySoundOnActivate();
                hasActivated = true;
            }
        }
    }
}
