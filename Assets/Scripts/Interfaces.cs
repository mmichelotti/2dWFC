public interface IQuantumStatable<T>
{
    public abstract QuantumState<T> State { get; set; }
    public T Entangled { get; set; }
    public abstract void InitializeState();
    public abstract void ResetState();
    public abstract void UpdateState();
    public void EntangleState() => Entangled = State.Entangle();

}
public interface IPositionable<T>
{
    public T Coordinate { get; set; }
}

public interface IDirectionable
{
    public Direction Direction { get; set; }
    public Direction Required { get; set; }
    public Direction Excluded { get; set; }
    public bool HasDirection(Direction dir);
}

public struct Directions
{
    public Direction Direction;
    public Direction Required;
    public Direction Excluded;

    public Directions(Direction direction, Direction required, Direction excluded) => (Direction, Required, Excluded) = (direction, required, excluded);

}