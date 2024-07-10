using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.CullingGroup;

public class QuantumCell : Cell, IQuantizable<Tile>
{
    public UnityEvent<QuantumCell> OnInitializeState { get; } = new();
    public UnityEvent<QuantumState<Tile>> OnCollapseState { get; } = new();
    public UnityEvent<QuantumState<Tile>> OnUpdateState { get; } = new();
    public UnityEvent<QuantumState<Tile>> OnResetState { get; } = new();


    [SerializeField] private TileSet tileSet;

    public QuantumState<Tile> State { get; set; }

    void Start() => InitializeState();
    public void InitializeState()
    {
        State = new(tileSet.AllConfigurations);
        OnInitializeState.Invoke(this);
    }
    public void ResetState()
    {
        InitializeState();
        DirectionsRequired = default;
        OnResetState.Invoke(State);
    }
    public void UpdateState()
    {
        List<Tile> newState = State.Superposition
            .Where(tile => tile.Directions.HasFlag(DirectionsRequired.Required) &&
                           (tile.Directions & DirectionsRequired.Excluded) == Directions.None)
            .ToList();

        State.Update(newState);
        OnUpdateState.Invoke(State);
    }
    public void CollapseState()
    {
        State.Collapse();
        OnCollapseState.Invoke(State);
    }
    public void ReobserveState(DirectionsRequired dr)
    {
        ResetState();
        Constrain(dr);
        UpdateState();
    }
}
