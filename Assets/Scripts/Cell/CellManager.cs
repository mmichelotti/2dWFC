using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Grid))]
public class CellManager : Manager
{
    [SerializeField] private bool debugCell;
    [SerializeField] private Cell defaultCell;
    [SerializeField, ConditionalHide("debugCell",true)] private CellDebugger debuggerCell;
    [SerializeField] private Directions startingPoint;
    private Grid grid;
    public Dictionary<Vector2Int, Cell> cellAtPosition { get; private set; } = new();
    private CellNeighborhood neighborhood;

    private void Start()
    {
        grid = GetComponent<Grid>();
        InitializeCells();
        ResetCells();
    }
    public void SetCell(Vector2Int pos)
    {
        Cell currentCell = cellAtPosition[pos];
        DirectionsRequired neighbourRequires = neighborhood.GetDirectionsRequired(pos);
        Directions outOfBounds = grid.Boundaries(pos);

        currentCell.DirectionsRequired = neighbourRequires.Exclude(outOfBounds);
        currentCell.UpdateState();
        currentCell.CollapseState();
        currentCell.Debug();
        neighborhood.UpdateState(pos);

        Vector2Int nextPos = neighborhood.LowestEntropy;


        //automated version
        /*
        if (cellAtPosition[nextPos].State.IsEntangled) return;
        SetCell(nextPos);
        */
        
    }

    public void RemoveCell(Vector2Int pos)
    {
        Cell currentCell = cellAtPosition[pos];
        currentCell.ResetState();
        neighborhood.UpdateEntropy(pos);
    }
    private void InitializeCell(Vector2Int pos, Transform parent)
    {
        var defCell = defaultCell.SpawnInGrid(grid, pos, parent);
        if(debugCell) debuggerCell.SpawnInGrid(grid, pos, parent);
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

    public void ResetCells()
    {
        foreach (var cell in cellAtPosition.Values) cell.ResetState();
        Vector2Int startingPos = grid.GetCoordinatesAt(startingPoint);
        neighborhood.ClearQueue();
        neighborhood.UpdateEntropy(startingPos);
        //SetCell(startingPos);
    }
}
