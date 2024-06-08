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

/// <summary>
/// Core of the Direction concepts, it holds many utility methods and it's copy pasted and revisisted from my previous project
/// </summary>
public static class DirectionUtility
{
    /// <summary>
    /// Bind directions to offset
    /// </summary>
    public static IReadOnlyDictionary<Directions, Vector2Int> OrientationOf { get; } = new Dictionary<Directions, Vector2Int>()
    {
        { Directions.Up,    Vector2Int.up },
        { Directions.Right, Vector2Int.right },
        { Directions.Down,  Vector2Int.down },
        { Directions.Left,  Vector2Int.left }
    };

    /// <summary>
    /// Convert a direction into a matrix space
    /// (for example the center of a Vector2 is 0,0 but for a matrix that starts from the bottom left is 0.5, 0.5)
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static Vector2 DirectionToMatrix(this Directions dir) => ((Vector2)dir.GetCompositeOffset() / 2f) + new Vector2(.5f, .5f);

    /// <summary>
    /// Gets the opposite ofa given direction 
    /// (for exmaple given right returns left)
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static Directions GetOpposite(this Directions dir) => (Directions)((int)dir >> 2 | ((int)dir & 0b0011) << 2);

    /// <summary>
    /// Returns if a direction contains another direction 
    /// (for example true if you confront top left with left)
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="toCheck"></param>
    /// <returns></returns>
    public static bool Contains(this Directions dir, Directions toCheck) => (dir & toCheck) == toCheck;

    /// <summary>
    /// Given a direction, returns its offset in Vector2Int (for example, given top right, it returns 1,1)
    /// </summary>
    /// <param name="compositeDir"></param>
    /// <returns></returns>
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


    /// <summary>
    /// Given 2 vectors, it returns their relative direction
    /// (for example if you compare 0,0 with 10,10, it returns top right)
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Directions GetDirectionTo(this Vector2 origin, Vector2 target) => (Directions)
        ( (Convert.ToInt32(target.y - origin.y > 0) * (int)Directions.Up)
        | (Convert.ToInt32(target.y - origin.y < 0) * (int)Directions.Down)
        | (Convert.ToInt32(target.x - origin.x > 0) * (int)Directions.Right)
        | (Convert.ToInt32(target.x - origin.x < 0) * (int)Directions.Left));

    /// <summary>
    /// Rotate clockwise a direction
    /// (for example, if you give up, it returns right)
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static Directions Bitshift(this Directions dir) => (Directions)((((int)dir >> 1) | ((int)dir << 3)) & (int)Directions.All);

    /// <summary>
    /// Neutralize a direction
    /// (for example if you have top down, it returns none)
    /// (for example if you have top right, it returns top right since there is nothing to neutralize)
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private static Directions Neutralize(this Directions dir)
    {
        int bits = (int)dir;
        int halfmask = (bits >> 2) ^ (bits & 0b0011);
        return (Directions)(bits & (halfmask | halfmask << 2));
    }

    /// <summary>
    /// Just custom String formatting since I had problem debugging it normally
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
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