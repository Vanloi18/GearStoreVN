using GearStore.Domain.Entities;
using GearStore.Domain.Enums;

namespace GearStore.Application.Interfaces;

/// <summary>
/// Repository interface for Order aggregate root
/// Defines data access contracts for Order entity
/// </summary>
public interface IOrderRepository
{
    // Basic CRUD Operations

    /// <summary>
    /// Gets an order by its unique identifier
    /// </summary>
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all orders
    /// </summary>
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new order
    /// </summary>
    Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing order
    /// </summary>
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an order
    /// </summary>
    Task DeleteAsync(Order order, CancellationToken cancellationToken = default);

    // Query Methods - Order Aggregate Specific

    /// <summary>
    /// Gets an order by its unique order number
    /// </summary>
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all orders for a specific user
    /// </summary>
    Task<IReadOnlyList<Order>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets orders by status
    /// </summary>
    Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an order number exists
    /// </summary>
    Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
}
