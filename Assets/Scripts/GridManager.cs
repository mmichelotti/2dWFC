using System.Collections.Generic;
using UnityEngine;
using System;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Cell prefab;
    [SerializeField] private MazeGrid grid;
    [SerializeField] private Directions startingPoint;

    private readonly Dictionary<Vector2Int, Cell> cellAtPosition = new();
    private QuantumNeighborhood<Cell, Tile> neighborhood;

    private Vector2Int nextPos;

    private void Start()
    {
        InitializeCells();
        ResetGrid();
    }
    private void SetCell(Vector2Int pos)
    {
        Cell currentCell = cellAtPosition[pos];
        DirectionsRequired neighbourRequires = neighborhood.GetDirectionsRequired(pos);
        Directions outOfBounds = grid.Boundaries(pos);
        currentCell.DirectionsRequired = neighbourRequires.Exclude(outOfBounds);
        currentCell.UpdateState();
        currentCell.EntangleState();
        currentCell.Instantiate();
        currentCell.Debug();
        neighborhood.UpdateState(pos);
        neighborhood.Enqueue(pos); // Enqueue neighbors after updating the state
        nextPos = neighborhood.LowestEntropy;
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

    private void DrawLine(Vector2Int pos)
    {
        Vector3 wsPos = grid.CoordinateToPosition(pos);
        Gizmos.DrawWireCube(wsPos, new Vector3(grid.Size.x, grid.Size.y, 0));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Action<Vector2Int> action = pos => DrawLine(pos);
        action.MatrixLoop(grid.Length);
    }
    public void ResetGrid()
    {
        foreach (var cell in cellAtPosition.Values) cell.ResetState();
        nextPos = grid.GetCoordinatesAt(startingPoint);
        neighborhood.ClearQueue();
        neighborhood.Enqueue(nextPos);
        SetCell(nextPos);
    }
}
