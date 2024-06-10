using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class Cell : MonoBehaviour, IQuantumStatable<Tile>, IPositionable<Vector2Int>, IDirectionable
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Tile> allTiles = new();
    public QuantumState<Tile> State { get; set; } = new();
    public Vector2Int Coordinate { get; set; }
    public Tile Entangled { get; set; }

    public Directions Directions { get; set; }
    public Direction Direction { get; set; }
    public Direction Required { get; set; }
    public Direction Excluded { get; set; }
    public bool HasDirection(Direction dir) => Entangled.Directions.HasFlag(dir);

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
        Direction = Direction.None;
        Directions = new();
        transform.rotation = Quaternion.identity;
    }

    public void UpdateState()
    {
        List<Tile> newState = new();
        foreach (var tile in State.Superposition)
        {
            if (tile.Directions.HasFlag(Required) && (tile.Directions & Excluded) == Direction.None)
            {
                newState.Add(tile);
            }
        }
        State = new QuantumState<Tile>(newState);
        Debug.Log($"Cell at {Coordinate} updated with {State.Density} possible tiles. Required directions: {Required}, Excluded directions: {Excluded}");
    }

    public void EntangleState() => Entangled = State.Entangle();
    public void Instantiate()
    {
        spriteRenderer.sprite = Entangled.Sprite;
        transform.Rotate(Entangled.Rotation);
    }

    public void DebugState()
    {
        Debug.Log($"{State.Entropy} entropy at {Coordinate}");
        foreach (var tile in State.Superposition) tile.DebugStatus();
    }

}
