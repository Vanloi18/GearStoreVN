namespace GearStore.Application.DTOs.Cart;

/// <summary>
/// DTO for shopping cart display
/// </summary>
public class CartDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    
    // Computed properties
    public int ItemCount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsEmpty { get; set; }
}

/// <summary>
/// DTO for cart item
/// </summary>
public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotal { get; set; }
    public int MaxStock { get; set; }
}
