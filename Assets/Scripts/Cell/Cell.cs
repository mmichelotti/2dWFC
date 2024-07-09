using UnityEngine;
public abstract class Cell : MonoBehaviour, IPositionable<Vector2Int>, IRequirable
{
    public Vector2Int Coordinate { get; set; }
    public DirectionsRequired DirectionsRequired { get; set; } = new();
    public abstract bool HasDirection(Directions dir);
    public void Constrain(DirectionsRequired dr) => DirectionsRequired = dr;
}