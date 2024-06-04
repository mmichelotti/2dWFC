using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private List<TileObject> allTiles = new();
    public List<TileObject> Entropy { get; set; } = new();
    public Vector2Int Coordinate { get; set; }
    public TileObject CurrentTile { get; set; }
    public bool IsStable { get; set; }
    public int EntropyStatus => Entropy.Count;
    private void Start()
    {
        Initialize();
        foreach (var tile in Entropy)
        {
            Debug.Log(tile.Directions);
        }
    }
    public void Initialize()
    {
        foreach (var tile in allTiles)
        {
            Entropy.Add(tile);

            TileObject ninetyDegrees = new(tile);
            ninetyDegrees.DirectionShift();
            Entropy.Add(ninetyDegrees);

            TileObject oneEightyDegrees = new(ninetyDegrees);
            oneEightyDegrees.DirectionShift();
            Entropy.Add(oneEightyDegrees);

            TileObject twoSeventyDegrees = new(oneEightyDegrees);
            twoSeventyDegrees.DirectionShift();
            Entropy.Add(twoSeventyDegrees);

        }
    }
}
