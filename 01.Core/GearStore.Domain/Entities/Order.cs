using GearStore.Domain.Common;
using GearStore.Domain.Enums;

namespace GearStore.Domain.Entities;

/// <summary>
/// Order aggregate root
/// Manages order items, status transitions, and enforces business rules for order processing
/// </summary>
public class Order : AuditableEntity
{
    // Private constructor for EF Core
    private Order()
    {
        _items = new List<OrderItem>();
        _statusHistories = new List<OrderStatusHistory>();
    }

    /// <summary>
    /// Creates a new Order instance
    /// </summary>
    public Order(
        string orderNumber,
        string customerName,
        string customerPhone,
        string shippingAddress,
        PaymentMethod paymentMethod,
        string? userId = null,
        string? customerEmail = null,
        string? notes = null) : this()
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException("Order number is required", nameof(orderNumber));

        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name is required", nameof(customerName));

        if (string.IsNullOrWhiteSpace(customerPhone))
            throw new ArgumentException("Customer phone is required", nameof(customerPhone));

        if (string.IsNullOrWhiteSpace(shippingAddress))
            throw new ArgumentException("Shipping address is required", nameof(shippingAddress));

        OrderNumber = orderNumber;
        CustomerName = customerName;
        CustomerPhone = customerPhone;
        ShippingAddress = shippingAddress;
        PaymentMethod = paymentMethod;
        UserId = userId;
        CustomerEmail = customerEmail;
        Notes = notes;

        Status = OrderStatus.Pending;
        PaymentStatus = PaymentStatus.Unpaid;

