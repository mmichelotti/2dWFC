using UnityEngine;
using TMPro;

public class CellDebugger : MonoBehaviour, IPositionable<Vector2Int>
{
    [SerializeField] private TMP_Text tmpText;
    public Vector2Int Coordinate { get; set; }
    public void SubscribeToCell(CellBehaviour cell)
    {
        cell.OnStateInitialized.AddListener(() => Set(Coordinate.ToString()));
        cell.OnStateUpdated.AddListener(() => SetText(cell.State.Entropy));
    }

    public void Set(string text) => tmpText.text = text;
    public void SetText(float n) => tmpText.text = n != 0 ? n.ToString("F2") : string.Empty; 
}
