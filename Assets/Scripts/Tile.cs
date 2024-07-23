using System.Collections.Generic;
using System;
using UnityEngine;

public enum RotationOption
{
    Default = 1,
    OneStep = 2,
    All = 4
}

[Serializable]
public struct Tile : IDirectionable, IFormattable, IProbable
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField, Range(0, 1)] public float Probability { get; set; }
    [field: SerializeField] public RotationOption PossibleRotations { get; set; }
    [field: SerializeField] public Directions Directions { get; set; }
    [field: SerializeField] public List<Directions> Connections { get; set; }
    public Vector3 Rotation { get; set; }

    public Tile(Tile tile) => 
        (Name, Sprite, Directions, Rotation, PossibleRotations, Probability, Connections) = 
        (tile.Name, tile.Sprite, tile.Directions, tile.Rotation, tile.PossibleRotations, tile.Probability, tile.Connections);

    private readonly static HashSet<Tile> calculatingConfigurations = new();
    public List<Tile> AllConfigurations
    {
        get
        {
            if (calculatingConfigurations.Contains(this)) return new List<Tile>();

            calculatingConfigurations.Add(this);

            List<Tile> allDirections = new();
            Tile toRotate = new(this);

            for (int i = 0; i < (int)PossibleRotations; i++)
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

        for (int i = 0; i < Connections.Count; i++)
            Connections[i] = Connections[i].Bitshift(Shift.Right);

        Rotation += new Vector3(0, 0, 90);
    }

    public readonly bool HasDirection(Directions dir) => Directions.HasFlag(dir);
    public readonly string ToString(string format, IFormatProvider formatProvider) => $"{Name}, {Directions.ToStringCustom()} roads with {Rotation.z} degrees rotation.";

}
