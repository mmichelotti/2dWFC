using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "TileSet", menuName = "ScriptableObjects/TileSet", order = 1)]
public class TileSet : ScriptableObject
{
    [field: SerializeField] public List<Tile> Tiles;

    // This will store the pre-initialized configurations
    [SerializeField]
    private List<Tile> allConfigurations;

    public List<Tile> AllConfigurations
    {
        get
        {
            if (allConfigurations == null || allConfigurations.Count == 0)
            {
                Debug.LogError("non ero configurato");
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
}