namespace BuildManager.Domain.UnitTests.Common;

public sealed class EntityTests
{
    [Fact]
    public void Constructor_WithDefaultId_ThrowsArgumentException()
    {
        var act = () => new TestEntity(default);

        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithId_SetsId()
    {
        var id = TestId.New();

        var entity = new TestEntity(id);

        entity.Id.ShouldBe(id);
    }
}
