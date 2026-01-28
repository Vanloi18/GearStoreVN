using GearStore.Application.DTOs.Cart;

namespace GearStore.Application.Interfaces.Services;

/// <summary>
/// Service interface for cart operations
/// Orchestrates cart business logic and coordinates between repositories
/// </summary>
public interface ICartService
{
    /// <summary>
    /// Gets the cart for a user or session
    /// </summary>
    Task<CartDto?> GetCartAsync(string? userId, string? sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an item to the cart
    /// </summary>
    Task<CartDto> AddToCartAsync(string? userId, string? sessionId, AddToCartDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the quantity of an item in the cart
    /// </summary>
    Task<CartDto> UpdateCartItemAsync(string? userId, string? sessionId, int productId, UpdateCartItemDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an item from the cart
    /// </summary>
    Task<CartDto> RemoveFromCartAsync(string? userId, string? sessionId, int productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all items from the cart
    /// </summary>
    Task ClearCartAsync(string? userId, string? sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Merges a guest cart into a user cart (after login)
    /// </summary>
    Task<CartDto> MergeCartAsync(string userId, string sessionId, CancellationToken cancellationToken = default);
}
