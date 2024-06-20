using UnityEngine;

public class CellHandler : MonoBehaviour, IPositionable<Vector2Int>, IInitializable
{
    public Cell Cell { get; private set; }
    public CellDebugger CellDebugger { get; private set; }
    public CellVisualizer CellVisualizer { get; private set; }
    public CellSpawner CellSpawner { get; private set; }

    public Vector2Int Coordinate { get; set; }


    private void Awake()
    {
        Cell = GetComponentInChildren<Cell>();
        CellDebugger = GetComponentInChildren<CellDebugger>();
        //CellVisualizer = GetComponentInChildren<CellVisualizer>();
        //CellSpawner = GetComponentInChildren<CellSpawner>();
    }

    private void UpdateText() => CellDebugger?.SetText(Cell.State.Entropy);
    public void Init()
    {
        Cell.InitializeState();
        CellDebugger?.Init();
    }

    public void ResetCell(DirectionsRequired dr)
    {
        Cell.ResetState();
        Cell.Constrain(dr);
        UpdateText();
    }

    public void UpdateCell(DirectionsRequired requiredDirections)
    {
        Cell.Constrain(requiredDirections);
        Cell.UpdateState();
    }

    public void CollapseCell()
    {
        Cell.CollapseState();
        UpdateText();
    }

    public void Debug()
    {
        Cell.Debug();
    }
}
