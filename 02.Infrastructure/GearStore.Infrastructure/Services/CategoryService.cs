using AutoMapper;
using GearStore.Application.DTOs.Category;
using GearStore.Application.Interfaces.Services;
using GearStore.Domain.Entities;
using GearStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GearStore.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly GearStoreDbContext _context;
    private readonly IMapper _mapper;

    public CategoryService(GearStoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        // Name uniqueness check? 
        if (await _context.Categories.AnyAsync(c => c.Name == dto.Name))
            throw new ArgumentException($"Category '{dto.Name}' already exists.");

        var category = _mapper.Map<Category>(dto);

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task UpdateAsync(int id, CreateCategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {id} not found");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Category name is required");
            
        var slug = dto.Name.ToLower().Replace(" ", "-");
        category.UpdateInfo(dto.Name, slug);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return;
        
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}
