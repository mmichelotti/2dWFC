using UnityEngine.Events;

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
    public abstract void InitializeState();
    public abstract void CollapseState(int? value);
    public abstract void UpdateState();
    public abstract void ResetState();
}