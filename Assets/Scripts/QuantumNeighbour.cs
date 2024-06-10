using System.Collections.Generic;
using UnityEngine;
using static DirectionUtility;

/// <summary>
/// Useful methods to talk with adjacents neighbours that have a QuantumState.
/// </summary>
/// <typeparam name="T">The type of the container</typeparam>
/// <typeparam name="T2">The type of the content</typeparam>
public class QuantumNeighbour<T,T2> where T : IQuantumStatable<T2>, IPositionable<Vector2Int>, IDirectionable, IDirectionableRequired
{
    private readonly Dictionary<Vector2Int, T> initialCells;
    public QuantumNeighbour(Dictionary<Vector2Int, T> initialCells) => this.initialCells = initialCells;
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
            Directions direction = pos.GetDirectionTo(neighborCell.Coordinate);
            (Directions required, Directions excluded) = (direction.GetOpposite(), Directions.None);

            if (!initialCells[pos].HasDirection(direction)) (required, excluded) = (excluded, required);

            neighborCell.DirectionsRequired.SetRequirements(required, excluded);
            neighborCell.UpdateState();
        }
    }
}
