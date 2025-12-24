using Common.Interfaces;

namespace Test;

internal class Item: IItem
{
    public string Name { get; init; }
    public int Weight { get; init; }
}
