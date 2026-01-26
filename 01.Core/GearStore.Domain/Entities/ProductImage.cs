using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

/// <summary>
/// Hình ảnh sản phẩm
/// </summary>
public class ProductImage : AuditableEntity
{
    public int ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsPrimary { get; set; } = false;

    // Navigation property
    public virtual Product Product { get; set; } = null!;
}