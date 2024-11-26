using UnityEngine;
using System.Collections.Generic;
using BoomMicCity.PlayerController;
using UnityEngine.SceneManagement;

public class CheckIfCorrect : MonoBehaviour
{
    [Tooltip("Correct dial positions add to this list. When list is up to 3, trigger animation")]
    public List<string> list = new List<string>();
    public PlayerController playerController;

    public void InitiateEnd()
    {
        if (list.Count == 3)
        {
            playerController.playerCanMove = false;
            SceneManager.LoadSceneAsync("05a_FallCutScene");
        }
    }
}
