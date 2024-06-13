using System.Collections.Generic;
using System.Linq;
using System;
public record QuantumState<T>
{
    public List<T> Superposition { get; private set; } = new();
    public T Collapsed => IsEntangled ? Superposition.First() : Collapse();
    public int Density => Superposition.Count;
    public bool IsEntangled => Entropy == 0;
    public float Entropy
    {
        get
        {
            float probability = 1.0f / Density;
            float entropy = -Density * probability * (float)System.Math.Log(probability);
            return Density == 0 ? 0 : entropy;
        }
    }
    public QuantumState() { }
    public QuantumState(IEnumerable<T> initialState) => Superposition = new List<T>(initialState);

    public void Add(T obj) => Superposition.Add(obj);
    public void Add(IEnumerable<T> list) => Superposition.AddRange(list);
    public void Update(IEnumerable<T> tiles) => Superposition = new List<T>(tiles);
    public T Collapse()
    {
        Superposition = Superposition.OrderBy(_ => Guid.NewGuid()).Take(1).ToList();
        return Superposition.First();
    }

}
