using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

/// <summary>
/// Product image entity
/// This is a child entity that cannot exist independently of Product
/// </summary>
public class ProductImage : AuditableEntity
{
    // Private constructor for EF Core
    private ProductImage() { }

    /// <summary>
    /// Creates a new ProductImage instance
    /// This constructor is internal - only Product aggregate can create images
    /// </summary>
    internal ProductImage(
        int productId,
        string imageUrl,
        string? altText = null,
        int displayOrder = 0,
        bool isPrimary = false)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Image URL is required", nameof(imageUrl));

        ProductId = productId;
        ImageUrl = imageUrl;
        AltText = altText;
        DisplayOrder = displayOrder;
        IsPrimary = isPrimary;
    }

    // Foreign Key
    public int ProductId { get; private set; }

    // Image data
    public string ImageUrl { get; private set; } = string.Empty;
    public string? AltText { get; private set; }
    public int DisplayOrder { get; private set; } = 0;
    public bool IsPrimary { get; private set; } = false;

    // Navigation property (no virtual keyword - pure domain)
    public Product Product { get; private set; } = null!;

    // Domain Methods

    /// <summary>
    /// Marks this image as the primary image
    /// This method is internal - only Product aggregate can modify images
    /// </summary>
    internal void MarkAsPrimary()
    {
        IsPrimary = true;
        MarkAsUpdated();
    }

    /// <summary>
    /// Unmarks this image as primary
    /// This method is internal - only Product aggregate can modify images
    /// </summary>
    internal void UnmarkAsPrimary()
    {
        IsPrimary = false;
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the display order
    /// This method is internal - only Product aggregate can modify images
    /// </summary>
    internal void UpdateDisplayOrder(int newOrder)
    {
        if (newOrder < 0)
            throw new ArgumentException("Display order cannot be negative", nameof(newOrder));

        DisplayOrder = newOrder;
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the alt text
    /// This method is internal - only Product aggregate can modify images
    /// </summary>
    internal void UpdateAltText(string? altText)
    {
        AltText = altText;
        MarkAsUpdated();
    }
}
