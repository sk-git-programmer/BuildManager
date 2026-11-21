namespace BuildManager.Domain.Common;

/// <summary>
/// Marker for events raised by aggregates. Events are dispatched after the changes are saved.
/// </summary>
public interface IDomainEvent;
