public interface IQuantizable<T> where T : IProbable
{
    public abstract QuantumState<T> State { get; set; }
    public abstract void InitializeState(bool invoke = true);
    public abstract void CollapseState(int? value, bool invoke = true);
    public abstract void UpdateState(bool invoke = true);
    public abstract void ResetState(bool invoke = true);
}