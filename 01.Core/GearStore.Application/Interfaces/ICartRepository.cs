using GearStore.Domain.Entities;

namespace GearStore.Application.Interfaces;

/// <summary>
/// Repository interface for Cart aggregate root
/// Defines data access contracts for Cart entity
/// </summary>
public interface ICartRepository
{
    // Basic CRUD Operations

    /// <summary>
    /// Gets a cart by its unique identifier
    /// </summary>
    Task<Cart?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all carts
    /// </summary>
    Task<IReadOnlyList<Cart>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new cart
    /// </summary>
    Task<Cart> AddAsync(Cart cart, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing cart
    /// </summary>
    Task UpdateAsync(Cart cart, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a cart
    /// </summary>
    Task DeleteAsync(Cart cart, CancellationToken cancellationToken = default);

    // Query Methods - Cart Aggregate Specific

    /// <summary>
    /// Gets the cart for a logged-in user
    /// </summary>
    Task<Cart?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the cart for a guest session
    /// </summary>
    Task<Cart?> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default);
}

