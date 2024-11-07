using UnityEngine;

public class EyeballTracker : MonoBehaviour
{
    public Transform target;

    public float speed = 1.0f;

    void Update()
    {
        Vector3 targetDirection = target.position - transform.position;

        float singleStep = speed * Time.deltaTime;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
