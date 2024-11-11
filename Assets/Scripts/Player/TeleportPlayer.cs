using BoomMicCity.PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerController;
    private Camera playerCamera;
    private Rigidbody playerRigidbody;
    public Transform targetTeleport;
    private bool FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player GameObject with tag 'Player' not found.");
            return false;
        }

        if (!player.TryGetComponent(out playerController))
        {
            Debug.LogWarning("Player GameObject with tag 'Player' missing " +
                "JustinController Component.");
            return false;
        }

        playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogWarning("Player GameObject with tag 'Player' missing Camera " +
                "Component in children.");
            return false;
        }

        if (!player.TryGetComponent(out playerRigidbody))
        {
            Debug.LogWarning("Player GameObject with tag 'Player' missing " +
                "Rigidbody Component.");
            return false;
        } else
            return true;
    }


    public void Teleportation()
    {
        Teleport(targetTeleport);
    }
    private void Teleport(Transform teleportTarget)
    { 
        if (FindPlayer())
        {
            playerController.SetPlayerPosition(teleportTarget.position);
            playerController.SetPlayerYaw(teleportTarget.rotation.eulerAngles.y);
            playerController.SetPlayerPitch(teleportTarget.rotation.eulerAngles.x);
        }
    }
}
