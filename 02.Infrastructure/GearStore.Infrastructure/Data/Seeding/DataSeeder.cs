using GearStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GearStore.Infrastructure.Data.Seeding;

/// <summary>
/// Class để seed dữ liệu mẫu vào database
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Đảm bảo database đã được tạo
        await context.Database.EnsureCreatedAsync();

        // Seed Categories
        await SeedCategoriesAsync(context);

        // Seed Chatbot Responses
        await SeedChatbotResponsesAsync(context);

        // Seed Sample Products (một vài sản phẩm mẫu để test)
        await SeedSampleProductsAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return; // Đã có dữ liệu

        var categories = new List<Category>
        {
            new() { Name = "CPU - Bộ vi xử lý", Slug = "cpu", Description = "Intel Core, AMD Ryzen", Icon = "fa-microchip", DisplayOrder = 1, IsActive = true },
            new() { Name = "Mainboard - Bo mạch chủ", Slug = "mainboard", Description = "ASUS, MSI, Gigabyte", Icon = "fa-server", DisplayOrder = 2, IsActive = true },
            new() { Name = "RAM - Bộ nhớ trong", Slug = "ram", Description = "DDR4, DDR5", Icon = "fa-memory", DisplayOrder = 3, IsActive = true },
            new() { Name = "VGA - Card màn hình", Slug = "vga", Description = "NVIDIA, AMD", Icon = "fa-display", DisplayOrder = 4, IsActive = true },
            new() { Name = "PSU - Nguồn máy tính", Slug = "psu", Description = "Corsair, Cooler Master", Icon = "fa-bolt", DisplayOrder = 5, IsActive = true },
            new() { Name = "Case - Vỏ máy tính", Slug = "case", Description = "ATX, mATX, Mini-ITX", Icon = "fa-cube", DisplayOrder = 6, IsActive = true },
            new() { Name = "Storage - Ổ cứng", Slug = "storage", Description = "SSD, HDD", Icon = "fa-hard-drive", DisplayOrder = 7, IsActive = true },
            new() { Name = "Cooling - Tản nhiệt", Slug = "cooling", Description = "Air, AIO, Custom Loop", Icon = "fa-fan", DisplayOrder = 8, IsActive = true }
        };

        await context.Categories.AddRangeAsync(categories);
    }

    private static async Task SeedChatbotResponsesAsync(ApplicationDbContext context)
    {
        if (await context.ChatbotResponses.AnyAsync())
            return;

        var responses = new List<ChatbotResponse>
        {
            new()
            {
                Keywords = "bảo hành,warranty,bh",
                ResponseText = "Sản phẩm chính hãng tại GearStore được bảo hành từ 12-36 tháng tùy theo loại linh kiện. CPU/RAM thường 36 tháng, VGA 24-36 tháng.",
                Category = "Warranty",
                Priority = 10,
                IsActive = true
            },
            new()
            {
                Keywords = "ship,giao hàng,vận chuyển,delivery",
                ResponseText = "GearStore hỗ trợ giao hàng COD toàn quốc. Phí ship: 30.000đ nội thành, 50.000đ ngoại thành. FREESHIP cho đơn hàng từ 1 triệu.",
                Category = "Shipping",
                Priority = 10,
                IsActive = true
            },
            new()
            {
                Keywords = "thanh toán,payment,pay",
                ResponseText = "Chúng tôi hỗ trợ 2 hình thức thanh toán: COD (tiền mặt khi nhận hàng) và Chuyển khoản ngân hàng.",
                Category = "Payment",
                Priority = 10,
                IsActive = true
            },
            new()
            {
                Keywords = "build pc,xây dựng cấu hình,chọn linh kiện,pc builder",
                ResponseText = "Bạn có thể sử dụng công cụ PC Builder của chúng tôi! Hệ thống sẽ tự động kiểm tra tương thích giữa các linh kiện và gợi ý nguồn phù hợp.",
                Category = "BuildPC",
                Priority = 15,
                IsActive = true
            },
            new()
            {
                Keywords = "giờ mở cửa,làm việc,working hours",
                ResponseText = "GearStore làm việc từ 8:00 - 21:00 hàng ngày, kể cả cuối tuần và lễ tết.",
                Category = "General",
                Priority = 5,
                IsActive = true
            },
            new()
            {
                Keywords = "liên hệ,hotline,phone,số điện thoại",
                ResponseText = "Bạn có thể liên hệ với chúng tôi qua Hotline: 1900-xxxx hoặc Email: support@gearstore.vn",
                Category = "Contact",
                Priority = 8,
                IsActive = true
            }
        };

        await context.ChatbotResponses.AddRangeAsync(responses);
    }

    private static async Task SeedSampleProductsAsync(ApplicationDbContext context)
    {
        if (await context.Products.AnyAsync())
            return;

        // Lấy CategoryId
        var cpuCategory = await context.Categories.FirstAsync(c => c.Slug == "cpu");
        var mainCategory = await context.Categories.FirstAsync(c => c.Slug == "mainboard");

        // Sample CPU
        var cpu = new Product
        {
            CategoryId = cpuCategory.Id,
            Name = "Intel Core i5-13600K",
            Slug = "intel-core-i5-13600k",
            SKU = "CPU-I5-13600K",
            Brand = "Intel",
            Price = 5500000,
            OriginalPrice = 6000000,
            Stock = 50,
            Description = "CPU Intel thế hệ 13, 14 nhân 20 luồng, xung nhịp tối đa 5.1GHz",
            ShortDescription = "14 nhân 20 luồng, Socket LGA1700",
            ThumbnailImage = "/images/products/i5-13600k.jpg",
            IsActive = true,
            IsFeatured = true
        };

        await context.Products.AddAsync(cpu);
        await context.SaveChangesAsync(); // Save để có Id

        // Specs cho CPU
        var cpuSpecs = new List<ProductSpec>
        {
            new() { ProductId = cpu.Id, SpecKey = "Socket", SpecValue = "LGA1700", DisplayName = "Socket", DisplayOrder = 1 },
            new() { ProductId = cpu.Id, SpecKey = "SupportedRAM", SpecValue = "DDR5,DDR4", DisplayName = "Hỗ trợ RAM", DisplayOrder = 2 },
            new() { ProductId = cpu.Id, SpecKey = "TDP", SpecValue = "125", DisplayName = "TDP (W)", DisplayOrder = 3 },
            new() { ProductId = cpu.Id, SpecKey = "Cores", SpecValue = "14", DisplayName = "Số nhân", DisplayOrder = 4 },
            new() { ProductId = cpu.Id, SpecKey = "Threads", SpecValue = "20", DisplayName = "Số luồng", DisplayOrder = 5 }
        };

        await context.ProductSpecs.AddRangeAsync(cpuSpecs);

        // Sample Mainboard
        var mainboard = new Product
        {
            CategoryId = mainCategory.Id,
            Name = "ASUS ROG STRIX B760-F GAMING WIFI",
            Slug = "asus-rog-strix-b760-f-gaming-wifi",
            SKU = "MAIN-ASUS-B760F",
            Brand = "ASUS",
            Price = 4200000,
            OriginalPrice = 4800000,
            Stock = 30,
            Description = "Mainboard ASUS B760, hỗ trợ Intel Gen 12/13, DDR5, Wi-Fi 6E",
            ShortDescription = "Socket LGA1700, DDR5, ATX",
            ThumbnailImage = "/images/products/asus-b760f.jpg",
            IsActive = true,
            IsFeatured = true
        };

        await context.Products.AddAsync(mainboard);
        await context.SaveChangesAsync();

        // Specs cho Mainboard
        var mainSpecs = new List<ProductSpec>
        {
            new() { ProductId = mainboard.Id, SpecKey = "Socket", SpecValue = "LGA1700", DisplayName = "Socket", DisplayOrder = 1 },
            new() { ProductId = mainboard.Id, SpecKey = "SupportedRAM", SpecValue = "DDR5", DisplayName = "Loại RAM", DisplayOrder = 2 },
            new() { ProductId = mainboard.Id, SpecKey = "FormFactor", SpecValue = "ATX", DisplayName = "Form Factor", DisplayOrder = 3 },
            new() { ProductId = mainboard.Id, SpecKey = "Chipset", SpecValue = "B760", DisplayName = "Chipset", DisplayOrder = 4 }
        };

        await context.ProductSpecs.AddRangeAsync(mainSpecs);
    }
}