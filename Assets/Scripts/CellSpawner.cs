using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CellSpawner : MonoBehaviour 
{
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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