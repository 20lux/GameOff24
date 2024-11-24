using UnityEngine;

public class MonolithRotation : MonoBehaviour
{

    [Tooltip("Enter degrees for monolith to rotate each round - should be unique to each monolith and value of 30, 60 or 90")]
    public float yDegrees = 30;
    public float[] rotationArray = new float[12];
    private Quaternion startRotation;
    private AudioSource audioSource;
    private const float XVAL = 0;
    private const float ZVAL = -90;
    public float yCorrect = 90f;
    private int count;
    public bool isCorrect = false;

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

    public void CheckIfCorrect()
    {
        if (yCorrect == rotationArray[count])
        {
            isCorrect = true;
        }
    }

    public void InitiateRotate()
    {
        audioSource.Play();

        if(count < 12)
        {
            transform.rotation = Quaternion.Euler(XVAL, rotationArray[count], ZVAL);
            count++;
        }
        else
        {
            count = 0;
            transform.rotation = Quaternion.Euler(XVAL, rotationArray[count], ZVAL);
        }
    }
}
