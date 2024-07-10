using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class QuantumCell : Cell, IQuantizable<Tile>
{
    public UnityEvent OnInitializeState { get; } = new();
    public UnityEvent OnCollapseState { get; } = new();
    public UnityEvent OnUpdateState { get; } = new();
    public UnityEvent OnResetState { get; } = new();


    [SerializeField] private TileSet tileSet;
    public QuantumState<Tile> State { get; set; }
    public bool HasDirection(Directions dir) => State.Collapsed.HasDirection(dir);

    private void Start() => InitializeState();
    public void InitializeState()
    {
        State = new(tileSet.AllConfigurations);
        OnInitializeState.Invoke();
    }
    public void ResetState()
    {
        InitializeState();
        DirectionsRequired = default;
        OnResetState.Invoke();
    }
    public void UpdateState()
    {
        List<Tile> newState = State.Superposition
            .Where(tile => tile.Directions.HasFlag(DirectionsRequired.Required) &&
                           (tile.Directions & DirectionsRequired.Excluded) == Directions.None)
            .ToList();

        State.Update(newState);
        OnUpdateState.Invoke();
    }
    public void CollapseState()
    {
        State.Collapse();
        OnCollapseState.Invoke();
    }
    public void ReobserveState(DirectionsRequired dr)
    {
        ResetState();
        Constrain(dr);
        UpdateState();
    }
}
