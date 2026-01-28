namespace GearStore.Application.Interfaces;

/// <summary>
/// Unit of Work interface for managing transactions across multiple repositories
/// Coordinates repository operations and ensures data consistency
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repository Properties

    /// <summary>
    /// Gets the Product repository
    /// </summary>
    IProductRepository Products { get; }

    /// <summary>
    /// Gets the Category repository
    /// </summary>
    ICategoryRepository Categories { get; }

    /// <summary>
    /// Gets the Brand repository
    /// </summary>
    IBrandRepository Brands { get; }

    /// <summary>
    /// Gets the Cart repository
    /// </summary>
    ICartRepository Carts { get; }

    /// <summary>
    /// Gets the Order repository
    /// </summary>
    IOrderRepository Orders { get; }

    // Transaction Methods

    /// <summary>
    /// Saves all changes made in this unit of work to the database
    /// </summary>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
