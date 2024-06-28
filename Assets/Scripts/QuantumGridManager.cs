using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Grid))]
public class QuantumGridManager : GridManager
{
    public void FillGrid()
    {
        while (collapsedCells < Cells.Count)
            CollapseCell(FindLowestEntropy().Key);
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
