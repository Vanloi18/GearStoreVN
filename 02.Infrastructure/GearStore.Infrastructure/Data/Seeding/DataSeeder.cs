using GearStore.Domain.Entities;
using GearStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GearStore.Infrastructure.Data.Seeding;

public static class DataSeeder
{
    public static async Task SeedAsync(GearStoreDbContext context) 
    {
        await SeedCategoriesAsync(context);
        await SeedBrandsAsync(context);
        await SeedProductsAsync(context);
        await SeedChatbotResponsesAsync(context);
    }

    private static async Task SeedCategoriesAsync(GearStoreDbContext context)
    {
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category("CPU", "cpu", 1),
                new Category("Mainboard", "mainboard", 2),
                new Category("RAM", "ram", 3),
                new Category("VGA", "vga", 4),
                new Category("PSU", "psu", 5),
                new Category("Case", "case", 6),
                new Category("Storage", "storage", 7)
            };
            
            // Add descriptions and icons
            foreach (var cat in categories)
            {
                if (cat.Name == "CPU")
                {
                    cat.UpdateInfo(cat.Name, cat.Slug, "Vi xử lý trung tâm, bộ não của máy tính");
                    cat.SetIcon("bi-cpu");
                }
                else if (cat.Name == "Mainboard")
                {
                    cat.UpdateInfo(cat.Name, cat.Slug, "Bo mạch chủ, kết nối các linh kiện");
                    cat.SetIcon("bi-motherboard");
                }
                else if (cat.Name == "RAM")
                {
                    cat.UpdateInfo(cat.Name, cat.Slug, "Bộ nhớ trong, xử lý đa nhiệm");
                    cat.SetIcon("bi-memory");
                }
                else if (cat.Name == "VGA")
                {
                    cat.UpdateInfo(cat.Name, cat.Slug, "Card đồ họa, xử lý hình ảnh");
                    cat.SetIcon("bi-gpu-card");
                }
            }

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedBrandsAsync(GearStoreDbContext context)
    {
        if (!await context.Brands.AnyAsync())
        {
            var brands = new List<Brand>
            {
                new Brand("Intel", "intel"),
                new Brand("AMD", "amd"),
                new Brand("ASUS", "asus"),
                new Brand("MSI", "msi"),
                new Brand("Gigabyte", "gigabyte"),
                new Brand("Corsair", "corsair"),
                new Brand("Kingston", "kingston"),
                new Brand("Samsung", "samsung"),
                new Brand("NVIDIA", "nvidia")
            };

            foreach (var brand in brands)
            {
                brand.SetLogo($"/images/brands/{brand.Slug}.png");
                if (brand.Name == "Intel" || brand.Name == "AMD" || brand.Name == "NVIDIA")
                {
                    brand.MarkAsFeatured();
                }
            }

            await context.Brands.AddRangeAsync(brands);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedProductsAsync(GearStoreDbContext context)
    {
        if (!await context.Products.AnyAsync())
        {
            // Get IDs for linking
            var categories = await context.Categories.ToListAsync();
            var brands = await context.Brands.ToListAsync();

            var cpuCat = categories.First(c => c.Slug == "cpu");
            var vgaCat = categories.First(c => c.Slug == "vga");
            var ramCat = categories.First(c => c.Slug == "ram");
            var mbCat = categories.First(c => c.Slug == "mainboard");

            var intel = brands.First(b => b.Slug == "intel");
            var amd = brands.First(b => b.Slug == "amd");
            var nvidia = brands.First(b => b.Slug == "nvidia");
            var asus = brands.First(b => b.Slug == "asus");
            var corsair = brands.First(b => b.Slug == "corsair");

            var products = new List<Product>();

            // CPUs
            var p1 = new Product("Intel Core i9-14900K", "intel-core-i9-14900k", cpuCat.Id, intel.Id, 14990000);
            p1.UpdateBasicInfo(p1.Name, p1.Slug, "CPU-INTEL-14900K", "Vi xử lý Intel Core i9 thế hệ 14 mạnh mẽ nhất", "24 nhân 32 luồng, xung nhịp lên tới 6.0GHz");
            p1.UpdateStock(50);
            p1.SetThumbnail("/images/products/i9-14900k.jpg");
            p1.MarkAsFeatured();
            products.Add(p1);

            var p2 = new Product("AMD Ryzen 9 7950X", "amd-ryzen-9-7950x", cpuCat.Id, amd.Id, 13590000);
            p2.UpdateBasicInfo(p2.Name, p2.Slug, "CPU-AMD-7950X", "Vi xử lý AMD Ryzen 7000 series đỉnh cao", "16 nhân 32 luồng, kiến trúc Zen 4");
            p2.UpdatePricing(13590000, 15990000); // Has discount
            p2.UpdateStock(30);
            p2.SetThumbnail("/images/products/ryzen-7950x.jpg");
            p2.MarkAsFeatured();
            products.Add(p2);

            // VGAs
            var p3 = new Product("ASUS ROG Strix GeForce RTX 4090", "asus-rog-strix-rtx-4090", vgaCat.Id, asus.Id, 50990000);
            p3.UpdateBasicInfo(p3.Name, p3.Slug, "VGA-ASUS-4090", "Card đồ họa mạnh nhất thế giới hiện nay", "24GB GDDR6X, DLSS 3, Ray Tracing");
            p3.UpdateStock(10);
            p3.SetThumbnail("/images/products/rog-4090.jpg");
            p3.MarkAsFeatured();
            products.Add(p3);

            // RAM
            var p4 = new Product("Corsair Vengeance RGB 32GB (2x16GB) DDR5 6000MHz", "corsair-vengeance-rgb-32gb-ddr5", ramCat.Id, corsair.Id, 3290000);
            p4.UpdateBasicInfo(p4.Name, p4.Slug, "RAM-CORSAIR-DDR5", "RAM DDR5 tốc độ cao với LED RGB", "Bus 6000MHz, CL36, XMP 3.0");
            p4.UpdateStock(100);
            p4.SetThumbnail("/images/products/corsair-ddr5.jpg");
            products.Add(p4);

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedChatbotResponsesAsync(GearStoreDbContext context)
    {
        if (!await context.ChatbotResponses.AnyAsync())
        {
            var responses = new List<ChatbotResponse>
            {
                new ChatbotResponse 
                { 
                    Keywords = "bảo hành", 
                    Answer = "Sản phẩm chính hãng bảo hành từ 12-36 tháng tùy loại ạ.", 
                    MatchType = "Contains",
                    Priority = 1 
                },
                new ChatbotResponse 
                { 
                    Keywords = "địa chỉ", 
                    Answer = "Shop ở Hà Nội, ship COD toàn quốc bạn nhé.", 
                    MatchType = "Contains",
                    Priority = 1 
                },
                new ChatbotResponse 
                { 
                    Keywords = "thanh toán", 
                    Answer = "Bên mình hỗ trợ thanh toán tiền mặt (COD) hoặc chuyển khoản.", 
                    MatchType = "Contains",
                    Priority = 1 
                }
            };
            await context.ChatbotResponses.AddRangeAsync(responses);
            await context.SaveChangesAsync();
        }
    }
}