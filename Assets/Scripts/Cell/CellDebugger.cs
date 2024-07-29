using UnityEngine;
using TMPro;

[RequireComponent(typeof(QuantumCell))]
public class CellDebugger : MonoBehaviour
{

    private TextMeshPro cellText;
    public void Awake()
    {

        cellText = CreatePrefab().GetComponent<TextMeshPro>();
        QuantumCell quantumCell = GetComponent<QuantumCell>();
        quantumCell.OnInitializeState.AddListener(state => SetText(state.Entropy));
        quantumCell.OnCollapseState.AddListener(state => SetText(state.Entropy));
        quantumCell.OnUpdateState.AddListener(state => SetText(state.Entropy));
    }

    public void SetText(float n) => cellText.text = n != 0 ? n.ToString("F2") : string.Empty;

    private GameObject CreatePrefab()
    {
        GameObject prefab = new ("CellDebugger");

        prefab.AddComponent<RectTransform>();
        prefab.transform.SetParent(transform, false);

        TextMeshPro cellText = prefab.AddComponent<TextMeshPro>();
        cellText.alignment = TextAlignmentOptions.Center;

        return prefab;
    }

    public void SetProperties(float fontSize, Color fontColor)
    {
        cellText.fontSize = fontSize;
        cellText.color = fontColor;
    }
}
