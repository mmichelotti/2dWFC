using UnityEngine;
using UnityEngine.Events;
public abstract class Cell : MonoBehaviour, IPositionable<Vector2Int>, IRequirable
{
    public UnityEvent<Vector2Int> OnCellSpawned { get; } = new();
    public UnityEvent<Vector2Int> OnCellRemoved { get; } = new();
    public Vector2Int Coordinate { get; set; }
    public DirectionsRequired DirectionsRequired { get; set; } = new();
    public void Constrain(DirectionsRequired dr) => DirectionsRequired = dr;
    public Cell Initialize(Vector2Int pos, Grid grid, Transform parent = null)
    {
        var cell = Instantiate(this, parent);
        cell.transform.position = grid.CoordinateToPosition(pos);
        cell.transform.localScale = new Vector2(grid.Area, grid.Area);
        cell.Coordinate = pos;
        return cell;
    }
}