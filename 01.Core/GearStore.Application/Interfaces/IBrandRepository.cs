using GearStore.Domain.Entities;

namespace GearStore.Application.Interfaces;

/// <summary>
/// Repository interface for Brand aggregate root
/// Defines data access contracts for Brand entity
/// </summary>
public interface IBrandRepository
{
    // Basic CRUD Operations

    /// <summary>
    /// Gets a brand by its unique identifier
    /// </summary>
    Task<Brand?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a brand by its unique identifier with all products loaded
    /// </summary>
    Task<Brand?> GetByIdWithProductsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all brands
    /// </summary>
    Task<IReadOnlyList<Brand>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new brand
    /// </summary>
    Task<Brand> AddAsync(Brand brand, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing brand
    /// </summary>
    Task UpdateAsync(Brand brand, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a brand
    /// </summary>
    Task DeleteAsync(Brand brand, CancellationToken cancellationToken = default);

    // Query Methods - Brand Specific

    /// <summary>
    /// Gets a brand by its URL-friendly slug
    /// </summary>
    Task<Brand?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active brands
    /// </summary>
    Task<IReadOnlyList<Brand>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all featured brands
    /// </summary>
    Task<IReadOnlyList<Brand>> GetFeaturedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets brands ordered by display order
    /// </summary>
    Task<IReadOnlyList<Brand>> GetOrderedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets brands that have products
    /// </summary>
    Task<IReadOnlyList<Brand>> GetBrandsWithProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets brands with active products only
    /// </summary>
    Task<IReadOnlyList<Brand>> GetBrandsWithActiveProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top brands by product count
    /// </summary>
    Task<IReadOnlyList<Brand>> GetTopBrandsByProductCountAsync(int count = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a brand with the given slug exists
    /// </summary>
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a brand has any products
    /// </summary>
    Task<bool> HasProductsAsync(int brandId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of brands
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of active brands
    /// </summary>
    Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of featured brands
    /// </summary>
    Task<int> GetFeaturedCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the product count for a specific brand
    /// </summary>
    Task<int> GetProductCountAsync(int brandId, CancellationToken cancellationToken = default);
}
