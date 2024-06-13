using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(CellSpawner))]
public class Cell : Point, IQuantizable<Tile>
{
    private CellSpawner spawner;
    [SerializeField] private TileType tileType;
    [SerializeField] private List<Tile> allTiles = new();
    public QuantumState<Tile> State { get; set; }
    public override bool HasDirection(Directions dir) => State.Collapsed.HasDirection(dir);
    public void InitializeState()
    {
        spawner = GetComponent<CellSpawner>();
        State = new();
        foreach (var tile in allTiles) State.Add(tile.AllConfigurations);
    }
    public void ResetState()
    {
        InitializeState();
        spawner.Cancel();
        DirectionsRequired = default;
        Directions = default;
    }
    public void UpdateState()
    {
        List<Tile> newState = State.Superposition
            .Where(tile => tile.Directions.HasFlag(DirectionsRequired.Required) &&
                           (tile.Directions & DirectionsRequired.Excluded) == Directions.None)
            .ToList();

        State.Update(newState);
    }
    public void CollapseState()
    {
        State.Collapse();
        spawner.Draw(State.Collapsed);
    }
    public void Debug()
    {
        UnityEngine.Debug.Log($"{State.Entropy} entropy at {Coordinate}");
        foreach (var tile in State.Superposition) tile.Debug();
    }
}
