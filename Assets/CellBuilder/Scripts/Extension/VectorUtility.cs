using System;
using UnityEngine;
using static IntegerUtility;

public static class VectorUtility
{
    public static int CompareTo(this Vector2Int a, Vector2Int b)
    {
        int result = a.x.CompareTo(b.x);
        if (result == 0)
        {
            result = a.y.CompareTo(b.y);
        }
        return result;
    }

    public static void MatrixLoop(this Action<Vector2Int> action, int length)
    {
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < length; y++)
            {
                action(new Vector2Int(x, y));
            }
        }
    }
    public static void MatrixLoop(this Action<Vector2Int> action, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                action(new Vector2Int(x, y));
            }
        }
    }

    public static Vector2Int RandomVector(Vector2Int min, Vector2Int max) => new
        (UnityEngine.Random.Range(min.x, max.x),
         UnityEngine.Random.Range(min.y, max.y));

    public static Vector2Int RandomVector(Vector2Int max) => new
        (PositiveRandom(max.x),
         PositiveRandom(max.y));

    public static Vector2Int RandomVector(int max) => new
        (PositiveRandom(max),
         PositiveRandom(max));
}
