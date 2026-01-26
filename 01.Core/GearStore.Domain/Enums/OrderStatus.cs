namespace GearStore.Domain.Enums;

/// <summary>
/// Trạng thái đơn hàng
/// </summary>
public enum OrderStatus
{
    Pending = 1,        // Chờ xử lý
    Processing = 2,     // Đang xử lý
    Shipping = 3,       // Đang giao hàng
    Completed = 4,      // Hoàn thành
    Cancelled = 5       // Đã hủy
}