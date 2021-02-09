﻿using System;

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;

    public void Reset(int capacity)
    {
        if (items == null || capacity > items.Length)
        {
            items = new T[capacity];
        }
        else
        {
            for (int i = 0; i < items.Length; ++i)
                items[i] = default;
        }
        Count = 0;
    }

    public void Add(T item)
    {
        item.HeapIndex = Count;
        items[Count] = item;
        SortUp(item);
        Count++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        Count--;
        items[0] = items[Count];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count { get; private set; }


    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            if (childIndexLeft < Count)
            {
                int swapIndex = childIndexLeft;
                if (childIndexRight < Count)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}