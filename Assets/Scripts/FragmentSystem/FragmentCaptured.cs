using UnityEngine;

namespace TheFall.FragmentController
{
    public class FragmentCaptured : MonoBehaviour
    {
        [Tooltip("Objects to animate after fragment is captured - usually platforms")]
        [SerializeField] private Animator[] activateObjects;
        [Tooltip("Inactive objects to activate once fragment is captured - usually platforms")]
        [SerializeField] private GameObject[] inactiveObjects;

        void Start()
        {
            for (int h = 0; h <inactiveObjects.Length; h++)
            {
                inactiveObjects[h].SetActive(false);
            }
        }

        public void OnFragmentCapture()
        {
            for (int i = 0; i < activateObjects.Length; i++)
            {
                activateObjects[i].SetBool("Activate", true);
            }

            for (int j = 0; j < inactiveObjects.Length; j++)
            {
                inactiveObjects[j].SetActive(true); // Can also have shader trigger here instead
            }
        }
    }
}
