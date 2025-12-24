namespace Common.Interfaces;

public interface IInventory
{
    IReadOnlyList<IItem> Items { get; }

    void AddItem(string name, int weight);

    bool RemoveItem(string name);
    
    IItem? FindItemByName(string substring);
}