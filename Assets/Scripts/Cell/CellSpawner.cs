using UnityEngine;

public class CellSpawner
{
    private QuantumCell quantumCell;
    private SpriteRenderer spriteRenderer;
    public CellSpawner(QuantumCell cell)
    {
        quantumCell = cell;
        spriteRenderer = cell.gameObject.GetComponent<SpriteRenderer>();
        quantumCell.OnCollapseState.AddListener(state => Set(state.Collapsed));
        quantumCell.OnResetState.AddListener(state => Reset());

        quantumCell.GetComponent<CellPainter>().OnIndexChange.AddListener(index => Preview(index));
        quantumCell.GetComponent<CellPainter>().OnUnhover.AddListener(_ => StopPreview());
    }

    private void Preview(int index)
    {
        if (quantumCell.State.HasCollapsed) return;
        Set(quantumCell.State.Superposition[index]);
    }
    private void StopPreview()
    {
        if (quantumCell.State.HasCollapsed) return;
        Reset();
    }
    private void Reset()
    {
        quantumCell.transform.rotation = Quaternion.identity;
        spriteRenderer.sprite = null;
    }

    private void Set(Tile tile)
    {
        spriteRenderer.sprite = tile.Sprite;
        quantumCell.transform.rotation = Quaternion.Euler(tile.Rotation);
    }
}