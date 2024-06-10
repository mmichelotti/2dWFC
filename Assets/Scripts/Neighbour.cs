using System.Collections.Generic;
using UnityEngine;
using static DirectionUtility;

public class Neighbour<T,T2> where T : IQuantumStatable<T2>, IPositionable<Vector2Int>, IDirectionable
{
    private readonly Dictionary<Vector2Int, T> initialCells;
    public Neighbour(Dictionary<Vector2Int, T> initialCells) => this.initialCells = initialCells;

    public List<T> GetNeighbours(Vector2Int pos, bool areEntangled)
    {
        List<T> neighbours = new();
        foreach (var off in OrientationOf.Values)
        {
            if (initialCells.TryGetValue(pos + off, out T adjacent))
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
            Direction direction = pos.GetDirectionTo(neighborCell.Coordinate);
            (Direction required, Direction excluded) = (direction.GetOpposite(), Direction.None);

            if (!initialCells[pos].HasDirection(direction)) (required, excluded) = (excluded, required);

            (neighborCell.Required, neighborCell.Excluded) = (required, excluded);
            neighborCell.UpdateState();
        }
    }
}
