using UnityEngine;

namespace TheFall.PlayerController
{
    public class ResetPlayer : MonoBehaviour
    {
        [Tooltip("Starting position of player in level")]
        [SerializeField] private Transform playerStartPos;

        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Player"))
            {
                col.gameObject.transform.position = playerStartPos.position;
            }
        }
    }
}
