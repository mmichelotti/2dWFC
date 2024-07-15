using UnityEngine;
using TMPro;
using UnityEditor.Experimental.GraphView;

[RequireComponent(typeof(QuantumCell))]
public class CellDebugger : MonoBehaviour
{
    [SerializeField] private float size = 2f;
    [SerializeField] private Color color = new(0,.8f,.7f,1f);
    private TextMeshPro cellText;

    public void Awake()
    {
        cellText = CreatePrefab().GetComponent<TextMeshPro>();

        QuantumCell quantumCell = GetComponent<QuantumCell>();
        quantumCell.OnInitializeState.AddListener(cell => SetText(cell.Coordinate));
        quantumCell.OnCollapseState.AddListener(state => SetText(state.Entropy));
        quantumCell.OnUpdateState.AddListener(state => SetText(state.Entropy));
    }

    public void SetText(float n) => cellText.text = n != 0 ? n.ToString("F2") : string.Empty;
    public void SetText(Vector2Int n) => cellText.text = n.ToString();

    private GameObject CreatePrefab()
    {
        GameObject prefab = new ("CellDebugger");

        prefab.AddComponent<RectTransform>();
        prefab.transform.SetParent(transform, false);

        TextMeshPro cellText = prefab.AddComponent<TextMeshPro>();
        cellText.fontSize = size;
        cellText.color = color;
        cellText.alignment = TextAlignmentOptions.Center;

        return prefab;
    }

}
