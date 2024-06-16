using UnityEngine;
using System.Collections.Generic;
using System;

[Flags]
public enum Rotations
{
    None = 0b0000,
    X = 0b0001,
    Y = 0b0010,
    Z = 0b0100,
    All = ~None
}

[Serializable]
public struct Rotation
{
    public Vector3 Angle;
    public Rotations Type;

    public Rotation(Vector3 angle, Rotations type) => (Angle,Type) = (angle,type);

    public static Rotation operator +(Rotation a, Vector3 b) => new(a.Angle + b, a.Type);
    public static Rotation operator +(Vector3 a, Rotation b) => new(a + b.Angle, b.Type);

    public static implicit operator Vector3 (Rotation rotation) => rotation.Angle;
    public static implicit operator Rotations (Rotation rotation) => rotation.Type;
}
[Serializable]
public struct Tile : IDirectionable, IFormattable
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public Directions Directions { get; set; }
    //[field: SerializeField] public Rotations 
    public Rotation Rotation { get; set; }
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
    public readonly string ToString(string format, IFormatProvider formatProvider) => $"{Name}, {Directions.ToStringCustom()} roads with {Rotation.Angle.z} degrees rotation.";

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
