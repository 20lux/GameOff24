using UnityEngine;

public class TitleRotation : MonoBehaviour
{
    [SerializeField] private Transform[] titleTransforms;
    [SerializeField] private Transform backWallColliderTransform;
    [SerializeField] private Transform frontWallColliderTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float frontWallOffset = 5f;

    private Quaternion startRotation = Quaternion.identity;
    private Quaternion endRotation = Quaternion.Euler(0, 180, 0);
    private Quaternion currentRotation;

    private float frontWallPos;
    private float backWallPos;

    private void Start()
    {
        frontWallPos = frontWallColliderTransform.position.z;
        backWallPos = backWallColliderTransform.position.z;
    }

    private void Update()
    {
        // Calculate the player's distance from the endPosition.
        float lerp = Mathf.InverseLerp(backWallPos, frontWallPos - frontWallOffset, playerTransform.position.z);

        currentRotation = Quaternion.Slerp(startRotation, endRotation, lerp);

        foreach (Transform tr in titleTransforms )
        {
            tr.rotation = currentRotation;
        }
    }
}
