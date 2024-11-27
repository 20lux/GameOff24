using UnityEngine;

public class UpdatePlayerLocation : MonoBehaviour
{
    public GameObject currentPlayerPos;

    void Start()
    {
        currentPlayerPos = GameObject.FindWithTag("PlayerPosition");
    }

    public void UpdatePlayerPosition()
    {
        currentPlayerPos.transform.position = transform.position;
    }
}
