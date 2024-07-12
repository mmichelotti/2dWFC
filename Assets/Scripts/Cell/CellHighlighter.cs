using System.Collections.Generic;
using UnityEngine;

public class CellHighigther : CellPainter
{
    private SpriteRenderer cellHighlighter;

    private readonly IReadOnlyDictionary<Painting, Color> colors = new Dictionary<Painting, Color>()
    {
        { Painting.Clear, Color.clear },
        { Painting.Drawing, new Color(0.62f, 1f, 0f, .25f) },
        { Painting.Erasing, new Color(1.0f, 0.334f, 0f, .25f) },
    };

    protected void Start()
    {
        base.Start();
        cellHighlighter = CreateCellHighlighter();
        WhileOnHover.AddListener(color => SetColor(color));
        OnUnhover.AddListener(color => SetColor(color));
        SetColor(Painting.Clear);
    }

    private SpriteRenderer CreateCellHighlighter()
    {
        GameObject highlighterObject = new GameObject("CellHighlighter");
        highlighterObject.transform.SetParent(transform, false);

        RectTransform rectTransform = highlighterObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(1, 1);  // Adjust size as needed

        SpriteRenderer sr = highlighterObject.AddComponent<SpriteRenderer>();
        sr.sprite = CreateWhiteSprite();
        sr.sortingLayerName = "Highlight";
        return sr;
    }

    private Sprite CreateWhiteSprite()
    {
        Texture2D texture = new(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        Rect rect = new (0, 0, 1, 1);
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 1);
    }

    private void SetColor(Painting mode) => cellHighlighter.color = colors[mode];
}
