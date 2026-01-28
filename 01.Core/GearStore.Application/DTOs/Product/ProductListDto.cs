namespace GearStore.Application.DTOs.Product;

/// <summary>
/// DTO for product list display (lightweight, for grids/cards)
/// </summary>
public class ProductListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public string? PrimaryImageUrl { get; set; }
    
    // Category and Brand info
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    
    // Computed properties
    public bool IsOnSale => OriginalPrice.HasValue && OriginalPrice > Price;
    public decimal? DiscountPercentage => OriginalPrice.HasValue && OriginalPrice > 0 
        ? Math.Round((OriginalPrice.Value - Price) / OriginalPrice.Value * 100, 2) 
        : null;
}
