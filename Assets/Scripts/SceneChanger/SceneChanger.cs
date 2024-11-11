using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    enum Scene 
    {
        None,
        Home,
        Forest,
        Cliff,
        End
    }

    private Scene scene;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            switch (scene)
            {
                case Scene.Home:
                    SceneManager.LoadSceneAsync("02_Home");
                    break;
                case Scene.Forest:
                    SceneManager.LoadSceneAsync("03_Forest");
                    break;
                case Scene.Cliff:
                    SceneManager.LoadSceneAsync("04_Cliff");
                    break;
                case Scene.End:
                    SceneManager.LoadSceneAsync("05_End");
                    break;
                default:
                    SceneManager.LoadSceneAsync("01_Title");
                    break;
            }
        }
    }
}
