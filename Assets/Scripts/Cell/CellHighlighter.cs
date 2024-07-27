using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CellPainter))]
public class CellHighlighter : MonoBehaviour
{
    public Color DrawColor { get; set; } = Color.white;
    public Color EraseColor { get; set; } = new(.4f, 0f, 1f, 1f);

    private SpriteRenderer cellHighlighter;
    private CellPainter cellPainter;
    private Dictionary<Painting, Color> colors = new()
    {
        { Painting.Clear, Color.clear }
    };
    private void Awake()
    {
        cellPainter = GetComponent<CellPainter>();
    }
    protected void Start()
    {
        cellHighlighter = CreatePreafab().GetComponent<SpriteRenderer>();
        cellPainter.WhileOnHover.AddListener(color => SetColor(color));
        cellPainter.OnUnhover.AddListener(color => SetColor(color));
        SetColor(Painting.Clear);
    }

    public void SetProperties(Color draw, Color erase)
    {
        colors[Painting.Drawing] = new(draw.r, draw.g, draw.b, .25f);
        colors[Painting.Erasing] = new(erase.r, erase.g, erase.b, .25f);   
    }

    private GameObject CreatePreafab()
    {
        GameObject prefab = new ("CellHighlighter");
        prefab.transform.SetParent(transform, false);

        prefab.AddComponent<RectTransform>();

        SpriteRenderer sr = prefab.AddComponent<SpriteRenderer>();
        sr.sprite = CreateWhiteSprite();
        sr.sortingLayerName = "Highlight";
        return prefab;
    }

    private static Sprite CreateWhiteSprite()
    {
        Texture2D texture = new(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        Rect rect = new (0, 0, 1, 1);
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 1);
    }

    private void SetColor(Painting mode) => cellHighlighter.color = colors[mode];

}
