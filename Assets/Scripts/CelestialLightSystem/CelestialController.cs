using UnityEngine;

public class CelestialController : MonoBehaviour
{
    [SerializeField] private Light celestialBody;
    [SerializeField] private float rotationSpeed = 15f;

    private bool celestialLeftInput;
    private bool celestialRightInput;
    public bool isActive = false;
    private int rotDir = 0;

    private void Update()
    {
        if(isActive)
        {
            // Cache inputs for celestial rotation.
            celestialLeftInput = InputManager.GetCelestialLeft;
            celestialRightInput = InputManager.GetCelestialRight;
            
            if (celestialLeftInput && celestialRightInput)
            {
                return;
            } else if (celestialLeftInput)
            {
                rotDir = 1;
            } else if (celestialRightInput)
            {
                rotDir = -1;
            } else
            {
                return;
            }

            // Perform rotation with given input.
            float yOffset = celestialBody.transform.eulerAngles.y + (Time.deltaTime * rotationSpeed * rotDir);
            celestialBody.transform.rotation = Quaternion.Euler(
                                                celestialBody.transform.eulerAngles.x, 
                                                yOffset, 
                                                celestialBody.transform.eulerAngles.z);
        }
    }

    public void ActivateSundialPower()
    {
        isActive = true;
    }
}
