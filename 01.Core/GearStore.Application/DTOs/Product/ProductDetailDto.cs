namespace GearStore.Application.DTOs.Product;

/// <summary>
/// DTO for product detail display (full information)
/// </summary>
public class ProductDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int Stock { get; set; }
    public int ViewCount { get; set; }
    public int SoldCount { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    
    // Category and Brand
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string? BrandLogoUrl { get; set; }
    
    // Images
    public List<ProductImageDto> Images { get; set; } = new();
    
    // Specifications
    public List<ProductSpecDto> Specifications { get; set; } = new();
    
    // Variants
    public List<ProductVariantDto> Variants { get; set; } = new();
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Computed properties
    public bool IsInStock => Stock > 0;
    public bool IsOnSale => OriginalPrice.HasValue && OriginalPrice > Price;
    public decimal? DiscountPercentage => OriginalPrice.HasValue && OriginalPrice > 0 
        ? Math.Round((OriginalPrice.Value - Price) / OriginalPrice.Value * 100, 2) 
        : null;
    public decimal? DiscountAmount => OriginalPrice.HasValue && OriginalPrice > Price 
        ? OriginalPrice.Value - Price 
        : null;
}

/// <summary>
/// DTO for product image
/// </summary>
public class ProductImageDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
}

/// <summary>
/// DTO for product specification
/// </summary>
public class ProductSpecDto
{
    public int Id { get; set; }
    public string SpecKey { get; set; } = string.Empty;
    public string SpecValue { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public int DisplayOrder { get; set; }
}
