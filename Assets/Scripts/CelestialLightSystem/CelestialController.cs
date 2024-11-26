using System.Collections;
using UnityEngine;

public class CelestialController : MonoBehaviour
{
    [SerializeField] private Light celestialBody;
    [SerializeField] private float rotationTime = 2f;

    private Transform celestialTF;

    private void Start()
    {
        celestialTF = celestialBody.transform;
    }

    public void RotateTo(float angle) 
    {
        StopAllCoroutines();
        StartCoroutine(RotateCelestialLight(angle));
    }

    private IEnumerator RotateCelestialLight(float inputAngle)
    {
        // Get starting Y axis rotation.
        float startAngle = celestialTF.rotation.eulerAngles.y;

        // Normalize the input angle to the range [0, 360]
        float endAngle = (inputAngle % 360 + 360) % 360;

        // Adjust for the shortest rotation path
        float shortestRotation = Mathf.DeltaAngle(startAngle, endAngle);

        // Calculate the target angle by adding the shortest rotation to the start angle
        float targetAngle = startAngle + shortestRotation;

        float ctr = 0;
        while(ctr < rotationTime)
        {
            ctr += Time.deltaTime;

            // Interpolation factor with easing
            float t = HelperScript.EaseInOut(ctr / rotationTime);

            // Interpolate the Y-axis rotation
            float currentAngle = Mathf.Lerp(startAngle, targetAngle, t);

            // Apply the rotation by only modifying the Y-axis
            celestialTF.rotation = Quaternion.Euler(
                celestialTF.rotation.eulerAngles.x,
                currentAngle,
                celestialTF.rotation.eulerAngles.z
            );

            yield return null;
        }

        // Ensure final rotation is set
        celestialTF.rotation = Quaternion.Euler(
            celestialTF.rotation.eulerAngles.x,
            endAngle,
            celestialTF.rotation.eulerAngles.z
        );
    }

}
