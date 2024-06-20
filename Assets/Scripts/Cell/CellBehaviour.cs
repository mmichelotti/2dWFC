using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CellSpawner))]
public class CellBehaviour : Point, IInitializable, IQuantizable<Tile>
{
    private CellSpawner spawner;
    [SerializeField] private TileSet tileSet;
    [SerializeField] private ParticleSystem ps;
    public QuantumState<Tile> State { get; set; }
    public override bool HasDirection(Directions dir) => State.Collapsed.HasDirection(dir);
    public void Init() => InitializeState();
    public void InitializeState()
    {
        spawner = GetComponent<CellSpawner>();
        State = new(tileSet.AllConfigurations);
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
        ps.gameObject.SetActive(true);
        ps.Play();
        Directions = State.Collapsed.Directions;
        spawner.Draw(State.Collapsed);
    }
    public void Debug()
    {
        UnityEngine.Debug.Log($"{State.Entropy} entropy at {Coordinate}");
        foreach (var tile in State.Superposition) UnityEngine.Debug.Log(tile);
    }
}
