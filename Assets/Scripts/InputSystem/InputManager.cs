using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference escapeAction;
    [SerializeField] private InputActionReference celestialLeft;
    [SerializeField] private InputActionReference celestialRight;

    // Boolean properties for easy access
    public static bool GetInteractDown => Instance?.interactAction.action.WasPressedThisFrame() ?? false;
    public static bool GetEscapeDown => Instance?.escapeAction.action.WasPressedThisFrame() ?? false;
    public static bool GetCelestialLeft => Instance?.celestialLeft.action.IsPressed() ?? false;
    public static bool GetCelestialRight => Instance?.celestialRight.action.IsPressed() ?? false;


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

    public static InputAction InteractAction => Instance.interactAction.action;
    public static InputAction EscapeAction => Instance.escapeAction.action;
    public static InputAction CelestialLeft => Instance.celestialLeft.action;
    public static InputAction CelestialRight => Instance.celestialRight.action;

    private void OnEnable()
    {
        interactAction.action.Enable();
        escapeAction.action.Enable();

        celestialLeft.action.Enable();
        Debug.Log("celestialLeft enabled");
        celestialRight.action.Enable();
    }

    private void OnDisable()
    {
        interactAction.action.Disable();
        escapeAction.action.Disable();
        celestialLeft.action.Disable();
        celestialRight.action.Disable();
    }
}
