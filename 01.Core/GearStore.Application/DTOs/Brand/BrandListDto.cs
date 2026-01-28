namespace GearStore.Application.DTOs.Brand;

/// <summary>
/// DTO for brand list display (lightweight, for filters/navigation)
/// </summary>
public class BrandListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public int ProductCount { get; set; }
}
