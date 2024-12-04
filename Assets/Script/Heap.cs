
using System;

using Unity.VisualScripting;

public class Heap<T> where T : IHeapItem<T>
{
    private T[] itemsArray;
    private int currentItemsCount;

    public int Count
    {
        get { return currentItemsCount; }
    }

    public Heap(int itemsArray)
    {
        this.itemsArray = new T[itemsArray];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemsCount;
        itemsArray[currentItemsCount] = item;
        SortUp(item);
        currentItemsCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = itemsArray[0];
        
        currentItemsCount--;
        itemsArray[0] = itemsArray[currentItemsCount];
        itemsArray[0].HeapIndex = 0;

        SortDown(itemsArray[0]);

        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public bool Contains(T item)
    {
        return Equals(itemsArray[item.HeapIndex], item);
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = (item.HeapIndex * 2) + 1;
            int childIndexRight = (item.HeapIndex * 2) + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemsCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemsCount)
                {
                    if (itemsArray[childIndexLeft].CompareTo(itemsArray[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(itemsArray[swapIndex]) < 0)
                {
                    Swap(item, itemsArray[swapIndex]);
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

    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while(true)
        {
            T parentItem = itemsArray[parentIndex];
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

    private void Swap(T itemA, T itemB)
    {
        itemsArray[itemA.HeapIndex] = itemB;
        itemsArray[itemB.HeapIndex] = itemA;

        int tmpItemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = tmpItemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}

