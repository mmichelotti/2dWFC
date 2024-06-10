public interface IPositionable<T>
{
    public T Coordinate { get; set; }
}

public interface IDirectionable
{
    public Directions Directions { get; set; }
    public bool HasDirection(Directions dir);
}
public interface IDirectionableRequired
{
    public DirectionsRequired DirectionsRequired { get; set; }
}
public interface IQuantumStatable<T>
{
    public abstract QuantumState<T> State { get; set; }
    public T Entangled { get; set; }
    public abstract void InitializeState();
    public abstract void ResetState();
    public abstract void UpdateState();
    public void EntangleState() => Entangled = State.Entangle();

}