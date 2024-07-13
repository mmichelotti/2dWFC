using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Grid))]
public class GridPainter : Manager
{
    public Grid Grid { get; private set; }
    [SerializeField] private QuantumCell quantumCell;
    public Dictionary<Vector2Int, QuantumCell> Cells { get; private set; } = new();
    private CellNeighborhood cellNeighborhood;

    private void Start()
    {
        Grid = GetComponent<Grid>();
        InitializeCells();
        ClearGrid();
    }

    private DirectionsRequired RequiredDirections(Vector2Int pos)
    {
        DirectionsRequired required = cellNeighborhood.GetDirectionsRequired(pos);
        return new(required.Required, required.Excluded | Grid.Boundaries(pos));
    }

    public void SpawnCell(Vector2Int pos)
    {
        Cells[pos].Constrain(RequiredDirections(pos));
        Cells[pos].CollapseState();
        cellNeighborhood.UpdateState(pos);
        cellNeighborhood.UpdateEntropy(pos);
    }

    public void RemoveCell(Vector2Int pos)
    {
        QuantumCell currentCell = Cells[pos];
        if (!currentCell.State.HasCollapsed) return;

        currentCell.ReobserveState(RequiredDirections(currentCell.Coordinate));
        foreach (var neighbor in cellNeighborhood.Get(pos, false).Values) neighbor.ReobserveState(RequiredDirections(neighbor.Coordinate));

        cellNeighborhood.UpdateEntropy(pos);
    }

    private void InitializeCells()
    {
        GameObject group = new("Cells");
        group.transform.parent = transform;
        Action<Vector2Int> initializeCell = pos =>
        {
            Cells.Add(pos, quantumCell.Initialize(pos, Grid, group.transform) as QuantumCell);
        };
        initializeCell.MatrixLoop(Grid.Length);
        cellNeighborhood = new(Cells);
    }


    public void FillGrid()
    {
        HashSet<Vector2Int> processedCells = new();
        Vector2Int currentPos = cellNeighborhood.LowestEntropy;

        while (!processedCells.Contains(currentPos))
        {
            SpawnCell(currentPos);
            processedCells.Add(currentPos);
            currentPos = cellNeighborhood.LowestEntropy;
        }
    }
    public void ClearGrid()
    {
        foreach (var cell in Cells.Values) cell.ResetState();
        cellNeighborhood.ClearQueue();
    }
}
