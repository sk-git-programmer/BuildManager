using BuildManager.Domain.Common;

namespace BuildManager.Domain.UnitTests.Common;

internal readonly record struct TestId(Guid Value)
{
    public static TestId New() => new(Guid.CreateVersion7());
}

internal sealed class TestEntity(TestId id) : Entity<TestId>(id);

internal sealed class TestAggregate(TestId id) : AggregateRoot<TestId>(id)
{
    public void RaiseEvent(IDomainEvent domainEvent) => Raise(domainEvent);
}

internal sealed record TestDomainEvent(TestId AggregateId) : IDomainEvent;
