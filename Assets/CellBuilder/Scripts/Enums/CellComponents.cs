[System.Flags]
public enum CellComponents
{
    None =              0b0000,
    CellPainter =       1 << 0,
    CellHighlighter =   1 << 1,
    CellDebugger =      1 << 2,
    CellParticle =      1 << 3,
    CellAudioPlayer =   1 << 4,
    All =               ~None
}