using GearStore.Application.Interfaces;
using GearStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace GearStore.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IUnitOfWork
/// Coordinates repository operations and manages transactions
/// </summary>
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly GearStoreDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    // Lazy-loaded repositories
    private IProductRepository? _products;
    private ICategoryRepository? _categories;
    private IBrandRepository? _brands;
    private ICartRepository? _carts;
    private IOrderRepository? _orders;

    public UnitOfWork(GearStoreDbContext context)
    {
        _context = context;
    }

    // Repository properties with lazy initialization

    public IProductRepository Products
    {
        get
        {
            _products ??= new ProductRepository(_context);
            return _products;
        }
    }

    public ICategoryRepository Categories
    {
        get
        {
            _categories ??= new CategoryRepository(_context);
            return _categories;
        }
    }

    public IBrandRepository Brands
    {
        get
        {
            _brands ??= new BrandRepository(_context);
            return _brands;
        }
    }

    public ICartRepository Carts
    {
        get
        {
            _carts ??= new CartRepository(_context);
            return _carts;
        }
    }

    public IOrderRepository Orders
    {
        get
        {
            _orders ??= new OrderRepository(_context);
            return _orders;
        }
    }

    // Transaction management

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    // Dispose pattern

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
    }
}
