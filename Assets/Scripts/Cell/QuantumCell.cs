using System.Collections.Generic;
using System.Linq;
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
    public void InitializeState(bool invoke = true)
    {
        State = new(TileSet.AllConfigurations);
        if(invoke) OnInitializeState.Invoke(State);
    }
    public void ResetState(bool invoke = true)
    {
        InitializeState();
        Constrain(default);
        if(invoke) OnResetState.Invoke(State);
    }
    
    public void UpdateState(bool invoke = true)
    {
        List<Tile> newState = State.Superposition
            .Where(tile => tile.Directions.HasFlag(DirectionsRequired.Required) &&
                           (tile.Directions & DirectionsRequired.Excluded) == Directions.None)
            .ToList();

        State.Update(newState);
        if(invoke) OnUpdateState.Invoke(State);
    }

    public void ObserveState(DirectionsRequired dr, bool invoke = true)
    {
        Constrain(dr);
        UpdateState(invoke);
    }
    public void CollapseState(int? index = null, bool invoke = true)
    {
        if (State.HasCollapsed) return;
        State.Collapse(index);
        if (invoke) OnCollapseState.Invoke(State);
    }

}