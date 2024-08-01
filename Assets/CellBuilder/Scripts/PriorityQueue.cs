using System.Collections.Generic;
using System;

public class PriorityQueue<T>
{
    private List<(T item, float priority)> elements = new List<(T, float)>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
        int c = elements.Count - 1;
        while (c > 0 && elements[c].priority < elements[(c - 1) / 2].priority)
        {
            (elements[c], elements[(c - 1) / 2]) = (elements[(c - 1) / 2], elements[c]);
            c = (c - 1) / 2;
        }
    }

    public T DequeueSmallest()
    {
        if (elements.Count == 0) throw new InvalidOperationException("The priority queue is empty");
        var element = elements[0];
        elements[0] = elements[^1];
        elements.RemoveAt(elements.Count - 1);

        int c = 0;
        while (true)
        {
            int min = c;
            int left = 2 * c + 1;
            int right = 2 * c + 2;

            if (left < elements.Count && elements[left].priority < elements[min].priority) min = left;
            if (right < elements.Count && elements[right].priority < elements[min].priority) min = right;

            if (min == c) break;

            (elements[c], elements[min]) = (elements[min], elements[c]);
            c = min;
        }

        return element.item;
    }
    public List<T> DequeueAllSmallest()
    {
        if (elements.Count == 0) throw new InvalidOperationException("The priority queue is empty");

        List<T> itemsWithSamePriority = new();
        var element = elements[0];
        var topPriority = element.priority;

        while (elements.Count > 0 && elements[0].priority == topPriority)
        {
            itemsWithSamePriority.Add(elements[0].item);
            elements[0] = elements[^1];
            elements.RemoveAt(elements.Count - 1);

            int c = 0;
            while (true)
            {
                int min = c;
                int left = 2 * c + 1;
                int right = 2 * c + 2;

                if (left < elements.Count && elements[left].priority < elements[min].priority) min = left;
                if (right < elements.Count && elements[right].priority < elements[min].priority) min = right;

                if (min == c) break;

                (elements[c], elements[min]) = (elements[min], elements[c]);
                c = min;
            }
        }

        return itemsWithSamePriority;
    }

    public T Peek()
    {
        if (elements.Count == 0) throw new InvalidOperationException("The priority queue is empty");
        return elements[0].item;
    }
    public void Clear()
    {
        elements.Clear();
    }

}
