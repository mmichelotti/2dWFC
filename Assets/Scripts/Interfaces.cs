public interface IPositionable<T> { public T Coordinate { get; set; } }
public interface IRequirable { public DirectionsRequired DirectionsRequired { get; set; } }
public interface IDebuggable { public abstract void Debug(); }
public interface IInitializable { public abstract void Init(); }
public interface IProbable { public float Probability { get; set; } }
public interface IDirectionable
{
    public Directions Directions { get; set; }
    public bool HasDirection(Directions dir);
}

public interface IQuantizable<T> where T : IProbable
{
    public abstract QuantumState<T> State { get; set; }
    public abstract void InitializeState(bool invoke = true);
    public abstract void CollapseState(int? value, bool invoke = true);
    public abstract void UpdateState(bool invoke = true);
    public abstract void ResetState(bool invoke = true);
}