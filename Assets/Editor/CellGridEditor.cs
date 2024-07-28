using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static ComponentUtility;

[CustomEditor(typeof(CellGrid))]
public class CellGridEditor : Editor
{
    SerializedProperty componentsProp;
    SerializedProperty tileSetProp;
    SerializedProperty vfxProp;
    SerializedProperty drawColorProp;
    SerializedProperty eraseColorProp;
    SerializedProperty sizeProp;
    SerializedProperty colorProp;

    SerializedProperty spawnProp;
    SerializedProperty eraseProp;
    SerializedProperty scrollProp;
    SerializedProperty hoverProp;

    private static Dictionary<CellComponents, ComponentProperties> componentPropertyMap;

    private struct ComponentProperties
    {
        public string Header;
        public string[] Components;
        public ComponentProperties(string header, string[] components) => (Header, Components) = (header, components);
    }

    void OnEnable()
    {
        componentsProp = serializedObject.FindProperty("components");
        tileSetProp = serializedObject.FindProperty("tileSet");

        ComponentProperties vfx = new ("Particle Settings", new[] { "vfx" });
        ComponentProperties highlight = new ("Highlight Settings", new[] { "drawColor", "eraseColor" });
        ComponentProperties debug = new ("Debug Settings", new[] { "fontSize", "fontColor" });
        ComponentProperties audio = new("Audio Settings", new[] { "spawn", "erase", "scroll", "hover" });

        serializedObject.FindProperty(vfx.Components[0]);
        drawColorProp = serializedObject.FindProperty(highlight.Components[0]);
        eraseColorProp = serializedObject.FindProperty(highlight.Components[1]);
        sizeProp = serializedObject.FindProperty(debug.Components[0]);
        colorProp = serializedObject.FindProperty(debug.Components[1]);

        spawnProp = serializedObject.FindProperty(audio.Components[0]);
        eraseProp = serializedObject.FindProperty(audio.Components[1]);
        scrollProp = serializedObject.FindProperty(audio.Components[2]);
        hoverProp = serializedObject.FindProperty(audio.Components[3]);

        componentPropertyMap = new Dictionary<CellComponents, ComponentProperties>
        {
            { CellComponents.CellParticle, vfx },
            { CellComponents.CellHighlighter, highlight },
            { CellComponents.CellDebugger, debug },
            { CellComponents.CellAudioPlayer, audio }
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(tileSetProp);
        EditorGUILayout.PropertyField(componentsProp);

        CellComponents components = (CellComponents)componentsProp.intValue;

        foreach (var component in ComponentMap.Where(c => components.HasFlag(c.Key)))
        {
            DrawComponents(component.Key);
        }

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        CellGrid cellGrid = (CellGrid)target;

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
            // Implement save functionality
        }
    }

    private void DrawComponents(CellComponents component)
    {
        if (componentPropertyMap.TryGetValue(component, out ComponentProperties property))
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(property.Header, EditorStyles.boldLabel);
            foreach (string propertyName in property.Components)
            {              
                SerializedProperty prop = serializedObject.FindProperty(propertyName);        
                if (prop != null) EditorGUILayout.PropertyField(prop);
            }
        }
    }
}
