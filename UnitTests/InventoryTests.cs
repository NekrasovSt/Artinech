using Common.Interfaces;
using Test;

namespace UnitTests;

public class InventoryTests
{
    private readonly IInventory _inventoryCollection = Factories.CreateInventoryCollection();

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(150)]
    public void ValidateWeightIncorrect(int weight)
    {
        // Act
        Assert.Throws<InvalidOperationException>(() => _inventoryCollection.AddItem("Some", weight));
    }

    [Fact]
    public void OverWeight()
    {
        // Arrange
        _inventoryCollection.AddItem("Test", 99);

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            _inventoryCollection.AddItem("Test2", 99));
    }

    [Fact]
    public void AddItemValidWeight()
    {
        // Act
        _inventoryCollection.AddItem("Test", 20);

        // Assert
        Assert.Single(_inventoryCollection.Items);
    }

    [Fact]
    public void AddDifferentItem()
    {
        // Act
        _inventoryCollection.AddItem("Test1", 20);
        _inventoryCollection.AddItem("Test2", 20);

        // Assert
        Assert.Equal(2, _inventoryCollection.Items.Count);
    }

    [Fact]
    public void AddItemsSumWeight()
    {
        // Act
        _inventoryCollection.AddItem("Test", 20);
        _inventoryCollection.AddItem("Test", 20);

        // Assert
        Assert.Single(_inventoryCollection.Items);
        Assert.Equal(40, _inventoryCollection.Items[0].Weight);
    }

    [Fact]
    public void RemoveItem()
    {
        _inventoryCollection.AddItem("Test", 20);

        // Act
        _inventoryCollection.RemoveItem("Test");

        // Assert
        Assert.Empty(_inventoryCollection.Items);
    }

    [Fact]
    public void RemoveNotExistingItem()
    {
        _inventoryCollection.AddItem("Test", 20);

        // Act
        _inventoryCollection.RemoveItem("Test!");

        // Assert
        Assert.Single(_inventoryCollection.Items);
    }

    [Fact]
    public void FindItem()
    {
        // arrange
        var items = new[] { "vasya", "petya", "olga" };
        foreach (var name in items)
        {
            _inventoryCollection.AddItem(name, 20);
        }

        // Act
        var exist = _inventoryCollection.FindItemByName("lg");

        Assert.NotNull(exist);
        Assert.Equal("olga", exist.Name);
    }

    [Fact]
    public void NotExistingItem()
    {
        // arrange
        var items = new[] { "vasya", "petya", "olga" };
        foreach (var name in items)
        {
            _inventoryCollection.AddItem(name, 20);
        }

        // Act
        var exist = _inventoryCollection.FindItemByName("unknown");

        Assert.Null(exist);
    }

    [Fact]
    public async Task MultiThreadAddItem()
    {
        // Act
        var task1 = Task.Run(() =>
        {
            for (int i = 0; i < 40; i++)
            {
                _inventoryCollection.AddItem("Test1", 1);
            }
        });

        var task2 = Task.Run(() =>
        {
            for (int i = 0; i < 40; i++)
            {
                _inventoryCollection.AddItem("Test2", 1);
            }
        });
        await Task.WhenAll(task1, task2);

        // Assert
        Assert.Equal(2, _inventoryCollection.Items.Count);
        Assert.Equal(40, _inventoryCollection.Items[0].Weight);
        Assert.Equal(40, _inventoryCollection.Items[1].Weight);
    }
}