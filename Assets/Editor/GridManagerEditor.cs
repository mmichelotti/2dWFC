using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CellGrid))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CellGrid gridManager = (CellGrid)target;
        EditorGUILayout.Space();

        if (GUILayout.Button("AutoFill"))
        {
            gridManager.FillGrid();
        }
        if (GUILayout.Button("Shuffle"))
        {
            gridManager.ClearGrid();
            gridManager.FillGrid();
        }

        if (GUILayout.Button("Clear"))
        {
            gridManager.ClearGrid();
        }
        if (GUILayout.Button("Save"))
        {

        }
    }
}
