using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject
{
    private List<T> items = new List<T>();

    [SerializeField] private GameEvent<List<T>> onItemsListChanged;

    public IReadOnlyList<T> Items => items;

    public void Add(T item)
    {
        if (items.Contains(item))
            return;

        items.Add(item);
        onItemsListChanged?.Raise(items);
    }

    public void Remove(T item)
    {
        if (!items.Contains(item))
            return;

        items.Remove(item);
        onItemsListChanged?.Raise(items);
    }
}