using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Cell prefab;
    private readonly Dictionary<Vector2Int, Cell> cellAtPosition = new();
    [field: SerializeField] public MazeGrid Grid { get; private set; }
    private Vector2Int LowestEntropy
    {
        get
        {
            (Vector2Int Position, int Entropy) lowestEntropy = (new(), int.MaxValue);

            foreach (var (pos, cell) in cellAtPosition)
            {
                if (cell.Entropy <= lowestEntropy.Entropy)
                {
                    lowestEntropy = (pos, cell.Entropy);
                }
            }
            return lowestEntropy.Position;
        }
    }

    private void Start()
    {
        InitializeCells();
        SetLowestEntropy();
    }

    private void SetLowestEntropy() => cellAtPosition[LowestEntropy].RandomSet();


    private void InitializeCells()
    {
        GameObject group = new("Cells");
        Action<Vector2Int> action = pos => InitializeCell(pos, group.transform);
        action.MatrixLoop(Grid.Length);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Action<Vector2Int> action = pos => DrawLine(pos);
        action.MatrixLoop(Grid.Length);
    }
    private void InitializeCell(Vector2Int pos, Transform parent)
    {
        Cell cell = Instantiate(prefab, parent);
        cell.transform.position = Grid.CoordinateToPosition(pos);
        cell.transform.localScale = Grid.Size;
        cell.Coordinate = pos;
        cellAtPosition.Add(pos, cell);
    }
    private void DrawLine(Vector2Int pos)
    {
        Vector3 wsPos = Grid.CoordinateToPosition(pos);
        Gizmos.DrawWireCube(wsPos, new Vector3(Grid.Size.x, Grid.Size.y, 0));
    }
}