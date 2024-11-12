using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private InputActionAsset inputActions;

    public static bool GetInteractDown => Instance?.inputActions.FindAction("Interact").WasPressedThisFrame() ?? false;
    public static bool GetEscapeDown => Instance?.inputActions.FindAction("Escape").WasPressedThisFrame() ?? false;
    public static bool GetCelestialLeft => Instance?.inputActions.FindAction("CelestialLeft").IsPressed() ?? false;
    public static bool GetCelestialRight => Instance?.inputActions.FindAction("CelestialRight").IsPressed() ?? false;
    public static Vector2 GetMove => Instance?.inputActions.FindAction("Move").ReadValue<Vector2>() ?? Vector2.zero;
    public static bool GetSprintDown => Instance?.inputActions.FindAction("Sprint").WasPressedThisFrame() ?? false;
    public static bool GetSprint => Instance?.inputActions.FindAction("Sprint").IsPressed() ?? false;
    public static bool GetJumpDown => Instance?.inputActions.FindAction("Jump").WasPressedThisFrame() ?? false;
    public static bool GetJump => Instance?.inputActions.FindAction("Jump").IsPressed() ?? false;


    private void Awake()
    {
        inputActions.Enable();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Persist this object across scenes
        //DontDestroyOnLoad(gameObject);
    }

    public static InputAction Interact => Instance.inputActions.FindAction("Interact");
    public static InputAction Escape => Instance.inputActions.FindAction("Escape");
    public static InputAction CelestialLeft => Instance.inputActions.FindAction("CelestialLeft");
    public static InputAction CelestialRight => Instance.inputActions.FindAction("CelestialRight");
    public static InputAction Move => Instance.inputActions.FindAction("Move");
    public static InputAction Sprint => Instance.inputActions.FindAction("Sprint");
    public static InputAction Jump => Instance.inputActions.FindAction("Jump");
}
