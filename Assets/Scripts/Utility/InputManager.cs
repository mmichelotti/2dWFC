using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Manager
{
    private InputActions inputActions;
    private CellManager gridManager;
    private void Awake()
    {
        inputActions = new();
        inputActions.Player.ResetGrid.performed += OnPress_R;
    }
    private void Start()
    {
        gridManager = GameManager.Instance.CellManager;
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnPress_R(InputAction.CallbackContext context)
    {
        gridManager.ResetCells();
    }
}
