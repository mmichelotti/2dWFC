using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileSet))]
public class TileSetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileSet tileSet = (TileSet)target;

        if (GUILayout.Button("Apply"))
        {
            tileSet.GenerateAllConfigurations();
            EditorUtility.SetDirty(tileSet);
        }
    }
}
