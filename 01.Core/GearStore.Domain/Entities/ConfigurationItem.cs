using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

/// <summary>
/// Chi tiết từng linh kiện trong cấu hình
/// </summary>
public class ConfigurationItem : BaseEntity
{
    public int ConfigurationId { get; set; }
    public int ProductId { get; set; }
    public int CategoryId { get; set; }  // Để biết linh kiện thuộc loại gì
    public int Quantity { get; set; } = 1;
    public decimal Price { get; set; }   // Lưu giá tại thời điểm thêm
    public int SavedConfigurationId { get; set; }

    // Navigation properties
    public virtual SavedConfiguration Configuration { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;

    // Helper method
    public decimal GetSubTotal() => Price * Quantity;
}