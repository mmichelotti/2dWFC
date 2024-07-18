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
    public UnityEvent<int> OnIndexChange { get; } = new();
    public int CurrentIndex { get; private set; }
    private InputManager InputManager => GameManager.Instance.InputManager;
    private UnityAction<int> scrollWheelListener;
    private UnityAction leftClickListener;
    private QuantumCell cell;
    private bool isHovered;
    private Painting currentPainting;
    protected void Start()
    {
        cell = GetComponentInParent<QuantumCell>();
        scrollWheelListener = value => UpdateIndex(value);
        leftClickListener = () => SetCell();
    }
    private void UpdateIndex(int value)
    {
        CurrentIndex += value;
        CurrentIndex %= cell.State.Density;
        if (CurrentIndex < 0) CurrentIndex += cell.State.Density;
        OnIndexChange.Invoke(CurrentIndex);
    }
    
    private void Update()
    { 
        if (isHovered)
        {
            currentPainting = InputManager.IsLeftShiftPressed ? Painting.Erasing : Painting.Drawing;
            WhileOnHover.Invoke(currentPainting);
        }
    }

    private void SetCell()
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        InputManager.OnScrollWheel.AddListener(scrollWheelListener);
        InputManager.WhileLeftMouseButton.AddListener(leftClickListener);
        OnHover.Invoke(Painting.Drawing);
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InputManager.OnScrollWheel.RemoveListener(scrollWheelListener);
        InputManager.WhileLeftMouseButton.RemoveListener(leftClickListener);
        OnUnhover.Invoke(Painting.Clear);
        isHovered = false;
    }
}
