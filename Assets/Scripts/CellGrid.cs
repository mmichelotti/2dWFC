using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Grid))]
public class CellGrid : Manager
{
    public Grid Grid { get; private set; }
    [SerializeField] private QuantumCell quantumCell;
    public Dictionary<Vector2Int, QuantumCell> Cells { get; private set; } = new();
    private QuantumGrid quantumGrid;

    private void Start()
    {
        Grid = GetComponent<Grid>();
        InitializeCells();
        ClearGrid();
    }

    private DirectionsRequired RequiredDirections(Vector2Int pos)
    {
        DirectionsRequired required = quantumGrid.GetDirectionsRequired(pos);
        return new(required.Required, required.Excluded | Grid.Boundaries(pos));
    }

    public void SpawnCell(Vector2Int pos)
    {
        Cells[pos].Constrain(RequiredDirections(pos));
        Cells[pos].CollapseState();
        quantumGrid.UpdateState(pos);
        quantumGrid.UpdateEntropy(pos);
    }

    public void RemoveCell(Vector2Int pos)
    {
        QuantumCell currentCell = Cells[pos];
        if (!currentCell.State.HasCollapsed) return;

        currentCell.ReobserveState(RequiredDirections(currentCell.Coordinate));
        foreach (var neighbor in quantumGrid.Get(pos, false).Values) neighbor.ReobserveState(RequiredDirections(neighbor.Coordinate));

        quantumGrid.UpdateEntropy(pos);
    }

    private void InitializeCells()
    {
        GameObject group = new("Cells");
        group.transform.parent = transform;
        Action<Vector2Int> initializeCell = pos =>
        {
            QuantumCell cell = Cell.Create(quantumCell, pos, this, group.transform) as QuantumCell;
            Cells.Add(pos, cell);
        };
        initializeCell.MatrixLoop(Grid.Size);
        quantumGrid = new(Cells);
    }


    public void FillGrid()
    {
        HashSet<Vector2Int> processedCells = new();
        Vector2Int currentPos = quantumGrid.LowestEntropy;

        while (!processedCells.Contains(currentPos))
        {
            SpawnCell(currentPos);
            processedCells.Add(currentPos);
            currentPos = quantumGrid.LowestEntropy;
        }
    }
    public void ClearGrid()
    {
        foreach (var cell in Cells.Values) cell.ResetState();
        quantumGrid.ClearQueue();
    }
}
