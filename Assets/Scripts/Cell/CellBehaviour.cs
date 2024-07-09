using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(CellSpawner))]
public class CellBehaviour : Point, IQuantizable<Tile>
{
    public UnityEvent OnStateInitialized { get; } = new();
    public UnityEvent OnStateCollapsed { get; } = new();
    public UnityEvent OnStateUpdated { get; } = new();

    private CellSpawner spawner;
    [SerializeField] private TileSet tileSet;
    [SerializeField] private ParticleSystem ps;
    public QuantumState<Tile> State { get; set; }
    public override bool HasDirection(Directions dir) => State.Collapsed.HasDirection(dir);

    private void Start()
    {
        InitializeState();
    }
    public void InitializeState()
    {
        spawner = GetComponent<CellSpawner>();
        State = new(tileSet.AllConfigurations);
        OnStateInitialized.Invoke();
    }
    public void ReobserveState(DirectionsRequired dr)
    {
        ResetState();
        Constrain(dr);
        UpdateState();
    }
    public void ResetState()
    {
        InitializeState();
        spawner.Cancel();
        DirectionsRequired = default;
    }
    public void UpdateState()
    {
        List<Tile> newState = State.Superposition
            .Where(tile => tile.Directions.HasFlag(DirectionsRequired.Required) &&
                           (tile.Directions & DirectionsRequired.Excluded) == Directions.None)
            .ToList();

        State.Update(newState);
        OnStateUpdated.Invoke();
    }
    public void CollapseState()
    {
        State.Collapse();
        ps.gameObject.SetActive(true);
        ps.Play();
        spawner.Draw(State.Collapsed);
        OnStateCollapsed.Invoke();
    }
}
