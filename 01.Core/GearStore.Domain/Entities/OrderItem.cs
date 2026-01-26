using GearStore.Domain.Common;
using GearStore.Domain.Enums;

namespace GearStore.Domain.Entities;

/// <summary>
/// Chi tiết sản phẩm trong đơn hàng
/// </summary>
public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    
    // Lưu thông tin sản phẩm tại thời điểm mua (đề phòng product bị xóa/sửa)
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSKU { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotal { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;

    // Helper method
    public void CalculateSubTotal()
    {
        SubTotal = Price * Quantity;
    }
}