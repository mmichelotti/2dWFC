using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuantumNeighborhood<T, T2> where T : IQuantizable<T2>, IPositionable<Vector2Int>, IDirectionable, IRequirable
{
    private readonly Dictionary<Vector2Int, T> initialCells;
    private readonly SortedSet<(float Entropy, Vector2Int Position)> entropyQueue;

    public QuantumNeighborhood(Dictionary<Vector2Int, T> initialCells)
    {
        this.initialCells = initialCells;
        entropyQueue = new SortedSet<(float Entropy, Vector2Int Position)>(new EntropyComparer());
    }
    public Vector2Int LowestEntropy
    {
        get
        {
            while (entropyQueue.Count > 0)
            {
                var lowestEntropyPos = entropyQueue.Min;
                entropyQueue.Remove(lowestEntropyPos);
                if (!initialCells[lowestEntropyPos.Position].State.IsEntangled)
                {
                    Enqueue(lowestEntropyPos.Position);
                    return lowestEntropyPos.Position;
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
        List<T> neighboursList = Get(pos, false);
        foreach (var neighbour in neighboursList)
        {
            if (!neighbour.State.IsEntangled)
            {
                entropyQueue.Add((neighbour.State.Entropy, neighbour.Coordinate));
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
    public void ClearQueue() => entropyQueue.Clear();
}
