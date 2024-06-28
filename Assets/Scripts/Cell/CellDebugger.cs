using UnityEngine;
using TMPro;

public class CellDebugger : MonoBehaviour, IInitializable, IPositionable<Vector2Int>
{
    [SerializeField]
    private TMP_Text tmpText;
    public Vector2Int Coordinate { get; set; }

    public void Init() => tmpText.text = Coordinate.ToString();

    public void Set(string text) => tmpText.text = text;

    public void SetText(float n) => tmpText.text = n != 0 ? n.ToString("F2") : string.Empty;

    private void Start()
    {
        var componentToDebug = transform.parent.GetComponentInChildren<CellBehaviour>();

        if (componentToDebug != null)
            return;

        componentToDebug.OnStateChanged.AddListener(state => SetText(state.Entropy));
    }
}
