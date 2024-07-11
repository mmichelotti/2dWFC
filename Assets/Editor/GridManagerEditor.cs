using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridPainter))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get the GridManager script instance
        GridPainter gridManager = (GridPainter)target;

        // Add a space for better readability
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
