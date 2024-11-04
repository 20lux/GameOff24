using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BoomMicCity.PlayerController
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        private Rigidbody rb;
        public AudioSource voiceBox;
        public TextMeshProUGUI subtitlesTMP;
        public TextMeshProUGUI standPromptTMP;
        public Image reticleImage;
        public string standPrompt = "Stand [C]";
        [SerializeField] private bool debugMode = false;

        #region Camera Movement Variables

        [Header("Camera Attributes")]
        public Camera playerCamera;
        public float fov = 80f;
        public bool invertCamera = false;
        public bool cameraCanMove = true;
        public float mouseSensitivity = 1.5f;
        public float maxLookAngle = 80f;

        [Header("Camera Smoothing")]
        public float smoothingFactor = 200f;

        // Internal Variables
        [HideInInspector] public float targetYaw = 0.0f;
        [HideInInspector] public float currentYaw = 0.0f;
        [HideInInspector] public float targetPitch = 0.0f;
        [HideInInspector] public float currentPitch = 0.0f;

        private float initialYRot = 0f;

        private float _yaw = 0.0f;
        private float _pitch = 0.0f;

        [HideInInspector]
        public float Yaw
        {
            get { return _yaw; }
            set { _yaw = value; }
        }

        [HideInInspector]
        public float Pitch
        {
            get { return _pitch; }
            set { _pitch = value; }
        }

        #endregion

        #region Movement Variables

        [Header("Movement Attributes")]
        public bool playerCanMove = true;
        private bool isSitting;
        [HideInInspector] public bool IsSitting => isSitting;
        private bool isInspecting;
        [HideInInspector] public bool IsInspecting => isInspecting;
        public readonly float initWalkSpeed = 2.25f;
        public float walkSpeed = 2.25f;
        public float maxVelocityChange = 5f;

        #endregion

        #region Sprint

        [Header("Sprint Attributes")]
        public bool enableSprint = true;
        public bool unlimitedSprint = false;
        public KeyCode sprintKey = KeyCode.LeftShift;
        public readonly float initSprintSpeed = 3.333f;
        public float sprintSpeed = 3.333f;
        public float sprintFOV = 80f;
        public float sprintFOVStepTime = 10f;

        // Internal Variables
        private bool isSprinting = false;

        private bool isSpeedTime = false;

        #endregion

        private void Awake()
        {
            // Load additional scenes if necessary.
            rb = GetComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Additive);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2, LoadSceneMode.Additive);

            // Set internal variables.
            playerCamera.fieldOfView = fov;
        }

        void Start()
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            initialYRot = transform.eulerAngles.y;

            // Initialize current yaw and pitch.
            currentYaw = initialYRot;
            targetYaw = initialYRot;
            currentPitch = playerCamera.transform.localEulerAngles.x;
            targetPitch = currentPitch;

            // Initialize _yaw and _pitch.
            _yaw = currentYaw;
            _pitch = currentPitch;

            SetSittingState(false); // Switch off Stand Prompt
        }

        private void Update()
        {
            //mouseSensitivity = settingsMenu.Msensitivity;
            #region Sprint

            if (enableSprint && isSprinting)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);
            } else
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, sprintFOVStepTime * Time.deltaTime);
            }

            #endregion

            ApplyPitchSmooth();
            ApplyYawSmooth();

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Alpha0) && debugMode)
            {
                float fastSpeed = 30;
                isSpeedTime = !isSpeedTime;
                Time.timeScale = isSpeedTime ? fastSpeed : 1;
            }

        }

        private void FixedUpdate()
        {
            ApplyMoveForce();
        }

        #region Player Position Methods

        public void SetPlayerPosition(Vector3 newPosition)
        {
            if (rb.isKinematic)
            {
                transform.position = newPosition; // Directly set position when kinematic
            } else
            {
                rb.MovePosition(newPosition); // Use physics method when non-kinematic
            }
        }

        #endregion

        #region Player Movement Methods

        private Vector3 CalculateMoveForce()
        {
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

                    return velocityChange;
                } else
                {
                    targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

                    Vector3 velocity = rb.linearVelocity;
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    velocityChange.y = 0;

                    return velocityChange;
                }
            } else
            {
                return Vector3.zero;
            }
        }

        private void ApplyMoveForce()
        {
            rb.AddForce(CalculateMoveForce(), ForceMode.VelocityChange);
        }

        #endregion

        #region Player Pitch Methods

        public void SetPlayerPitch(float newPitch)
        {
            // Update both current and target pitch to the new value
            targetPitch = newPitch;
            currentPitch = newPitch;
            _pitch = newPitch; // Update the internal pitch value
            playerCamera.transform.localRotation = Quaternion.Euler(newPitch, 0, 0);
        }

        private void CalculatePitch()
        {
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertCamera ? 1 : -1);
            targetPitch += mouseY;
            targetPitch = Mathf.Clamp(targetPitch, -maxLookAngle, maxLookAngle);
        }

        private void ApplyPitchSmooth()
        {
            if (cameraCanMove)
            {
                CalculatePitch();

                currentPitch = Mathf.LerpAngle(currentPitch, targetPitch, smoothingFactor * Time.deltaTime);
                _pitch = currentPitch; // Update the internal pitch value
                SetPlayerPitchSmooth(currentPitch);
            }
        }

        public void SetPlayerPitchSmooth(float newPitch)
        {
            currentPitch = newPitch;
            targetPitch = newPitch;
            _pitch = newPitch;
            playerCamera.transform.localRotation = Quaternion.Euler(newPitch, 0, 0);
        }

        #endregion

        #region Player Yaw Methods

        public void SetPlayerYaw(float newYaw)
        {
            // Update both current and target yaw to the new value
            targetYaw = newYaw;
            currentYaw = newYaw;
            _yaw = newYaw; // Update the internal yaw value
            rb.rotation = Quaternion.Euler(0, newYaw, 0);
        }

        private void CalculateYaw()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * (invertCamera ? -1 : 1);
            targetYaw += mouseX;
        }

        private void ApplyYawSmooth()
        {
            if (cameraCanMove)
            {
                CalculateYaw();

                currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, smoothingFactor * Time.deltaTime);
                _yaw = currentYaw; // Update the internal yaw value
                SetPlayerYawSmooth(currentYaw);
            }
        }

        public void SetPlayerYawSmooth(float newYaw)
        {
            currentYaw = newYaw;
            targetYaw = newYaw;
            _yaw = newYaw;

            if (rb.isKinematic)
            {
                transform.rotation = Quaternion.Euler(0, newYaw, 0); // Directly set rotation when kinematic
            } else
            {
                rb.MoveRotation(Quaternion.Euler(0, newYaw, 0)); // Use physics method when non-kinematic
            }
        }

        #endregion

        public void SetSittingState(bool value)
        {
            isSitting = value;
            standPromptTMP.text = value ? standPrompt : string.Empty;
        }
        public void SetInspectionState(bool value)
        {
            isInspecting = value;
        }

