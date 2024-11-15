using UnityEngine;

namespace TheFall.FragmentController
{
    public class FragmentCaptured : MonoBehaviour
    {
        [Tooltip("Objects to animate after fragment is captured - usually platforms")]
        [SerializeField] private Animator[] animatedObjects;
        
        [Tooltip("Inactive objects to activate once fragment is captured - usually platforms")]
        [SerializeField] private GameObject[] staticObjects;
        [SerializeField] private 


        void Start()
        {
            if (staticObjects.Length != 0)
            {
                for (int h = 0; h < staticObjects.Length; h++)
                {
                    staticObjects[h].SetActive(false);
                }
            }
        }

        public void OnFragmentCaptured()
        {
            ActivateInactiveObjects();
            AnimateActivatedObjects();
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
    }
}
