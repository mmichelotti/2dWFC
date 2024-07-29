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
    private UnityAction middleClickListener;
    private QuantumCell quantumCell;
    private bool isHovered;
    private Painting currentPainting;
    protected void Start()
    {
        quantumCell = GetComponentInParent<QuantumCell>();
        scrollWheelListener = value => UpdateIndex(value);
        leftClickListener = () => SetCell();
        middleClickListener = () => SetCell(CurrentIndex);
    }
    private void UpdateIndex(int value)
    {
        CurrentIndex += value;
        CurrentIndex %= quantumCell.State.Density;
        if (CurrentIndex < 0) CurrentIndex += quantumCell.State.Density;
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
                quantumCell.CellGrid.SpawnCell(quantumCell.Coordinate);
                break;
            case Painting.Erasing:
                quantumCell.CellGrid.RemoveCell(quantumCell.Coordinate);
                break;
        }
    }
    private void SetCell(int index) => quantumCell.CellGrid.SpawnCell(quantumCell.Coordinate, index);
    public void OnPointerEnter(PointerEventData eventData)
    {
        InputManager.OnScrollWheel.AddListener(scrollWheelListener);
        InputManager.WhileLeftMouseButton.AddListener(leftClickListener);
        InputManager.OnMiddleMouseButtonEnter.AddListener(middleClickListener); // Changed from OnMiddleMouseButtonEnter to WhileMiddleMouseButton
        OnHover.Invoke(Painting.Drawing);
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InputManager.OnScrollWheel.RemoveListener(scrollWheelListener);
        InputManager.WhileLeftMouseButton.RemoveListener(leftClickListener);
        InputManager.OnMiddleMouseButtonEnter.RemoveListener(middleClickListener); // Changed from OnMiddleMouseButtonExit to WhileMiddleMouseButton
        OnUnhover.Invoke(Painting.Clear);
        isHovered = false;
    }

}
