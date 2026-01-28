using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

/// <summary>
/// Order item entity representing a product in an order
/// This is a child entity that cannot exist independently of Order
/// </summary>
public class OrderItem : AuditableEntity
{
    // Private constructor for EF Core
    private OrderItem() { }

    /// <summary>
    /// Creates a new OrderItem instance
    /// This constructor is internal - only Order aggregate can create OrderItems
    /// </summary>
    internal OrderItem(
        int orderId,
        int productId,
        string productName,
        string? productSKU,

        int? variantId,
        string? variantName,
        decimal price,
        int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required", nameof(productName));

        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        ProductSKU = productSKU;
        VariantId = variantId;
        VariantName = variantName;
        Price = price;
        Quantity = quantity;
    }

    // Foreign Keys
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public int? VariantId { get; private set; }

    // Product snapshot at time of order (prevents issues if product is deleted/modified)
    public string ProductName { get; private set; } = string.Empty;
    public string? VariantName { get; private set; }
    public string? ProductSKU { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    // Navigation properties (no virtual keyword - pure domain)
    public Order Order { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    // Domain Methods - Business Logic

    /// <summary>
    /// Updates the quantity of this order item
    /// This method is internal - only Order aggregate can modify OrderItems
    /// </summary>
    internal void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

        Quantity = newQuantity;
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the price (useful if price changes before order is confirmed)
    /// This method is internal - only Order aggregate can modify OrderItems
    /// </summary>
    internal void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative", nameof(newPrice));

        Price = newPrice;
        MarkAsUpdated();
    }

    /// <summary>
    /// Calculates the subtotal for this order item (Price × Quantity)
    /// </summary>
    public decimal GetSubTotal()
    {
        return Price * Quantity;
    }

    /// <summary>
    /// Increases the quantity by a specified amount
    /// This method is internal - only Order aggregate can modify OrderItems
    /// </summary>
    internal void IncreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        Quantity += amount;
        MarkAsUpdated();
    }

    /// <summary>
    /// Decreases the quantity by a specified amount
    /// This method is internal - only Order aggregate can modify OrderItems
    /// </summary>
    internal void DecreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        if (Quantity - amount <= 0)
            throw new InvalidOperationException("Resulting quantity would be zero or negative. Use RemoveItem instead.");

        Quantity -= amount;
        MarkAsUpdated();
    }
}
