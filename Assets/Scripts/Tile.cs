using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// TIle is a scriptable object useful to create tiles.
/// Here i can drag a texture, and i can tell that texture which roads have
/// Also stores a rotation information since it can be rotated by 0, 90, 180, 270 degrees
/// </summary>
[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile", order = 1)]
public class Tile : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public Directions Directions { get; set; }

    
    public Vector3 Rotation { get; set; }

    /// <summary>
    /// Base constructor to create a copy of itself
    /// </summary>
    /// <param name="tile"></param>
    public Tile(Tile tile) => (Name, Sprite, Directions, Rotation) = (tile.Name, tile.Sprite, tile.Directions, tile.Rotation);

    /// <summary>
    /// Its a property that given a tile, it returns a list of 4 tiles, one for each rotation
    /// </summary>
    public List<Tile> AllConfigurations
    {
        get
        {
            List<Tile> allDirections = new();
            Tile toRotate = new(this);
            for (int i = 0; i < 4; i++)
            {
                allDirections.Add(new(toRotate));
                toRotate.Rotate();
            }
            return allDirections;
        }
    }


    /// <summary>
    /// The act of rotating a tile, save its Direction enum as well as the physical transform rotation
    /// </summary>
    private void Rotate()
    {
        Directions = Directions.Bitshift();
        Rotation += new Vector3(0, 0, 90);
    }

    /// <summary>
    /// Useful for debugging
    /// </summary>
    public void DebugStatus() => Debug.Log($"{Name}, {Directions.ToStringCustom()} roads with {Rotation.z} degrees rotation.");
}
