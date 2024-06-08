using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A cell is a class that eventually wants to be a simple tile.
/// Until it's not defined as a tile, it remains a cell, which can store multiple tiles.
/// It has a space in the world, and it's responsable for being instantiated, to rotate accordingly its transform and its tile direction
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Cell : MonoBehaviour
{
    /// <summary>
    /// All tiles  is just a list from the inspector.
    /// You feed the tile you have, then the superposition is updated, it has all the basic tiles in each rotation version
    /// </summary>

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


    /// <summary>
    /// Initialize the QuantumState with all the tiles possible directions
    /// Bassically it all the basic tiles, rotated 4 times and saved in the superposition
    /// </summary>
    public void Initialize()
    {
        foreach (var tile in allTiles)
        {
            State.Add(tile.AllConfigurations);
        }
        Debug.Log($"Cell at {Coordinate} initialized with {State.Density} configurations.");
    }


    /// <summary>
    /// Upddate state reads the required and excluded direciton, based on these directions, it creates a new state where the super position has less entropy
    /// 
    /// </summary>
    /// <param name="requiredDirections"></param>
    /// <param name="excludedDirections"></param>
    public void UpdateState(Directions requiredDirections, Directions excludedDirections)
    {
        List<Tile> newState = new();
        foreach (var tile in State.Superposition)
        {
            if (tile.Directions.Contains(requiredDirections) && (tile.Directions & excludedDirections) == Directions.None)
            {
                newState.Add(tile);
            }
        }
        State = new(newState);
        Debug.Log($"Cell at {Coordinate} updated with {State.Density} possible tiles. Required directions: {requiredDirections}, Excluded directions: {excludedDirections}");
    }


    /// <summary>
    /// Set is the physical act of setting, textures, transforms, everything that is instance relatable
    /// </summary>
    /// <param name="tile"></param>
    public void Set(Tile tile)
    {
        spriteRenderer.sprite = tile.Sprite;
        transform.Rotate(tile.Rotation);
        CurrentTile = tile;
    }


    /// <summary>
    /// Just helpful for debugging
    /// </summary>
    public void DebugStatus()
    {
        Debug.Log($"{State.Entropy} entropy at {Coordinate}");
        foreach (var tile in State.Superposition) tile.DebugStatus();
    }
}
