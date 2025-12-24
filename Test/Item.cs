using Common.Interfaces;

namespace Test;

internal class Item: IItem
{
    public string Name { get; set; }
    public int Weight { get; set; }
}
