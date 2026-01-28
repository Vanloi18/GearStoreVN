using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

/// <summary>
/// Product specification entity (EAV Pattern)
/// IMPORTANT: Used for compatibility checking in PC Builder
/// This is a child entity that cannot exist independently of Product
/// </summary>
public class ProductSpec : BaseEntity
{
    // Private constructor for EF Core
    private ProductSpec() { }

    /// <summary>
    /// Creates a new ProductSpec instance
    /// This constructor is internal - only Product aggregate can create specifications
    /// </summary>
    internal ProductSpec(
        int productId,
        string specKey,
        string specValue,
        string? displayName = null,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(specKey))
            throw new ArgumentException("Specification key is required", nameof(specKey));

        if (string.IsNullOrWhiteSpace(specValue))
            throw new ArgumentException("Specification value is required", nameof(specValue));

        ProductId = productId;
        SpecKey = specKey;
        SpecValue = specValue;
        DisplayName = displayName;
        DisplayOrder = displayOrder;
    }

    // Foreign Key
    public int ProductId { get; private set; }

    // Specification data (immutable after creation)
    public string SpecKey { get; private set; } = string.Empty;     // Socket, DDR, TDP, FormFactor...
    public string SpecValue { get; private set; } = string.Empty;   // LGA1700, DDR5, 125W, ATX...
    public string? DisplayName { get; private set; }                // "Socket CPU", "Loại RAM"
    public int DisplayOrder { get; private set; } = 0;

    // Navigation property (no virtual keyword - pure domain)
    public Product Product { get; private set; } = null!;

    // Domain Methods - Query only (specifications are immutable)

    /// <summary>
    /// Checks if this specification matches a given key
    /// </summary>
    public bool IsKey(string key)
    {
        return SpecKey.Equals(key, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if this specification matches a given value
    /// </summary>
    public bool HasValue(string value)
    {
        return SpecValue.Equals(value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if this specification key-value pair matches
    /// </summary>
    public bool Matches(string key, string value)
    {
        return IsKey(key) && HasValue(value);
    }
}
