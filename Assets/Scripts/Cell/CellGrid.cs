using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;


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

    public GameObject CreateCellPrefab (Transform parent = null)
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
        StartCoroutine(FillGridCoroutine());
    }
    public IEnumerator FillGridCoroutine()
    {
        HashSet<Vector2Int> processedCells = new();
        Vector2Int currentPos = Grid.GetCoordinatesAt(Directions2D.All);

        while (!processedCells.Contains(currentPos))
        {
            SpawnCell(currentPos);
            processedCells.Add(currentPos);
            currentPos = quantumGrid.LowestEntropy;

            yield return new WaitForSeconds(0f); 
        }
    }
    public void ClearGrid()
    {
        foreach (var (pos,cell) in Cells)
        {
            cell.ResetState();
            cell.ObserveState(ExcludeGrid(pos));
        }
        quantumGrid.ClearQueue();
    }
}
