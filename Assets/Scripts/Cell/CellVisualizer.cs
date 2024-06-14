using UnityEngine;
using System.Collections.Generic;
public enum Painting
{
    None,
    Drawing,
    Erasing
}
[RequireComponent(typeof(SpriteRenderer))]
public class CellVisualizer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private readonly IReadOnlyDictionary<Painting, Color> colors = new Dictionary<Painting, Color>()
    {
        { Painting.None,    Color.clear },
        { Painting.Drawing, new(0.62f, 1f, 0f, .5f)},
        { Painting.Erasing, new(1.0f, 0.334f, 0f, .5f)},
    };
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Painting mode)
    {
        spriteRenderer.color = colors[mode];
    }

    public void SetColor(bool isErasing)
    {
        spriteRenderer.color = isErasing ? colors[Painting.Erasing] : colors[Painting.Drawing];
    }

    public void ClearColor() => spriteRenderer.color = colors[default];
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}