using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

public class InputManager : Manager
{
    private InputActions inputActions;
    public UnityEvent OnLeftShiftEnter { get; } = new();
    public UnityEvent OnLeftShiftExit { get; } = new();
    public UnityEvent<int> OnScrollWheel { get; } = new();
    public bool IsLeftShiftPressed { get; private set; }
    public bool IsLeftMouseButtonPressed { get; private set; }
    private void Awake()
    {
        inputActions = new();

        inputActions.Player.LeftShift.performed += ctx =>
        {
            IsLeftShiftPressed = true;
            OnLeftShiftEnter.Invoke();
        };
        inputActions.Player.LeftShift.canceled += ctx =>
        {
            IsLeftShiftPressed = false;
            OnLeftShiftExit.Invoke();
        };

        inputActions.Player.LeftMouseButton.performed += ctx => IsLeftMouseButtonPressed = true;
        inputActions.Player.LeftMouseButton.canceled += ctx => IsLeftMouseButtonPressed = false;

        inputActions.Player.ScrollWheel.performed += ctx =>
        {
            OnScrollWheel.Invoke(Math.Sign(ctx.ReadValue<Vector2>().y));
        };
    }
    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();
}
