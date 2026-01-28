using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

/// <summary>
/// Shopping cart aggregate root
/// Manages cart items and enforces business rules for the shopping cart
/// </summary>
public class Cart : AuditableEntity
{
    // Private constructor for EF Core
    private Cart() 
    {
        _items = new List<CartItem>();
    }

    /// <summary>
    /// Creates a new Cart instance for a logged-in user
    /// </summary>
    public Cart(string userId) : this()
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
    }

    /// <summary>
    /// Creates a new Cart instance for a guest session
    /// </summary>
    public static Cart CreateGuestCart(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));

        return new Cart
        {
            SessionId = sessionId
        };
    }

    // Properties
    public string? UserId { get; private set; }
    public string? SessionId { get; private set; }

    // Navigation properties (no virtual keyword - pure domain)
    private readonly List<CartItem> _items = new();
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    // Domain Methods - Business Logic

    /// <summary>
    /// Adds a product to the cart or updates quantity if already exists
    /// </summary>
    public void AddItem(int productId, int? variantId, string productName, string? variantName, decimal price, int quantity = 1)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required", nameof(productName));

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId && i.VariantId == variantId);

        if (existingItem != null)
        {
            // Update quantity if item already exists
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            // Add new item
            var cartItem = new CartItem(this.Id, productId, variantId, productName, variantName, price, quantity);
            _items.Add(cartItem);
        }

        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the quantity of a specific cart item
    /// </summary>
    public void UpdateItemQuantity(int productId, int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        
        if (item == null)
            throw new InvalidOperationException($"Product with ID {productId} not found in cart");

        item.UpdateQuantity(newQuantity);
        MarkAsUpdated();
    }

    /// <summary>
    /// Removes a specific item from the cart
    /// </summary>
    public void RemoveItem(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        
        if (item == null)
            throw new InvalidOperationException($"Product with ID {productId} not found in cart");

        _items.Remove(item);
        MarkAsUpdated();
    }

    /// <summary>
    /// Clears all items from the cart
    /// </summary>
    public void Clear()
    {
        _items.Clear();
        MarkAsUpdated();
    }

    /// <summary>
    /// Calculates the total amount of all items in the cart
    /// </summary>
    public decimal GetTotalAmount()
    {
        return _items.Sum(item => item.GetSubTotal());
    }

    /// <summary>
    /// Gets the total number of items in the cart
    /// </summary>
    public int GetItemCount()
    {
        return _items.Sum(item => item.Quantity);
    }

    /// <summary>
    /// Gets the number of unique products in the cart
    /// </summary>
    public int GetUniqueProductCount()
    {
        return _items.Count;
    }

    /// <summary>
    /// Checks if the cart is empty
    /// </summary>
    public bool IsEmpty()
    {
        return !_items.Any();
    }

    /// <summary>
    /// Checks if a specific product is in the cart
    /// </summary>
    public bool ContainsProduct(int productId)
    {
        return _items.Any(i => i.ProductId == productId);
    }

    /// <summary>
    /// Gets the quantity of a specific product in the cart
    /// </summary>
    public int GetProductQuantity(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        return item?.Quantity ?? 0;
    }

    /// <summary>
    /// Validates cart before checkout (can be extended with business rules)
    /// </summary>
    public void ValidateForCheckout()
    {
        if (IsEmpty())
            throw new InvalidOperationException("Cannot checkout an empty cart");

        if (_items.Any(i => i.Quantity <= 0))
            throw new InvalidOperationException("Cart contains items with invalid quantities");

        if (_items.Any(i => i.Price < 0))
            throw new InvalidOperationException("Cart contains items with invalid prices");
    }

    /// <summary>
    /// Converts guest cart to user cart when user logs in
    /// </summary>
    public void ConvertToUserCart(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (!string.IsNullOrWhiteSpace(UserId))
            throw new InvalidOperationException("Cart is already associated with a user");

        UserId = userId;
        SessionId = null; // Clear session ID after conversion
        MarkAsUpdated();
    }

    /// <summary>
    /// Merges another cart into this cart (useful when guest cart merges with user cart)
    /// </summary>
    public void MergeWith(Cart otherCart)
    {
        if (otherCart == null)
            throw new ArgumentNullException(nameof(otherCart));

        if (otherCart.Id == this.Id)
            throw new InvalidOperationException("Cannot merge cart with itself");

        foreach (var item in otherCart.Items)
        {
            AddItem(item.ProductId, item.VariantId, item.ProductName, item.VariantName, item.Price, item.Quantity);
        }
    }
}
