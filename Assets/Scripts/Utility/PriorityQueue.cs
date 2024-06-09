using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<T> data;
    private readonly Comparer<T> comparer;

    public PriorityQueue(Comparer<T> comparer)
    {
        this.data = new List<T>();
        this.comparer = comparer;
    }

    public void Enqueue(T item)
    {
        data.Add(item);
        int ci = data.Count - 1; // child index; start at end
        while (ci > 0)
        {
            int pi = (ci - 1) / 2; // parent index
            if (comparer.Compare(data[ci], data[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
            T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
            ci = pi;
        }
    }

    public T Dequeue()
    {
        int li = data.Count - 1; // last index (before removal)
        T frontItem = data[0];   // fetch the front
        data[0] = data[li];
        data.RemoveAt(li);

        --li; // last index (after removal)
        int pi = 0; // parent index. start at front of pq
        while (true)
        {
            int ci = pi * 2 + 1; // left child index of parent
            if (ci > li) break;  // no children so done
            int rc = ci + 1;     // right child
            if (rc <= li && comparer.Compare(data[rc], data[ci]) < 0) // if there is a right child (ci + 1), and it is smaller than left child, use the right child instead
                ci = rc;
            if (comparer.Compare(data[pi], data[ci]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
            T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp; // swap parent and child
            pi = ci;
        }
        return frontItem;
    }

    public T Peek()
    {
        if (data.Count == 0)
            throw new InvalidOperationException("Priority queue is empty.");
        return data[0];
    }

    public bool Remove(T item)
    {
        int index = data.IndexOf(item);
        if (index == -1) return false;

        int li = data.Count - 1;
        if (index != li)
        {
            data[index] = data[li];
            data.RemoveAt(li);
            HeapifyDown(index);
            HeapifyUp(index);
        }
        else
        {
            data.RemoveAt(li);
        }
        return true;
    }

    public int Count => data.Count;

    private void HeapifyUp(int index)
    {
        int ci = index;
        while (ci > 0)
        {
            int pi = (ci - 1) / 2;
            if (comparer.Compare(data[ci], data[pi]) >= 0) break;
            T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
            ci = pi;
        }
    }

    private void HeapifyDown(int index)
    {
        int li = data.Count - 1;
        int pi = index;
        while (true)
        {
            int ci = pi * 2 + 1;
            if (ci > li) break;
            int rc = ci + 1;
            if (rc <= li && comparer.Compare(data[rc], data[ci]) < 0)
                ci = rc;
            if (comparer.Compare(data[pi], data[ci]) <= 0) break;
            T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp;
            pi = ci;
        }
    }
}