using System.Collections.Generic;
using System.Linq;
public record QuantumState<T> where T : IProbable
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
    public QuantumState(IEnumerable<T> list) => (Superposition, HasCollapsed) = (new List<T>(list), false);
    public void Add(T obj) => Superposition.Add(obj);
    public void Add(IEnumerable<T> list) => Superposition.AddRange(list);
    public void Update(IEnumerable<T> tiles) => Superposition = new List<T>(tiles);
    
    public T Collapse()
    {
        HasCollapsed = true;
        Superposition = WeightedRandomTile();
        return Superposition.First();
    }
    public T Collapse(int index)
    {
        HasCollapsed = true;
        if (!index.IsBetween(Density)) return Collapse();
        Superposition = new List<T> { Superposition[index] };
        return Superposition.First();
    }

    private List<T> WeightedRandomTile()
    {
        float totalProbability = Superposition.Sum(tile => tile.Probability);
        float randomPoint = UnityEngine.Random.value * totalProbability;

        foreach (T tile in Superposition)
        {
            if (randomPoint < tile.Probability)
            {
                return new List<T> { tile };
            }
            else
            {
                randomPoint -= tile.Probability;
            }
        }
        
        return new List<T> { Superposition.First() };
    }

    public void Debug()
    {
        foreach (var item in Superposition)
        {
            UnityEngine.Debug.LogError($"Density{Density}");
            UnityEngine.Debug.LogError(item);
        }
    }
}
