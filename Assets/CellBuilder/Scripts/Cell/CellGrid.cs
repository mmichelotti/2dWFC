using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

[RequireComponent(typeof(Grid))]
public class CellGrid : Manager
{
    public Grid Grid { get; private set; }
    public Dictionary<Vector2Int, QuantumCell> Cells { get; private set; } = new();
    [SerializeField] private TileSet tileSet;
    [SerializeField] private CellComponents components;
    public static IReadOnlyDictionary<CellComponents, Type> ComponentMap { get; } = new Dictionary<CellComponents, Type>()
    {
        { CellComponents.CellPainter, typeof(CellPainter) },
        { CellComponents.CellHighlighter, typeof(CellHighlighter) },
        { CellComponents.CellDebugger, typeof(CellDebugger) },
        { CellComponents.CellParticle, typeof(CellParticle) },
        { CellComponents.CellAudioPlayer, typeof(CellAudioPlayer) }
    };

    #region flag fields
    [SerializeField] private ParticleSystem vfx;
    [SerializeField] private Color drawColor = Color.white;
    [SerializeField] private Color eraseColor = new(.4f, 0f, 1f, 1f);
    [SerializeField] private float fontSize = 3f;
    [SerializeField] private Color fontColor = new(0, .8f, .7f, 1f);
    [SerializeField]
    private List<AudioKVP> audioTypes = new();
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
            QuantumCell quantumCell = CreateCellPrefab(group.transform).GetComponent<QuantumCell>();
            quantumCell.Initialize(pos, this);
            quantumCell.InitializeState();
            quantumCell.ObserveState(new(default, Grid.Boundaries(pos)), false);
            Cells.Add(pos, quantumCell);
        };
        initializeCell.MatrixLoop(Grid.Size);
        quantumGrid = new QuantumGrid(Cells);
    }
    public void SpawnCell(Vector2Int pos, int? index = null)
    {
        Cells[pos].UpdateState();
        Cells[pos].CollapseState(index);
        quantumGrid.UpdateState(pos);
        quantumGrid.UpdateEntropy(pos);
    }

    public void RemoveCell(Vector2Int pos)
    {
        QuantumCell currentCell = Cells[pos];
        if (!currentCell.State.HasCollapsed) return;

        currentCell.ResetState();
        currentCell.ObserveState(quantumGrid.GetDirectionsRequired(pos).Exclude(Grid.Boundaries(pos)));
        foreach (var neighbor in quantumGrid.Get(pos, false).Values)
        {
            neighbor.ResetState();
            neighbor.ObserveState(quantumGrid.GetDirectionsRequired(neighbor.Coordinate).Exclude(Grid.Boundaries(neighbor.Coordinate)));
        }

        quantumGrid.UpdateEntropy(pos);
    }

    public void FillGrid(bool coroutine)
    {
        if (coroutine) StartCoroutine(FillGridCoroutine());
        else FillGridNormal();
    }

    public void FillGridNormal()
    {
        HashSet<Vector2Int> processedCells = new();
        Vector2Int currentPos = Grid.GetCoordinatesAt(Directions2D.All);
        while (!processedCells.Contains(currentPos))
        {
            SpawnCell(currentPos);
            processedCells.Add(currentPos);
            currentPos = quantumGrid.LowestEntropy;
        }
    }
    public IEnumerator FillGridCoroutine()
    {
        HashSet<Vector2Int> processedCells = new();
        Vector2Int currentPos = Grid.GetCoordinatesAt(Directions2D.All);

        // Process the initial position
        SpawnCell(currentPos);
        processedCells.Add(currentPos);
        while (true)
        {
            var lowestEntropyPositions = quantumGrid.LowestEntropyList;
            if (lowestEntropyPositions.Count == 0) break; // No more cells to process

            foreach (var pos in lowestEntropyPositions)
            {
                if (!processedCells.Contains(pos))
                {
                    SpawnCell(pos);
                    processedCells.Add(pos);
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }


    public void ClearGrid()
    {
        foreach (var (pos,cell) in Cells)
        {
            cell.ResetState();
            cell.ObserveState(new(default, Grid.Boundaries(pos)), false);
        }
        quantumGrid.ClearQueue();
    }

    public GameObject CreateCellPrefab(Transform parent = null)
    {
        GameObject cellPrefab = new("Cell");
        QuantumCell quantumCell = cellPrefab.AddComponent<QuantumCell>();
        quantumCell.TileSet = tileSet;
        Dictionary<AudioType, AudioSample> audioTypeDictionary = audioTypes.ToDictionary(pair => pair.AudioType, pair => pair.AudioSample);
        var propertySetters = new Dictionary<Type, Action<object>>
        {
            { typeof(CellParticle), obj => ((CellParticle)obj).SetProperties(vfx) },
            { typeof(CellHighlighter), obj => ((CellHighlighter)obj).SetProperties(drawColor, eraseColor) },
            { typeof(CellDebugger), obj => ((CellDebugger)obj).SetProperties(fontSize, fontColor) },
            { typeof(CellAudioPlayer), obj => ((CellAudioPlayer)obj).SetProperties(audioTypeDictionary) }
        };

        foreach (var component in ComponentMap.Where(c => components.HasFlag(c.Key)))
        {
            var addedComponent = cellPrefab.AddComponent(component.Value);
            if (propertySetters.TryGetValue(component.Value, out var setter)) setter(addedComponent);
        }

        cellPrefab.transform.parent = parent;
        return cellPrefab;
    }
}

/*
[BurstCompile]
public struct SpawnCellJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Vector2Int> Positions;

    public SpawnCellJob(NativeArray<Vector2Int> positions) => (Positions) = (positions);

    public void Execute(int index)
    {
        // The job does not directly interact with QuantumCell
        // Instead, it just processes the positions
        Vector2Int pos = Positions[index];
        // Add any position-based calculations or operations here
    }
}

 //to put within CelLGrid
private IEnumerator FillGridJobs()
{
    Stopwatch stopwatch = new();

    // To handle re-entry, stop any running instances
    StopCoroutine(nameof(FillGridJobs));

    HashSet<Vector2Int> processedCells = new();
    Vector2Int currentPos = Grid.GetCoordinatesAt(Directions2D.All);

    while (processedCells.Count < Grid.Count)
    {
        NativeList<Vector2Int> positions = new(Allocator.Persistent);

        // Collect positions to process
        for (int i = 0; i < 64; i++)
        {
            if (processedCells.Contains(currentPos))
            {
                currentPos = quantumGrid.LowestEntropy;
                continue;
            }

            positions.Add(currentPos);
            processedCells.Add(currentPos);
            currentPos = quantumGrid.LowestEntropy;
        }

        SpawnCellJob spawnCellJob = new(positions);

        JobHandle jobHandle = spawnCellJob.Schedule(positions.Length, 1);
        yield return new WaitUntil(() => jobHandle.IsCompleted);

        jobHandle.Complete();
        foreach (var pos in positions) SpawnCell(pos);
        positions.Dispose();

        yield return new WaitForEndOfFrame();  // Ensure coroutine continues in the next frame
    }
    areJobsCompleted = true;
}
*/