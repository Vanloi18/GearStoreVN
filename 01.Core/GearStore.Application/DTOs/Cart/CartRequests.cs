namespace GearStore.Application.DTOs.Cart;

/// <summary>
/// Request DTO for adding item to cart
/// </summary>
public class AddToCartRequest
{
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public int ProductId { get; set; }
    public int? VariantId { get; set; }
    public int Quantity { get; set; } = 1;
}

/// <summary>
/// Request DTO for updating cart item quantity
/// </summary>
public class UpdateCartItemRequest
{
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// Request DTO for removing item from cart
/// </summary>
public class RemoveFromCartRequest
{
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public int ProductId { get; set; }
}

/// <summary>
/// Request DTO for clearing cart
/// </summary>
public class ClearCartRequest
{
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
}

/// <summary>
/// Request DTO for merging guest cart to user cart
/// </summary>
public class MergeCartRequest
{
    public string SessionId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
