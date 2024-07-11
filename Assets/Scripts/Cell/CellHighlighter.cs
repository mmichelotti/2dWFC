using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(QuantumCell))]
public class CellHighlighter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer cellHighlighterPrefab;
    private SpriteRenderer cellHighlighter;
    private readonly IReadOnlyDictionary<Painting, Color> colors = new Dictionary<Painting, Color>()
    {
        { Painting.None,    Color.clear },
        { Painting.Drawing, new(0.62f, 1f, 0f, .25f)},
        { Painting.Erasing, new(1.0f, 0.334f, 0f, .25f)},
    };
    private void Start()
    {
        cellHighlighter = Instantiate(cellHighlighterPrefab,transform);
        cellHighlighter.color = colors[default];
    }
    public void SetColor(Painting mode)
    {
        cellHighlighter.color = colors[mode];
    }
}
