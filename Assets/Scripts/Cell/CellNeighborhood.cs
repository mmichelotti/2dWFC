using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellNeighborhood
{
    private readonly Dictionary<Vector2Int, Cell> initialCells;
    private readonly Dictionary<(Vector2Int, Vector2Int), Directions> directionCache;
    private PriorityQueue<Vector2Int> entropyQueue;

    public CellNeighborhood(Dictionary<Vector2Int, Cell> initialCells)
    {
        this.initialCells = initialCells;
        entropyQueue = new();
        directionCache = new();
    }

    public Vector2Int LowestEntropy
    {
        get
        {
            while (entropyQueue.Count > 0)
            {
                var lowestEntropyPos = entropyQueue.Dequeue();
                if (!initialCells[lowestEntropyPos].State.HasCollapsed)
                {
                    return lowestEntropyPos;
                }
            }
            return Vector2Int.zero;
        }
    }

    public Dictionary<Directions,Cell> Get(Vector2Int pos, bool haveCollapsed)
    {
        Dictionary<Directions, Cell> neighbours = new();
        foreach (var (dir,off) in DirectionUtility.OrientationOf)
        {
            if (initialCells.TryGetValue(pos + off, out Cell adjacent))
            {
                if (adjacent.State.HasCollapsed == haveCollapsed) neighbours.Add(dir, adjacent);
            }
        }
        return neighbours;
    }

    public void UpdateEntropy(Vector2Int pos)
    {
        foreach (var neighbor in Get(pos, false).Values)
        {
            entropyQueue.Enqueue(neighbor.Coordinate, neighbor.State.Entropy);
        }
    }

    public void UpdateState(Vector2Int pos)
    {
        foreach (var neighborCell in Get(pos, false).Values)
        {
            Directions dir = GetDirection(pos, neighborCell.Coordinate);
            DirectionsRequired required = new(dir.GetOpposite());

            if (!initialCells[pos].HasDirection(dir)) required.Flip();

            neighborCell.DirectionsRequired = required;
            neighborCell.UpdateState();
        }
        UpdateEntropy(pos);
    }
    public List<Cell> CollapseCertain(Vector2Int pos)
    {
        List<Cell> certainNeighbours = new();
        foreach (var neighbourCell in Get(pos, false).Values)
        {
            if (neighbourCell.State.Entropy == 0 && !neighbourCell.State.HasCollapsed) 
            {
                neighbourCell.CollapseState();
                certainNeighbours.Add(neighbourCell);
            }
        }
        return certainNeighbours;
    }


    private Directions GetDirection(Vector2Int from, Vector2Int to)
    {
        if (!directionCache.TryGetValue((from, to), out var direction))
        {
            direction = from.GetDirectionTo(to);
            directionCache[(from, to)] = direction;
        }
        return direction;
    }

    public DirectionsRequired GetDirectionsRequired(Vector2Int pos)
    {
        DirectionsRequired dirRequired = new();
        foreach (var (dir,cell) in Get(pos, true))
        {
            if(cell.Directions.GetOpposite().HasFlag(dir)) dirRequired.Required |= dir;
            else dirRequired.Excluded |= dir;
        }
        return dirRequired;
    }
    public void ClearQueue() => entropyQueue = new PriorityQueue<Vector2Int>();
}
