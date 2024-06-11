using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuantumNeighborhood<T, T2> where T : IQuantizable<T2>, IPositionable<Vector2Int>, IDirectionable, IRequirable
{
    private readonly Dictionary<Vector2Int, T> initialCells;
    private PriorityQueue<Vector2Int> entropyQueue;

    public QuantumNeighborhood(Dictionary<Vector2Int, T> initialCells)
    {
        this.initialCells = initialCells;
        entropyQueue = new PriorityQueue<Vector2Int>();
    }

    public Vector2Int LowestEntropy
    {
        get
        {
            while (entropyQueue.Count > 0)
            {
                var lowestEntropyPos = entropyQueue.Dequeue();
                if (!initialCells[lowestEntropyPos].State.IsEntangled)
                {
                    Enqueue(lowestEntropyPos);
                    return lowestEntropyPos;
                }
            }
            return Vector2Int.zero;
        }
    }

    public List<T> Get(Vector2Int pos, bool areEntangled)
    {
        List<T> neighbours = new();
        foreach (var off in DirectionUtility.OrientationOf.Values)
        {
            if (initialCells.TryGetValue(pos + off, out T adjacent))
            {
                if (adjacent.State.IsEntangled == areEntangled) neighbours.Add(adjacent);
            }
        }
        return neighbours;
    }

    public void Enqueue(Vector2Int pos)
    {
        List<T> neighborsList = Get(pos, false);
        foreach (var neighbor in neighborsList)
        {
            if (!neighbor.State.IsEntangled)
            {
                entropyQueue.Enqueue(neighbor.Coordinate, neighbor.State.Entropy);
            }
        }
    }

    public void UpdateState(Vector2Int pos)
    {
        foreach (var neighborCell in Get(pos, false))
        {
            Directions dir = pos.GetDirectionTo(neighborCell.Coordinate);
            DirectionsRequired required = new(dir.GetOpposite());

            if (!initialCells[pos].HasDirection(dir)) required.Flip();

            neighborCell.DirectionsRequired = required;
            neighborCell.UpdateState();
        }
    }

    public DirectionsRequired GetDirectionsRequired(Vector2Int pos)
    {
        DirectionsRequired dirRequired = new();

        List<T> entangledNeighbours = Get(pos, true);
        foreach (var cell in entangledNeighbours)
        {
            Directions dir = cell.Directions.GetOpposite();
            if (cell.HasDirection(dir)) dirRequired.Required |= dir;
            else dirRequired.Excluded |= dir;
        }
        return dirRequired;
    }

    public void ClearQueue() => entropyQueue = new PriorityQueue<Vector2Int>();
}
