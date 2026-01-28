using GearStore.Domain.Common;
using GearStore.Domain.Enums;

namespace GearStore.Domain.Entities;

/// <summary>
/// Order status change history entity for tracking and auditing
/// This is a child entity that cannot exist independently of Order
/// </summary>
public class OrderStatusHistory : AuditableEntity
{
    // Private constructor for EF Core
    private OrderStatusHistory() { }

    /// <summary>
    /// Creates a new OrderStatusHistory instance
    /// This constructor is internal - only Order aggregate can create status history entries
    /// </summary>
    internal OrderStatusHistory(
        int orderId,
        OrderStatus? oldStatus,
        OrderStatus newStatus,
        string? notes = null,
        string? changedBy = null)
    {
        OrderId = orderId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        Notes = notes;
        ChangedBy = changedBy;
        ChangedAt = DateTime.UtcNow;
    }

    // Foreign Key
    public int OrderId { get; private set; }

    // Status tracking (using enum instead of string)
    public OrderStatus? OldStatus { get; private set; }
    public OrderStatus NewStatus { get; private set; }

    // Audit information
    public string? Notes { get; private set; }
    public string? ChangedBy { get; private set; }  // UserId of admin or system
    public DateTime ChangedAt { get; private set; }

    // Navigation property (no virtual keyword - pure domain)
    public Order Order { get; private set; } = null!;

    // Domain Methods - Query only (no modifications after creation)

    /// <summary>
    /// Checks if this was a status upgrade (moving forward in the order lifecycle)
    /// </summary>
    public bool IsStatusUpgrade()
    {
        if (!OldStatus.HasValue)
            return true; // Initial status creation

        return NewStatus > OldStatus.Value;
    }

    /// <summary>
    /// Checks if this was a cancellation
    /// </summary>
    public bool IsCancellation()
    {
        return NewStatus == OrderStatus.Cancelled;
    }

    /// <summary>
    /// Checks if this was a completion
    /// </summary>
    public bool IsCompletion()
    {
        return NewStatus == OrderStatus.Completed;
    }
}
