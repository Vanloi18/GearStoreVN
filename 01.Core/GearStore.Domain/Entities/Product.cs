using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

/// <summary>
/// Product entity representing items sold in the e-commerce store
/// </summary>
public class Product : AuditableEntity
{
    // Private constructor for EF Core
    private Product() { }

    /// <summary>
    /// Creates a new Product instance
    /// </summary>
    public Product(string name, string slug, int categoryId, int brandId, decimal price)
    {
        Name = name;
        Slug = slug;
        CategoryId = categoryId;
        BrandId = brandId;
        Price = price;
    }

    // Foreign Keys
    public int CategoryId { get; private set; }
    public int BrandId { get; private set; }

    // Basic Info
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? SKU { get; private set; }

    // Pricing
    public decimal Price { get; private set; }
    public decimal? OriginalPrice { get; private set; }

    // Inventory
    public int Stock { get; private set; } = 0;

    // Description
    public string? Description { get; private set; }
    public string? ShortDescription { get; private set; }
    public string? ThumbnailImage { get; private set; }

    // Status
    public bool IsActive { get; private set; } = true;
    public bool IsFeatured { get; private set; } = false;

    // Statistics
    public int ViewCount { get; private set; } = 0;
    public int SoldCount { get; private set; } = 0;

    // Navigation properties (no virtual keyword - pure domain)
    public Category Category { get; private set; } = null!;
    public Brand Brand { get; private set; } = null!;
    public ICollection<ProductSpec> Specs { get; private set; } = new List<ProductSpec>();
    public ICollection<ProductImage> Images { get; private set; } = new List<ProductImage>();
    public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
    public ICollection<ConfigurationItem> ConfigurationItems { get; private set; } = new List<ConfigurationItem>();
    public ICollection<ProductVariant> Variants { get; private set; } = new List<ProductVariant>();

    // Domain Methods - Business Logic

    /// <summary>
    /// Adds a specification to this product
    /// </summary>
    public ProductSpec AddSpec(string specKey, string specValue, string? displayName = null, int displayOrder = 0)
    {
        var spec = new ProductSpec(Id, specKey, specValue, displayName, displayOrder);
        Specs.Add(spec);
        MarkAsUpdated();
        return spec;
    }

    /// <summary>
    /// Updates product basic information
    /// </summary>
    public void UpdateBasicInfo(string name, string slug, string? sku, string? description, string? shortDescription)
    {
        Name = name;
        Slug = slug;
        SKU = sku;
        Description = description;
        ShortDescription = shortDescription;
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates product pricing
    /// </summary>
    public void UpdatePricing(decimal price, decimal? originalPrice = null)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        if (originalPrice.HasValue && originalPrice.Value < price)
            throw new ArgumentException("Original price cannot be less than current price", nameof(originalPrice));

        Price = price;
        OriginalPrice = originalPrice;
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates product stock quantity
    /// </summary>
    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Stock cannot be negative", nameof(quantity));

        Stock = quantity;
        MarkAsUpdated();
    }

    /// <summary>
    /// Decreases stock when product is sold
    /// </summary>
    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (Stock < quantity)
            throw new InvalidOperationException($"Insufficient stock. Available: {Stock}, Requested: {quantity}");

        Stock -= quantity;
        SoldCount += quantity;
        MarkAsUpdated();
    }

    /// <summary>
    /// Increases stock when product is restocked
    /// </summary>
    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        Stock += quantity;
        MarkAsUpdated();
    }

    /// <summary>
    /// Sets the thumbnail image
    /// </summary>
    public void SetThumbnail(string imageUrl)
    {
        ThumbnailImage = imageUrl;
        MarkAsUpdated();
    }

    /// <summary>
    /// Marks product as featured
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
    /// Activates the product
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    /// <summary>
    /// Deactivates the product
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    /// <summary>
    /// Increments view count
    /// </summary>
    public void IncrementViewCount()
    {
        ViewCount++;
    }

    /// <summary>
    /// Calculates discount percentage
    /// </summary>
    public decimal GetDiscountPercentage()
    {
        if (OriginalPrice.HasValue && OriginalPrice.Value > Price)
        {
            return Math.Round((OriginalPrice.Value - Price) / OriginalPrice.Value * 100, 0);
        }
        return 0;
    }

    /// <summary>
    /// Checks if product is in stock
    /// </summary>
    public bool IsInStock() => Stock > 0;

    /// <summary>
    /// Checks if product has discount
    /// </summary>
    public bool HasDiscount() => OriginalPrice.HasValue && OriginalPrice.Value > Price;

    /// <summary>
    /// Calculates discount amount
    /// </summary>
    public decimal GetDiscountAmount()
    {
        if (HasDiscount())
        {
            return OriginalPrice!.Value - Price;
        }
        return 0;
    }
}