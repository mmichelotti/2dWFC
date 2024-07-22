using UnityEngine;
public abstract class Cell : MonoBehaviour, IPositionable<Vector2Int>, IRequirable
{
    public Vector2Int Coordinate { get; set; }
    public DirectionsRequired DirectionsRequired { get; set; } = new();
    public void Constrain(DirectionsRequired dr) => DirectionsRequired = dr;
    public CellGrid CellGrid { get; private set; }
    public void Initialize(Vector2Int pos, CellGrid cellGrid)
    {
        Coordinate = pos;
        CellGrid = cellGrid;
        transform.position = cellGrid.Grid.CoordinateToPosition(pos);
        transform.localScale = new Vector2(cellGrid.Grid.Area, cellGrid.Grid.Area);
    }
}
