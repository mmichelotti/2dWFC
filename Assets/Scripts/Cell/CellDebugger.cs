using UnityEngine;
using TMPro;

[RequireComponent(typeof(QuantumCell))]
public class CellDebugger : MonoBehaviour
{
    [SerializeField] private TMP_Text cellTextPrefab;
    private TMP_Text cellText;
    private QuantumCell quantumCell;

    public void Awake()
    {
        cellText = Instantiate(cellTextPrefab, transform);
        quantumCell = GetComponent<QuantumCell>();
        quantumCell.OnInitializeState.AddListener(cell => SetText(cell.Coordinate));
        quantumCell.OnCollapseState.AddListener(state => SetText(state.Entropy));
        quantumCell.OnUpdateState.AddListener(state => SetText(state.Entropy));
    }

    public void SetText(float n) => cellText.text = n != 0 ? n.ToString("F2") : string.Empty;
    public void SetText(Vector2Int n) => cellText.text = n.ToString();

}
