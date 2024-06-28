using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(Grid))]
public class GridManager : Manager
{
    [SerializeField]
    Cell cellPrefab;

    public Dictionary<Vector2Int, Cell> Cells { get; } = new();

    // public Dictionary<Vector2Int, CellBehaviour> cellsBehaviour { get; private set; } = new();
    // private readonly Dictionary<Vector2Int, CellDebugger> cellsDebugger = new();

    private CellNeighborhood cellNeighborhood;
    protected int collapsedCells;
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
        collapsedCells++;

        // Check if collapse also neighbors
        cellNeighborhood.UpdateState(pos);
        cellNeighborhood.UpdateEntropy(pos);
        collapsedCells += cellNeighborhood.CollapseCertain(pos).Count;
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

            collapsedCells--;
        }
    }

    private void InitializeCell(Vector2Int position, Transform parent)
    {
        // var currentCellBehaviour = cellDebuggerPF.SpawnInGrid(Grid, pos, parent);
        // var currentCellDebugger = cellBehaviourPF.SpawnInGrid(Grid, pos, parent);

        var currentCell = Instantiate(cellPrefab, parent);
        currentCell.Coordinate = position;
        Cells.Add(position, currentCell);
        collapsedCells++;
    }

    private void InitializeCells()
    {
        GameObject group = new("Cells");
        group.transform.parent = transform;
        Action<Vector2Int> action = pos => InitializeCell(pos, group.transform);
        action.MatrixLoop(Grid.Length);
        // cellNeighborhood = new(Cells);
    }

    public void ClearGrid()
    {
        foreach (var cell in Cells.Values)
            cell.GetComponent<CellBehaviour>().ResetState();
        cellNeighborhood.ClearQueue();
        collapsedCells = 0;
    }
}
