using GearStore.Application.DTOs.Admin;
using GearStore.Application.Interfaces.Services;
using GearStore.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GearStore.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly GearStoreDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminService(GearStoreDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        // 1. Total Users
        var totalUsers = await _userManager.Users.CountAsync();

        // 2. Total Orders
        var totalOrders = await _context.Orders.CountAsync();

        // 3. Total Revenue
        // Calculate revenue from non-cancelled orders
        // Revenue = Sum(OrderItems.Price * Quantity) + ShippingFee - Discount
        var totalRevenue = await _context.Orders
            .Where(o => o.Status != Domain.Enums.OrderStatus.Cancelled)
            .Select(o => new 
            {
                SubTotal = o.OrderItems.Sum(i => i.Price * i.Quantity),
                o.ShippingFee,
                o.Discount
            })
            .SumAsync(x => x.SubTotal + x.ShippingFee - x.Discount);

        // 4. Total Products
        var totalProducts = await _context.Products.CountAsync();

        return new DashboardStatsDto
        {
            TotalUsers = totalUsers,
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            TotalProducts = totalProducts
        };
    }

    public async Task<IEnumerable<AdminUserDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var adminUserDtos = new List<AdminUserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            adminUserDtos.Add(new AdminUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                Roles = roles,
                // These might be in custom claims or profile, but for now we use Email as placeholder if not found
                FirstName = "User", 
                LastName = user.UserName!
            });
        }
        return adminUserDtos;
    }

    public async Task UpdateUserRoleAsync(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found.");

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, newRole);
    }


}
