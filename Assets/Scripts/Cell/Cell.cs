using UnityEngine;
using UnityEngine.Events;
public abstract class Cell : MonoBehaviour, IPositionable<Vector2Int>, IRequirable
{
    public Vector2Int Coordinate { get; set; }
    public DirectionsRequired DirectionsRequired { get; set; } = new();
    public void Constrain(DirectionsRequired dr) => DirectionsRequired = dr;
    public CellGrid CellGrid { get; private set; }

    public static Cell Create(Cell original, Vector2Int pos, CellGrid cellGrid, Transform parent = null) 
    {
        Cell cell = Instantiate(original, parent);
        cell.Initialize(pos, cellGrid);
        return cell;
    }
    private void Initialize(Vector2Int pos, CellGrid cellGrid)
    {
        Coordinate = pos;
        CellGrid = cellGrid;
        transform.position = cellGrid.Grid.CoordinateToPosition(pos);
        transform.localScale = new Vector2(cellGrid.Grid.Area, cellGrid.Grid.Area);
    }
}
