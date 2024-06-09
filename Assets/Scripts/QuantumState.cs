using System.Collections.Generic;
using static Extensions;



/// <summary>
/// This is the core of the idea
/// Every object can be express as a quantum state, has superposition, entropy and entanglement
/// </summary>
/// <typeparam name="T"></typeparam>
public record QuantumState<T>
{
    public List<T> Superposition { get; set; } = new();
    public T Defined => IsEntangled ? Superposition[0] : default;
    public int Density => Superposition.Count;
    public bool IsEntangled => Entropy == 0;
    public int RandomIndex => PositiveRandom(Density);
    public float Entropy
    {
        get
        {
            float probability = 1.0f / Density;
            float entropy = -Density * probability * (float)System.Math.Log(probability);
            return Density == 0 ? 0 : entropy;
        }
    }

    /// <summary>
    /// Helpful constructor that can help me create QuantumState composed by single or multiple items
    /// </summary>
    public QuantumState() => Superposition = new();
    public QuantumState(T tile) => Superposition = new() { tile };
    public QuantumState(List<T> tiles) => Superposition = new(tiles);

    /// <summary>
    /// The act of entangling, within its possibilities, just pick one at random and entangle it. 
    /// It returns the object entangled
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public T Entangle()
    {
        Superposition = new List<T> { Superposition[RandomIndex] };
        return Superposition[0];
    }

    /// <summary>
    /// Useful methods to manipulate the superposition list
    /// </summary>
    /// <param name="obj"></param>
    public void Add(T obj) => Superposition.Add(obj);
    public void Add(List<T> list) => Superposition.AddRange(list);
    public void Collapse() => Superposition.Clear();

}
