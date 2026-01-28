using GearStore.Application.DTOs.Order;
using GearStore.Domain.Enums;

namespace GearStore.Application.Interfaces.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(string userId, CreateOrderDto dto);
    Task<OrderResponseDto> CheckoutAsync(string userId, OrderCreateDto dto);
    Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(string userId);
    Task<OrderResponseDto> GetOrderByIdAsync(int orderId, string? userId);
    Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync(); // Admin only
    Task UpdateOrderStatusAsync(int orderId, OrderStatus status); // Admin only
    Task<GearStore.Application.DTOs.Admin.DashboardStatsDto> GetDashboardStatsAsync();
}
