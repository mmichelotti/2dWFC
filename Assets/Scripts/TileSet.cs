using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TileSet", menuName = "ScriptableObjects/TileSet", order = 1)]
public class TileSet : ScriptableObject
{
    [field: SerializeField] public List<Tile> Tiles;

    public List<Tile> AllConfigurations
    {
        get
        {
            List<Tile> allConfigurations = new();
            foreach (Tile tile in Tiles)
            {
                allConfigurations.AddRange(tile.AllConfigurations);
            }
            return allConfigurations;
        }
    }
}
