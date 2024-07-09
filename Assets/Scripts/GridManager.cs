using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(Grid))]
public class GridManager : Manager
{
    //grid manager should just talk to a cell handler, not to specific cell behaviours
    [SerializeField] private CellBehaviour cellBehaviourPF;
    [SerializeField] private CellDebugger cellDebuggerPF;
    //cell visualizer
    //cell drawer
    
    public Dictionary<Vector2Int, CellBehaviour> cellsBehaviour { get; private set; } = new();
    private readonly Dictionary<Vector2Int, CellDebugger> cellsDebugger = new();

    private CellNeighborhood cellNeighborhood;
    private int collapsedCells;
    public Grid Grid { get; private set; }

    private void Start()
    {
        Grid = GetComponent<Grid>();
        InitializeCells();
        ClearGrid();
    }

    private void UpdateText()
    {
        foreach (var (pos, cell) in cellsDebugger) cell.SetText(cellsBehaviour[pos].State.Entropy);   

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
            case Painting.Drawing: SpawnCell(pos);
                break;
            case Painting.Erasing: RemoveCell(pos);
                break;
            default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private void SpawnCell(Vector2Int pos)
    {
        CellBehaviour currentCell = cellsBehaviour[pos];
        if (!currentCell.State.HasCollapsed)
        {
            currentCell.Constrain(RequiredDirections(pos));
            currentCell.UpdateState();
            currentCell.CollapseState();

            cellNeighborhood.UpdateState(pos);
            cellNeighborhood.UpdateEntropy(pos);

            foreach (var neighbour in cellNeighborhood.CollapseCertain(pos)) collapsedCells++;
            collapsedCells++;
            UpdateText();
        }

    }

    private void RemoveCell(Vector2Int pos)
    {
        CellBehaviour currentCell = cellsBehaviour[pos];
        if (currentCell.State.HasCollapsed)
        {
            currentCell.ReobserveState(RequiredDirections(currentCell.Coordinate));
            foreach (var neighbor in cellNeighborhood.Get(pos, false).Values) neighbor.ReobserveState(RequiredDirections(neighbor.Coordinate));
            cellNeighborhood.UpdateEntropy(pos);

            collapsedCells--;
        }
    }
    private void InitializeCell(Vector2Int pos, Transform parent)
    {

        var currentCellDebugger = Instantiate(cellDebuggerPF, parent);
        currentCellDebugger.transform.position = Grid.CoordinateToPosition(pos);
        currentCellDebugger.transform.localScale = (Vector2)Grid.Size;
        cellsDebugger.Add(pos, currentCellDebugger);






        var currentCellBehaviour = Instantiate(cellBehaviourPF, parent);
        currentCellBehaviour.transform.position = Grid.CoordinateToPosition(pos);
        currentCellBehaviour.transform.localScale = (Vector2)Grid.Size;
        currentCellBehaviour.Coordinate = pos;
        cellsBehaviour.Add(pos, currentCellBehaviour);


        currentCellDebugger.SubscribeToCell(currentCellBehaviour);
        collapsedCells++;
    }

    private void InitializeCells()
    {
        GameObject group = new("Cells");
        group.transform.parent = transform;
        Action<Vector2Int> action = pos => InitializeCell(pos, group.transform);
        action.MatrixLoop(Grid.Length);
        cellNeighborhood = new(cellsBehaviour);
    }

    public void FillGrid()
    {
        Vector2Int nextPos = cellNeighborhood.LowestEntropy;
        int internalCounter = 0;
        while (collapsedCells < cellsBehaviour.Count)
        {
            SpawnCell(nextPos);
            nextPos = cellNeighborhood.LowestEntropy;
            internalCounter++;
        }
    }
    public void ClearGrid()
    {
        foreach (var cell in cellsBehaviour.Values) cell.ResetState();
        cellNeighborhood.ClearQueue();
        collapsedCells = 0;
    }
}