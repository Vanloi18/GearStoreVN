using GearStore.Domain.Common;
namespace GearStore.Domain.Entities;

/// <summary>
/// Sản phẩm
/// </summary>
public class Product : AuditableEntity
{
    // Foreign Keys
    public int CategoryId { get; set; }

    // Basic Info
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public string? Brand { get; set; }

    // Pricing
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }

    // Inventory
    public int Stock { get; set; } = 0;

    // Description
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? ThumbnailImage { get; set; }

    // Status
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;

    // Statistics
    public int ViewCount { get; set; } = 0;
    public int SoldCount { get; set; } = 0;

    // Navigation properties
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<ProductSpec> Specs { get; set; } = new List<ProductSpec>();
    public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<ConfigurationItem> ConfigurationItems { get; set; } = new List<ConfigurationItem>();

    // Helper methods
    public decimal GetDiscountPercentage()
    {
        if (OriginalPrice.HasValue && OriginalPrice.Value > Price)
        {
            return Math.Round((OriginalPrice.Value - Price) / OriginalPrice.Value * 100, 0);
        }
        return 0;
    }

    public bool IsInStock() => Stock > 0;
}