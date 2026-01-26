using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

/// <summary>
/// Danh mục sản phẩm (CPU, Mainboard, RAM, VGA...)
/// </summary>
public class Category : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}