using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(SpriteRenderer))]
public class QuantumCell : Cell, IQuantizable<Tile>
{
    public UnityEvent<QuantumState<Tile>> OnInitializeState { get; } = new();
    public UnityEvent<QuantumState<Tile>> OnCollapseState { get; } = new();
    public UnityEvent<QuantumState<Tile>> OnUpdateState { get; } = new();
    public UnityEvent<QuantumState<Tile>> OnResetState { get; } = new();

    public TileSet TileSet { get; set; }
    public QuantumState<Tile> State { get; set; }

    private void Start()
    {
        new CellSpawner(this);
    }

    public void InitializeState()
    {
        State = new(TileSet.AllConfigurations);
        OnInitializeState.Invoke(State);
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
    public void CollapseState(int index)
    {
        if (State.HasCollapsed) return;
        UpdateState();
        State.Collapse(index);
        OnCollapseState.Invoke(State);
    }
}