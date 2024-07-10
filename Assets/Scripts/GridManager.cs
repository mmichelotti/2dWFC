using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Linq;

[RequireComponent(typeof(Grid))]
public class GridManager : Manager
{
    public Grid Grid { get; private set; }
    [SerializeField] private QuantumCell quantumCell; 
    public Dictionary<Vector2Int, QuantumCell> Cells { get; private set; } = new();
    private CellNeighborhood cellNeighborhood;

    private int collapsedCells;

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

    public void SetCell(Vector2Int pos, Painting mode)
    {
        switch (mode)
        {
            case Painting.Drawing: SpawnCell(pos);
                break;
            case Painting.Erasing: RemoveCell(pos);
                break;
            default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private void SpawnCell(Vector2Int pos)
    {
        QuantumCell currentCell = Cells[pos];
        if (currentCell.State.HasCollapsed) return;

        currentCell.Constrain(RequiredDirections(pos));
        currentCell.UpdateState();
        currentCell.CollapseState();

        cellNeighborhood.UpdateState(pos);
        cellNeighborhood.UpdateEntropy(pos);

        collapsedCells += (from neighbour 
                           in cellNeighborhood.CollapseCertain(pos)
                           select neighbour).Count() + 1;

    }

    private void RemoveCell(Vector2Int pos)
    {
        QuantumCell currentCell = Cells[pos];
        if (!currentCell.State.HasCollapsed) return;

        currentCell.ReobserveState(RequiredDirections(currentCell.Coordinate));
        foreach (var neighbor in cellNeighborhood.Get(pos, false).Values) neighbor.ReobserveState(RequiredDirections(neighbor.Coordinate));

        cellNeighborhood.UpdateEntropy(pos);

        collapsedCells--;

    }
    private void InitializeCell(Vector2Int pos, Transform parent)
    {
        var currentCell = Instantiate(quantumCell, parent);
        currentCell.Coordinate = pos;
        Cells.Add(pos, currentCell);

        collapsedCells++;
    }

    private void InitializeCells()
    {
        GameObject group = new("Cells");
        group.transform.parent = transform;
        Action<Vector2Int> action = pos => InitializeCell(pos, group.transform);
        action.MatrixLoop(Grid.Length);
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
        collapsedCells = 0;
    }
}