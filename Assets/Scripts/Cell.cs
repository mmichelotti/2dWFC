using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class Cell : MonoBehaviour, IQuantumStatable<Tile>, IPositionable<Vector2Int>, IDirectionable, IRequirable, IDebuggable
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Tile> allTiles = new();

    public Vector2Int Coordinate { get; set; }
    public DirectionsRequired DirectionsRequired { get; set; } = new();

    public Directions Directions { get; set; }
    public QuantumState<Tile> State { get; set; } = new();
    public Tile Entangled { get; set; }
    public bool HasDirection(Directions dir) => Entangled.HasDirection(dir);

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeState();
    }

    public void InitializeState()
    {
        State.Collapse();
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
        List<Tile> newState = new();
        foreach (var tile in State.Superposition)
        {
            if (tile.Directions.HasFlag(DirectionsRequired.Required) && (tile.Directions & DirectionsRequired.Excluded) == Directions.None)
            {
                newState.Add(tile);
            }
        }
        State = new QuantumState<Tile>(newState);
        UnityEngine.Debug.Log($"Cell at {Coordinate} updated with {State.Density} possible tiles. Required directions: {DirectionsRequired.Required}, Excluded directions: {DirectionsRequired.Excluded}");
    }

    public void EntangleState() => Entangled = State.Entangle();
    public void Instantiate()
    {
        spriteRenderer.sprite = Entangled.Sprite;
        transform.Rotate(Entangled.Rotation);
    }

    public void Debug()
    {
        UnityEngine.Debug.Log($"{State.Entropy} entropy at {Coordinate}");
        foreach (var tile in State.Superposition) tile.Debug();
    }

}
