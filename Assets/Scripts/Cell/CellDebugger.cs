using UnityEngine;
using TMPro;

public class CellDebugger : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpText;
    public void SubscribeToCell(CellBehaviour cell)
    {
        cell.OnStateUpdated.AddListener(() => SetText(cell.State.Entropy));
        cell.OnStateInitialized.AddListener(() => SetText(cell.Coordinate));
    }

    public void SetText(float n) => tmpText.text = n != 0 ? n.ToString("F2") : string.Empty;
    public void SetText(Vector2Int n) => tmpText.text = n.ToString();
}
