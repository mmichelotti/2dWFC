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

        if (CellManager.Cells.TryGetValue(gridCoordinate, out QuantumCell cell))
        {
            Cursor.visible = false;
            cellVisualizer.SetPosition(grid.CoordinateToPosition(gridCoordinate));

            Painting current = InputManager.IsLeftShiftPressed ? Painting.Erasing : Painting.Drawing;
            cellVisualizer.SetColor(current);
            if (InputManager.IsLeftMouseButtonPressed) CellManager.SetCell(gridCoordinate, current);
        }
        else
        {
            Cursor.visible = true;
            cellVisualizer.ClearColor();
        }
    }
}