using UnityEngine;
public abstract class Cell : MonoBehaviour, IPositionable<Vector2Int>, IRequirable
{
    Grid Grid => GameManager.Instance.GridManager.Grid;
    public Vector2Int Coordinate { get; set; }
    public DirectionsRequired DirectionsRequired { get; set; } = new();
    public void Constrain(DirectionsRequired dr) => DirectionsRequired = dr;
    public Cell Spawn(Vector2Int pos, Transform parent = null)
    {
        var cell = Instantiate(this, parent);
        cell.transform.position = Grid.CoordinateToPosition(pos);
        cell.transform.localScale = new Vector2(Grid.Area, Grid.Area);
        cell.Coordinate = pos;
        return cell;
    }
}