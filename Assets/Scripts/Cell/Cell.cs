using UnityEngine;

public class Cell : MonoBehaviour, IPositionable<Vector2Int>
{
    [SerializeField]
    GameObject behaviorPrefab;

    [SerializeField]
    GameObject debuggerPrefab;

    public Vector2Int Coordinate
    {
        get => coordinate;
        set
        {
            coordinate = value;
            var grid = GameManager.Instance.GridManager.Grid;
            transform.position = grid.CoordinateToPosition(coordinate);
            transform.localScale = (Vector2)grid.Size;
        }
    }
    private Vector2Int coordinate;

    private void Awake()
    {
        _ = Instantiate(behaviorPrefab, transform);
        _ = Instantiate(debuggerPrefab, transform);
    }

    private void Start()
    {
        behaviorPrefab.GetComponent<CellBehaviour>().Init();
        behaviorPrefab.GetComponent<CellDebugger>().Init();
    }
}
