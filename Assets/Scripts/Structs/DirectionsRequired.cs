public struct DirectionsRequired : System.IFormattable
{
    public Directions2D Required { get; set; }
    public Directions2D Excluded { get; set; }
    public DirectionsRequired(Directions2D onlyPositive) => (Required, Excluded) = (onlyPositive, default);
    public DirectionsRequired(Directions2D required, Directions2D excluded) => (Required, Excluded) = (required, excluded);
    public void Flip() => (Required, Excluded) = (Excluded, Required);
    public DirectionsRequired Exclude(Directions2D dir) => new(Required, Excluded | dir);
    public string ToString(string str, System.IFormatProvider format) => $"Required: {Required.ToStringCustom()}, Excluded {Excluded.ToStringCustom()}";
}