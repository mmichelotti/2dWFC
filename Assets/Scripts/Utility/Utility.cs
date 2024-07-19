using System.Collections.Generic;
using UnityEngine;


public class ConditionalHideAttribute : PropertyAttribute
{
    public string ConditionalSourceField;
    public bool HideInInspector;

    public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector = false)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = hideInInspector;
    }
}


public class Manager : MonoBehaviour { }
public class EntropyComparer : IComparer<(float Entropy, Vector2Int Position)>
{    
    public int Compare((float Entropy, Vector2Int Position) x, (float Entropy, Vector2Int Position) y)
    {
        int result = x.Entropy.CompareTo(y.Entropy);
        if (result == 0)
        {
            // If entropies are equal, compare by position to ensure no duplicates
            result = x.Position.CompareTo(y.Position);
        }
        return result;
    }
}
