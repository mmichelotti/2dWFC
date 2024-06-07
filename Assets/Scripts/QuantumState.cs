using System.Collections.Generic;
using System;
using static Extensions;
public record QuantumState<T>
{
    public List<T> Superposition { get; set; } = new();
    public int Density => Superposition.Count;
    public bool Entangled => Superposition.Count == 1;
    public T ObserveRandom => Superposition[PositiveRandom(Density)];
    public float Entropy
    {   get
        {
            float probability = 1.0f / Density;
            float entropy = -Density * probability * (float)System.Math.Log(probability);
            return Density == 0 ? 0 : entropy; //density cant be 0, at minimum should be 1
        }
    }

    public QuantumState() => Superposition = new();
    public QuantumState(T tile) => Superposition = new() { tile };
    public QuantumState(List<T> tile) => Superposition = new(tile);
    public void Add(List<T> list) => Superposition.AddRange(list);
    public void Add(T tile) => Superposition.Add(tile);
    public void Collapse() => Superposition.Clear();
}
