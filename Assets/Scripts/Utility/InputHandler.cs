using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private InputActions inputActions;
    private GridManager gridManager;
    private void Awake()
    {
        inputActions = new InputActions();
        gridManager = FindObjectOfType<GridManager>();
        inputActions.Player.ResetGrid.performed += OnPress_R;
        Debug.Log(gridManager);
        Debug.Log(inputActions);
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
        gridManager.ResetGrid();
    }
}
