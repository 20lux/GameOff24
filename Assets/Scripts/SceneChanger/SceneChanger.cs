using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class SceneChanger : MonoBehaviour
{
    public enum Scene 
    {
        None,
        Home,
        Forest,
        Cliff,
        End
    }

    [Tooltip("Select scene for the trigger to transition to")]
    public Scene scene;
    private BoxCollider bc;

    void Start()
    {
        bc.isTrigger = true;
    }

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
