using System.Collections.Generic;
using UnityEngine;
using System;
using static Extensions;

[RequireComponent(typeof(Grid))]
public class CellGrid : Manager
{
    public Grid Grid { get; private set; }
    public Dictionary<Vector2Int, QuantumCell> Cells { get; private set; } = new();
    [SerializeField] private TileSet tileSet;
    [SerializeField] private ParticleSystem vfx;

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
        return required.Exclude(Grid.Boundaries(pos));
    }
    private DirectionsRequired ExcludeGrid(Vector2Int pos)
    {
        DirectionsRequired dr = new();
        return dr.Exclude(Grid.Boundaries(pos));
    }
    public void SpawnCell(Vector2Int pos)
    {
        Cells[pos].Constrain(ExcludeGrid(pos));
        Cells[pos].CollapseState();
        quantumGrid.UpdateState(pos);
        quantumGrid.UpdateEntropy(pos);
    }

    public void SpawnCell(Vector2Int pos, int index)
    {
        Cells[pos].Constrain(ExcludeGrid(pos));
        Cells[pos].CollapseState(index);
        quantumGrid.UpdateState(pos);
        quantumGrid.UpdateEntropy(pos);
    }

    public void RemoveCell(Vector2Int pos)
    {
        QuantumCell currentCell = Cells[pos];
        if (!currentCell.State.HasCollapsed) return;

        currentCell.ReobserveState(RequiredDirections(pos));
        foreach (var (dir, neighbor) in quantumGrid.Get(pos, false)) neighbor.ReobserveState(RequiredDirections(neighbor.Coordinate));

        quantumGrid.UpdateEntropy(pos);
    }

    private void InitializeCells()
    {
        GameObject group = new("Cells");
        group.transform.parent = transform;

        Action<Vector2Int> initializeCell = pos =>
        {
            QuantumCell quantumCell = CreateCellPrefab(tileSet, vfx, group.transform).GetComponent<QuantumCell>();
            quantumCell.Initialize(pos, this);
            DirectionsRequired dr = ExcludeGrid(pos);
            Debug.LogError(dr);
            Cells.Add(pos, quantumCell);
        };
        initializeCell.MatrixLoop(Grid.Size);
        quantumGrid = new QuantumGrid(Cells);
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
