using System;
using UnityEngine;

public class Grid : MonoBehaviour
{
    #region properties
    [field: SerializeField] public int Length { get; set; }
    [field: SerializeField] public int Area { get; set; }
    public Vector2Int RandomCoordinate => Extensions.RandomVector(Length);
    private Vector2 LocalPosition => transform.localPosition;
    #endregion

    #region methods
    public Vector2Int GetCoordinatesAt(Directions dir)
    {
        Vector2 vector = (dir.DirectionToMatrix() * (Length - 1));
        return new((int)vector.x, (int)vector.y);
    }
    public Directions Boundaries(Vector2Int pos)
    {
        Directions exclude = default;
        foreach (var (dir, off) in DirectionUtility.OrientationOf)
        {
            if (!IsWithinGrid(pos + off)) exclude |= dir;
        }
        return exclude;
    }
    public Vector3 CoordinateToPosition(Vector2Int pos) => new(GetHalfPoint(Area,pos.x) + LocalPosition.x, GetHalfPoint(Area, pos.y) + LocalPosition.y);

    public Vector2Int ToGridCoordinate(Vector3 wsPos)
    {
        int x = Mathf.FloorToInt((wsPos.x + Area / 2) / Area) + Length / 2;
        int y = Mathf.FloorToInt((wsPos.y + Area / 2) / Area) + Length / 2;
        return new(x, y);
    }
    private float GetHalfPoint(float tileDimension, int gridIndex) => tileDimension * (gridIndex - Length / 2);
    public bool IsWithinGrid(Vector2Int pos) => pos.x >= 0 && pos.y >= 0 && pos.x < Length && pos.y < Length;
    #endregion

    #region gizmos
    private void DrawLine(Vector2Int pos)
    {
        Vector3 wsPos = CoordinateToPosition(pos);
        Gizmos.DrawWireCube(wsPos, new Vector3(Area, Area, 0));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Action<Vector2Int> action = pos => DrawLine(pos);
        action.MatrixLoop(Length);
    }
    #endregion
}
