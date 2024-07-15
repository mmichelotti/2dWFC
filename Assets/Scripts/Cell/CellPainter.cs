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

    private Vector2Int cellCoordinate;
    private CellGrid cellGrid;
    private bool isHovered;

    public UnityEvent<Painting> OnUnhover { get; } = new();
    public UnityEvent<Painting> WhileOnHover { get; } = new();

    protected void Start()
    {
        QuantumCell cell = GetComponentInParent<QuantumCell>();
        cellCoordinate = cell.Coordinate;
        cellGrid = cell.CellGrid;
    }

    private void Update()
    {
        if (isHovered)
        {
            var currentPainting = InputManager.IsLeftShiftPressed ? Painting.Erasing : Painting.Drawing;
            WhileOnHover.Invoke(currentPainting);
            if (InputManager.IsLeftMouseButtonPressed)
            {
                if (cellGrid.Cells.TryGetValue(cellCoordinate, out QuantumCell cell))
                {
                    switch (currentPainting)
                    {
                        case Painting.Drawing:
                            cellGrid.SpawnCell(cellCoordinate);
                            break;
                        case Painting.Erasing:
                            cellGrid.RemoveCell(cellCoordinate);
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
