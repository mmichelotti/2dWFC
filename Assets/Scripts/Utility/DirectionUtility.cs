using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Flags]
public enum Directions 
{
    None = 0b0000,
    Up = 0b0001,
    Right = 0b0010,
    Down = 0b0100,
    Left = 0b1000,
    All = ~None
}


//0001
//0010
//0100
//1000
//
public enum Shift
{
    Right,
    Left
}
public struct DirectionsRequired : IFormattable
{
    public Directions Required { get; set; }
    public Directions Excluded { get; set; }

    public DirectionsRequired(Directions onlyPositive) => (Required, Excluded) = (onlyPositive, default);
    public DirectionsRequired(Directions required, Directions excluded) => (Required, Excluded) = (required, excluded);

    //is it correct to assign data to immutable objects
    //since it is a struct and not a class?ù
    //i feel like this is wrong, this is supposed to be a Class if i want to handle the data like that
    public void Flip() => (Required, Excluded) = (Excluded, Required);
    public DirectionsRequired Exclude(Directions dir) => new(Required, Excluded | dir);
    public DirectionsRequired Include(Directions dir) => new(dir | Required, Excluded);

    public string ToString(string str, IFormatProvider format) => $"Required: {Required.ToStringCustom()}, Excluded {Excluded.ToStringCustom()}";

}

public static class DirectionUtility
{
    public static IReadOnlyDictionary<Directions, Vector2Int> OrientationOf { get; } = new Dictionary<Directions, Vector2Int>()
    {
        { Directions.Up,    Vector2Int.up },
        { Directions.Right, Vector2Int.right },
        { Directions.Down,  Vector2Int.down },
        { Directions.Left,  Vector2Int.left }
    };

    #region operators
    public static Directions Plus(this Directions dir, Directions toAdd) => dir | toAdd;
    public static Directions PlusEqual(this ref Directions dir, Directions toAdd) => dir |= toAdd;
    public static Directions Minus(this Directions dir, Directions toRemove) => dir & ~toRemove;
    public static Directions MinusEqual(this ref Directions dir, Directions toRemove) => dir &= ~toRemove;
    #endregion

    public static Vector2 DirectionToMatrix(this Directions dir) => ((Vector2)dir.GetCompositeOffset() / 2f) + new Vector2(.5f, .5f);

    public static Directions GetOpposite(this Directions dir) => (Directions)((int)dir >> 2 | ((int)dir & 0b0011) << 2);

    public static bool Contains(this Directions dir, Directions toCheck) => (dir & toCheck) == toCheck;

    public static Vector2Int GetCompositeOffset(this Directions compositeDir)
    {
        Vector2Int result = Vector2Int.zero;
        int bitshifter = 0;
        foreach (var (dir, offset) in OrientationOf)
        {
            //bitwise operation returns 1 if compositeDir has it, 0 if it doesnt
            result += ((int)(compositeDir & dir) >> bitshifter) * offset;
            bitshifter++;
        }
        return result;
    }

    public static Directions GetDirectionTo(this Vector2 origin, Vector2 target) => (Directions)
        ( (Convert.ToInt32(target.y - origin.y > 0) * (int)Directions.Up)
        | (Convert.ToInt32(target.y - origin.y < 0) * (int)Directions.Down)
        | (Convert.ToInt32(target.x - origin.x > 0) * (int)Directions.Right)
        | (Convert.ToInt32(target.x - origin.x < 0) * (int)Directions.Left));
    public static Directions GetDirectionTo(this Vector2Int origin, Vector2Int target) => (Directions)
        ( (Convert.ToInt32(target.y - origin.y > 0) * (int)Directions.Up)
        | (Convert.ToInt32(target.y - origin.y < 0) * (int)Directions.Down)
        | (Convert.ToInt32(target.x - origin.x > 0) * (int)Directions.Right)
        | (Convert.ToInt32(target.x - origin.x < 0) * (int)Directions.Left));


    public static Directions Bitshift(this Directions dir, Shift shift)
    {
        return shift switch
        {
            Shift.Right => (Directions)((((int)dir >> 1) | ((int)dir << 3)) & (int)Directions.All),
            Shift.Left => (Directions)((((int)dir << 1) | ((int)dir >> 3)) & (int)Directions.All),
            _ => throw new ArgumentException("This shift is not available.")
        };
    }

    private static Directions Neutralize(this Directions dir)
    {
        int bits = (int)dir;
        int halfmask = (bits >> 2) ^ (bits & 0b0011);
        return (Directions)(bits & (halfmask | halfmask << 2));
    }

    public static string ToStringCustom(this Directions dir)
    {
        if (dir == Directions.None) return "None";
        if (dir == Directions.All) return "All";

        var names = Enum.GetValues(typeof(Directions))
                        .Cast<Directions>()
                        .Where(d => d != Directions.None && dir.HasFlag(d))
                        .Select(d => d.ToString());

        return string.Join(", ", names);
    }

}