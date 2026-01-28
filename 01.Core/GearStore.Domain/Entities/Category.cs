using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

/// <summary>
/// Product category entity (CPU, Mainboard, RAM, VGA, etc.)
/// </summary>
public class Category : AuditableEntity
{
    // Private constructor for EF Core
    private Category() { }

    /// <summary>
    /// Creates a new Category instance
    /// </summary>
    public Category(string name, string slug, int displayOrder = 0)
    {
        Name = name;
        Slug = slug;
        DisplayOrder = displayOrder;
    }

    // Properties
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Icon { get; private set; }
    public int DisplayOrder { get; private set; } = 0;
    public bool IsActive { get; private set; } = true;

    // Navigation properties (no virtual keyword - pure domain)
    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    // Domain Methods - Business Logic

    /// <summary>
    /// Updates category basic information
    /// </summary>
    public void UpdateInfo(string name, string slug, string? description = null)
    {
        Name = name;
        Slug = slug;
        Description = description;
        MarkAsUpdated();
    }

    /// <summary>
    /// Sets the category icon
    /// </summary>
    public void SetIcon(string iconUrl)
    {
        Icon = iconUrl;
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the display order for sorting
    /// </summary>
    public void UpdateDisplayOrder(int order)
    {
        if (order < 0)
            throw new ArgumentException("Display order cannot be negative", nameof(order));

        DisplayOrder = order;
        MarkAsUpdated();
    }

    /// <summary>
    /// Activates the category
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    /// <summary>
    /// Deactivates the category
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    /// <summary>
    /// Checks if category has products
    /// </summary>
    public bool HasProducts() => Products.Any();

    /// <summary>
    /// Gets the count of active products in this category
    /// </summary>
    public int GetActiveProductCount() => Products.Count(p => p.IsActive);

    /// <summary>
    /// Gets the count of products in stock
    /// </summary>
    public int GetInStockProductCount() => Products.Count(p => p.IsInStock());
}