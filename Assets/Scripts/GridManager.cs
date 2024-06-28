using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(Grid))]
public class GridManager : Manager
{
    [SerializeField]
    Cell cellPrefab;

    //grid manager should just talk to a cell handler, not to specific cell behaviors
    [SerializeField]
    private CellBehaviour cellBehaviourPF;

    [SerializeField]
    private CellDebugger cellDebuggerPF;

    //cell visualizer
    //cell drawer

    public Dictionary<Vector2Int, Cell> Cells { get; } = new();

    // public Dictionary<Vector2Int, CellBehaviour> cellsBehaviour { get; private set; } = new();
    // private readonly Dictionary<Vector2Int, CellDebugger> cellsDebugger = new();

    private CellNeighborhood cellNeighborhood;
    protected int spawnedCells;
    public Grid Grid { get; private set; }

    private void Start()
    {
        Grid = GetComponent<Grid>();
        InitializeCells();
        ClearGrid();
    }

    private DirectionsRequired RequiredDirections(Vector2Int pos)
    {
        //Exclude grid boundaries
        DirectionsRequired required = cellNeighborhood.GetDirectionsRequired(pos);
        return new(required.Required, required.Excluded | Grid.Boundaries(pos));
    }

    public void SetCell(Vector2Int pos, Painting mode)
    {
        switch (mode)
        {
            case Painting.Drawing:
                CollapseCell(pos);
                break;
            case Painting.Erasing:
                RemoveCell(pos);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    protected void CollapseCell(Vector2Int pos)
    {
        var currentCell = Cells[pos].GetComponent<CellBehaviour>();

        if (currentCell.State.HasCollapsed)
            return;

        currentCell.Collapse(RequiredDirections(pos));
        spawnedCells++;

        // Check if collapse also neighbors
        cellNeighborhood.UpdateState(pos);
        cellNeighborhood.UpdateEntropy(pos);
        spawnedCells += cellNeighborhood.CollapseCertain(pos).Count;
    }

    private void RemoveCell(Vector2Int pos)
    {
        var currentCell = Cells[pos].GetComponent<CellBehaviour>();
        if (currentCell.State.HasCollapsed)
        {
            currentCell.ReobserveState(RequiredDirections(currentCell.Coordinate));
            foreach (var neighbor in cellNeighborhood.Get(pos, false).Values)
                neighbor.ReobserveState(RequiredDirections(neighbor.Coordinate));
            cellNeighborhood.UpdateEntropy(pos);

            spawnedCells--;
        }
    }

    private void InitializeCell(Vector2Int position, Transform parent)
    {
        // var currentCellBehaviour = cellDebuggerPF.SpawnInGrid(Grid, pos, parent);
        // var currentCellDebugger = cellBehaviourPF.SpawnInGrid(Grid, pos, parent);

        var currentCell = Instantiate(cellPrefab, parent);
        currentCell.Coordinate = position;
        Cells.Add(position, currentCell);
        spawnedCells++;
    }

    private void InitializeCells()
    {
        GameObject group = new("Cells");
        group.transform.parent = transform;
        Action<Vector2Int> action = pos => InitializeCell(pos, group.transform);
        action.MatrixLoop(Grid.Length);
        // cellNeighborhood = new(Cells);
    }

    public void FillGrid()
    {
        Vector2Int nextPos = FindLowestEntropy().Key;
        int internalCounter = 0;
        while (spawnedCells < Cells.Count)
        {
            CollapseCell(nextPos);
            nextPos = FindLowestEntropy().Key;
            internalCounter++;
        }
    }

    public void ClearGrid()
    {
        foreach (var cell in Cells.Values)
            cell.GetComponent<CellBehaviour>().ResetState();
        cellNeighborhood.ClearQueue();
        spawnedCells = 0;
    }

    /// <summary>
    ///! Very bad code. An urge arises to use PriorityQueue to save the day.
    /// </summary>
    /// <returns></returns>
    KeyValuePair<Vector2Int, Cell> FindLowestEntropy() =>
        // TODO MAKE IT MORE PERFORMANT
        //! Very bad
        Cells
            .OrderBy((cellInfo) => cellInfo.Value.GetComponent<CellBehaviour>().State.Entropy)
            .First();
}
