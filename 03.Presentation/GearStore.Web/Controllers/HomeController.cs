using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GearStore.Web.Models;
using GearStore.Infrastructure.Data; // Để dùng DbContext
using Microsoft.EntityFrameworkCore;

namespace GearStore.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly GearStoreDbContext _context; // Khai báo DbContext

    // Tiêm DbContext vào Constructor
    public HomeController(ILogger<HomeController> logger, GearStoreDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Lấy danh sách Category từ Database gửi sang View
        var categories = await _context.Categories.ToListAsync();
        return View(categories);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}