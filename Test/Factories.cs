using Common.Interfaces;

namespace Test;

public class Factories
{
    public static IInventory CreateInventoryCollection()
    {
        return new Inventory();
    }
}