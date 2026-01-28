namespace GearStore.Application.DTOs.Category;

/// <summary>
/// DTO for category detail display (full information with products)
/// </summary>
public class CategoryDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    
    // Product counts
    public int TotalProductCount { get; set; }
    public int ActiveProductCount { get; set; }
    public int InStockProductCount { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
