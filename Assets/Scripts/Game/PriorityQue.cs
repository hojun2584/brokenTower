using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyHeap<T>
{
    const int root = 1;
    const int minSize = 10;
    public int size = 0; // 현재 힙에 있는 요소의 개수
    public int capacity;

    readonly int multipleArray = 2;
    public T[] values;
    public Func<T, T, bool> Compare;

    public MyHeap( Func<T, T, bool> Compare , int initialCapacity = minSize )
    {
        this.Compare = Compare;
        values = new T[initialCapacity];
        capacity = initialCapacity;
    }

    public void InsertValue(T value)
    {
        if (capacity <= size + 1) // 배열 크기 조정
        {
            capacity *= multipleArray;
            Array.Resize(ref values, capacity);
        }

        size++;
        values[size] = value;
        ReUp(size);
    }

    public T Peek()
    {
        if (size == 0)
            throw new InvalidOperationException("Heap is empty");

        return values[root];
    }

    public T Pop()
    {
        if (size == 0)
            throw new InvalidOperationException("Heap is empty");

        T result = values[root];
        values[root] = values[size];
        size--;
        ReDown(root);

        return result;
    }

    void Swap(int parent, int child)
    {
        T temp = values[parent];
        values[parent] = values[child];
        values[child] = temp;
    }

    void ReUp(int child)
    {
        int parent = child / 2;

        if (parent == 0)
            return;

        if (Compare(values[parent], values[child]))
        {
            Swap(parent, child);
            ReUp(parent);
        }
    }

    void ReDown(int parent)
    {
        int child = parent * 2;

        if (child > size)
            return;

        if (child + 1 <= size && Compare(values[child], values[child + 1]))
            child = child + 1;

        if (Compare(values[parent], values[child]))
        {
            Swap(parent, child);
            ReDown(child);
        }
    }

    public void Print()
    {
        for (int i = 1; i <= size; i++)
        {
            Debug.Log(values[i]);
        }
    }

    public void Clear()
    {
        values = new T[minSize];
    }
}

public class CustomPriorityQue<T>
{
    MyHeap<T> heap;

    public int Capacity
    {
        get => heap.capacity;
    }

    public int Count
    {
        get => heap.size;
    }

    public CustomPriorityQue(Func<T, T, bool> Compare)
    {
        heap = new MyHeap<T>(Compare);
    }

    public void Enque(T item)
    {
        heap.InsertValue(item);
    }

    public T Deque()
    {
        return heap.Pop();
    }
    public T Peek()
    {
        return heap.Peek();
    }

    public void Clear()
    {
        heap.Clear();
    }

}
