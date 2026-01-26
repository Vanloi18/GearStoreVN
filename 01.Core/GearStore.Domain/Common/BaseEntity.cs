namespace GearStore.Domain.Common;

/// <summary>
/// Base entity cho tất cả entities có Id và timestamp
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
}

/// <summary>
/// Base entity có audit fields (CreatedAt, UpdatedAt)
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}