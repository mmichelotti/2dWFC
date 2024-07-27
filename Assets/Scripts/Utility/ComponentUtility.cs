using UnityEngine;
using System;
using System.Collections.Generic;

[Flags]
public enum CellComponents
{
    None = 0,
    CellPainter = 1 << 0,
    CellHighlighter = 1 << 1,
    CellDebugger = 1 << 2,
    CellParticle = 1 << 3,
    All = ~None
}

public static class ComponentUtility
{
    public static IReadOnlyDictionary<CellComponents, Type> ComponentMap { get; } = new Dictionary<CellComponents, Type>()
    {
        { CellComponents.CellPainter, typeof(CellPainter) },
        { CellComponents.CellHighlighter, typeof(CellHighlighter) },
        { CellComponents.CellDebugger, typeof(CellDebugger) },
        { CellComponents.CellParticle, typeof(CellParticle) }
    };

    public static GameObject CreateCellPrefab
        (TileSet tileSet, ParticleSystem ps = null, Transform parent = null, CellComponents components = CellComponents.All, 
        Color drawColor = default, Color eraseColor = default, float size = 2f, Color color = default)
    {

        GameObject cellPrefab = new("Cell");
        QuantumCell quantumCell = cellPrefab.AddComponent<QuantumCell>();
        quantumCell.TileSet = tileSet;

        Dictionary<Type, object[]> propertyValues = new Dictionary<Type, object[]>
        {
            { typeof(CellParticle), new object[] { ps } },
            { typeof(CellHighlighter), new object[] { drawColor, eraseColor } },
            { typeof(CellDebugger), new object[] { size, color } }
        };
        foreach (var component in ComponentMap)
        {
            if ((components & component.Key) != 0)
            {
                var addedComponent = cellPrefab.AddComponent(component.Value);

                if (addedComponent is CellParticle cellParticle)
                {
                    cellParticle.SetProperties(ps);
                }
                if (addedComponent is CellHighlighter cellHighlighter)
                {
                    cellHighlighter.SetProperties(drawColor, eraseColor);
                }
                if (addedComponent is CellDebugger cellDebugger)
                {
                    cellDebugger.SetProperties(size,color);
                }
            }
        }

        cellPrefab.transform.parent = parent;
        return cellPrefab;
    }
}

