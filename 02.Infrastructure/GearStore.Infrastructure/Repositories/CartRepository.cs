using GearStore.Application.Interfaces;
using GearStore.Domain.Entities;
using GearStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GearStore.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of ICartRepository
/// </summary>
public class CartRepository : ICartRepository
{
    private readonly GearStoreDbContext _context;

    public CartRepository(GearStoreDbContext context)
    {
        _context = context;
    }

    // Basic CRUD Operations

    public async Task<Cart?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Cart>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Cart> AddAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        await _context.Carts.AddAsync(cart, cancellationToken);
        return cart;
    }

    public Task UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        _context.Carts.Update(cart);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        _context.Carts.Remove(cart);
        return Task.CompletedTask;
    }

    // Query Methods - Cart Specific

    public async Task<Cart?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<Cart?> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.SessionId == sessionId, cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .AsNoTracking()
            .AnyAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<bool> ExistsBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .AsNoTracking()
            .AnyAsync(c => c.SessionId == sessionId, cancellationToken);
    }
}
