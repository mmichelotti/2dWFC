using System.Collections.Generic;
using UnityEngine;
using static DirectionUtility;

public class Neighbour
{
    private readonly Dictionary<Vector2Int, Cell> initialCells;
    public Neighbour(Dictionary<Vector2Int, Cell> initialCells) => this.initialCells = initialCells;

    public List<Cell> GetNeighbours(Vector2Int pos, bool areEntangled)
    {
        List<Cell> neighbours = new();
        foreach (var off in OrientationOf.Values)
        {
            if (initialCells.TryGetValue(pos + off, out Cell adjacent))
            {
                if (adjacent.State.IsEntangled == areEntangled) neighbours.Add(adjacent);

            }
        }
        return neighbours;
    }
    
    public void UpdateNeighboursState(Vector2Int pos)
    {
        foreach (var neighborCell in GetNeighbours(pos, false))
        {
            Directions direction = pos.GetDirectionTo(neighborCell.Coordinate);
            (Directions required, Directions excluded) = (direction.GetOpposite(), Directions.None);

            if (!initialCells[pos].HasDirection(direction)) (required, excluded) = (excluded, required);

            neighborCell.UpdateState(required, excluded);
        }
    }
}
