using System.Collections.Generic;

public record QuantumState<T>
{
    public List<T> Superposition { get; set; } = new();
    public T Defined => IsEntangled ? Superposition[0] : default;
    public int Density => Superposition.Count;
    public bool IsEntangled => Entropy == 0;
    public int RandomIndex => Extensions.PositiveRandom(Density);
    public float Entropy
    {
        get
        {
            float probability = 1.0f / Density;
            float entropy = -Density * probability * (float)System.Math.Log(probability);
            return Density == 0 ? 0 : entropy;
        }
    }

    public QuantumState() => Superposition = new();
    public QuantumState(T tile) => Superposition = new() { tile };
    public QuantumState(List<T> tiles) => Superposition = new(tiles);

    public T Entangle()
    {
        Superposition = new List<T> { Superposition[RandomIndex] };
        return Superposition[0];
    }
    public void Add(T obj) => Superposition.Add(obj);
    public void Add(List<T> list) => Superposition.AddRange(list);
    public void Collapse() => Superposition.Clear();

}
