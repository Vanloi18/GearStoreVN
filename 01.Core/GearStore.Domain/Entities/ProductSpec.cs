using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

/// <summary>
/// Thông số kỹ thuật sản phẩm (EAV Pattern)
/// QUAN TRỌNG: Dùng để kiểm tra tương thích trong PC Builder
/// </summary>
public class ProductSpec : BaseEntity
{
    public int ProductId { get; set; }
    public string SpecKey { get; set; } = string.Empty;     // Socket, DDR, TDP, FormFactor...
    public string SpecValue { get; set; } = string.Empty;   // LGA1700, DDR5, 125W, ATX...
    public string? DisplayName { get; set; }                // "Socket CPU", "Loại RAM"
    public int DisplayOrder { get; set; } = 0;

    // Navigation property
    public virtual Product Product { get; set; } = null!;
}