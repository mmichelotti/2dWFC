using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Cell : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private List<Tile> allTiles = new();
    public QuantumState<Tile> State { get; private set; } = new();
    public Vector2Int Coordinate { get; set; }
    public Tile CurrentTile { get; private set; }

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

    public void UpdateState(Directions dir)
    {
        List<Tile> newState = new();
        foreach (var tile in State.Superposition)
        {
            if (tile.Directions.Contains(dir))
            {
                newState.Add(tile);
            }
        }
        State = new QuantumState<Tile>(newState);
        Debug.Log($"Cell at {Coordinate} updated with {State.Density} possible tiles.");
    }

    public void Entangle()
    {
        Tile tile = State.Entangle(State.RandomIndex);
        Set(tile);
        CurrentTile = tile;
        DebugStatus();
    }

    private void Set(Tile tile)
    {
        spriteRenderer.sprite = tile.Sprite;
        transform.Rotate(tile.Rotation);
    }

    public void DebugStatus()
    {
        Debug.Log($"{State.Entropy} entropy at {Coordinate}");
        foreach (var tile in State.Superposition) tile.DebugStatus();
    }
}
