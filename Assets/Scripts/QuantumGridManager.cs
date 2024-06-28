using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Grid))]
public class QuantumGridManager : GridManager
{
    public void FillGrid()
    {
        Vector2Int nextPos = FindLowestEntropy().Key;
        int internalCounter = 0;
        while (spawnedCells < Cells.Count)
        {
            CollapseCell(nextPos);
            nextPos = FindLowestEntropy().Key;
            internalCounter++;
        }
    }

    /// <summary>
    ///! Very bad code. An urge arises to use PriorityQueue to save the day.
    /// </summary>
    /// <returns></returns>
    KeyValuePair<Vector2Int, Cell> FindLowestEntropy() =>
        // TODO MAKE IT MORE PERFORMANT
        //! Very bad
        Cells
            .OrderBy((cellInfo) => cellInfo.Value.GetComponent<CellBehaviour>().State.Entropy)
            .First();

    void UpdateNeighborhood() { }
}
