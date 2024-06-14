using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CellVisualizer))]
public class CellDrawer : MonoBehaviour
{
    private CellManager CellManager => GameManager.Instance.CellManager;
    private InputManager InputManager => GameManager.Instance.InputManager;

    [SerializeField] private Grid grid;

    private CellVisualizer cellVisualizer;

    private void Awake()
    {
        cellVisualizer = GetComponent<CellVisualizer>();
    }

    void Update()
    {
        Vector2Int gridCoordinate = InputManager.GetMouseGridCoordinate(grid);

        if (CellManager.cellAtPosition.TryGetValue(gridCoordinate, out Cell cell))
        {

            cellVisualizer.SetPosition(grid.CoordinateToPosition(gridCoordinate));
            cellVisualizer.SetColor(InputManager.IsLeftShiftPressed);

            if (InputManager.IsLeftMouseButtonPressed)
            {
                if (InputManager.IsLeftShiftPressed)
                {
                    if (cell.State.IsEntangled) CellManager.RemoveCell(gridCoordinate);
                }
                else
                {
                    if (!cell.State.IsEntangled) CellManager.SetCell(gridCoordinate);
                }
            }


            if (cell.State.IsEntangled)
            {
                Debug.Log($"I'm entangled at {gridCoordinate}");
                cell.Debug();
            }
        }
        else
        {
            cellVisualizer.ClearColor();
        }
    }
}
