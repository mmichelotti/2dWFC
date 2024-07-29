using System;
using UnityEngine;

public class Grid : MonoBehaviour
{
    #region properties
    [field: SerializeField] public Vector2Int Size { get; set; }
    [field: SerializeField] public int Area { get; set; }
    public int Count => Size.x * Size.y;
    public Vector2Int RandomCoordinate => VectorUtility.RandomVector(Size);
    private Vector2 LocalPosition => transform.localPosition;
    #endregion

    #region methods
    public Vector2Int GetCoordinatesAt(Directions2D dir)
    {
        Vector2 vector = dir.DirectionToMatrix() * new Vector2(Size.x - 1, Size.y - 1);
        return new Vector2Int((int)vector.x, (int)vector.y);
    }

    public Directions2D Boundaries(Vector2Int pos)
    {
        Directions2D exclude = default;
        foreach (var (dir, off) in DirectionUtility.OrientationOf)
        {
            if (!IsWithinGrid(pos + off)) exclude |= dir;
        }
        return exclude;
    }

    public Vector3 CoordinateToPosition(Vector2Int pos) =>
        new Vector3(GetHalfPoint(Area, pos.x, Size.x) + LocalPosition.x, GetHalfPoint(Area, pos.y, Size.y) + LocalPosition.y);

    public Vector2Int ToGridCoordinate(Vector3 wsPos)
    {
        int x = Mathf.FloorToInt((wsPos.x + Area / 2) / Area) + Size.x / 2;
        int y = Mathf.FloorToInt((wsPos.y + Area / 2) / Area) + Size.y / 2;
        return new Vector2Int(x, y);
    }

    private float GetHalfPoint(float tileDimension, int gridIndex, int gridSize) => tileDimension * (gridIndex - gridSize / 2);

    public bool IsWithinGrid(Vector2Int pos) => pos.x >= 0 && pos.y >= 0 && pos.x < Size.x && pos.y < Size.y;
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
        action.MatrixLoop(Size);
    }
    #endregion
}
