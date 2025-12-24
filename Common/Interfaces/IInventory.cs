namespace Common.Interfaces;

public interface IInventory<T> where T : IItem
{
    IReadOnlyList<T> Items { get; }

    void AddItem(T item);

    bool RemoveItem(T item);
    
    T? FindItemByName(string substring);
}