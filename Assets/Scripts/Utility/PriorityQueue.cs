using System;
using System.Collections.Generic;
public class PriorityQueue<T>
{
    private List<(T item, float priority)> elements = new List<(T, float)>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
        elements.Sort((x, y) => x.priority.CompareTo(y.priority));
    }

    public T Dequeue()
    {
        if (elements.Count == 0) throw new InvalidOperationException("The priority queue is empty");
        var element = elements[0];
        elements.RemoveAt(0);
        return element.item;
    }

    public T Peek()
    {
        if (elements.Count == 0) throw new InvalidOperationException("The priority queue is empty");
        return elements[0].item;
    }
}
