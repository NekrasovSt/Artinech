using System.Collections.Concurrent;
using Common.Interfaces;
using Test.Validation;

namespace Test;

internal class Inventory<T> : IInventory<T> where T : IItem
{
    private readonly Dictionary<string, T> _items = new();
    private readonly ReaderWriterLockSlim _lock = new();
    public IReadOnlyList<T> Items
    {
        get
        {
            _lock.EnterReadLock();
            try
            {
                return _items.Values.ToList().AsReadOnly();
            }
            finally
            {
                _lock.ExitReadLock();                
            }
        }
    }

    private readonly ItemValidator _itemValidator = new();

    public void AddItem(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (!_itemValidator.Validate(item).IsValid)
        {
            throw new InvalidOperationException();
        }
        _lock.EnterWriteLock();
        try
        {
            var currentSum = _items.Values.Sum(i => i.Weight);
            if (currentSum + item.Weight > ItemValidator.MAX_WEIGHT)
            {
                throw new InvalidOperationException();
            }

            if (_items.TryGetValue(item.Name, out var exist))
            {
                exist.Weight = item.Weight + exist.Weight;
            }
            else
            {
                _items.TryAdd(item.Name, item);
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool RemoveItem(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _lock.EnterWriteLock();
        try
        {
            return _items.Remove(item.Name, out _);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public T? FindItemByName(string substring)
    {
        _lock.EnterReadLock();
        try
        {
            var key = _items.Keys.FirstOrDefault(i => i.Contains(substring, StringComparison.CurrentCultureIgnoreCase));
            if (string.IsNullOrEmpty(key))
            {
                return default;
            }

            return _items.GetValueOrDefault(key);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
}