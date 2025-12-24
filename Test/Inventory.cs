using System.Collections.Concurrent;
using Common.Interfaces;
using Test.Validation;

namespace Test;

internal class Inventory : IInventory
{
    private readonly Dictionary<string, int> _items = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public IReadOnlyList<IItem> Items
    {
        get
        {
            _lock.EnterReadLock();
            try
            {
                return _items.Keys.Select(i => new Item() { Name = i, Weight = _items[i] }).ToList().AsReadOnly();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    private readonly ItemValidator _itemValidator = new();

    public void AddItem(string name, int weight)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (!_itemValidator.Validate(new Item() { Name = name, Weight = weight }).IsValid)
        {
            throw new InvalidOperationException();
        }

        _lock.EnterWriteLock();
        try
        {
            var currentSum = _items.Values.Sum();
            if (currentSum + weight > ItemValidator.MAX_WEIGHT)
            {
                throw new InvalidOperationException();
            }

            if (_items.TryGetValue(name, out var exist))
            {
                _items[name] = exist + weight;
            }
            else
            {
                _items.TryAdd(name, weight);
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool RemoveItem(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        _lock.EnterWriteLock();
        try
        {
            return _items.Remove(name, out _);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public IItem? FindItemByName(string substring)
    {
        _lock.EnterReadLock();
        try
        {
            var key = _items.Keys.FirstOrDefault(i => i.Contains(substring, StringComparison.CurrentCultureIgnoreCase));
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (_items.TryGetValue(key, out var exist))
            {
                return new Item() { Name = key, Weight = exist };
            }

            return null;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
}