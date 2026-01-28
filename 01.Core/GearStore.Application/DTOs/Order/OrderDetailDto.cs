using GearStore.Domain.Enums;

namespace GearStore.Application.DTOs.Order;

/// <summary>
/// DTO for order detail display (full order information)
/// </summary>
public class OrderDetailDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    
    // Customer information
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string? UserId { get; set; }
    
    // Order status
    public OrderStatus Status { get; set; }
    
    // Payment information
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    
    // Financial details
    public decimal SubTotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    
    // Notes
    public string? Notes { get; set; }
    
    // Order items
    public List<OrderItemDto> Items { get; set; } = new();
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Computed
    public int ItemCount { get; set; }
}

/// <summary>
/// DTO for order item (product snapshot in order)
/// </summary>
public class OrderItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSKU { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotal { get; set; }
}
