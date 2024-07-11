using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class QuantumCell : Cell, IQuantizable<Tile>
{
    public UnityEvent<QuantumCell> OnInitializeState { get; } = new();
    public UnityEvent<QuantumState<Tile>> OnCollapseState { get; } = new();
    public UnityEvent<QuantumState<Tile>> OnUpdateState { get; } = new();
    public UnityEvent<QuantumState<Tile>> OnResetState { get; } = new();


    [SerializeField] private TileSet tileSet;

    public QuantumState<Tile> State { get; set; }

    private void Start()
    {
        InitializeState();
    }

    public void InitializeState()
    {
        State = new(tileSet.AllConfigurations);
        OnInitializeState.Invoke(this);
    }
    public void ResetState()
    {
        InitializeState();
        Constrain(default);
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

    public void ReobserveState(DirectionsRequired dr)
    {
        ResetState();
        Constrain(dr);
        UpdateState();
    }
    public void CollapseState()
    {
        if (State.HasCollapsed) return;
        UpdateState();
        State.Collapse();
        OnCollapseState.Invoke(State);
    }
}
