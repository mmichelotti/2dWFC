using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CellPainter))]
public class CellHighlighter : MonoBehaviour
{
    [SerializeField] private Color drawColor = Color.white;
    [SerializeField] private Color eraseColor = new(.4f, 0f, 1f, 1f);
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
        colors.Add(Painting.Drawing, new(drawColor.r, drawColor.g, drawColor.b, .25f));
        colors.Add(Painting.Erasing, new(eraseColor.r, eraseColor.g, eraseColor.b, .25f));
        cellHighlighter = CreatePreafab().GetComponent<SpriteRenderer>();
        cellPainter.WhileOnHover.AddListener(color => SetColor(color));
        cellPainter.OnUnhover.AddListener(color => SetColor(color));
        SetColor(Painting.Clear);
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
