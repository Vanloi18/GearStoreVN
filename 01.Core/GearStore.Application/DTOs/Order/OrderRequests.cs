using GearStore.Domain.Enums;

namespace GearStore.Application.DTOs.Order;

/// <summary>
/// Request DTO for creating a new order
/// </summary>
public class CreateOrderRequest
{
    public string? UserId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
    
    // Order items (from cart or direct)
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Request DTO for order item in create order
/// </summary>
public class CreateOrderItemRequest
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSKU { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// Request DTO for updating order status
/// </summary>
public class UpdateOrderStatusRequest
{
    public int OrderId { get; set; }
    public OrderStatus NewStatus { get; set; }
    public string? Notes { get; set; }
    public string? ChangedBy { get; set; }
}

/// <summary>
/// Request DTO for cancelling an order
/// </summary>
public class CancelOrderRequest
{
    public int OrderId { get; set; }
    public string? Reason { get; set; }
    public string? CancelledBy { get; set; }
}

/// <summary>
/// Request DTO for updating order payment status
/// </summary>
public class UpdatePaymentStatusRequest
{
    public int OrderId { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}
