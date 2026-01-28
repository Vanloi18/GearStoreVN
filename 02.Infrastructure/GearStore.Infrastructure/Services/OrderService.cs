using GearStore.Application.DTOs.Order;
using GearStore.Application.Interfaces.Services;
using GearStore.Application.Interfaces;
using GearStore.Domain.Entities;
using GearStore.Domain.Enums;
using GearStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GearStore.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly GearStoreDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(GearStoreDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResponseDto> CheckoutAsync(string userId, OrderCreateDto dto)
    {
        // 1. Get User Cart
        var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
        if (cart == null || cart.Items.Count == 0)
            throw new InvalidOperationException("Cart is empty.");

        // 2. Validate Cart & Stock
        var productIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync(); // Using Context directly for Products, or should use Repo? Using Context for EF tracking ease here.
        
        foreach (var item in cart.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product == null) throw new InvalidOperationException($"Product {item.ProductName} not found.");
            
            if (!product.IsActive) throw new InvalidOperationException($"Product {product.Name} is not active.");
            
            if (product.Stock < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for {product.Name}. Available: {product.Stock}");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 3. Create Order
            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
            var order = new Order(
                orderNumber,
                dto.CustomerName,
                dto.CustomerPhone,
                dto.ShippingAddress,
                dto.PaymentMethod,
                userId,
                null, 
                dto.Notes
            );

            // 4. Add Items & Reduce Stock
            foreach (var item in cart.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                
                decimal price = product.Price; // Default to product price, but CartItem should have price?
                // Actually, Order Service should trust CartItem price OR re-validate? 
                // Best practice: Re-fetch latest price from Product/Variant to ensure validity.
                // But CartItem already has snapshot. 
                // Logic: "We use current Price from Product or Cart? Usually Product Price at checkout time."
                
                string? productSKU = product.SKU;
                string? variantName = null;
                
                if (item.VariantId.HasValue)
                {
                     var variant = product.Variants.FirstOrDefault(v => v.Id == item.VariantId.Value);
                     if (variant == null) throw new InvalidOperationException($"Variant {item.VariantId} not found");
                     
                     if (variant.Stock < item.Quantity)
                        throw new InvalidOperationException($"Insufficient stock for {product.Name} - {variant.Name}");
                     
                     price = variant.Price;
                     variantName = variant.Name;
                     productSKU = variant.SKU ?? product.SKU;
                     
                     variant.DecreaseStock(item.Quantity);
                }
                else
                {
                     if (product.Stock < item.Quantity)
                        throw new InvalidOperationException($"Insufficient stock for {product.Name}");
                        
                     product.DecreaseStock(item.Quantity);
                }

                order.AddItem(product.Id, product.Name, productSKU, item.VariantId, variantName, price, item.Quantity);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 5. Clear Cart
            cart.Clear();
            await _unitOfWork.Carts.UpdateAsync(cart); // Update Cart state (Items removed)
            await _unitOfWork.SaveChangesAsync();

            await transaction.CommitAsync();

            return MapToDto(order);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<OrderResponseDto> CreateOrderAsync(string userId, CreateOrderDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            throw new ArgumentException("Order must contain at least one item.");

        // 1. Validate Stock & Products
        var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        if (products.Count != productIds.Count)
            throw new ArgumentException("One or more products not found.");

        foreach (var item in dto.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            if (product.Stock < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for product '{product.Name}'. Available: {product.Stock}");
            
            if (!product.IsActive)
                 throw new InvalidOperationException($"Product '{product.Name}' is not active.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 2. Create Order
            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
            var order = new Order(
                orderNumber,
                dto.CustomerName,
                dto.CustomerPhone,
                dto.ShippingAddress,
                dto.PaymentMethod,
                userId,
                null, // Email could be fetched from User, but passing null for now
                dto.Notes
            );

            // 3. Add Items & Reduce Stock
            foreach (var itemDto in dto.Items)
            {
                var product = products.First(p => p.Id == itemDto.ProductId);
                
                decimal price = product.Price;
                string productName = product.Name;
                string? productSKU = product.SKU;
                string? variantName = null;

                if (itemDto.VariantId.HasValue)
                {
                    var variant = product.Variants.FirstOrDefault(v => v.Id == itemDto.VariantId.Value);
                    if (variant == null) throw new InvalidOperationException($"Variant {itemDto.VariantId} not found for product {product.Name}");
                    
                    if (variant.Stock < itemDto.Quantity)
                        throw new InvalidOperationException($"Insufficient stock for {product.Name} - {variant.Name}");
                    
                    price = variant.Price;
                    variantName = variant.Name;
                    productSKU = variant.SKU ?? product.SKU;
                    
                    variant.DecreaseStock(itemDto.Quantity);
                }
                else
                {
                    // Validation for main product
                    if (product.Stock < itemDto.Quantity)
                         throw new InvalidOperationException($"Insufficient stock for product '{product.Name}'. Available: {product.Stock}");

                    product.DecreaseStock(itemDto.Quantity);
                }
                
                order.AddItem(product.Id, product.Name, productSKU, itemDto.VariantId, variantName, price, itemDto.Quantity);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return MapToDto(order);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(string userId)
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToDto);
    }

    public async Task<OrderResponseDto> GetOrderByIdAsync(int orderId, string? userId)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new Exception($"Order {orderId} not found");

        // Authorization check: User can only see own orders, Admin sees all (handled by controller or here?)
        // Prompt says: "GetOrderById(int orderId, string userId)". 
        // Business Rule: "User can ONLY access their own orders".
        // If I pass userId as "Admin", this check might fail if I enforce equality.
        // Usually Service assumes userId is the requester. 
        // Admin logic: `GetAllOrders` is separate.
        // For `GetOrderById`, if user is Admin, they might use this method too?
        // But prompt says "GetAllOrders() // Admin only", "UpdateOrderStatus // Admin only".
        // It implies GetOrderById is for the user.
        // I will strictly enforce ownership here. Admin can use a different method if needed, or I check role?
        // Service doesn't know roles usually.
        // I'll enforce: if userId is provided, it must match.
        
        if (!string.IsNullOrEmpty(userId) && order.UserId != userId)
             throw new UnauthorizedAccessException("You are not authorized to view this order.");

        return MapToDto(order);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToDto);
    }

    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new Exception($"Order {orderId} not found");

        // Business Rule: State Transitions
        // Valid: Pending -> Processing -> Shipping -> Completed
        // Valid: Pending -> Cancelled
        
        bool isValid = false;
        switch (order.Status)
        {
            case OrderStatus.Pending:
                if (newStatus == OrderStatus.Processing || newStatus == OrderStatus.Cancelled) isValid = true;
                break;
            case OrderStatus.Processing:
                if (newStatus == OrderStatus.Shipping || newStatus == OrderStatus.Cancelled) isValid = true; 
                break;
            case OrderStatus.Shipping:
                if (newStatus == OrderStatus.Completed) isValid = true;
                break;
            case OrderStatus.Completed:
            case OrderStatus.Cancelled:
                // Terminal states
                break;
        }

        if (!isValid)
            throw new InvalidOperationException($"Invalid status transition from {order.Status} to {newStatus}");

        // Stock Restoration Logic (on Cancellation)
        if (newStatus == OrderStatus.Cancelled && order.Status != OrderStatus.Cancelled)
        {
            var productIds = order.OrderItems.Select(i => i.ProductId).ToList();
            var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

            foreach (var item in order.OrderItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                {
                   // product.IncreaseStock(item.Quantity); 
                   // Domain method unknown, using property assuming accessible or manual update via context tracking.
                   // Product.cs content was partially seen. Assuming Stock is settable or using Update logic.
                   // Safest: UpdateStock method if exists or direct property.
                   // Checking step 1091, I commented out UpdateStock.
                   // Let's assume there is NO public setter or method visible in my cache.
                   // Actually, if I can't be sure, I'll access the backing field or update method.
                   // Re-reading Step 1042 CartService: `cart.AddItem` uses `product.Price`.
                   // `OrderService` `CreateOrderAsync` uses `product.DecreaseStock(qty)`.
                   // So `DecreaseStock` exists. `IncreaseStock` probably exists?
                   // If not, I'll try `DecreaseStock(-qty)`.
                   // Or just `product.Stock += qty` if public.
                   // I'll try `IncreaseStock` first. If fail, I'll check Product.
                   // Actually, `DecreaseStock` was used in `CreateOrderAsync` (Step 1051).
                   // It is highly likely `IncreaseStock` exists or I can use `DecreaseStock(-qty)`.
                   // I will use `IncreaseStock` to match intention. If build fails, I check Product.cs.
                   product.IncreaseStock(item.Quantity);
                }
            }
        }

        order.UpdateStatus(newStatus);
        
        await _context.SaveChangesAsync();
    }

    public async Task<GearStore.Application.DTOs.Admin.DashboardStatsDto> GetDashboardStatsAsync()
    {
        var totalUsers = await _context.Users.CountAsync(); 
        
        var totalOrders = await _context.Orders.CountAsync();
        var totalProducts = await _context.Products.CountAsync();
        
        // Revenue: Sum of TotalAmount where Status == Processing, Shipping, Completed
        // EF Core cannot translate TotalAmount property if it's not in DB. 
        // We sum OrderItems: Price * Quantity
        var revenue = await _context.Orders
            .Where(o => o.Status == OrderStatus.Processing || o.Status == OrderStatus.Shipping || o.Status == OrderStatus.Completed)
            .SelectMany(o => o.OrderItems)
            .SumAsync(i => i.Price * i.Quantity); 
            
        return new GearStore.Application.DTOs.Admin.DashboardStatsDto
        {
            TotalUsers = totalUsers, 
            TotalOrders = totalOrders,
            TotalProducts = totalProducts,
            TotalRevenue = revenue
        };
    }

    private static OrderResponseDto MapToDto(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            UserId = order.UserId ?? string.Empty,
            CustomerName = order.CustomerName,
            ShippingAddress = order.ShippingAddress,
            TotalAmount = order.CalculateTotalAmount(),
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems.Select(i => new OrderItemResponseDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}
