using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CellVisualizer))]
public class CellDrawer : MonoBehaviour
{
    private GridManager CellManager => GameManager.Instance.GridManager;
    private InputManager InputManager => GameManager.Instance.InputManager;

    [SerializeField] private Grid grid;

    private CellVisualizer cellVisualizer;

    private Dictionary<Vector2Int, bool> isDrawn = new();

    private void Awake()
    {
        cellVisualizer = GetComponent<CellVisualizer>();
    }

    void Update()
    {
        Vector2Int gridCoordinate = InputManager.GetMouseGridCoordinate(grid);

        if (CellManager.cellAtPosition.TryGetValue(gridCoordinate, out Cell cell))
        {
            Cursor.visible = false;
            cellVisualizer.SetPosition(grid.CoordinateToPosition(gridCoordinate));
            cellVisualizer.SetColor(InputManager.IsLeftShiftPressed);

            if (InputManager.IsLeftMouseButtonPressed)
            {
                if (InputManager.IsLeftShiftPressed)
                {
                    if (cell.State.HasCollapsed) CellManager.RemoveCell(gridCoordinate);
                }
                else
                {
                    if (!cell.State.HasCollapsed) CellManager.SetCell(gridCoordinate);
                }
            }


            if (cell.State.HasCollapsed)
            {
                Debug.Log($"I'm entangled at {gridCoordinate}");
                cell.Debug();
            }
        }
        else
        {
            Cursor.visible = true;
            cellVisualizer.ClearColor();
        }
    }
}
