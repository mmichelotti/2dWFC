using UnityEngine;
using TMPro;

public class CellDebugger : MonoBehaviour, IInitializable, IPositionable<Vector2Int>
{
    [SerializeField] private TMP_Text tmpText;
    public Vector2Int Coordinate { get; set; }
    public void Init() => tmpText.text = Coordinate.ToString();

    public void Set(string text) => tmpText.text = text;
}