        // Log initial status
        _statusHistories.Add(new OrderStatusHistory(this.Id, null, OrderStatus.Pending, "Order created"));
    }

    // Properties
    public string? UserId { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;

    // Customer information
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public string? CustomerEmail { get; private set; }
    public string ShippingAddress { get; private set; } = string.Empty;

    // Order financial information
    public decimal ShippingFee { get; private set; } = 0;
    public decimal Discount { get; private set; } = 0;

    // Status and payment
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public PaymentMethod PaymentMethod { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Unpaid;

    // Notes
    public string? Notes { get; private set; }
    public string? AdminNotes { get; private set; }

    // Navigation properties (no virtual keyword - pure domain)
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _items.AsReadOnly();

    private readonly List<OrderStatusHistory> _statusHistories = new();
    public IReadOnlyCollection<OrderStatusHistory> StatusHistories => _statusHistories.AsReadOnly();

    // Domain Methods - Order Item Management

    /// <summary>
    /// Adds a product to the order or updates quantity if already exists
    /// </summary>
    public void AddItem(int productId, string productName, string? productSKU, int? variantId, string? variantName, decimal price, int quantity)
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Cannot add items to a completed order");

        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot add items to a cancelled order");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId && i.VariantId == variantId);

        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var orderItem = new OrderItem(this.Id, productId, productName, productSKU, variantId, variantName, price, quantity);
            _items.Add(orderItem);
        }

        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the quantity of a specific order item
    /// </summary>
    public void UpdateItemQuantity(int productId, int newQuantity)
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Cannot modify a completed order");

        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot modify a cancelled order");

        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

        var item = _items.FirstOrDefault(i => i.ProductId == productId);

        if (item == null)
            throw new InvalidOperationException($"Product with ID {productId} not found in order");

        item.UpdateQuantity(newQuantity);
        MarkAsUpdated();
    }

    /// <summary>
    /// Removes a specific item from the order
    /// </summary>
    public void RemoveItem(int productId)
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Cannot remove items from a completed order");

        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot remove items from a cancelled order");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);

        if (item == null)
            throw new InvalidOperationException($"Product with ID {productId} not found in order");

        _items.Remove(item);
        MarkAsUpdated();
    }

    // Domain Methods - Status Management

    /// <summary>
    /// Updates the order status with validation and history tracking
    /// </summary>
    public void UpdateStatus(OrderStatus newStatus, string? notes = null, string? changedBy = null)
    {
        if (Status == newStatus)
            return; // No change needed

        // Validate status transitions
        ValidateStatusTransition(newStatus);

        var oldStatus = Status;
        Status = newStatus;

        // Log status change
        _statusHistories.Add(new OrderStatusHistory(this.Id, oldStatus, newStatus, notes, changedBy));

        MarkAsUpdated();
    }

    /// <summary>
    /// Starts processing the order (Pending → Processing)
    /// </summary>
    public void StartProcessing(string? notes = null, string? changedBy = null)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot start processing order with status {Status}");

        UpdateStatus(OrderStatus.Processing, notes ?? "Order processing started", changedBy);
    }

    /// <summary>
    /// Starts shipping the order (Processing → Shipping)
    /// </summary>
    public void StartShipping(string? notes = null, string? changedBy = null)
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException($"Cannot start shipping order with status {Status}");

        UpdateStatus(OrderStatus.Shipping, notes ?? "Order shipped", changedBy);
    }

    /// <summary>
    /// Completes the order (Shipping → Completed)
    /// </summary>
    public void Complete(string? notes = null, string? changedBy = null)
    {
        if (Status != OrderStatus.Shipping)
            throw new InvalidOperationException($"Cannot complete order with status {Status}");

        UpdateStatus(OrderStatus.Completed, notes ?? "Order completed", changedBy);

        // Auto-mark as paid for COD orders
        if (PaymentMethod == PaymentMethod.COD && PaymentStatus == PaymentStatus.Unpaid)
        {
            MarkAsPaid();
        }
    }

    /// <summary>
    /// Cancels the order (only from Pending or Processing)
    /// </summary>
    public void Cancel(string? reason = null, string? cancelledBy = null)
    {
        if (!CanBeCancelled())
            throw new InvalidOperationException($"Cannot cancel order with status {Status}");

        UpdateStatus(OrderStatus.Cancelled, reason ?? "Order cancelled", cancelledBy);
    }

    // Domain Methods - Payment Management

    /// <summary>
    /// Marks the order as paid
    /// </summary>
    public void MarkAsPaid()
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot mark cancelled order as paid");

        PaymentStatus = PaymentStatus.Paid;
        MarkAsUpdated();
    }

    /// <summary>
    /// Marks the order as unpaid
    /// </summary>
    public void MarkAsUnpaid()
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Cannot mark completed order as unpaid");

        PaymentStatus = PaymentStatus.Unpaid;
        MarkAsUpdated();
    }

    // Domain Methods - Financial Management

    /// <summary>
    /// Updates the shipping fee
    /// </summary>
    public void UpdateShippingFee(decimal fee)
    {
        if (fee < 0)
            throw new ArgumentException("Shipping fee cannot be negative", nameof(fee));

        if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot update shipping fee for completed or cancelled orders");

        ShippingFee = fee;
        MarkAsUpdated();
    }

    /// <summary>
    /// Applies a discount to the order
    /// </summary>
    public void ApplyDiscount(decimal discount)
    {
        if (discount < 0)
            throw new ArgumentException("Discount cannot be negative", nameof(discount));

        if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot apply discount to completed or cancelled orders");

        Discount = discount;
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates customer information
    /// </summary>
    public void UpdateCustomerInfo(string name, string phone, string? email, string address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Customer name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Customer phone is required", nameof(phone));

        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Shipping address is required", nameof(address));

        if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot update customer info for completed or cancelled orders");

        CustomerName = name;
        CustomerPhone = phone;
        CustomerEmail = email;
        ShippingAddress = address;
        MarkAsUpdated();
    }

    /// <summary>
    /// Adds admin notes to the order
    /// </summary>
    public void AddAdminNotes(string notes)
    {
        AdminNotes = string.IsNullOrWhiteSpace(AdminNotes)
            ? notes
            : $"{AdminNotes}\n{notes}";

        MarkAsUpdated();
    }

    // Domain Methods - Calculations

    /// <summary>
    /// Calculates the subtotal from all order items
    /// </summary>
    public decimal CalculateSubTotal()
    {
        return _items.Sum(item => item.GetSubTotal());
    }

    /// <summary>
    /// Calculates the total amount (SubTotal + ShippingFee - Discount)
    /// </summary>
    public decimal CalculateTotalAmount()
    {
        return CalculateSubTotal() + ShippingFee - Discount;
    }

    // Domain Methods - Validation and Queries

    /// <summary>
    /// Validates the order before checkout
    /// </summary>
    public void ValidateForCheckout()
    {
        if (!_items.Any())
            throw new InvalidOperationException("Cannot checkout an order with no items");

        if (_items.Any(i => i.Quantity <= 0))
            throw new InvalidOperationException("Order contains items with invalid quantities");

        if (_items.Any(i => i.Price < 0))
            throw new InvalidOperationException("Order contains items with invalid prices");

        if (string.IsNullOrWhiteSpace(CustomerName))
            throw new InvalidOperationException("Customer name is required");

        if (string.IsNullOrWhiteSpace(CustomerPhone))
            throw new InvalidOperationException("Customer phone is required");

        if (string.IsNullOrWhiteSpace(ShippingAddress))
            throw new InvalidOperationException("Shipping address is required");
    }

    /// <summary>
    /// Checks if the order can be cancelled
    /// </summary>
    public bool CanBeCancelled()
    {
        return Status == OrderStatus.Pending || Status == OrderStatus.Processing;
    }

    /// <summary>
    /// Checks if the order can be completed
    /// </summary>
    public bool CanBeCompleted()
    {
        return Status == OrderStatus.Shipping;
    }

    /// <summary>
    /// Checks if the order is empty
    /// </summary>
    public bool IsEmpty()
    {
        return !_items.Any();
    }

    /// <summary>
    /// Gets the total number of items in the order
    /// </summary>
    public int GetItemCount()
    {
        return _items.Sum(item => item.Quantity);
    }

    /// <summary>
    /// Gets the number of unique products in the order
    /// </summary>
    public int GetUniqueProductCount()
    {
        return _items.Count;
    }

    // Private helper methods

    private void ValidateStatusTransition(OrderStatus newStatus)
    {
        // Cannot change status of cancelled orders
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot change status of a cancelled order");

        // Cannot change status of completed orders
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Cannot change status of a completed order");

        // Validate specific transitions
        switch (newStatus)
        {
            case OrderStatus.Processing:
                if (Status != OrderStatus.Pending)
                    throw new InvalidOperationException($"Cannot transition from {Status} to Processing");
                break;

            case OrderStatus.Shipping:
                if (Status != OrderStatus.Processing)
                    throw new InvalidOperationException($"Cannot transition from {Status} to Shipping");
                break;

            case OrderStatus.Completed:
                if (Status != OrderStatus.Shipping)
                    throw new InvalidOperationException($"Cannot transition from {Status} to Completed");
                break;

            case OrderStatus.Cancelled:
                if (!CanBeCancelled())
                    throw new InvalidOperationException($"Cannot cancel order with status {Status}");
                break;
        }
    }
}
