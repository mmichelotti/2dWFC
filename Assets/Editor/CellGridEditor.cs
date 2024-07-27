using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CellGrid))]
public class CellGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CellGrid cellGrid = (CellGrid)target;
        EditorGUILayout.Space();

        if (GUILayout.Button("AutoFill"))
        {
            cellGrid.FillGrid();
        }
        if (GUILayout.Button("Shuffle"))
        {
            cellGrid.ClearGrid();
            cellGrid.FillGrid();
        }

        if (GUILayout.Button("Clear"))
        {
            cellGrid.ClearGrid();
        }
        if (GUILayout.Button("Save"))
        {

        }
    }
}
