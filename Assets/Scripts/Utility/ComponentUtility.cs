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
}

