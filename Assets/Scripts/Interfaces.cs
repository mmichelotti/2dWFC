public interface IPositionable<T>
{
    public T Coordinate { get; set; }
}

public interface IDirectionable
{
    public Directions Directions { get; set; }
    public bool HasDirection(Directions dir);
}
public interface IRequirable
{
    public DirectionsRequired DirectionsRequired { get; set; }
}
public interface IDebuggable
{
    public abstract void Debug();
}
public interface IQuantizable<T>
{
    public abstract QuantumState<T> State { get; set; }
    public abstract void InitializeState();
    public abstract void ResetState();
    public abstract void UpdateState();
    public virtual void EntangleState() => State.Entangle();
}