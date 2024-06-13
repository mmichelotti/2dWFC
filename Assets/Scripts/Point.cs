using UnityEngine;

public abstract class Point : MonoBehaviour, IPositionable<Vector2Int>, IDirectionable, IRequirable
{
    public Vector2Int Coordinate { get; set; }
    public DirectionsRequired DirectionsRequired { get; set; } = new();
    public Directions Directions { get; set; }
    public abstract bool HasDirection(Directions dir);
}