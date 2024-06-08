using UnityEngine;
using static Extensions;

/// <summary>
/// Just a grid with useful functionality like transforming grid space into world space 
/// Its a scriptable object without any specific reason
/// </summary>
[CreateAssetMenu(fileName = "Grid", menuName = "ScriptableObjects/Grid", order = 1)]
public class MazeGrid : ScriptableObject
{
    #region properties
    [field: SerializeField] public int Length { get; set; } = 11;
    [field: SerializeField] public Vector2Int Size { get; set; } = new(20, 10);
    public Vector2Int RandomCoordinate => RandomVector(Size);
    #endregion

    #region methods
    public Vector2 GetCoordinatesAt(Directions dir) => dir.DirectionToMatrix() * (Length - 1);
    public Vector3 CoordinateToPosition(Vector2Int pos) => new(GetHalfPoint(Size.x, (int)pos.x), GetHalfPoint(Size.y, (int)pos.y));
    private float GetHalfPoint(float tileDimension, int gridIndex) => tileDimension * (gridIndex - Length / 2);
    public bool IsWithinGrid(Vector2Int pos) => pos.x >= 0 && pos.y >= 0 && pos.x < Length && pos.y < Length;
    #endregion
}
