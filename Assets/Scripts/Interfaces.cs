public interface IPositionable<T>
{
    public T Coordinate { get; set; }
}

public interface IDirectionable
{
    public Directions Directions { get; set; }
    public bool HasDirection(Direction dir);
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

public record Directions
{
    public Direction Identity { get; set; }
    public Direction Required { get; private set; }
    public Direction Excluded { get; private set; }
    public Direction Opposite => Identity.GetOpposite();
    public bool Contains(Direction dir) => Identity.HasFlag(dir);
    public void SetRequirements(Direction required, Direction excluded) => (Required, Excluded) = (required, excluded);
}