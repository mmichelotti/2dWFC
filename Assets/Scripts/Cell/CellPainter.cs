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
    public UnityEvent<Painting> OnHover { get; } = new();
    public UnityEvent<Painting> OnUnhover { get; } = new();
    public UnityEvent<Painting> WhileOnHover { get; } = new();
    public int CurrentIndex { get; private set; }
    private InputManager InputManager => GameManager.Instance.InputManager;
    private UnityAction<int> scrollWheelListener; 
    private QuantumCell cell;
    private bool isHovered;

    protected void Start()
    {
        cell = GetComponentInParent<QuantumCell>();
        scrollWheelListener = value => UpdateIndex(value); 
    }
    private void UpdateIndex(int value)
    {
        CurrentIndex += value;
        CurrentIndex = CurrentIndex % cell.State.Density;
        if (CurrentIndex < 0) CurrentIndex += cell.State.Density;
    }
    
    private void Update()
    { 
        if (isHovered)
        {
            Debug.LogError($"coordinate {cell.Coordinate} have {CurrentIndex}");
            var currentPainting = InputManager.IsLeftShiftPressed ? Painting.Erasing : Painting.Drawing;
            WhileOnHover.Invoke(currentPainting);
            if (InputManager.IsLeftMouseButtonPressed)
            {
                switch (currentPainting)
                {
                    case Painting.Drawing:
                        cell.CellGrid.SpawnCell(cell.Coordinate);
                        break;
                    case Painting.Erasing:
                        cell.CellGrid.RemoveCell(cell.Coordinate);
                        break;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        InputManager.OnScrollWheel.AddListener(scrollWheelListener);
        OnHover.Invoke(Painting.Drawing);
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InputManager.OnScrollWheel.RemoveListener(scrollWheelListener);
        OnUnhover.Invoke(Painting.Clear);
        isHovered = false;
    }

}
