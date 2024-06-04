using System.Collections.Generic;
using UnityEngine;
using System;

[System.Flags]
public enum Directions
{
    None = 0b0000,
    Up = 0b0001,
    Right = 0b0010,
    Down = 0b0100,
    Left = 0b1000,
}
//ho capito cosa sono le record struct ma non si possono usare
//quindi credo di avere capito cosa sono i record
//chiedere ad ale come effettivamente potrebbero essere implementati dei record al posto di classi
public static class DirectionUtility
{
    public static IReadOnlyDictionary<Directions, Vector2Int> OrientationOf { get; } = new Dictionary<Directions, Vector2Int>()
    {
        { Directions.Up,    Vector2Int.up },
        { Directions.Right, Vector2Int.right },
        { Directions.Down,  Vector2Int.down },
        { Directions.Left,  Vector2Int.left }
    };

    //with new Orientation structor there is no need to access a method but just to cast the struct as a quaternion
    public static Vector2 DirectionToMatrix(this Directions dir) => ((Vector2)dir.GetCompositeOffset() / 2f) + new Vector2(.5f, .5f);

    //supponendo che enum directions non venga esteso (cosa che non dovrebbe succedere dato che ora che � un flag copre tutte le possibili direzioni)
    public static Directions GetOpposite(this Directions dir) => (Directions)((int)dir >> 2 | ((int)dir & 0b0011) << 2);

    //implict cast of Orientation


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
        ((Convert.ToInt32(target.y - origin.y > 0) * (int)Directions.Up)
        | (Convert.ToInt32(target.y - origin.y < 0) * (int)Directions.Down)
        | (Convert.ToInt32(target.x - origin.x > 0) * (int)Directions.Right)
        | (Convert.ToInt32(target.x - origin.x < 0) * (int)Directions.Left));

    public static Directions GetDirectionTo(this Vector2Int origin, Vector2Int target) => (Directions)
        ((Convert.ToInt32(target.y - origin.y > 0) * (int)Directions.Up)
        | (Convert.ToInt32(target.y - origin.y < 0) * (int)Directions.Down)
        | (Convert.ToInt32(target.x - origin.x > 0) * (int)Directions.Right)
        | (Convert.ToInt32(target.x - origin.x < 0) * (int)Directions.Left));

    public static Directions Bitshift(this Directions dir) => (Directions)((((int)dir << 1) | ((int)dir >> 3)) & 0b1111);
    private static Directions Neutralize(this Directions dir)
    {
        int bits = (int)dir;
        int halfmask = (bits >> 2) ^ (bits & 0b0011);
        return (Directions)(bits & (halfmask | halfmask << 2));
    }

}