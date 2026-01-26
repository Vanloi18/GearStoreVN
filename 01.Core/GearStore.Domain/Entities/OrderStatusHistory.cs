using GearStore.Domain.Common;
using GearStore.Domain.Enums;

namespace GearStore.Domain.Entities;

/// <summary>
/// Lịch sử thay đổi trạng thái đơn hàng
/// Dùng để tracking và audit
/// </summary>
public class OrderStatusHistory : BaseEntity
{
    public int OrderId { get; set; }
    public string? OldStatus { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? ChangedBy { get; set; }  // UserId của admin
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Order Order { get; set; } = null!;
}