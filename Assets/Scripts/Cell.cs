using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class Cell : MonoBehaviour
{

    [SerializeField] private List<Tile> allTiles = new();
    public List<Tile> PossibleTiles { get; set; }
    public int Entropy => PossibleTiles.Count;
    public Vector2Int Coordinate { get; set; }
    public Tile CurrentTile { get; set; }
    public bool IsStable { get; set; }

    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Initialize();
        
    }
    public void Initialize()
    {
        PossibleTiles = new();
        foreach (var tile in allTiles)
        {
            PossibleTiles.Add(tile);
            Tile rotate = new(tile);
            for (int i = 0; i < 3; i++)
            {
                RotateTile(rotate);
                PossibleTiles.Add(new(rotate));
            }
        }
    }
    public void RandomSet()
    {
        int randomTile = Random.Range(0, Entropy);
        Tile selectedTile = PossibleTiles[randomTile];

        spriteRenderer.sprite = selectedTile.Sprite;
        transform.Rotate(selectedTile.Rotation);

        PossibleTiles.Clear();
        PossibleTiles.Add(selectedTile);
        
    }
    public void DebugStatus()
    {
        Debug.LogWarning($"Amount of entropy {Entropy}");
        Debug.LogWarning("Possibility Tiles: ");
        foreach (var item in PossibleTiles)
        {
            Debug.Log(item.Directions.ToStringCustom());
        }
    }
    private void RotateTile(Tile tile)
    {
        tile.DirectionShift();
        tile.Rotation += new Vector3(0, 0, 90);
    }
}
