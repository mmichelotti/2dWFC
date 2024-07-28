using System.Collections.Generic;
using UnityEngine;
using System;
using static ComponentUtility;


[RequireComponent(typeof(Grid))]
public class CellGrid : Manager
{
    public Grid Grid { get; private set; }
    public Dictionary<Vector2Int, QuantumCell> Cells { get; private set; } = new();
    [SerializeField] private TileSet tileSet;
    [SerializeField] private CellComponents components;

    #region flag fields
    [SerializeField] private ParticleSystem vfx;
    [SerializeField] private Color drawColor = Color.white;
    [SerializeField] private Color eraseColor = new(.4f, 0f, 1f, 1f);
    [SerializeField] private float fontSize = 3f;
    [SerializeField] private Color fontColor = new(0, .8f, .7f, 1f);

    [SerializeField] private AudioClip spawn;
    [SerializeField] private AudioClip erase;
    [SerializeField] private AudioClip scroll;
    [SerializeField] private AudioClip hover;
    #endregion

    private QuantumGrid quantumGrid;

    private void Awake()
    {
        Grid = GetComponent<Grid>();
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        GameObject group = new("Cells");
        group.transform.parent = transform;

        Action<Vector2Int> initializeCell = pos =>
        {
            AudioClip[] audioClip = new AudioClip[]
            {
                spawn, erase, scroll, hover
            };
            QuantumCell quantumCell = CreateCellPrefab(tileSet, vfx, group.transform, components, drawColor, eraseColor, fontSize, fontColor, audioClip).GetComponent<QuantumCell>();
            quantumCell.Initialize(pos, this);
            quantumCell.InitializeState();
            quantumCell.ObserveState(ExcludeGrid(pos), false);
            Cells.Add(pos, quantumCell);
        };
        initializeCell.MatrixLoop(Grid.Size);
        quantumGrid = new QuantumGrid(Cells);
    }
    private DirectionsRequired ExcludeGrid(Vector2Int pos)
    {
        DirectionsRequired dr = new();
        return dr.Exclude(Grid.Boundaries(pos));
    }
    private DirectionsRequired RequiredDirections(Vector2Int pos)
    {
        DirectionsRequired required = quantumGrid.GetDirectionsRequired(pos);
        return required.Exclude(Grid.Boundaries(pos));
    }
    
    public void SpawnCell(Vector2Int pos, int? index = null)
    {
        Cells[pos].ObserveState(ExcludeGrid(pos));
        Cells[pos].CollapseState(index);
        quantumGrid.UpdateState(pos);
        quantumGrid.UpdateEntropy(pos);
    }

    public void RemoveCell(Vector2Int pos)
    {
        QuantumCell currentCell = Cells[pos];
        if (!currentCell.State.HasCollapsed) return;

        currentCell.ResetState();
        currentCell.ObserveState(RequiredDirections(pos));
        foreach (var neighbor in quantumGrid.Get(pos, false).Values)
        {
            neighbor.ResetState();
            neighbor.ObserveState(RequiredDirections(neighbor.Coordinate));
        }

        quantumGrid.UpdateEntropy(pos);
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
