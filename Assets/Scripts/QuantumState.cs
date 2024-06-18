using System.Collections.Generic;
using System.Linq;
using System;
public record QuantumState<T>
{
    public List<T> Superposition { get; private set; }
    public T Collapsed => HasCollapsed ? Superposition.First() : Collapse();
    public int Density => Superposition.Count;
    public bool HasCollapsed { get; private set; }
    public float Entropy
    {
        get
        {
            float probability = 1.0f / Density;
            float entropy = -Density * probability * (float)System.Math.Log(probability);
            return Density == 0 ? 0 : entropy;
        }
    }
    public QuantumState() => (Superposition, HasCollapsed) = (new(), false);
    public QuantumState(IEnumerable<T> list) => (Superposition, HasCollapsed) = (new List<T>(list),false);
    public void Add(T obj) => Superposition.Add(obj);
    public void Add(IEnumerable<T> list) => Superposition.AddRange(list);
    public void Update(IEnumerable<T> tiles) => Superposition = new List<T>(tiles);
    public T Collapse()
    {
        HasCollapsed = true;
        Superposition = Superposition.OrderBy(_ => Guid.NewGuid()).Take(1).ToList();
        return Superposition.First();
    }

}
