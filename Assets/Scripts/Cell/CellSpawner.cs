using UnityEngine;

[RequireComponent(typeof(QuantumCell),typeof(SpriteRenderer))]
public class CellSpawner : MonoBehaviour 
{
    private SpriteRenderer spriteRenderer;
    private QuantumCell quantumCell;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        quantumCell = GetComponent<QuantumCell>();
        quantumCell.OnCollapseState.AddListener(state => Draw(state.Collapsed));
        quantumCell.OnResetState.AddListener(state => Cancel());
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