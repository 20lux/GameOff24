using UnityEngine;

namespace TheFall.MovingPlatforms
{
    [RequireComponent(typeof(AudioSource))]
    public class MovingPlatforms : MonoBehaviour
    {
        private AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        public void PlaySoundOnActivate()
        {
            audioSource.loop = false;
            audioSource.Play();
        }
    }
}
