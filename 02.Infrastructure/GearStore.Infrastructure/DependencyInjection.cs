using GearStore.Application.Interfaces;
using GearStore.Application.Interfaces.Services;
using GearStore.Application.Services;
using GearStore.Infrastructure.Data;
using GearStore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GearStore.Infrastructure;

/// <summary>
/// Dependency Injection configuration for Infrastructure layer
/// Registers DbContext, Repositories, UnitOfWork, and Application Services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        string connectionString)
    {
        // ========================================
        // 1. DbContext Registration
        // ========================================
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        services.AddDbContext<GearStoreDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));

        // ========================================
        // 2. Repository Registration (Scoped)
        // ========================================
        // Each repository is scoped to the HTTP request lifetime
        // Ensures one DbContext instance per request
        
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        // ========================================
        // 3. UnitOfWork Registration (Scoped)
        // ========================================
        // UnitOfWork coordinates all repositories and manages transactions
        // Scoped lifetime ensures one UnitOfWork per request
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ========================================
        // 4. Application Services Registration (Scoped)
        // ========================================
        // Business logic services that orchestrate domain operations
        // Scoped lifetime ensures services share the same UnitOfWork per request
        
        services.AddScoped<ICartService, CartService>();
        
        // Services implemented in Infrastructure (using DbContext directly)
        services.AddScoped<IOrderService, GearStore.Infrastructure.Services.OrderService>();
        services.AddScoped<IProductService, GearStore.Infrastructure.Services.ProductService>();
        services.AddScoped<ICategoryService, GearStore.Infrastructure.Services.CategoryService>();
        services.AddScoped<IBrandService, GearStore.Infrastructure.Services.BrandService>();
        services.AddScoped<IAdminService, GearStore.Infrastructure.Services.AdminService>();
        services.AddScoped<IAuthService, GearStore.Infrastructure.Services.AuthService>();
        services.AddScoped<IUserService, GearStore.Infrastructure.Services.UserService>();

        return services;
    }
}