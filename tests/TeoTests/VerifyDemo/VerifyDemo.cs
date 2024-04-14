namespace TeoTests.VerifyDemo;

public class VerifyDemo
{
    [Fact]
    public async Task SimplyDemo()
    {
        // Arrange
        var book = ArrangeSimply();

        // Act
        var actual = Act(book);

        // Assert
        await Verify(actual);
    }

    [Fact]
    public async Task ComplexDemo()
    {
        // Arrange
        var items = ArrangeComplex();

        // Act
        var actual = Act(items);

        // Assert
        await Verify(actual);
    }

    [Fact]
    public void DeprecatedApproach()
    {
        // Arrange
        var items = ArrangeComplex();

        // Act
        var actual = Act(items);

        // Assert
        var expectedFirstItem = items.First();
        var actualFirstItem = ((List<Item>) actual).First();
        Assert.Equal(expectedFirstItem.Id, actualFirstItem.Id);
        Assert.Equal(expectedFirstItem.ExternalId, actualFirstItem.ExternalId);
        Assert.Equal(expectedFirstItem.Name, actualFirstItem.Name);
        Assert.Equal(expectedFirstItem.Amount, actualFirstItem.Amount);
        Assert.Equal(expectedFirstItem.CreatedAt, actualFirstItem.CreatedAt);
        Assert.Equal(expectedFirstItem.Timestamp, actualFirstItem.Timestamp);
        Assert.Equal(expectedFirstItem.IsDeleted, actualFirstItem.IsDeleted);

        var expectedSecondItem = items.Last();
        var actualSecondItem = ((List<Item>) actual).Last();
        Assert.Equal(expectedSecondItem.Id, actualSecondItem.Id);
        Assert.Equal(expectedSecondItem.ExternalId, actualSecondItem.ExternalId);
        Assert.Equal(expectedSecondItem.Name, actualSecondItem.Name);
        Assert.Equal(expectedSecondItem.Amount, actualSecondItem.Amount);
        Assert.Equal(expectedSecondItem.CreatedAt, actualSecondItem.CreatedAt);
        Assert.Equal(expectedSecondItem.Timestamp, actualSecondItem.Timestamp);
        Assert.Equal(expectedSecondItem.IsDeleted, actualSecondItem.IsDeleted);
    }

    private static object Act(object input) => input;

    private static object ArrangeSimply() =>
        new
        {
            Id = Guid.NewGuid(),
            Name = "Book",
            Amount = 64,
            Attribute = new
            {
                Title = "Surreal Numbers",
                Author = "Donald Ervin Knuth",
                CreatedAt = DateTime.UtcNow
            },
            EBook = true
        };

    private static List<Item> ArrangeComplex()
    {
        var externalId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        var items = new List<Item>
        {
            new(
                Id: Guid.NewGuid(),
                externalId,
                Name: "Knuth",
                Amount: 1,
                createdAt,
                Timestamp: DateTime.UtcNow.AddTicks(128),
                IsDeleted: false
            ),
            new(
                Id: Guid.NewGuid(),
                externalId,
                Name: "Conway",
                Amount: 2,
                createdAt,
                Timestamp: DateTime.UtcNow.AddTicks(256),
                IsDeleted: false
            )
        };

        return items;
    }

    private record Item(
        Guid Id,
        Guid ExternalId,
        string Name,
        int Amount,
        DateTime CreatedAt,
        DateTime Timestamp,
        bool IsDeleted
    );
}