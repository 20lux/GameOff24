using UnityEngine;

public class UpdatePlayerLocation : MonoBehaviour
{
    private Transform currentPlayerPos;

    public void UpdatePlayerPosition()
    {
        var player = GameObject.FindWithTag("Player");
        currentPlayerPos = player.transform;
        transform.position = currentPlayerPos.position;
    }
}
