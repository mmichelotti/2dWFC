using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

public class InputManager : Manager
{
    private RoadInputAction inputActions;
    public UnityEvent OnLeftShiftEnter { get; } = new();
    public UnityEvent OnLeftShiftExit { get; } = new();
    public UnityEvent OnLeftMouseButtonEnter { get; } = new();
    public UnityEvent OnLeftMouseButtonExit { get; } = new();
    public UnityEvent WhileLeftMouseButton { get; } = new();
    public UnityEvent OnMiddleMouseButtonEnter { get; } = new();
    public UnityEvent OnMiddleMouseButtonExit { get; } = new();
    public UnityEvent WhileMiddleMouseButton { get; } = new();
    public UnityEvent<int> OnScrollWheel { get; } = new();
    public bool IsLeftShiftPressed { get; private set; }
    public bool IsLeftMouseButtonPressed { get; private set; }
    public bool IsMiddleMouseButtonPressed { get; private set; }
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

        inputActions.Player.LeftMouseButton.performed += ctx =>
        {
            IsLeftMouseButtonPressed = true;
            OnLeftMouseButtonEnter.Invoke();
        };
        inputActions.Player.LeftMouseButton.canceled += ctx =>
        {
            IsLeftMouseButtonPressed = false;
            OnLeftMouseButtonExit.Invoke();
        };

        inputActions.Player.MiddleMouseButton.performed += ctx =>
        {
            IsMiddleMouseButtonPressed = true;
            OnMiddleMouseButtonEnter.Invoke();
        };
        inputActions.Player.MiddleMouseButton.canceled += ctx =>
        {
            IsMiddleMouseButtonPressed = false;
            OnMiddleMouseButtonExit.Invoke();
        };

        inputActions.Player.ScrollWheel.performed += ctx =>
        {
            OnScrollWheel.Invoke(Math.Sign(ctx.ReadValue<Vector2>().y));
        };
    }
    private void Update()
    {
        if (IsLeftMouseButtonPressed)
        {
            WhileLeftMouseButton.Invoke();
        }
    }
    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();
}
