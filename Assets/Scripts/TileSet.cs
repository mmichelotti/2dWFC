using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public struct Tile : IDirectionable, IEquatable<Tile>, IFormattable
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public Directions Directions { get; set; }
    public Vector3 Rotation { get; set; }
    public Tile(Tile tile) => (Name, Sprite, Directions, Rotation) = (tile.Name, tile.Sprite, tile.Directions, tile.Rotation);

    private readonly static HashSet<Tile> calculatingConfigurations = new();
    public List<Tile> AllConfigurations
    {
        get
        {
            if (calculatingConfigurations.Contains(this)) return new List<Tile>();

            calculatingConfigurations.Add(this);

            List<Tile> allDirections = new();
            Tile toRotate = new(this);
            for (int i = 0; i < 4; i++)
            {
                allDirections.Add(new(toRotate));
                toRotate.Rotate();
            }
            calculatingConfigurations.Remove(this);
            return allDirections;
        }
    }

    private void Rotate()
    {
        Directions = Directions.Bitshift(Shift.Right);
        Rotation += new Vector3(0, 0, 90);
    }
    public readonly bool HasDirection(Directions dir) => Directions.HasFlag(dir);
    public readonly bool Equals(Tile other) => (Name, Rotation, Directions) == (other.Name, other.Rotation, other.Directions);
    public readonly string ToString(string format, IFormatProvider formatProvider) => $"{Name}, {Directions.ToStringCustom()} roads with {Rotation.z} degrees rotation.";

}

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