#if UNITY_EDITOR
        // Draw the player capsule for visibility.
        private void OnDrawGizmos()
        {
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
            if (capsuleCollider != null)
            {
                // Get capsule properties
                Vector3 capsuleCenter = capsuleCollider.center;
                float radius = capsuleCollider.radius;
                float height = capsuleCollider.height / 2f - radius; // Subtract the radius for the top/bottom hemispheres
                Transform transform = capsuleCollider.transform;

                // Calculate the capsule's top and bottom points in world space
                Vector3 topPoint = transform.TransformPoint(capsuleCenter + Vector3.up * height);
                Vector3 bottomPoint = transform.TransformPoint(capsuleCenter - Vector3.up * height);
                Vector3 center = transform.TransformPoint(capsuleCenter);

                // Set the color for Gizmos (optional)
                Gizmos.color = Color.cyan;

                // Draw cylinder lines connecting top and bottom hemispheres
                Gizmos.DrawLine(topPoint + transform.right * radius, bottomPoint + transform.right * radius);
                Gizmos.DrawLine(topPoint - transform.right * radius, bottomPoint - transform.right * radius);
                Gizmos.DrawLine(topPoint + transform.forward * radius, bottomPoint + transform.forward * radius);
                Gizmos.DrawLine(topPoint - transform.forward * radius, bottomPoint - transform.forward * radius);

                // Draw hemispheres at top and bottom points using Handles
                Handles.color = Color.cyan;
                Handles.DrawWireArc(topPoint, transform.right, transform.forward, -180f, radius);  // Top hemisphere (XZ)
                Handles.DrawWireArc(topPoint, transform.forward, transform.right, 180f, radius);  // Top hemisphere (YZ)

                Handles.DrawWireArc(bottomPoint, transform.right, transform.forward, 180, radius);  // Bottom hemisphere (XZ)
                Handles.DrawWireArc(bottomPoint, transform.forward, transform.right, -180, radius);  // Bottom hemisphere (YZ)
            }
        }
#endif
    }
}
