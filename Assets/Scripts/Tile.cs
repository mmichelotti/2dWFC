using System.Collections.Generic;
using System;
using UnityEngine;

public enum RotationOption
{
    None,
    OneStep,
    Full
}

[Serializable]
public struct Tile : IDirectionable, IFormattable
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public Directions Directions { get; set; }
    public Vector3 Rotation { get; set; }

    // New field to control the rotation option
    [field: SerializeField] public RotationOption RotationOption { get; set; }

    public Tile(Tile tile) => (Name, Sprite, Directions, Rotation, RotationOption) = (tile.Name, tile.Sprite, tile.Directions, tile.Rotation, tile.RotationOption);

    private readonly static HashSet<Tile> calculatingConfigurations = new();
    public List<Tile> AllConfigurations
    {
        get
        {
            if (calculatingConfigurations.Contains(this)) return new List<Tile>();

            calculatingConfigurations.Add(this);

            List<Tile> allDirections = new();
            Tile toRotate = new(this);

            switch (RotationOption)
            {
                case RotationOption.None:
                    allDirections.Add(new(toRotate));
                    break;

                case RotationOption.OneStep:
                    allDirections.Add(new(toRotate));
                    toRotate.Rotate();
                    allDirections.Add(new(toRotate));
                    break;

                case RotationOption.Full:
                    for (int i = 0; i < 4; i++)
                    {
                        allDirections.Add(new(toRotate));
                        toRotate.Rotate();
                    }
                    break;
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
    public readonly string ToString(string format, IFormatProvider formatProvider) => $"{Name}, {Directions.ToStringCustom()} roads with {Rotation.z} degrees rotation.";
}
