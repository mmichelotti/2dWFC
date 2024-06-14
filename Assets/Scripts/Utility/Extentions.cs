using System;
using UnityEngine;
public static class Extensions
{
    public static T SpawnInGrid<T>(this T prefab, Grid grid, Vector2Int pos, Transform parent = null) where T : MonoBehaviour, IInitializable, IPositionable<Vector2Int>
    {
        T cell = UnityEngine.GameObject.Instantiate(prefab, parent);
        cell.transform.position = grid.CoordinateToPosition(pos);
        cell.transform.localScale = (Vector2)grid.Size;
        cell.Coordinate = pos;
        cell.Init();
        return cell;
    }
    public static T MakeSingleton<T>(this T manager) where T : MonoBehaviour
    {
        T[] instances = UnityEngine.Object.FindObjectsByType<T>(FindObjectsSortMode.None);

        if (instances.Length > 1)
        {
            UnityEngine.Object.Destroy(manager.gameObject);
            return instances[0];
        }
        else
        {
            UnityEngine.Object.DontDestroyOnLoad(manager.gameObject);
            return manager;
        }
    }

    public static T GetManager<T>(this MonoBehaviour manager) where T : Manager
    {
        T[] instances = UnityEngine.Object.FindObjectsByType<T>(FindObjectsSortMode.None);

        if (instances.Length > 0)
        {
            return instances[0];
        }
        else
        {
            Debug.LogError("Manager of type " + typeof(T) + " not found in scene!");
            return null;
        }
    }
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
    public static int PositiveRandom(int max) => UnityEngine.Random.Range(0, max);
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

