using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Cell : MonoBehaviour
{
    [SerializeField] private List<Tile> allTiles = new();
    public QuantumState<Tile> State { get; private set; } = new();
    public Vector2Int Coordinate { get; set; }
    public Tile CurrentTile { get; private set; }

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Initialize();
    }
    public void Initialize()
    {
        foreach (var tile in allTiles) State.Add(tile.AllConfigurations);
    }

    public void RandomSet()
    {
        Tile selectedTile = State.ObserveRandom;

        spriteRenderer.sprite = selectedTile.Sprite;
        transform.Rotate(selectedTile.Rotation);

        State = new(selectedTile);
        DebugCellStatus();
    }

    public void DebugCellStatus()
    {
        Debug.Log($"{State.Entropy} entropy at {Coordinate}");
        foreach (var tile in State.Superposition) tile.DebugStatus();
    }
}
