using UnityEngine;
using TMPro;

[RequireComponent(typeof(QuantumCell))]
public class CellDebugger : MonoBehaviour
{
    [SerializeField] private TMP_Text cellTextPrefab;
    private TMP_Text cellText;
    private QuantumCell cell;

    public void Start()
    {
        cellText = Instantiate(cellTextPrefab, transform);
        cell = GetComponent<QuantumCell>();
        cell.OnCollapseState.AddListener(() => SetText(cell.State.Entropy));
        cell.OnUpdateState.AddListener(() => SetText(cell.State.Entropy));
        cell.OnInitializeState.AddListener(() => SetText(cell.Coordinate));
    }

    public void SetText(float n) => cellText.text = n != 0 ? n.ToString("F2") : string.Empty;
    public void SetText(Vector2Int n) => cellText.text = n.ToString();
}
