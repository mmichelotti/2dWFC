using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class DirectionUtility
{
    public static IReadOnlyDictionary<Directions2D, Vector2Int> OrientationOf { get; } = new Dictionary<Directions2D, Vector2Int>()
    {
        { Directions2D.Up,    Vector2Int.up },
        { Directions2D.Right, Vector2Int.right },
        { Directions2D.Down,  Vector2Int.down },
        { Directions2D.Left,  Vector2Int.left }
    };

    public static Vector2 DirectionToMatrix(this Directions2D dir) => ((Vector2)dir.GetCompositeOffset() / 2f) + new Vector2(.5f, .5f);

    public static Directions2D GetOpposite(this Directions2D dir) => (Directions2D)((int)dir >> 2 | ((int)dir & 0b0011) << 2);

    public static bool Contains(this Directions2D dir, Directions2D toCheck) => (dir & toCheck) == toCheck;

    public static Vector2Int GetCompositeOffset(this Directions2D compositeDir)
    {
        Vector2Int result = Vector2Int.zero;
        byte bitshifter = 0;
        foreach (var (dir, offset) in OrientationOf)
        {
            //bitwise operation returns 1 if compositeDir has it, 0 if it doesnt
            result += ((byte)(compositeDir & dir) >> bitshifter) * offset;
            bitshifter++;
        }
        return result;
    }

    public static Directions2D GetDirectionTo(this Vector2 origin, Vector2 target) => (Directions2D)
        ( (Convert.ToInt32(target.y - origin.y > 0) * (int)Directions2D.Up)
        | (Convert.ToInt32(target.y - origin.y < 0) * (int)Directions2D.Down)
        | (Convert.ToInt32(target.x - origin.x > 0) * (int)Directions2D.Right)
        | (Convert.ToInt32(target.x - origin.x < 0) * (int)Directions2D.Left));
    public static Directions2D GetDirectionTo(this Vector2Int origin, Vector2Int target) => (Directions2D)
        ( (Convert.ToInt32(target.y - origin.y > 0) * (int)Directions2D.Up)
        | (Convert.ToInt32(target.y - origin.y < 0) * (int)Directions2D.Down)
        | (Convert.ToInt32(target.x - origin.x > 0) * (int)Directions2D.Right)
        | (Convert.ToInt32(target.x - origin.x < 0) * (int)Directions2D.Left));


    public static Directions2D Bitshift(this Directions2D dir, Directions1D shift)
    {
        return shift switch
        {
            Directions1D.Right => (Directions2D)((((int)dir >> 1) | ((int)dir << 3)) & (int)Directions2D.All),
            Directions1D.Left => (Directions2D)((((int)dir << 1) | ((int)dir >> 3)) & (int)Directions2D.All),
            _ => throw new ArgumentException("This shift is not available.")
        };
    }

    private static Directions2D Neutralize(this Directions2D dir)
    {
        int bits = (int)dir;
        int halfmask = (bits >> 2) ^ (bits & 0b0011);
        return (Directions2D)(bits & (halfmask | halfmask << 2));
    }

    public static string ToStringCustom(this Directions2D dir)
    {
        if (dir == Directions2D.None) return "None";
        if (dir == Directions2D.All) return "All";

        var names = Enum.GetValues(typeof(Directions2D))
                        .Cast<Directions2D>()
                        .Where(d => d != Directions2D.None && dir.HasFlag(d))
                        .Select(d => d.ToString());

        return string.Join(", ", names);
    }

}