using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

/*
This script is to be attached to an empty object with a trigger collider placed
in a transitional area within a scene to crossfade two ambient audio clips.
*/

namespace TheFall.AudioControl
{
    public class AudioCrossFader : MonoBehaviour
    {
        [Tooltip("Audio mixer that has the exposed parameters")]
        public AudioMixer mixer;
        [Tooltip("Enter exposed parameter from mixer name to fade from")]
        public string exposedParam1;
        [Tooltip("Enter exposed parameter from mixer name to fade to")]
        public string exposedParam2;
        [Tooltip("Duration of crossover fade")]
        public float duration = 3f;
        bool fading = false;

        void OnTriggerEnter (Collider col)
        {
            if (col.CompareTag("Player"))
            {
                CrossfadeGroups();
            }
        }

        public void CrossfadeGroups()
        {
            if (!fading)
            {
                StartCoroutine(Crossfade(exposedParam1, exposedParam2, duration));
            }
        }

        IEnumerator Crossfade(string param1, string param2, float fadeTime)
        {
            fading = true;
            float currentTime = 0;

            while (currentTime <= fadeTime)
            {
                currentTime += Time.deltaTime;

                mixer.SetFloat(param1, Mathf.Log10(Mathf.Lerp(1, 0.0001f, currentTime / fadeTime)) * 20);
                mixer.SetFloat(param2, Mathf.Log10(Mathf.Lerp(0.0001f, 1, currentTime / fadeTime)) * 20);

                yield return null;
            }

            fading = false;
        }
    }
}