using UnityEngine;
using System.Collections.Generic;

namespace BoomMicCity.PlayerController
{
    public class FirstPersonController : MonoBehaviour
    {
        private Rigidbody rb;

        #region Camera Movement Variables

        public Camera playerCamera;
        public float fov = 80f;
        public bool invertCamera = false;
        public bool cameraCanMove = true;
        public float mouseSensitivity = 1.5f;
        public float maxLookAngle = 80f;
        public float framesOfSmoothing = 12;

        // Internal Variables
        private float _yaw = 0.0f;
        private float _pitch = 0.0f;

        private List<float> _rotArrayX = new List<float>();
        private List<float> _rotArrayY = new List<float>();

        private float initialYRot = 0f;

        public bool jCase;

        #endregion

        #region Movement Variables

        public bool playerCanMove = true;
        public float walkSpeed = 5f;
        public float maxVelocityChange = 10f;

        // Internal Variables

        #endregion

        #region Sprint

        public bool enableSprint = true;
        public bool unlimitedSprint = false;
        public KeyCode sprintKey = KeyCode.LeftShift;
        public float sprintSpeed = 7f;
        public float sprintFOV = 80f;
        public float sprintFOVStepTime = 10f;

        // Internal Variables
        private bool isSprinting = false;

        #endregion

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            // Set internal variables
            playerCamera.fieldOfView = fov;
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            initialYRot = transform.eulerAngles.y;
        }

        private void Update()
        {

            #region Sprint

            if (enableSprint && isSprinting)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);
            } else
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, sprintFOVStepTime * Time.deltaTime);
            }

            #endregion

            HandleCameraPitchRotation();
        }

        private void FixedUpdate()
        {
            HandlePlayerMovement();
        }

        private void LateUpdate()
        {
            HandlePlayerYawRotation();
        }


        private void HandlePlayerMovement()
        {
            #region Movement

            if (playerCanMove)
            {
                Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

                if (enableSprint && Input.GetKey(sprintKey))
                {
                    targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

                    Vector3 velocity = rb.linearVelocity;
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    velocityChange.y = 0;

                    rb.AddForce(velocityChange, ForceMode.VelocityChange);

                } else
                {
                    targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

                    Vector3 velocity = rb.linearVelocity;
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    velocityChange.y = 0;

                    rb.AddForce(velocityChange, ForceMode.VelocityChange);
                }
            }

            #endregion
        }


        private void HandleCameraPitchRotation()
        {
            if (jCase)
            {

                if (!invertCamera)
                {
                    _pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
                } else
                {
                    // Inverted Y
                    _pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
                }

                // Clamp pitch between lookAngle
                _pitch = Mathf.Clamp(_pitch, -maxLookAngle, maxLookAngle);

                playerCamera.transform.localEulerAngles = new Vector3(_pitch, 0, 0);
            } else
            {
                if (!cameraCanMove)
                    return;

                // Calculate the mouse input
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertCamera ? -1 : 1) * Time.timeScale;

                // Add mouse input to rotation and apply smoothing
                _pitch += mouseY;

                _pitch = Mathf.Clamp(_pitch, -maxLookAngle, maxLookAngle);

                _rotArrayY.Add(_pitch);


                if (_rotArrayY.Count > framesOfSmoothing)
                {
                    _rotArrayY.RemoveAt(0);
                }

                float avgPitch = 0f;

                foreach (float rot in _rotArrayY)
                {
                    avgPitch += rot;
                }
                avgPitch /= _rotArrayY.Count;

                // Apply the rotation
                Quaternion yQuaternion = Quaternion.AngleAxis(avgPitch, Vector3.left);

                playerCamera.transform.localRotation = yQuaternion; // Rotate the player camera
            }
        }

        private void HandlePlayerYawRotation()
        {
            if (jCase)
            {
                _yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

                // Calculate the yaw rotation quaternion
                Quaternion yawRotation = Quaternion.Euler(0, _yaw, 0);
                rb.MoveRotation(yawRotation);
            } else
            {
                if (!cameraCanMove)
                    return;

                // Calculate the mouse input
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.timeScale;

                // Add mouse input to rotation and apply smoothing
                _yaw += mouseX;

                _rotArrayX.Add(_yaw);

                if (_rotArrayX.Count > framesOfSmoothing)
                {
                    _rotArrayX.RemoveAt(0);
                }

                float avgYaw = 0f;

                foreach (float rot in _rotArrayX)
                {
                    avgYaw += rot;
                }
                avgYaw /= _rotArrayX.Count;

                // Apply the rotation using MoveRotation without multiplying by originalPlayerRotation
                Quaternion yawRotation = Quaternion.Euler(0, avgYaw + initialYRot, 0);
                rb.MoveRotation(yawRotation);
            }
        }
    }
}
