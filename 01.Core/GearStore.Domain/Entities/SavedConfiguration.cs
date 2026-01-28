using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

/// <summary>
/// Cấu hình PC đã lưu
/// User có thể lưu nhiều cấu hình để tham khảo hoặc mua sau
/// </summary>
public class SavedConfiguration : AuditableEntity
{
    // Foreign Key - NULL nếu khách chưa đăng nhập
    public string? UserId { get; set; }

    public string? ConfigurationName { get; set; }
    public decimal TotalPrice { get; set; }
    public int? EstimatedWattage { get; set; }  // Tổng công suất ước tính (W)
    public string? Notes { get; set; }
    public bool IsPublic { get; set; } = false; // Cho phép người khác xem
    public decimal TotalEstimatedPrice { get; set; }

    // Navigation property
    public virtual ICollection<ConfigurationItem> Items { get; set; } = new List<ConfigurationItem>();

    // Helper method
    public void CalculateTotalPrice()
    {
        TotalPrice = Items.Sum(i => i.Price * i.Quantity);
    }

    public void CalculateEstimatedWattage()
    {
        // Logic tính toán tổng công suất từ TDP của các linh kiện
        // Sẽ implement chi tiết ở Application layer
        EstimatedWattage = 0; // Placeholder
    }
}
