using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CellVisualizer))]
public class CellDrawer : MonoBehaviour
{
    private GridManager CellManager => GameManager.Instance.GridManager;
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

    public void Test()
    {
        Ray cameraToMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(cameraToMouse, out RaycastHit hit))
        {
            Vector2Int pos = hit.collider.GetComponentInParent<QuantumCell>().Coordinate;
            Cursor.visible = false;
            Painting current = InputManager.IsLeftShiftPressed ? Painting.Erasing : Painting.Drawing;
            cellVisualizer.SetColor(current);
            if (InputManager.IsLeftMouseButtonPressed) CellManager.SetCell(pos, current);
        }
        else
        {
            Cursor.visible = true;
            cellVisualizer.ClearColor();
        }
    }
}