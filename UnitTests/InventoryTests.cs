using Common.Interfaces;
using Test;
using UnitTests.Mock;

namespace UnitTests;

public class InventoryTests
{
    private readonly IInventory<TestItem> _inventoryCollection;

    public InventoryTests()
    {
        _inventoryCollection = Factories.CreateInventoryCollection<TestItem>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(150)]
    public void ValidateWeightIncorrect(int weight)
    {
        // arrange
        var item = new TestItem
        {
            Weight = weight
        };

        // Act
        Assert.Throws<InvalidOperationException>(() => _inventoryCollection.AddItem(item));
    }

    [Fact]
    public void OverWeight()
    {
        // Arrange
        var item = new TestItem
        {
            Weight = 99,
            Name = "Test"
        };
        _inventoryCollection.AddItem(item);

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            _inventoryCollection.AddItem(new TestItem() { Name = "Test2", Weight = 99 }));
    }

    [Fact]
    public void AddItemValidWeight()
    {
        // arrange
        var item = new TestItem
        {
            Weight = 20,
            Name = "Test"
        };

        // Act
        _inventoryCollection.AddItem(item);

        // Assert
        Assert.Single(_inventoryCollection.Items);
    }

    [Fact]
    public void AddDifferentItem()
    {
        // arrange
        var item1 = new TestItem
        {
            Weight = 20,
            Name = "Test1"
        };
        var item2 = new TestItem
        {
            Weight = 20,
            Name = "Test2"
        };

        // Act
        _inventoryCollection.AddItem(item1);
        _inventoryCollection.AddItem(item2);

        // Assert
        Assert.Equal(2, _inventoryCollection.Items.Count);
    }

    [Fact]
    public void AddItemsSumWeight()
    {
        // arrange
        var item1 = new TestItem
        {
            Weight = 20,
            Name = "Test"
        };
        var item2 = new TestItem
        {
            Weight = 20,
            Name = "Test"
        };

        // Act
        _inventoryCollection.AddItem(item1);
        _inventoryCollection.AddItem(item2);

        // Assert
        Assert.Single(_inventoryCollection.Items);
        Assert.Equal(40, _inventoryCollection.Items[0].Weight);
    }

    [Fact]
    public void RemoveItem()
    {
        // arrange
        var item = new TestItem
        {
            Weight = 20,
            Name = "Test"
        };
        _inventoryCollection.AddItem(item);

        // Act
        _inventoryCollection.RemoveItem(new TestItem() { Name = "Test" });

        // Assert
        Assert.Empty(_inventoryCollection.Items);
    }

    [Fact]
    public void RemoveNotExistingItem()
    {
        // arrange
        var item = new TestItem
        {
            Weight = 20,
            Name = "Test"
        };
        _inventoryCollection.AddItem(item);

        // Act
        _inventoryCollection.RemoveItem(new TestItem() { Name = "Test!" });

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
            var item = new TestItem
            {
                Weight = 20,
                Name = name
            };
            _inventoryCollection.AddItem(item);
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
            var item = new TestItem
            {
                Weight = 20,
                Name = name
            };
            _inventoryCollection.AddItem(item);
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
                _inventoryCollection.AddItem(new TestItem() { Name = "Test1", Weight = 1 });
            }
        });
        
        var task2 = Task.Run(() =>
        {
            for (int i = 0; i < 40; i++)
            {
                _inventoryCollection.AddItem(new TestItem() { Name = "Test2", Weight = 1 });
            }
        });
        await Task.WhenAll(task1, task2);
        
        // Assert
        Assert.Equal(2, _inventoryCollection.Items.Count);
        Assert.Equal(40, _inventoryCollection.Items[0].Weight);
        Assert.Equal(40, _inventoryCollection.Items[1].Weight);
    }
}