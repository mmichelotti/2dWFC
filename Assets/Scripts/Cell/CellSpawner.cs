using UnityEngine;

[RequireComponent(typeof(QuantumCell),typeof(SpriteRenderer))]
public class CellSpawner : MonoBehaviour 
{
    private SpriteRenderer spriteRenderer;
    private QuantumCell quantumCell;
    private Grid Grid => GameManager.Instance.GridManager.Grid;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        quantumCell = GetComponent<QuantumCell>();
        quantumCell.OnInitializeState.AddListener(cell => Spawn(cell.Coordinate));
        quantumCell.OnCollapseState.AddListener(state => Draw(state.Collapsed));
        quantumCell.OnResetState.AddListener(state => Cancel());
    }
    private void Spawn(Vector2Int pos)
    {
        transform.position = Grid.CoordinateToPosition(pos);
        transform.localScale = new Vector2(Grid.Area, Grid.Area);
    }
    private void Cancel()
    {
        transform.rotation = default;
        spriteRenderer.sprite = default;
    }
    private void Draw(Tile tile)
    {
        spriteRenderer.sprite = tile.Sprite;
        transform.Rotate(tile.Rotation);
    }
}