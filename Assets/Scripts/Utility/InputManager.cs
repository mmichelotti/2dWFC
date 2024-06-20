using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Manager
{
    private InputActions inputActions;
    private GridManager cellManager;
    public bool IsLeftShiftPressed { get; private set; }
    public bool IsLeftMouseButtonPressed { get; private set; }
    private void Awake()
    {
        inputActions = new();
        inputActions.Player.LeftShift.performed += ctx => IsLeftShiftPressed = true;
        inputActions.Player.LeftShift.canceled += ctx => IsLeftShiftPressed = false;
        inputActions.Player.LeftMouseButton.performed += ctx => IsLeftMouseButtonPressed = true;
        inputActions.Player.LeftMouseButton.canceled += ctx => IsLeftMouseButtonPressed = false;
    }

    private void Start()
    {
        cellManager = GameManager.Instance.GridManager;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    
    
    public Vector2Int GetMouseGridCoordinate(Grid grid)
    {
        Debug.Log(grid);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new(Vector3.forward, grid.transform.position);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 localPosition = grid.transform.InverseTransformPoint(hitPoint);
            Vector2Int pos = grid.ToGridCoordinate(localPosition);
            return pos;
        }
        return -Vector2Int.one;
    }
}
