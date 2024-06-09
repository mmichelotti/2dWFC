using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class Cell : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Tile> allTiles = new();
    public QuantumState<Tile> State { get; private set; } = new();
    public Vector2Int Coordinate { get; set; }
    public Tile EntangledTile { get; private set; }
    public Directions Direction { get; set; }

    public bool HasDirection(Directions dir) => EntangledTile.Directions.HasFlag(dir);

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Initialize();
    }

    public void Initialize()
    {
        foreach (var tile in allTiles)
        {
            State.Add(tile.AllConfigurations);
        }
        Debug.Log($"Cell at {Coordinate} initialized with {State.Density} configurations.");
    }

    public void UpdateState(Directions requiredDirections, Directions excludedDirections)
    {
        List<Tile> newState = new();
        foreach (var tile in State.Superposition)
        {
            if (tile.Directions.HasFlag(requiredDirections) && (tile.Directions & excludedDirections) == Directions.None)
            {
                newState.Add(tile);
            }
        }
        State = new QuantumState<Tile>(newState);
        Debug.Log($"Cell at {Coordinate} updated with {State.Density} possible tiles. Required directions: {requiredDirections}, Excluded directions: {excludedDirections}");
    }

    public void EntangleState() => EntangledTile = State.Entangle();
    public void Instantiate()
    {
        spriteRenderer.sprite = EntangledTile.Sprite;
        transform.Rotate(EntangledTile.Rotation);
    }

    public void DebugState()
    {
        Debug.Log($"{State.Entropy} entropy at {Coordinate}");
        foreach (var tile in State.Superposition) tile.DebugStatus();
    }
}
