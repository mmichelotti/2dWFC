using UnityEngine;

[RequireComponent(typeof(QuantumCell),typeof(SpriteRenderer))]
public class CellSpawner : MonoBehaviour 
{
    private SpriteRenderer spriteRenderer;
    private QuantumCell quantumCell;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        quantumCell = GetComponent<QuantumCell>();
        quantumCell.OnCollapseState.AddListener(() => Draw(quantumCell.State.Collapsed));
        quantumCell.OnResetState.AddListener(() => Cancel());
    }
    public void Cancel()
    {
        transform.rotation = default;
        spriteRenderer.sprite = default;
    }
    public void Draw(Tile tile)
    {
        spriteRenderer.sprite = tile.Sprite;
        transform.Rotate(tile.Rotation);
    }
}