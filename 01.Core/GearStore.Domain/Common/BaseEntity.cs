namespace GearStore.Domain.Common;

/// <summary>
/// Base entity for all domain entities with identity
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    /// Determines whether this entity is transient (not yet persisted)
    /// </summary>
    public bool IsTransient() => Id == default;

    /// <summary>
    /// Equality comparison based on Id
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (IsTransient() || other.IsTransient())
            return false;

        return Id == other.Id;
    }

    /// <summary>
    /// Hash code based on Id
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(BaseEntity? left, BaseEntity? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(BaseEntity? left, BaseEntity? right)
    {
        return !(left == right);
    }
}

/// <summary>
/// Base entity with audit tracking (creation and modification timestamps)
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    /// <summary>
    /// Timestamp when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Timestamp when the entity was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Constructor initializes CreatedAt
    /// </summary>
    protected AuditableEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the entity as updated
    /// </summary>
    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}