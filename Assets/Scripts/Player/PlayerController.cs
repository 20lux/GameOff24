using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BoomMicCity.PlayerController
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        public AudioSource voiceBox;
        public TextMeshProUGUI subtitlesTMP;
        public TextMeshProUGUI standPromptTMP;
        public TextMeshProUGUI playerMovementDebugTMP;
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

        [HideInInspector] public float targetYaw = 0.0f;
        [HideInInspector] public float currentYaw = 0.0f;
        [HideInInspector] public float targetPitch = 0.0f;
        [HideInInspector] public float currentPitch = 0.0f;
        

        // Internal Variables
        private float initialYRot = 0f;
        private float _yaw = 0.0f;
        private float _pitch = 0.0f;
        private float previousStepCamY;

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

        [HideInInspector] public bool IsSitting => isSitting;
        [HideInInspector] public bool IsInspecting => isInspecting;
        public readonly float initWalkSpeed = 2.25f;
        [SerializeField] private float walkSpeed = 2.25f;
        [SerializeField] private float maxVelocityChange = 5f;
        [SerializeField] private float jumpForce = 3f;
        [SerializeField] private float stepHeight = .3f;

        // Internal Variables
        private bool isSprinting = false;
        private bool grounded;

        #endregion

        #region Sprint

        [Header("Sprint Attributes")]
        public bool enableSprint = true;
        public bool unlimitedSprint = false;
        public KeyCode sprintKey = KeyCode.LeftShift;
        public readonly float initSprintSpeed = 3.333f;
        public float sprintSpeed = 3.333f;
        public float sprintFOV = 80f;
        public float sprintFOVTransTime = 10f;

        #endregion

        // Internal Variables
        private Rigidbody rb;
        private CapsuleCollider capsule;
        private bool isSitting;
        private bool isInspecting;

        // Debug Variables
        private StringBuilder sb;


        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            capsule = GetComponent<CapsuleCollider>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            // Set internal variables.
            playerCamera.fieldOfView = fov;
        }

        void Start()
        {
            // Initialize string builder
            sb = new StringBuilder(); 

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

            // Initialize previous camera Y position
            previousStepCamY = playerCamera.transform.localPosition.y;
        }

        private void Update()
        {
            //mouseSensitivity = settingsMenu.Msensitivity;
            #region Sprint

            if (enableSprint && isSprinting)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVTransTime * Time.deltaTime);
            } else
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, sprintFOVTransTime * Time.deltaTime);
            }

            #endregion

            ApplyPitchSmooth();
            ApplyYawSmooth();

            PrintPlayerMovementDebug();
        }

        private void FixedUpdate()
        {
            grounded = IsGrounded();

            if (!playerCanMove) return;

            if (grounded)
            {
                if (InputManager.Jump.IsPressed())
                {
                    Debug.Log("Jump");
                    ApplyJumpForce();
                }
                //ApplyGroundedMoveForce();
                ApplyAirborneMoveForce();
            }
            else
            {
                ApplyAirborneMoveForce();
            }
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

        private bool IsGrounded()
        {
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
            float radius = capsuleCollider.radius * 0.9f;
            float distanceToGround = (capsuleCollider.bounds.extents.y) - radius + 0.1f; // Add a small buffer.
            Vector3 origin = transform.position + capsuleCollider.center;
            return Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hitInfo, distanceToGround);
        }

        private Vector3 CalculateGroundedMoveForce()
        {
            // Get the player's input direction
            Vector2 inputVector = InputManager.GetMove;
            Vector3 inputDirection = new(inputVector.x, 0, inputVector.y);
            Vector3 moveDirection = transform.TransformDirection(inputDirection).normalized;

            if (moveDirection.sqrMagnitude < 0.01f)
            {
                // No movement input
                return Vector3.zero;
            }

            // Determine speed
            float speed = (enableSprint && Input.GetKey(sprintKey)) ? sprintSpeed : walkSpeed;
            Vector3 targetVelocity = moveDirection * speed;

            // Get the capsule collider
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
            Vector3 capsuleCenter = transform.TransformPoint(capsuleCollider.center);
            float capsuleHeight = capsuleCollider.height;
            float capsuleRadius = capsuleCollider.radius * 0.9f; // Slightly reduce to avoid getting stuck

            float heightOffset = (capsuleHeight / 2) - capsuleRadius;
            Vector3 capsuleBottom = capsuleCenter + Vector3.down * heightOffset;
            Vector3 capsuleTop = capsuleCenter + Vector3.up * heightOffset;

            // Step 1: Trace upward by stepHeight
            bool hitUp = Physics.CapsuleCast(capsuleBottom, capsuleTop, capsuleRadius, Vector3.up, out RaycastHit hitUpInfo, stepHeight);
            float upDistance = hitUp ? hitUpInfo.distance : stepHeight;
            Vector3 upPosition = transform.position + Vector3.up * upDistance;

            // Update capsule positions for the upward position
            Vector3 upCapsuleBottom = capsuleBottom + Vector3.up * upDistance;
            Vector3 upCapsuleTop = capsuleTop + Vector3.up * upDistance;

            // Step 2: Trace forward along the move direction
            bool hitForward = Physics.CapsuleCast(upCapsuleBottom, upCapsuleTop, capsuleRadius, moveDirection, out RaycastHit hitForwardInfo, targetVelocity.magnitude * Time.fixedDeltaTime);
            float forwardDistance = hitForward ? hitForwardInfo.distance : targetVelocity.magnitude * Time.fixedDeltaTime;
            Vector3 forwardPosition = upPosition + moveDirection * forwardDistance;

            // Update capsule positions for the forward position
            Vector3 forwardCapsuleBottom = upCapsuleBottom + moveDirection * forwardDistance;
            Vector3 forwardCapsuleTop = upCapsuleTop + moveDirection * forwardDistance;

            // Step 3: Trace downward to original Y position
            bool hitDown = Physics.CapsuleCast(forwardCapsuleBottom, forwardCapsuleTop, capsuleRadius, Vector3.down, out RaycastHit hitDownInfo, upDistance);
            float downDistanceActual = hitDown ? hitDownInfo.distance : upDistance;
            Vector3 finalPosition = forwardPosition + Vector3.down * downDistanceActual;

            // Calculate the target velocity as the movement needed to reach the final position within the frame
            Vector3 desiredVelocity = (finalPosition - transform.position) / Time.fixedDeltaTime;

            // Calculate the velocity change
            Vector3 velocityChange = desiredVelocity - rb.linearVelocity;

            // Clamp the velocity change to prevent abrupt movements
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0; // Ignore vertical changes for smoother grounded movement

            return velocityChange;
        }


        private void ApplyGroundedMoveForce()
        {
            rb.AddForce(CalculateGroundedMoveForce(), ForceMode.VelocityChange);
        }

        private Vector3 CalculateAirborneMoveForce()
        {
            if (playerCanMove)
            {
                Vector2 inputVector = InputManager.GetMove;
                Vector3 inputDirection = new (inputVector.x, 0, inputVector.y);
                Vector3 moveDirection = transform.TransformDirection(inputDirection).normalized;

                // Determine Speed
                float speed = (enableSprint && InputManager.Sprint.IsPressed()) ? sprintSpeed : walkSpeed;
                //Vector3 velocityXZ = 

                Vector3 targetVelocity = moveDirection * speed;

                Vector3 velocity = rb.linearVelocity;
                Vector3 velocityChange = targetVelocity - velocity;
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                return velocityChange;

            } else
            {
                return Vector3.zero;
            }
        }

        private void ApplyAirborneMoveForce()
        {
            rb.AddForce(CalculateAirborneMoveForce(), ForceMode.VelocityChange);
        }

        private Vector3 CalculateJumpForce()
        {
            return Vector3.up * jumpForce;
        }

        private void ApplyJumpForce()
        {
            rb.AddForce(CalculateJumpForce(), ForceMode.VelocityChange);
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

        private void PrintPlayerMovementDebug()
        {
            sb.Clear(); // Wipe string builder data

            Vector2 inputVector2 = InputManager.GetMove;
            sb.Append($"<color=#00FF00>in|({inputVector2})</color>\n");

            float speed = InputManager.Sprint.IsPressed() ? sprintSpeed : walkSpeed;
            speed = rb.linearVelocity.magnitude * speed;
            sb.Append($"<color=#FFFFFF>s|{speed}</color>\n");

            sb.Append($"<color=#FFFF00>v|{rb.linearVelocity}</color>\n");

            sb.Append($"grounded|{IsGrounded()}\n");

            sb.Append($"falling|{!IsGrounded()}\n");

            RaycastHit[] results = new RaycastHit[10];
            int hits = Physics.SphereCastNonAlloc(transform.position, capsule.radius +0.01f, -transform.up, results, (capsule.height/2)+0.05f);
            float slope = 0;
            if ( hits > 0 )
            {
                slope = Vector3.Dot(transform.up, results[0].normal);
            }
            
            sb.Append($"slope|{slope}\n");

            playerMovementDebugTMP.text = sb.ToString();

            if (rb.linearVelocity.magnitude > 0)
            {
                DrawArrow(transform.position, rb.linearVelocity.normalized, .1f, .3f, Color.yellow);
            }
        }

        private GameObject[] lineObjects;
        private LineRenderer[] lineRenderers;

        private void InitRuntimeLineRenderers()
        {
            int totalLines = 9; // 4 for the base, 4 for the sides, 1 for the arrow shaft
            lineObjects = new GameObject[totalLines];
            lineRenderers = new LineRenderer[totalLines];


            for (int i = 0; i < totalLines; i++)
            {
                // Create a new child GameObject for each line segment
                lineObjects[i] = new GameObject("LineSegment" + i);
                lineObjects[i].transform.SetParent(transform);

                // Set Debugger Line Material to AlwaysOnTop
                //lineRenderers[i].material = new Material(Shader.Find("Unlit/AlwaysOnTop"));

                // Add LineRenderer to the child GameObject
                lineRenderers[i] = lineObjects[i].AddComponent<LineRenderer>();
                lineRenderers[i].startWidth = 0.0025f;
                lineRenderers[i].endWidth = 0.0025f;
                lineRenderers[i].material = new Material(Shader.Find("Sprites/Default"));
                lineRenderers[i].startColor = Color.yellow;
                lineRenderers[i].endColor = Color.yellow;
                lineRenderers[i].positionCount = 2;
            }
        }

        private void DrawArrow(Vector3 originPoint, Vector3 normal, float arrowHeadSize, float arrowShaftLength, Color yellow)
        {
            if (lineRenderers == null || lineRenderers.Length != 9)
            {
                InitRuntimeLineRenderers();
            }

            // Calculate the arrow shaft endpoints based on the originPoint
            Vector3 arrowShaftStart = originPoint;
            Vector3 arrowShaftEnd = arrowShaftStart + (normal * arrowShaftLength);

            // Draw the arrow shaft line
            lineRenderers[0].SetPosition(0, arrowShaftStart);
            lineRenderers[0].SetPosition(1, arrowShaftEnd);

            // Create a rotation that aligns the base of the arrowhead with the normal vector
            Quaternion rotation = Quaternion.LookRotation(normal);

            // Calculate the base vertices around the arrowShaftEnd in local space
            Vector3 halfSize = Vector3.one * (arrowHeadSize / 2);
            Vector3[] baseVertices = new Vector3[]
            {
                arrowShaftEnd + rotation * new Vector3(halfSize.x,  halfSize.z, 0),   // Top-right
                arrowShaftEnd + rotation * new Vector3(-halfSize.x, halfSize.z, 0),   // Top-left
                arrowShaftEnd + rotation * new Vector3(-halfSize.x, -halfSize.z, 0),  // Bottom-left
                arrowShaftEnd + rotation * new Vector3(halfSize.x, -halfSize.z, 0)    // Bottom-right
            };

            // Position the apex pointing away from the base
            Vector3 apex = arrowShaftEnd + (normal * arrowHeadSize);

            // Draw the base square of the pyramid
            for (int i = 0; i < 4; i++)
            {
                lineRenderers[i + 1].SetPosition(0, baseVertices[i]);
                lineRenderers[i + 1].SetPosition(1, baseVertices[(i + 1) % 4]);
            }

            // Draw the sides connecting the apex to each base vertex
            for (int i = 0; i < 4; i++)
            {
                lineRenderers[5 + i].SetPosition(0, apex);
                lineRenderers[5 + i].SetPosition(1, baseVertices[i]);
            }
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
