using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CheatInput : MonoBehaviour
{
    public KeyCode[] CheatCode;
    public UnityEvent CheatEvent;
    public float AllowedDelay = 1f;

    private float _delayTimer;
    private int _index = 0;

    void Update()
    {
        _delayTimer += Time.deltaTime;
        if (_delayTimer > AllowedDelay)
        {
            ResetCheatInput();
        }

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(CheatCode[_index]))
            {
                _index++;
                _delayTimer = 0f;
            }
            else
            {
                ResetCheatInput();
            }
        }

        if (_index == CheatCode.Length)
        {
            ResetCheatInput();
            CheatEvent.Invoke();
        }
    }

    void ResetCheatInput()
    {
        _index = 0;
        _delayTimer = 0f;
    }

    public void Cheat()
    {
        if (SceneManager.GetActiveScene().name != "06_End")
        {
            var currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadSceneAsync(currentScene.buildIndex + 1);
        }
    }
}