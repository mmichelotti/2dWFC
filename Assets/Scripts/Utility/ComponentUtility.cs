using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Flags]
public enum CellComponents
{
    None = 0,
    CellPainter = 1 << 0,
    CellHighlighter = 1 << 1,
    CellDebugger = 1 << 2,
    CellParticle = 1 << 3,
    CellAudioPlayer = 1 << 4,
    All = ~None
}

public static class ComponentUtility
{
    public static IReadOnlyDictionary<CellComponents, Type> ComponentMap { get; } = new Dictionary<CellComponents, Type>()
    {
        { CellComponents.CellPainter, typeof(CellPainter) },
        { CellComponents.CellHighlighter, typeof(CellHighlighter) },
        { CellComponents.CellDebugger, typeof(CellDebugger) },
        { CellComponents.CellParticle, typeof(CellParticle) },
        { CellComponents.CellAudioPlayer, typeof(CellAudioPlayer) }
    };

    public static GameObject CreateCellPrefab
        (TileSet tileSet, ParticleSystem ps = null, Transform parent = null, CellComponents components = CellComponents.All,
        Color drawColor = default, Color eraseColor = default, float size = 2f, Color color = default, AudioClip[] audioClips = default)
    {
        GameObject cellPrefab = new("Cell");
        QuantumCell quantumCell = cellPrefab.AddComponent<QuantumCell>();
        quantumCell.TileSet = tileSet;

        // Define actions for setting properties
        var propertySetters = new Dictionary<Type, Action<object>>
        {
            { typeof(CellParticle), obj => ((CellParticle)obj).SetProperties(ps) },
            { typeof(CellHighlighter), obj => ((CellHighlighter)obj).SetProperties(drawColor, eraseColor) },
            { typeof(CellDebugger), obj => ((CellDebugger)obj).SetProperties(size, color) },
            { typeof(CellAudioPlayer), obj => ((CellAudioPlayer)obj).SetProperties(audioClips) }
        };

        foreach (var component in ComponentMap.Where(c => components.HasFlag(c.Key)))
        {
            var addedComponent = cellPrefab.AddComponent(component.Value);
            if (propertySetters.TryGetValue(component.Value, out var setter)) setter(addedComponent);
        }

        cellPrefab.transform.parent = parent;
        return cellPrefab;
    }

}

