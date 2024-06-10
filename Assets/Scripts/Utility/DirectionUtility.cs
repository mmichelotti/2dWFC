using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Flags]
public enum Direction 
{
    None = 0b0000,
    Up = 0b0001,
    Right = 0b0010,
    Down = 0b0100,
    Left = 0b1000,
    All = ~None
}

public static class DirectionUtility
{
    public static IReadOnlyDictionary<Direction, Vector2Int> OrientationOf { get; } = new Dictionary<Direction, Vector2Int>()
    {
        { Direction.Up,    Vector2Int.up },
        { Direction.Right, Vector2Int.right },
        { Direction.Down,  Vector2Int.down },
        { Direction.Left,  Vector2Int.left }
    };

    #region operators
    public static Direction Plus(this Direction dir, Direction toAdd) => dir | toAdd;
    public static Direction PlusEqual(this ref Direction dir, Direction toAdd) => dir |= toAdd;
    public static Direction Minus(this Direction dir, Direction toRemove) => dir & ~toRemove;
    public static Direction MinusEqual(this ref Direction dir, Direction toRemove) => dir &= ~toRemove;
    #endregion

    public static Vector2 DirectionToMatrix(this Direction dir) => ((Vector2)dir.GetCompositeOffset() / 2f) + new Vector2(.5f, .5f);

    public static Direction GetOpposite(this Direction dir) => (Direction)((int)dir >> 2 | ((int)dir & 0b0011) << 2);

    public static bool Contains(this Direction dir, Direction toCheck) => (dir & toCheck) == toCheck;

    public static Vector2Int GetCompositeOffset(this Direction compositeDir)
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

    public static Direction GetDirectionTo(this Vector2 origin, Vector2 target) => (Direction)
        ( (Convert.ToInt32(target.y - origin.y > 0) * (int)Direction.Up)
        | (Convert.ToInt32(target.y - origin.y < 0) * (int)Direction.Down)
        | (Convert.ToInt32(target.x - origin.x > 0) * (int)Direction.Right)
        | (Convert.ToInt32(target.x - origin.x < 0) * (int)Direction.Left));
    public static Direction GetDirectionTo(this Vector2Int origin, Vector2Int target) => (Direction)
        ( (Convert.ToInt32(target.y - origin.y > 0) * (int)Direction.Up)
        | (Convert.ToInt32(target.y - origin.y < 0) * (int)Direction.Down)
        | (Convert.ToInt32(target.x - origin.x > 0) * (int)Direction.Right)
        | (Convert.ToInt32(target.x - origin.x < 0) * (int)Direction.Left));


    public static Direction Bitshift(this Direction dir) => (Direction)((((int)dir >> 1) | ((int)dir << 3)) & (int)Direction.All);

    private static Direction Neutralize(this Direction dir)
    {
        int bits = (int)dir;
        int halfmask = (bits >> 2) ^ (bits & 0b0011);
        return (Direction)(bits & (halfmask | halfmask << 2));
    }

    public static string ToStringCustom(this Direction dir)
    {
        if (dir == Direction.None) return "None";
        if (dir == Direction.All) return "All";

        var names = Enum.GetValues(typeof(Direction))
                        .Cast<Direction>()
                        .Where(d => d != Direction.None && dir.HasFlag(d))
                        .Select(d => d.ToString());

        return string.Join(", ", names);
    }

}