using UnityEngine;

public class MonolithRotation : MonoBehaviour
{

    [Tooltip("Enter degrees for monolith to rotate each round - should be unique to each monolith and value of 30, 60 or 90")]
    public float yDegrees = 30;
    private Quaternion startRotation;
    private AudioSource audioSource;
    private const float XVAL = 0;
    private const float ZVAL = -90;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startRotation = transform.rotation;
    }

    void Update()
    {
        if (transform.rotation.y > 360)
        {
            ResetRotations();
        }
    }

    public void ResetRotations()
    {
        transform.rotation = startRotation;
    }

    public void InitiateRotate()
    {
        audioSource.Play();

        var angle = transform.rotation.eulerAngles.y + yDegrees;
        transform.localRotation = Quaternion.Euler(XVAL, angle, ZVAL);
    }
}
