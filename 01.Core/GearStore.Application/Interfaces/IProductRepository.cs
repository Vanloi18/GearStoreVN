using GearStore.Domain.Entities;

namespace GearStore.Application.Interfaces;

/// <summary>
/// Repository interface for Product aggregate root
/// Defines data access contracts for Product entity
/// </summary>
public interface IProductRepository
{
    // Basic CRUD Operations

    /// <summary>
    /// Gets a product by its unique identifier
    /// </summary>
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product by its unique identifier with all related entities loaded
    /// </summary>
    Task<Product?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product by its unique identifier with variants loaded
    /// </summary>
    Task<Product?> GetByIdWithVariantsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all products
    /// </summary>
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new product
    /// </summary>
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing product
    /// </summary>
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a product by its identifier
    /// </summary>
    Task DeleteAsync(Product product, CancellationToken cancellationToken = default);

    // Query Methods - Product Aggregate Specific

    /// <summary>
    /// Gets a product by its URL-friendly slug
    /// </summary>
    Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product by its SKU code
    /// </summary>
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all products in a specific category
    /// </summary>
    Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all products from a specific brand
    /// </summary>
    Task<IReadOnlyList<Product>> GetByBrandIdAsync(int brandId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active products
    /// </summary>
    Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets featured products (limited count for homepage/promotions)
    /// </summary>
    Task<IReadOnlyList<Product>> GetFeaturedProductsAsync(int count = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products that are in stock
    /// </summary>
    Task<IReadOnlyList<Product>> GetInStockProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products with low stock (below threshold)
    /// </summary>
    Task<IReadOnlyList<Product>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches products by keyword (name, description, SKU)
    /// </summary>
    Task<IReadOnlyList<Product>> SearchAsync(string keyword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products within a price range
    /// </summary>
    Task<IReadOnlyList<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most viewed products
    /// </summary>
    Task<IReadOnlyList<Product>> GetMostViewedAsync(int count = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the best selling products
    /// </summary>
    Task<IReadOnlyList<Product>> GetBestSellersAsync(int count = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets new arrival products (recently created)
    /// </summary>
    Task<IReadOnlyList<Product>> GetNewArrivalsAsync(int count = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products on sale (with discount)
    /// </summary>
    Task<IReadOnlyList<Product>> GetProductsOnSaleAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a product with the given slug exists
    /// </summary>
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a product with the given SKU exists
    /// </summary>
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of products
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of active products
    /// </summary>
    Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);
}
