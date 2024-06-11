using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(Grid))]
public class CellManager : MonoBehaviour
{
    [SerializeField] private Cell prefab;
    [SerializeField] private Directions startingPoint;

    private Grid grid;
    private readonly Dictionary<Vector2Int, Cell> cellAtPosition = new();
    private QuantumNeighborhood<Cell, Tile> neighborhood;

    private void Start()
    {
        grid = GetComponent<Grid>();
        InitializeCells();
        ResetCells();
    }
    private void SetCell(Vector2Int pos)
    {
        Cell currentCell = cellAtPosition[pos];
        DirectionsRequired neighbourRequires = neighborhood.GetDirectionsRequired(pos);
        Directions outOfBounds = grid.Boundaries(pos);

        currentCell.DirectionsRequired = neighbourRequires.Exclude(outOfBounds);
        currentCell.UpdateState();
        currentCell.EntangleState();
        currentCell.Debug();
        neighborhood.UpdateState(pos);

        Vector2Int nextPos = neighborhood.LowestEntropy;
        if (cellAtPosition[nextPos].State.IsEntangled) return;
        SetCell(nextPos);
    }

    private void InitializeCell(Vector2Int pos, Transform parent)
    {
        Cell cell = Instantiate(prefab, parent);
        cell.transform.position = grid.CoordinateToPosition(pos);
        cell.transform.localScale = (Vector2)grid.Size;
        cell.Coordinate = pos;
        cellAtPosition.Add(pos, cell);
    }

    private void InitializeCells()
    {
        GameObject group = new("Cells");
        Action<Vector2Int> action = pos => InitializeCell(pos, group.transform);
        action.MatrixLoop(grid.Length);
        neighborhood = new(cellAtPosition);
    }

    public void ResetCells()
    {
        foreach (var cell in cellAtPosition.Values) cell.ResetState();
        Vector2Int startingPos = grid.GetCoordinatesAt(startingPoint);
        neighborhood.ClearQueue();
        neighborhood.UpdateEntropy(startingPos);
        SetCell(startingPos);
    }
}