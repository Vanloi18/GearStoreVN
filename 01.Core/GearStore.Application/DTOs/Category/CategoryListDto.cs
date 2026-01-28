namespace GearStore.Application.DTOs.Category;

/// <summary>
/// DTO for category list display (lightweight, for navigation/menus)
/// </summary>
public class CategoryListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
}
