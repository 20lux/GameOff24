using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference escapeAction;
    [SerializeField] private InputActionReference celestialLeft;
    [SerializeField] private InputActionReference celestialRight;
    [SerializeField] private InputActionAsset inputActions;

    public static bool GetInteractDown => Instance?.interactAction.action.WasPressedThisFrame() ?? false;
    public static bool GetEscapeDown => Instance?.escapeAction.action.WasPressedThisFrame() ?? false;
    public static bool GetCelestialLeft => Instance?.celestialLeft.action.IsPressed() ?? false;
    public static bool GetCelestialRight => Instance?.celestialRight.action.IsPressed() ?? false;
    public static Vector2 GetMove => Instance?.inputActions.FindAction("Move").ReadValue<Vector2>() ?? Vector2.zero;
    public static bool GetJump => Instance?.inputActions.FindAction("Jump").ReadValue<bool>() ?? false;


    private void Awake()
    {
        interactAction.action.actionMap.Enable();
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
