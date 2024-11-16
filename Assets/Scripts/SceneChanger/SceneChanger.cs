using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SceneChanger : MonoBehaviour
{
    private Vector3 currentPlayerPosition;
    public enum Scene 
    {
        None,
        Home,
        Forest,
        Cliff,
        Fall,
        End
    }

    [Tooltip("Select scene for the trigger to transition to")]
    public Scene scene;
    [Tooltip("Speed of scene fade transition")]
    public float speed = 1f;
    private BoxCollider bc;

    void Start()
    {
        bc = GetComponent<BoxCollider>();
        bc.isTrigger = true;
    }

    void OnTriggerEnter(Collider col)
    {
        col.gameObject.transform.position = currentPlayerPosition;
        // put player in position in new loaded scene

        if (col.CompareTag("Player"))
        {
            switch (scene)
            {
                case Scene.Home:
                    Initiate.Fade("02_Home", Color.black, speed);
                    break;
                case Scene.Forest:
                    Initiate.Fade("03_Forest", Color.black, speed);
                    break;
                case Scene.Cliff:
                    Initiate.Fade("04_Cliff", Color.black, speed);
                    break;
                case Scene.Fall:
                    Initiate.Fade("05_Fall", Color.black, speed);
                    break;
                case Scene.End:
                    Initiate.Fade("06_End", Color.black, speed);
                    break;
                default:
                    Initiate.Fade("01_Title", Color.black, speed);
                    break;
            }
        }
    }
}
