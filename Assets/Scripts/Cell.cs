using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class Cell : MonoBehaviour, IQuantizable<Tile>, IPositionable<Vector2Int>, IDirectionable, IRequirable, IDebuggable
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Tile> allTiles = new();

    public Vector2Int Coordinate { get; set; }
    public DirectionsRequired DirectionsRequired { get; set; } = new();

    public Directions Directions { get; set; }
    public QuantumState<Tile> State { get; set; }
    public Tile Entangled { get; set; }
    public bool HasDirection(Directions dir) => Entangled.HasDirection(dir);

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeState();
    }

    public void InitializeState()
    {
        State = new();
        foreach (var tile in allTiles) State.Add(tile.AllConfigurations);
    }

    public void ResetState()
    {
        InitializeState(); 
        spriteRenderer.sprite = null;
        DirectionsRequired = new();
        Directions = Directions.None;
        transform.rotation = Quaternion.identity;
    }

    public void UpdateState()
    {
        List<Tile> newState = State.Superposition
            .Where(tile => tile.Directions.HasFlag(DirectionsRequired.Required) &&
                           (tile.Directions & DirectionsRequired.Excluded) == Directions.None)
            .ToList();

        State.Update(newState);
    }

    public void EntangleState()
    {
        Entangled = State.Entangle();
        spriteRenderer.sprite = Entangled.Sprite;
        transform.Rotate(Entangled.Rotation);
    }

    public void Debug()
    {
        UnityEngine.Debug.Log($"{State.Entropy} entropy at {Coordinate}");
        foreach (var tile in State.Superposition) tile.Debug();
    }

}
