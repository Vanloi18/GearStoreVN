using GearStore.Domain.Enums;

namespace GearStore.Application.DTOs.Order;

/// <summary>
/// Request DTO for checkout (converting cart to order)
/// </summary>
public class CheckoutRequest
{
    public string? SessionId { get; set; }
    public string? UserId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
}
