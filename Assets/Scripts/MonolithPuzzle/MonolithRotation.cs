using UnityEngine;

public class MonolithRotation : MonoBehaviour
{

    [Tooltip("Enter degrees for monolith to rotate each round - should be unique to each monolith and value of 45, 90 or 135")]
    public float yDegrees = 45;
    private AudioSource audioSource;
    private const float XVAL = 0;
    private const float ZVAL = -90;
    public float yCorrect = 90f;

    private Quaternion initialRotation = Quaternion.identity;
    private Quaternion nextRotation;
    private Quaternion currentRotation;
    private bool isRotating = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentRotation = initialRotation;
        nextRotation = currentRotation;
    }

    void Update()
    {
        if (isRotating)
        {
            transform.rotation = nextRotation;
            CheckIfCorrect();
            isRotating = false;
        }
    }

    public void ResetRotations()
    {
        transform.rotation = initialRotation;
    }

    void CheckIfCorrect()
    {
        if (transform.rotation.y == yCorrect)
        {
            Debug.Log("Correct orientation!");
        }
    }

    public void InitiateRotate()
    {
        isRotating = true;
        audioSource.Play();
        Debug.Log("Initiating rotation!");

        var yToTurn = nextRotation.eulerAngles.y + yDegrees;
        nextRotation = Quaternion.Euler(XVAL, yToTurn, ZVAL);
        Debug.Log("Turned " + yDegrees + " degrees on Y axis!");
    }
}
