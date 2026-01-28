using GearStore.Application.Interfaces;
using GearStore.Domain.Entities;
using GearStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GearStore.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IBrandRepository
/// </summary>
public class BrandRepository : IBrandRepository
{
    private readonly GearStoreDbContext _context;

    public BrandRepository(GearStoreDbContext context)
    {
        _context = context;
    }

    // Basic CRUD Operations

    public async Task<Brand?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<Brand?> GetByIdWithProductsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .Include(b => b.Products)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Brand>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Brand> AddAsync(Brand brand, CancellationToken cancellationToken = default)
    {
        await _context.Brands.AddAsync(brand, cancellationToken);
        return brand;
    }

    public Task UpdateAsync(Brand brand, CancellationToken cancellationToken = default)
    {
        _context.Brands.Update(brand);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Brand brand, CancellationToken cancellationToken = default)
    {
        _context.Brands.Remove(brand);
        return Task.CompletedTask;
    }

    // Query Methods - Brand Specific

    public async Task<Brand?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Slug == slug, cancellationToken);
    }

    public async Task<IReadOnlyList<Brand>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .Where(b => b.IsActive)
            .OrderBy(b => b.DisplayOrder)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Brand>> GetFeaturedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .Where(b => b.IsFeatured && b.IsActive)
            .OrderBy(b => b.DisplayOrder)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Brand>> GetOrderedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .OrderBy(b => b.DisplayOrder)
            .ThenBy(b => b.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Brand>> GetBrandsWithProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .Include(b => b.Products)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Brand>> GetBrandsWithActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .Where(b => b.IsActive && b.Products.Any(p => p.IsActive))
            .OrderBy(b => b.DisplayOrder)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Brand>> GetTopBrandsByProductCountAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .Where(b => b.IsActive)
            .OrderByDescending(b => b.Products.Count(p => p.IsActive))
            .Take(count)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .AsNoTracking()
            .AnyAsync(b => b.Slug == slug, cancellationToken);
    }

    public async Task<bool> HasProductsAsync(int brandId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .AnyAsync(p => p.BrandId == brandId, cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .AsNoTracking()
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .Where(b => b.IsActive)
            .AsNoTracking()
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetFeaturedCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .Where(b => b.IsFeatured && b.IsActive)
            .AsNoTracking()
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetProductCountAsync(int brandId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .CountAsync(p => p.BrandId == brandId, cancellationToken);
    }
}
