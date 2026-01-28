using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

/// <summary>
/// Brand entity representing product manufacturers (Intel, AMD, ASUS, MSI, etc.)
/// </summary>
public class Brand : AuditableEntity
{
    // Private constructor for EF Core
    private Brand() { }

    /// <summary>
    /// Creates a new Brand instance
    /// </summary>
    public Brand(string name, string slug)
    {
        Name = name;
        Slug = slug;
    }

    // Properties
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public int DisplayOrder { get; private set; } = 0;
    public bool IsActive { get; private set; } = true;
    public bool IsFeatured { get; private set; } = false;

    // Navigation properties (no virtual keyword - pure domain)
    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    // Domain Methods - Business Logic

    /// <summary>
    /// Updates brand basic information
    /// </summary>
    public void UpdateInfo(string name, string slug, string? description = null)
    {
        Name = name;
        Slug = slug;
        Description = description;
        MarkAsUpdated();
    }

    /// <summary>
    /// Sets the brand logo
    /// </summary>
    public void SetLogo(string logoUrl)
    {
        LogoUrl = logoUrl;
        MarkAsUpdated();
    }

    /// <summary>
    /// Sets the brand website URL
    /// </summary>
    public void SetWebsite(string websiteUrl)
    {
        WebsiteUrl = websiteUrl;
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
    /// Marks brand as featured
    /// </summary>
    public void MarkAsFeatured()
    {
        IsFeatured = true;
        MarkAsUpdated();
    }

    /// <summary>
    /// Removes featured status
    /// </summary>
    public void UnmarkAsFeatured()
    {
        IsFeatured = false;
        MarkAsUpdated();
    }

    /// <summary>
    /// Activates the brand
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    /// <summary>
    /// Deactivates the brand
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    /// <summary>
    /// Checks if brand has products
    /// </summary>
    public bool HasProducts() => Products.Any();

    /// <summary>
    /// Gets the count of active products for this brand
    /// </summary>
    public int GetActiveProductCount() => Products.Count(p => p.IsActive);

    /// <summary>
    /// Gets the count of products in stock
    /// </summary>
    public int GetInStockProductCount() => Products.Count(p => p.IsInStock());
}
