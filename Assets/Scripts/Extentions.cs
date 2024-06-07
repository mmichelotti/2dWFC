using System;
using UnityEngine;

public static class Extensions
{
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
    public static Vector2Int RandomVector(Vector2Int min, Vector2Int max) => new
        (UnityEngine.Random.Range(min.x, max.x),
         UnityEngine.Random.Range(min.y, max.y));

    public static Vector2Int RandomVector(Vector2Int max) => new
        (UnityEngine.Random.Range(0, max.x),
         UnityEngine.Random.Range(0, max.y));
}