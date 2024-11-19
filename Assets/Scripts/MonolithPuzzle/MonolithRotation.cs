using UnityEngine;

public class MonolithRotation : MonoBehaviour
{
    public enum AmountToRotate
    {
        Fortyfive,
        Ninety,
        OneThirtyFive
    }

    [Tooltip("Number of degrees for the monolith to turn - ideally unique values")]
    public AmountToRotate amountToRotate;
    [Tooltip("Speed at which the monolith turns")]
    public float speed = 1f;
    [Tooltip("Add sound of monolith rotating")]
    public AudioSource audioSource;
    [Tooltip("Checked - clockwise, unchecked - counterclockwise")]
    public bool rotationIsClockwise = false;
    private bool isRotating = false;
    private float finalRotation;
    private float initialRotation;

    void Start()
    {
        initialRotation = transform.rotation.y;
        finalRotation = transform.rotation.y + DegreeToRotate();
        speed = audioSource.clip.length;
    }

    void Update()
    {
        if (isRotating)
        {
            while (initialRotation >= finalRotation)
            {
                initialRotation += Time.deltaTime * speed;
                transform.rotation = Quaternion.Euler(0, initialRotation, 0);
            }

            isRotating = false;
        }
    }

    public void InitateRotate()
    {
        isRotating = true;
    }

    int DegreeToRotate()
    {
        var degInit = 0;
        switch (amountToRotate)
        {
            case AmountToRotate.Fortyfive:
                if (rotationIsClockwise)
                {
                    degInit = 45;
                }
                else
                {
                    degInit = -45;
                }
                break;
            case AmountToRotate.Ninety:
                if (rotationIsClockwise)
                {
                    degInit = 90;
                }
                else
                {
                    degInit = -90;
                }
                break;
            case AmountToRotate.OneThirtyFive:
                if (rotationIsClockwise)
                {
                    degInit = 135;
                }
                else
                {
                    degInit = -135;
                }
                break;
        }
        return degInit;
    }
}
