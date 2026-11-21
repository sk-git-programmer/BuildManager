namespace BuildManager.Domain.UnitTests.Common;

public sealed class AggregateRootTests
{
    [Fact]
    public void DomainEvents_WhenCreated_IsEmpty()
    {
        var aggregate = new TestAggregate(TestId.New());

        aggregate.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void Raise_WithEvent_AddsEventToDomainEvents()
    {
        var aggregate = new TestAggregate(TestId.New());
        var domainEvent = new TestDomainEvent(aggregate.Id);

        aggregate.RaiseEvent(domainEvent);

        aggregate.DomainEvents.ShouldBe([domainEvent]);
    }

    [Fact]
    public void Raise_WithNullEvent_ThrowsArgumentNullException()
    {
        var aggregate = new TestAggregate(TestId.New());

        var act = () => aggregate.RaiseEvent(null!);

        Should.Throw<ArgumentNullException>(act);
    }

    [Fact]
    public void ClearDomainEvents_AfterRaise_RemovesAllEvents()
    {
        var aggregate = new TestAggregate(TestId.New());
        aggregate.RaiseEvent(new TestDomainEvent(aggregate.Id));

        aggregate.ClearDomainEvents();

        aggregate.DomainEvents.ShouldBeEmpty();
    }
}
