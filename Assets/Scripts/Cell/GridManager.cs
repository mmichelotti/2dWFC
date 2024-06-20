using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Grid))]
public class GridManager : Manager
{
    [SerializeField] private bool debugCell;
    [SerializeField] private Cell defaultCell;
    [SerializeField, ConditionalHide("debugCell",true)] private CellDebugger debuggerCell;
    public Grid Grid { get; private set; }

    //to fix this double dictionary
    public Dictionary<Vector2Int, Cell> cellAtPosition { get; private set; } = new();
    private readonly Dictionary<Vector2Int, CellDebugger> debuggerAtPosition = new();


    private readonly HashSet<Cell> counter = new();


    private CellNeighborhood neighborhood;

    private void Start()
    {
        Grid = GetComponent<Grid>();
        InitializeCells();
        ResetCells();
    }

    private void UpdateText()
    {
        foreach (var (pos, cell) in debuggerAtPosition) cell.SetText(cellAtPosition[pos].State.Entropy);   
    }

    private DirectionsRequired RequiredDirections(Vector2Int pos)
    {
        DirectionsRequired required = neighborhood.GetDirectionsRequired(pos);
        Directions outOfBounds = Grid.Boundaries(pos);
        return required.Exclude(outOfBounds);
    }


    public void SetCell(Vector2Int pos)
    {
        Cell currentCell = cellAtPosition[pos];
        currentCell.Constrain(RequiredDirections(pos));
        currentCell.UpdateState();
        currentCell.CollapseState();
        currentCell.Debug();
        neighborhood.UpdateState(pos);
        counter.Add(currentCell);
        foreach (var neighbour in neighborhood.CollapseCertain(pos)) counter.Add(neighbour);

        UpdateText();
    }

    public void RemoveCell(Vector2Int pos)
    {
        Cell currentCell = cellAtPosition[pos];

        ResetCell(currentCell);
        foreach (var neighbor in neighborhood.Get(pos, false).Values) ResetCell(neighbor);

        counter.Remove(currentCell);

        UpdateText();
    }


    private void InitializeCell(Vector2Int pos, Transform parent)
    {
        if (debugCell)
        {
            var debCell = debuggerCell.SpawnInGrid(Grid, pos, parent);
            debuggerAtPosition.Add(pos, debCell);
        }
        var defCell = defaultCell.SpawnInGrid(Grid, pos, parent);

        cellAtPosition.Add(pos, defCell);
    }

    private void InitializeCells()
    {
        GameObject group = new("Cells");
        group.transform.parent = transform;
        Action<Vector2Int> action = pos => InitializeCell(pos, group.transform);
        action.MatrixLoop(Grid.Length);
        neighborhood = new(cellAtPosition);
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
    private void ResetCell(Cell cell)
    {
        cell.ResetState();
        cell.Constrain(RequiredDirections(cell.Coordinate));
        cell.UpdateState();
    }
    public void ResetCells()
    {
        foreach (var cell in cellAtPosition.Values) cell.ResetState();
        neighborhood.ClearQueue();
        counter.Clear();
        UpdateText();
    }
}
