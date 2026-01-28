namespace GearStore.Application.DTOs.Brand;

/// <summary>
/// DTO for brand detail display (full information with product statistics)
/// </summary>
public class BrandDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    
    // Product counts
    public int TotalProductCount { get; set; }
    public int ActiveProductCount { get; set; }
    public int InStockProductCount { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
