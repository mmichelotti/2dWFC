using UnityEngine;
using TMPro;

public class CellDebugger : MonoBehaviour, IPositionable<Vector2Int>
{
    private CellBehaviour cellBehaviour;
    [SerializeField] private TMP_Text tmpText;
    public Vector2Int Coordinate { get; set; }
    public void Start()
    {
        tmpText.text = Coordinate.ToString();
    }
    public void Set(string text) => tmpText.text = text;
    public void SetText(float n) => tmpText.text = n != 0 ? n.ToString("F2") : string.Empty; 
}
