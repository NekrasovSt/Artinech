using Common.Interfaces;

namespace Test;

public class Factories
{
    public static IInventory<T> CreateInventoryCollection<T>() where T : IItem
    {
        return new Inventory<T>();
    }
}