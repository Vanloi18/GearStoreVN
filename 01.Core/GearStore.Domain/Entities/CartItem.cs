using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

/// <summary>
/// Cart item entity representing a product in the shopping cart
/// This is a child entity that cannot exist independently of Cart
/// </summary>
public class CartItem : AuditableEntity
{
    // Private constructor for EF Core
    private CartItem() { }

    /// <summary>
    /// Creates a new CartItem instance
    /// This constructor is internal - only Cart aggregate can create CartItems
    /// </summary>
    internal CartItem(int cartId, int productId, int? variantId, string productName, string? variantName, decimal price, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required", nameof(productName));

        CartId = cartId;
        ProductId = productId;
        ProductId = productId;
        VariantId = variantId;
        ProductName = productName;
        VariantName = variantName;
        Price = price;
        Quantity = quantity;
    }

    // Foreign Keys
    public int CartId { get; private set; }
    public int ProductId { get; private set; }
    public int? VariantId { get; private set; }

    // Product snapshot at time of adding to cart
    public string ProductName { get; private set; } = string.Empty;
    public string? VariantName { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    // Navigation properties (no virtual keyword - pure domain)
    public Cart Cart { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    // Domain Methods - Business Logic

    /// <summary>
    /// Updates the quantity of this cart item
    /// This method is internal - only Cart aggregate can modify CartItems
    /// </summary>
    internal void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

        Quantity = newQuantity;
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the price (useful when product price changes while in cart)
    /// This method is internal - only Cart aggregate can modify CartItems
    /// </summary>
    internal void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative", nameof(newPrice));

        Price = newPrice;
        MarkAsUpdated();
    }

    /// <summary>
    /// Calculates the subtotal for this cart item
    /// </summary>
    public decimal GetSubTotal()
    {
        return Price * Quantity;
    }

    /// <summary>
    /// Increases the quantity by a specified amount
    /// This method is internal - only Cart aggregate can modify CartItems
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
    /// This method is internal - only Cart aggregate can modify CartItems
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
