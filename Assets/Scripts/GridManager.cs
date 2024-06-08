using System;
using System.Collections.Generic;
using UnityEngine;
using static Extensions;
using static DirectionUtility;
using Unity.VisualScripting;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Cell prefab;
    private readonly Dictionary<Vector2Int, Cell> cellAtPosition = new();

    [field: SerializeField] public MazeGrid Grid { get; private set; }
    private Vector2Int RandomCell => RandomVector(Grid.Size);
    private Vector2Int LowestEntropy
    {
        get
        {
            (Vector2Int Position, float Entropy) lowestEntropy = (new(), float.PositiveInfinity);

            foreach (var (pos, cell) in cellAtPosition)
            {
                if (cell.State.IsEntangled) continue;
                if (cell.State.Entropy < lowestEntropy.Entropy) lowestEntropy = (pos, cell.State.Entropy);

            }
            return lowestEntropy.Position;
        }
    }


    private void Start()
    {
        InitializeCells();
        SetCell(RandomCell);
    }


    private void SetCell(Vector2Int pos)
    {
        Directions toRespect = Directions.None;
        foreach (var (dir, off) in OrientationOf)
        {
            if (cellAtPosition.TryGetValue(pos + off, out Cell adjacent))
            {
                if (adjacent.State.IsEntangled)
                {
                    toRespect |= adjacent.CurrentTile.Directions & dir.GetOpposite();
                }
                else
                {
                    if (UnityEngine.Random.value >= .5f)
                    {
                        toRespect |= dir;
                    }
                }
            }
        }
        cellAtPosition[pos].UpdateState(toRespect);
        cellAtPosition[pos].Entangle();
        Vector2Int nextPos = LowestEntropy;
        if (cellAtPosition[nextPos].State.IsEntangled) return;
        SetCell(nextPos);
    }


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
        cell.transform.localScale = (Vector2)Grid.Size;
        cell.Coordinate = pos;
        cellAtPosition.Add(pos, cell);
    }

    private void DrawLine(Vector2Int pos)
    {
        Vector3 wsPos = Grid.CoordinateToPosition(pos);
        Gizmos.DrawWireCube(wsPos, new Vector3(Grid.Size.x, Grid.Size.y, 0));
    }
}