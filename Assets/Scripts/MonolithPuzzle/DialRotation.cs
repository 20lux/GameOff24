using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DialRotation : MonoBehaviour
{
    [Tooltip("Used to associate a monolith with this dial")]
    public MonolithRotation monolithRotation;
    private Quaternion startState;
    [Tooltip("Determines when the rotation is considered correct")]
    public float correctRotation;
    [Tooltip("Used to trigger final animation sequence")]
    public CheckIfCorrect checkIfCorrect;
    private AudioSource audioSource;
    private float angle;
    private bool isCorrect = false;
    
    void Start()
    {
        startState = transform.rotation;
        audioSource = GetComponent<AudioSource>();
    }

    public void RotateDial()
    {
        angle = transform.rotation.eulerAngles.z + monolithRotation.yDegrees;
        Debug.Log(angle);
        if (angle < 0)
        {
            angle = 360 + angle;
            Debug.Log(angle);
        }
        transform.localRotation = Quaternion.Euler(315, 0, angle);
    }

    public void CheckIfCorrect()
    {
        if (angle == correctRotation)
        {
            audioSource.Play();
            gameObject.layer = LayerMask.NameToLayer("Default");
            isCorrect = true;
            checkIfCorrect.list.Add("1");
        }
    }

    public void ResetDial()
    {
        if (!isCorrect)
        {
            transform.rotation = startState;
        }
    }
}
