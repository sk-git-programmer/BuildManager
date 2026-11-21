namespace BuildManager.Domain.Common;

/// <summary>
/// Base class for entities identified by a strongly-typed ID.
/// Equality is not overridden: compare identities explicitly (<c>a.Id == b.Id</c>).
/// </summary>
public abstract class Entity<TId>
    where TId : struct, IEquatable<TId>
{
    protected Entity(TId id)
    {
        if (id.Equals(default))
        {
            throw new ArgumentException("Entity id must not be the default value.", nameof(id));
        }

        Id = id;
    }

    public TId Id { get; }
}
