using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Extensions;
using static DirectionUtility;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Cell prefab;
    [SerializeField] private MazeGrid grid;
    [SerializeField] private Directions startingPoint;

    private static readonly Dictionary<Vector2Int, Cell> cellAtPosition = new();
    private static QuantumNeighbour<Cell,Tile> Neighbours { get; } = new(cellAtPosition);

    private bool allCellsEntangled;
    private Vector2Int nextPos;

    private Vector2Int LowestEntropy
    {
        get
        {
            List<(Vector2Int Position, float Entropy)> lowestEntropyCells = new();
            float lowestEntropyValue = float.PositiveInfinity;

            foreach (var (pos, cell) in cellAtPosition)
            {
                if (cell.State.IsEntangled) continue;

                if (cell.State.Entropy < lowestEntropyValue)
                {
                    lowestEntropyCells.Clear();
                    lowestEntropyCells.Add((pos, cell.State.Entropy));
                    lowestEntropyValue = cell.State.Entropy;
                }
                else if (cell.State.Entropy == lowestEntropyValue)
                {
                    lowestEntropyCells.Add((pos, cell.State.Entropy));
                }
            }

            return (lowestEntropyCells.Count != 0) ? lowestEntropyCells[PositiveRandom(lowestEntropyCells.Count)].Position : Vector2Int.zero;
        }
    }

    private void Start()
    {
        InitializeCells();
        nextPos = grid.GetCoordinatesAt(startingPoint);
        allCellsEntangled = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) ResetGrid();
        if (!allCellsEntangled) SetCells();
    }

    private void SetCells()
    {
        if (cellAtPosition[nextPos].State.IsEntangled)
        {
            allCellsEntangled = true;
            return;
        }
        SetCell(nextPos);
        nextPos = LowestEntropy;     
    }

    private void SetCell(Vector2Int pos)
    {
        (Directions required, Directions excluded) = (Directions.None, Directions.None);

        List<Cell> entangledNeighbours = Neighbours.GetNeighbours(pos, true);
        foreach (var cell in entangledNeighbours)
        {
            Directions dir = cell.Directions.GetOpposite();
            if (cell.HasDirection(dir)) required.PlusEqual(dir);
            else excluded.PlusEqual(dir);

        }

        foreach (var (dir, off) in OrientationOf)
        {
            if (!grid.IsWithinGrid(pos + off)) excluded.PlusEqual(dir);
        }

        Cell currentCell = cellAtPosition[pos];
        currentCell.DirectionsRequired = new(required, excluded);
        currentCell.UpdateState();
        currentCell.EntangleState();
        currentCell.Instantiate();
        currentCell.DebugState();
        Neighbours.UpdateNeighboursState(pos);
    }

    private void InitializeCell(Vector2Int pos, Transform parent)
    {
        Cell cell = Instantiate(prefab, parent);
        cell.transform.position = grid.CoordinateToPosition(pos);
        cell.transform.localScale = (Vector2)grid.Size;
        cell.Coordinate = pos;
        cellAtPosition.Add(pos, cell);
    }

    private void InitializeCells()
    {
        GameObject group = new("Cells");
        Action<Vector2Int> action = pos => InitializeCell(pos, group.transform);
        action.MatrixLoop(grid.Length);
    }

    private void DrawLine(Vector2Int pos)
    {
        Vector3 wsPos = grid.CoordinateToPosition(pos);
        Gizmos.DrawWireCube(wsPos, new Vector3(grid.Size.x, grid.Size.y, 0));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Action<Vector2Int> action = pos => DrawLine(pos);
        action.MatrixLoop(grid.Length);
    }

    public void ResetGrid()
    {
        // Clear existing cells
        foreach (var cell in cellAtPosition.Values)
        {
            cell.ResetState();
        }
        nextPos = grid.GetCoordinatesAt(startingPoint);
        allCellsEntangled = false;
    }
}
