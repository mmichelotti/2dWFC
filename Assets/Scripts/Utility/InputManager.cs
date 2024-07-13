using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputManager : Manager
{
    private InputActions inputActions;
    public UnityEvent OnLeftShiftEnter { get; } = new();
    public UnityEvent OnLeftShiftExit { get; } = new();
    public bool IsLeftShiftPressed { get; private set; }
    public bool IsLeftMouseButtonPressed { get; private set; }
    private void Awake()
    {
        inputActions = new();
        inputActions.Player.LeftShift.performed += ctx => IsLeftShiftPressed = true;
        inputActions.Player.LeftShift.canceled += ctx => IsLeftShiftPressed = false;
        inputActions.Player.LeftMouseButton.performed += ctx => IsLeftMouseButtonPressed = true;
        inputActions.Player.LeftMouseButton.canceled += ctx => IsLeftMouseButtonPressed = false;
        inputActions.Player.LeftShift.performed += ctx => OnLeftShiftEnter.Invoke();
        inputActions.Player.LeftShift.canceled += ctx => OnLeftShiftExit.Invoke();
    }
    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();
}
