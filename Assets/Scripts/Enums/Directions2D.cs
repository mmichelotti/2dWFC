[System.Flags]
public enum Directions2D
{
    None = 0b0000,
    Up = 0b0001,
    Right = 0b0010,
    Down = 0b0100,
    Left = 0b1000,
    All = ~None
}