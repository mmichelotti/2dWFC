using System.Collections.Generic;
using UnityEngine;
using static Extensions;


[RequireComponent(typeof(SpriteRenderer))]
public class Cell : MonoBehaviour
{
    [SerializeField] private List<Tile> allTiles = new();
    public List<Tile> PossibleTiles { get; set; } = new();
    public int Entropy => PossibleTiles.Count;
    public Vector2Int Coordinate { get; set; }
    public Tile CurrentTile { get; set; }

    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Initialize();
    }
    public void Initialize()
    {
        foreach (var tile in allTiles) PossibleTiles.AddRange(tile.AllConfigurations);
    }

    public void RandomSet()
    {
        Tile selectedTile = PossibleTiles[PositiveRandom(Entropy)];

        spriteRenderer.sprite = selectedTile.Sprite;
        transform.Rotate(selectedTile.Rotation);

        PossibleTiles = new() { selectedTile };
        DebugCellStatus();
    }

    public void DebugCellStatus()
    {
        Debug.Log($"{Entropy} entropy at {Coordinate}");
        foreach (var tile in PossibleTiles) tile.DebugStatus();
    }
}
