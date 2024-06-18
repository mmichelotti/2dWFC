using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Grid))]
public class CellManager : Manager
{
    [SerializeField] private bool debugCell;
    [SerializeField] private Cell defaultCell;
    [SerializeField, ConditionalHide("debugCell",true)] private CellDebugger debuggerCell;
    private Grid grid;

    //to fix this double dictionary
    public Dictionary<Vector2Int, Cell> cellAtPosition { get; private set; } = new();
    public Dictionary<Vector2Int, CellDebugger> debuggerAtPosition { get; private set; } = new();
    private HashSet<Cell> counter = new HashSet<Cell>();


    private CellNeighborhood neighborhood;

    private void Start()
    {
        grid = GetComponent<Grid>();
        InitializeCells();
        ResetCells();
    }

    private void UpdateText()
    {
        foreach (var (pos, cell) in debuggerAtPosition) cell.Set(cellAtPosition[pos].State.Entropy);
        
    }

    private DirectionsRequired GetRequiredDirections(Vector2Int pos)
    {
        DirectionsRequired neighbourRequires = neighborhood.GetDirectionsRequired(pos);
        Directions outOfBounds = grid.Boundaries(pos);
        return neighbourRequires.Exclude(outOfBounds);
    }
    public void RemoveCell(Vector2Int pos)
    {
        Cell currentCell = cellAtPosition[pos];
        currentCell.ResetState();
        currentCell.DirectionsRequired = GetRequiredDirections(pos);
        currentCell.UpdateState();
        neighborhood.UpdateEntropy(pos);

        counter.Remove(currentCell);
        UpdateText();
    }
    private void InitializeCell(Vector2Int pos, Transform parent)
    {
        if (debugCell)
        {
            var debCell = debuggerCell.SpawnInGrid(grid, pos, parent);
            debuggerAtPosition.Add(pos, debCell);
        }
        var defCell = defaultCell.SpawnInGrid(grid, pos, parent);

        cellAtPosition.Add(pos, defCell);
    }
    private void InitializeCells()
    {
        GameObject group = new("Cells");
        group.transform.parent = transform;
        Action<Vector2Int> action = pos => InitializeCell(pos, group.transform);
        action.MatrixLoop(grid.Length);
        neighborhood = new(cellAtPosition);
    }
    public void SetCell(Vector2Int pos)
    {
        Cell currentCell = cellAtPosition[pos];
        currentCell.DirectionsRequired = GetRequiredDirections(pos);
        currentCell.UpdateState();
        currentCell.CollapseState();
        currentCell.Debug();
        neighborhood.UpdateState(pos);
        counter.Add(currentCell);
        foreach (var neighbour in neighborhood.CollapseCertain(pos)) counter.Add(neighbour);
        UpdateText();
    }
    public void AutoFill()
    {
        Vector2Int nextPos = neighborhood.LowestEntropy;
        while (counter.Count < cellAtPosition.Count)
        {
            SetCell(nextPos);
            nextPos = neighborhood.LowestEntropy;
        }    
    }
    public void ResetCells()
    {
        foreach (var cell in cellAtPosition.Values) cell.ResetState();
        neighborhood.ClearQueue();
        counter.Clear();
        UpdateText();
    }
}
