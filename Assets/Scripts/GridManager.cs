using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Cell prefab;
    private readonly Dictionary<Vector2Int, Cell> cellAtPosition = new();
    [field: SerializeField] public MazeGrid Grid { get; private set; }

    private void Start()
    {
        InitializeCells();
    }

    private void InitializeCells()
    {
        Action<Vector2Int> InitializeCell = (pos) =>
        {
            Cell cell = Instantiate(prefab);
            cell.transform.position = Grid.CoordinateToPosition(pos);
            cell.transform.localScale = Grid.Size;
            cell.Coordinate = pos;
            cellAtPosition.Add(pos, cell);
        };

        InitializeCell.MatrixLoop(Grid.Length);
    }
    private void OnDrawGizmos()
    {
        Action<Vector2Int> DrawLine = (pos) =>
        {
            Vector3 wsPos = Grid.CoordinateToPosition(pos);
            Gizmos.DrawWireCube(wsPos, new Vector3(Grid.Size.x, Grid.Size.y, 0));
        };

        Gizmos.color = Color.black;
        DrawLine.MatrixLoop(Grid.Length);
    }
}