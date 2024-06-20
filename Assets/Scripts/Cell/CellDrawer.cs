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

        if (CellManager.cellsBehaviour.TryGetValue(gridCoordinate, out CellBehaviour cell))
        {
            Cursor.visible = false;
            cellVisualizer.SetPosition(grid.CoordinateToPosition(gridCoordinate));
            cellVisualizer.SetColor(InputManager.IsLeftShiftPressed);

            if (InputManager.IsLeftMouseButtonPressed)
            {
                if (InputManager.IsLeftShiftPressed)
                {
                    CellManager.RemoveCell(gridCoordinate);
                }
                else
                {
                    CellManager.SetCell(gridCoordinate);
                }
            }
        }
        else
        {
            Cursor.visible = true;
            cellVisualizer.ClearColor();
        }
    }
}