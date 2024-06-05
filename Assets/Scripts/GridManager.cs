using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Cell prefab;

    private readonly Dictionary<Vector2Int, Cell> tileAtPosition = new();
    [field: SerializeField] public MazeGrid Grid { get; private set; }

    private void Start()
    {
        Instantiate(prefab).DebugStatus();

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        for (int x = 0; x < Grid.Length; x++)
        {
            for (int y = 0; y < Grid.Length; y++)
            {
                Vector3 pos = Grid.CoordinateToPosition(new Vector2Int(x, y));
                Gizmos.DrawWireCube(pos, new Vector3(Grid.Size.x, Grid.Size.y, 0));
            }
        }
    }
}