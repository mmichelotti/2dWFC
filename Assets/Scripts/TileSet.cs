using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "TileSet", menuName = "ScriptableObjects/TileSet", order = 1)]
public class TileSet : ScriptableObject
{
    [field: SerializeField] public List<Tile> Tiles;

    [SerializeField]
    private List<Tile> allConfigurations;

    public List<Tile> AllConfigurations
    {
        get
        {
            if (allConfigurations == null || allConfigurations.Count == 0)
            {
                GenerateAllConfigurations();
            }
            return allConfigurations;
        }
    }

    public void GenerateAllConfigurations()
    {
        allConfigurations = new List<Tile>();
        foreach (Tile tile in Tiles)
        {
            allConfigurations.AddRange(tile.AllConfigurations);
        }
    }

    public Tile PickTile()
    {
        float totalProbability = Tiles.Sum(tile => tile.Probability);
        float randomPoint = UnityEngine.Random.value * totalProbability;

        foreach (Tile tile in Tiles)
        {
            if (randomPoint < tile.Probability)
            {
                return tile;
            }
            else
            {
                randomPoint -= tile.Probability;
            }
        }

        return Tiles[0]; // Fallback in case of rounding errors
    }
}
