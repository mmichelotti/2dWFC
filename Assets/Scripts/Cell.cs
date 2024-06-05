using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private List<Tile> allTiles = new();
    public List<Tile> Entropy { get; set; }
    public Vector2Int Coordinate { get; set; }
    public Tile CurrentTile { get; set; }
    public bool IsStable { get; set; }
    public int EntropyStatus => Entropy.Count;
    private void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        Entropy = new();
        foreach (var tile in allTiles)
        {
            Entropy.Add(tile);
            Tile rotate = new(tile);
            for (int i = 0; i < 3; i++)
            {
                RotateTile(rotate);
                Entropy.Add(new(rotate));
            }
        }
    }
    public void DebugStatus()
    {
        Debug.LogWarning($"Amount of entropy {EntropyStatus}");
        Debug.LogWarning("Possibility Tiles: ");
        foreach (var item in Entropy)
        {
            Debug.Log(item.Directions.ToStringCustom());
        }
    }
    private void RotateTile(Tile tile)
    {
        tile.DirectionShift();
        transform.Rotate(0, 0, 90);
    }
}
