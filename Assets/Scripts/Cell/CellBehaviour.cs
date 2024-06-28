using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(CellSpawner))]
public class CellBehaviour : Point, IInitializable, IQuantizable<Tile>
{
    public UnityEvent<QuantumState<Tile>> OnStateChanged;
    public UnityEvent OnStateCollapsed;

    private CellSpawner spawner;

    [SerializeField]
    private TileSet tileSet;

    [SerializeField]
    private ParticleSystem particles;

    public QuantumState<Tile> State
    {
        get => state;
        set
        {
            state = value;
            OnStateChanged.Invoke(state);
        }
    }
    private QuantumState<Tile> state;

    public override bool HasDirection(Directions dir) => State.Collapsed.HasDirection(dir);

    public void Init() => InitializeState();

    public void InitializeState()
    {
        spawner = GetComponent<CellSpawner>();
        State = new(tileSet.AllConfigurations);
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
            .Where(
                tile =>
                    tile.Directions.HasFlag(DirectionsRequired.Required)
                    && (tile.Directions & DirectionsRequired.Excluded) == Directions.None
            )
            .ToList();

        State.Update(newState);
    }

    public void CollapseState()
    {
        State.Collapse();
        OnStateCollapsed.Invoke();
        particles.gameObject.SetActive(true);
        particles.Play();
        spawner.Draw(State.Collapsed);
    }

    public void Collapse(DirectionsRequired requiredDirections = new())
    {
        Constrain(requiredDirections);
        CollapseState();
    }
}
