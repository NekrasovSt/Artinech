using Common.Interfaces;

namespace UnitTests.Mock;

public class TestItem : IItem
{
    public string Name { get; set; }
    public int Weight { get; set; }
}