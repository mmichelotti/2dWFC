using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuantumGrid
{
    private readonly Dictionary<Vector2Int, QuantumCell> initialCells;
    private PriorityQueue<Vector2Int> entropyQueue;

    public QuantumGrid(Dictionary<Vector2Int, QuantumCell> initialCells)
    {
        this.initialCells = initialCells;
        entropyQueue = new();
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

    public Dictionary<Directions,QuantumCell> Get(Vector2Int pos, bool haveCollapsed)
    {
        Dictionary<Directions, QuantumCell> neighbours = new();
        foreach (var (dir,off) in DirectionUtility.OrientationOf)
        {
            if (initialCells.TryGetValue(pos + off, out QuantumCell adjacent))
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

    public void ResetState(Vector2Int pos, DirectionsRequired dr)
    {
        foreach (var neighbor in Get(pos, false).Values) neighbor.ReobserveState(dr);
    }
    public void UpdateState(Vector2Int pos)
    {
        foreach (var (dir,cell) in Get(pos, false))
        {
            DirectionsRequired required = new(dir.GetOpposite());

            if (!initialCells[pos].State.Collapsed.HasDirection(dir))
                (required.Required, required.Excluded) = (required.Excluded, required.Required);

            cell.DirectionsRequired = required;
            cell.UpdateState();
        }
    }
    public List<QuantumCell> CollapseCertain(Vector2Int pos)
    {
        List<QuantumCell> certainNeighbours = new();
        foreach (var neighbourCell in Get(pos, false).Values)
        {
            if (neighbourCell.State.Entropy == 0) 
            {
                neighbourCell.CollapseState();
                certainNeighbours.Add(neighbourCell);
            }
        }
        return certainNeighbours;
    }
 
    public DirectionsRequired GetDirectionsRequired(Vector2Int pos)
    {
        DirectionsRequired dirRequired = new();
        foreach (var (dir,cell) in Get(pos, true))
        {
            if(cell.State.Collapsed.Directions.GetOpposite().HasFlag(dir)) dirRequired.Required |= dir;
            else dirRequired.Excluded |= dir;
        }
        return dirRequired;
    }

    public void ClearQueue() => entropyQueue.Clear();

}
