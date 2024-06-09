using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DirectionUtility;

/// <summary>
/// Class responsible for instantiating cells within a grid
/// </summary>
public class GridManager : MonoBehaviour
{
    [SerializeField] private Cell prefab;
    private static readonly Dictionary<Vector2Int, Cell> cellAtPosition = new();
    private static Neighbour Neighbours { get; } = new(cellAtPosition);
    [field: SerializeField] public MazeGrid Grid { get; private set; }

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
        StartCoroutine(SetCellsCoroutine(.01f));
    }

    private IEnumerator SetCellsCoroutine(float time)
    {
        Vector2Int nextPos = Grid.RandomCoordinate;
        while (true)
        {
            if (cellAtPosition[nextPos].State.IsEntangled) yield break;
            SetCell(nextPos);
            Neighbours.UpdateNeighboursState(nextPos);
            yield return new WaitForSeconds(time);
            nextPos = LowestEntropy;
        }
    }

    private void SetCell(Vector2Int pos)
    {
        Cell currentCell = cellAtPosition[pos];
        (Directions requiredDirections, Directions excludedDirections) = (Directions.None, Directions.None);


        List<Cell> entangledNeighbours = Neighbours.GetNeighbours(pos, true);
        foreach (var cell in entangledNeighbours)
        {
            Directions dir = cell.Direction.GetOpposite();
            if (cell.HasDirection(dir)) requiredDirections.PlusEqual(dir);
            else excludedDirections.PlusEqual(dir);

        }

        foreach (var (dir, off) in OrientationOf)
        {
            if (!Grid.IsWithinGrid(pos + off)) excludedDirections.PlusEqual(dir);
        }

        currentCell.UpdateState(requiredDirections, excludedDirections);
        currentCell.Set(currentCell.State.Entangle());
        currentCell.DebugStatus();
    }

    private void InitializeCell(Vector2Int pos, Transform parent)
    {
        Cell cell = Instantiate(prefab, parent);
        cell.transform.position = Grid.CoordinateToPosition(pos);
        cell.transform.localScale = (Vector2)Grid.Size;
        cell.Coordinate = pos;
        cellAtPosition.Add(pos, cell);
    }

    private void InitializeCells()
    {
        GameObject group = new("Cells");
        Action<Vector2Int> action = pos => InitializeCell(pos, group.transform);
        action.MatrixLoop(Grid.Length);
    }

    private void DrawLine(Vector2Int pos)
    {
        Vector3 wsPos = Grid.CoordinateToPosition(pos);
        Gizmos.DrawWireCube(wsPos, new Vector3(Grid.Size.x, Grid.Size.y, 0));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Action<Vector2Int> action = pos => DrawLine(pos);
        action.MatrixLoop(Grid.Length);
    }
}
