using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public enum Painting
{
    Clear,
    Drawing,
    Erasing
}

[RequireComponent(typeof(QuantumCell), typeof(BoxCollider))]
public class CellPainter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InputManager InputManager => GameManager.Instance.InputManager;
    private GridPainter GridPainter => GameManager.Instance.GridManager;

    private Vector2Int coordinate;
    private bool isHovered;

    public UnityEvent<Painting> OnUnhover { get; } = new();
    public UnityEvent<Painting> WhileOnHover { get; } = new();

    protected void Start()
    {
        coordinate = GetComponentInParent<QuantumCell>().Coordinate;
    }

    private void Update()
    {
        if (isHovered)
        {
            var currentPainting = InputManager.IsLeftShiftPressed ? Painting.Erasing : Painting.Drawing;
            WhileOnHover.Invoke(currentPainting);
            if (InputManager.IsLeftMouseButtonPressed)
            {
                if (GridPainter.Cells.TryGetValue(coordinate, out QuantumCell cell))
                {
                    switch (currentPainting)
                    {
                        case Painting.Drawing:
                            GridPainter.SpawnCell(coordinate);
                            break;
                        case Painting.Erasing:
                            GridPainter.RemoveCell(coordinate);
                            break;
                    }
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnUnhover.Invoke(Painting.Clear);
        isHovered = false;
    }
}
