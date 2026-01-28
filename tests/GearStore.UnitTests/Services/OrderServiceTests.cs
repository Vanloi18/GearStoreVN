using FluentAssertions;
using GearStore.Application.DTOs.Order;
using GearStore.Application.Interfaces;
using GearStore.Application.Services;
using GearStore.Domain.Entities;
using GearStore.Domain.Enums;
using GearStore.UnitTests.Helpers;
using Moq;
using Xunit;

namespace GearStore.UnitTests.Services;

public class OrderServiceTests : IDisposable
{
    private readonly GearStore.Infrastructure.Data.GearStoreDbContext _context;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContextWithData(Guid.NewGuid().ToString());
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        _unitOfWorkMock.Setup(u => u.Orders).Returns(new GearStore.Infrastructure.Repositories.OrderRepository(_context));
        _unitOfWorkMock.Setup(u => u.Products).Returns(new GearStore.Infrastructure.Repositories.ProductRepository(_context));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _orderService = new OrderService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_ValidOrder_ShouldCreateSuccessfully()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Slug = "test-product",
            SKU = "TEST-001",
            Price = 100,
            Stock = 10,
            CategoryId = 1,
            BrandId = 1,
            IsActive = true
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var createDto = new CreateOrderDto
        {
            UserId = "user-123",
            Items = new List<OrderItemDto>
            {
                new OrderItemDto { ProductId = 1, Quantity = 2, Price = 100 }
            },
            ShippingAddress = "123 Test St",
            PaymentMethod = "CreditCard"
        };

        // Act
        var result = await _orderService.CreateOrderAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Pending);
        result.TotalAmount.Should().Be(200);
        result.OrderItems.Should().HaveCount(1);
        
        var updatedProduct = await _context.Products.FindAsync(1);
        updatedProduct!.Stock.Should().Be(8);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ValidTransition_ShouldUpdateSuccessfully()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = "user-123",
            Status = OrderStatus.Pending,
            TotalAmount = 100,
            ShippingAddress = "Test Address",
            PaymentMethod = "CreditCard"
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Act
        var result = await _orderService.UpdateOrderStatusAsync(order.Id, OrderStatus.Processing);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Processing);
        
        var statusHistory = await _context.OrderStatusHistories
            .Where(h => h.OrderId == order.Id)
            .ToListAsync();
        statusHistory.Should().HaveCount(1);
        statusHistory.First().NewStatus.Should().Be(OrderStatus.Processing);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_InvalidTransition_ShouldThrowException()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = "user-123",
            Status = OrderStatus.Completed,
            TotalAmount = 100,
            ShippingAddress = "Test Address",
            PaymentMethod = "CreditCard"
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _orderService.UpdateOrderStatusAsync(order.Id, OrderStatus.Pending)
        );
    }

    [Fact]
    public async Task CancelOrderAsync_PendingOrder_ShouldRestoreStock()
    {
        // Arrange
        var product = new Product
        {
            Id = 2,
            Name = "Test Product 2",
            Slug = "test-product-2",
            SKU = "TEST-002",
            Price = 50,
            Stock = 5,
            CategoryId = 1,
            BrandId = 1,
            IsActive = true
        };
        _context.Products.Add(product);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = "user-123",
            Status = OrderStatus.Pending,
            TotalAmount = 100,
            ShippingAddress = "Test Address",
            PaymentMethod = "CreditCard"
        };
        order.AddItem(product, 2, 50);
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var initialStock = product.Stock;

        // Act
        await _orderService.UpdateOrderStatusAsync(order.Id, OrderStatus.Cancelled);

        // Assert
        var updatedProduct = await _context.Products.FindAsync(2);
        updatedProduct!.Stock.Should().Be(initialStock + 2);
        
        var cancelledOrder = await _context.Orders.FindAsync(order.Id);
        cancelledOrder!.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public async Task GetOrdersByUserIdAsync_ExistingUser_ShouldReturnOrders()
    {
        // Arrange
        var userId = "user-456";
        var order1 = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Pending,
            TotalAmount = 100,
            ShippingAddress = "Address 1",
            PaymentMethod = "CreditCard"
        };
        var order2 = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Completed,
            TotalAmount = 200,
            ShippingAddress = "Address 2",
            PaymentMethod = "PayPal"
        };
        _context.Orders.AddRange(order1, order2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _orderService.GetOrdersByUserIdAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(o => o.UserId.Should().Be(userId));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
