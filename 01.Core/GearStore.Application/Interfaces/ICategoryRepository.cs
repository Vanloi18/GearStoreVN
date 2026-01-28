using GearStore.Domain.Entities;

namespace GearStore.Application.Interfaces;

/// <summary>
/// Repository interface for Category aggregate root
/// Defines data access contracts for Category entity
/// </summary>
public interface ICategoryRepository
{
    // Basic CRUD Operations

    /// <summary>
    /// Gets a category by its unique identifier
    /// </summary>
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a category by its unique identifier with all products loaded
    /// </summary>
    Task<Category?> GetByIdWithProductsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all categories
    /// </summary>
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new category
    /// </summary>
    Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing category
    /// </summary>
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a category
    /// </summary>
    Task DeleteAsync(Category category, CancellationToken cancellationToken = default);

    // Query Methods - Category Specific

    /// <summary>
    /// Gets a category by its URL-friendly slug
    /// </summary>
    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active categories
    /// </summary>
    Task<IReadOnlyList<Category>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets categories ordered by display order
    /// </summary>
    Task<IReadOnlyList<Category>> GetOrderedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets categories that have products
    /// </summary>
    Task<IReadOnlyList<Category>> GetCategoriesWithProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets categories with active products only
    /// </summary>
    Task<IReadOnlyList<Category>> GetCategoriesWithActiveProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a category with the given slug exists
    /// </summary>
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a category has any products
    /// </summary>
    Task<bool> HasProductsAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of categories
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of active categories
    /// </summary>
    Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the product count for a specific category
    /// </summary>
    Task<int> GetProductCountAsync(int categoryId, CancellationToken cancellationToken = default);
}
