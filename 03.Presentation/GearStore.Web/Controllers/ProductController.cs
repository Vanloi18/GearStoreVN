using GearStore.Infrastructure.Data;
using GearStore.Domain.Entities;
using GearStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GearStore.Web.Controllers;

public class ProductController : Controller
{
    private readonly GearStoreDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment; // Để lấy đường dẫn thư mục wwwroot

    public ProductController(GearStoreDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    // 1. Hiển thị danh sách sản phẩm
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.Include(p => p.Category).ToListAsync();
        return View(products);
    }

    // 2. Hiển thị Form thêm mới
    [HttpGet]
    public IActionResult Create()
    {
        // Load danh sách Category vào Dropdown
        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
        return View();
    }

    // 3. Xử lý khi bấm nút Save
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            // 1. Xử lý ảnh (Giữ nguyên code cũ của bạn)
            string? uniqueFileName = null;
            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }
            }

            // 2. Tạo Product với constructor (name, slug, categoryId, brandId, price)
            // Tạm thời dùng brandId = 1 (cần thêm BrandId vào form nếu muốn chọn brand)
            var slug = model.Name.ToLower().Replace(" ", "-") + "-" + DateTime.Now.Ticks;
            var product = new Product(model.Name, slug, model.CategoryId, 1, model.Price);
            
            // Cập nhật description và thumbnail image  
            product.UpdateBasicInfo(model.Name, slug, null, model.Description, null);
            if (uniqueFileName != null)
            {
                product.SetThumbnail("/images/products/" + uniqueFileName);
            }

            // 3. XỬ LÝ THÔNG SỐ KỸ THUẬT qua domain method
            if (model.Specs != null && model.Specs.Any())
            {
                foreach (var spec in model.Specs)
                {
                    if (!string.IsNullOrEmpty(spec.Name) && !string.IsNullOrEmpty(spec.Value))
                    {
                        product.AddSpec(spec.Name, spec.Value);
                    }
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
        return View(model);
    }
}