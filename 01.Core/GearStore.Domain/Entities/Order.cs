using GearStore.Domain.Common;
using GearStore.Domain.Enums;

namespace GearStore.Domain.Entities;

/// <summary>
/// Đơn hàng
/// </summary>
public class Order : AuditableEntity
{
    // Foreign Key - NULL nếu khách vãng lai
    public string? UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    // Thông tin khách hàng
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;

    // Thông tin đơn hàng
    public decimal SubTotal { get; set; }
    public decimal ShippingFee { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public decimal TotalAmount { get; set; }

    // Trạng thái & Thanh toán
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    // Ghi chú
    public string? Notes { get; set; }
    public string? AdminNotes { get; set; }

    // Navigation properties
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<OrderStatusHistory> StatusHistories { get; set; } = new List<OrderStatusHistory>();

    // Helper methods
    public void CalculateTotalAmount()
    {
        TotalAmount = SubTotal + ShippingFee - Discount;
    }

    public string GetStatusText()
    {
        return Status switch
        {
            OrderStatus.Pending => "Chờ xử lý",
            OrderStatus.Processing => "Đang xử lý",
            OrderStatus.Shipping => "Đang giao hàng",
            OrderStatus.Completed => "Hoàn thành",
            OrderStatus.Cancelled => "Đã hủy",
            _ => "Không xác định"
        };
    }

    public string GetPaymentMethodText()
    {
        return PaymentMethod switch
        {
            Enums.PaymentMethod.COD => "Tiền mặt (COD)",
            Enums.PaymentMethod.BankTransfer => "Chuyển khoản",
            _ => "Không xác định"
        };
    }

    public bool CanBeCancelled()
    {
        return Status == OrderStatus.Pending || Status == OrderStatus.Processing;
    }
}
