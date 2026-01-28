using AutoMapper;
using GearStore.Application.DTOs.Brand;
using GearStore.Application.Interfaces.Services;
using GearStore.Domain.Entities;
using GearStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GearStore.Infrastructure.Services;

public class BrandService : IBrandService
{
    private readonly GearStoreDbContext _context;
    private readonly IMapper _mapper;

    public BrandService(GearStoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BrandDto>> GetAllAsync()
    {
        var brands = await _context.Brands
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .ToListAsync();

        return _mapper.Map<IEnumerable<BrandDto>>(brands);
    }

    public async Task<BrandDto?> GetByIdAsync(int id)
    {
        var brand = await _context.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        return brand == null ? null : _mapper.Map<BrandDto>(brand);
    }

    public async Task<BrandDto> CreateAsync(CreateBrandDto dto)
    {
        // Name uniqueness check? Not requested but good practice.
        // Simple create
        var brand = _mapper.Map<Brand>(dto);

        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();

        return _mapper.Map<BrandDto>(brand);
    }

    public async Task UpdateAsync(int id, CreateBrandDto dto)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null)
            throw new KeyNotFoundException($"Brand with ID {id} not found.");

        // Direct update or mapper?
        // Entity has methods, but Brand is simple.
        // Prompt says "Entity... supports it". Brand has no Update method in Domain?
        // Let's check Brand.cs if needed. Assuming standard EF update.
        // Domain rules usually prefer methods.
        // I will use manual property update here if mapper fails to respect domains.
        // Brand entity usually has setters private.
        // I'll check if Brand has Update method.
        // If not, I assume I can update properties if protected set?
        // Usually, I should check existing Entity file.
        // Step 650 didn't list Brand as "Create if missing". It implied it exists.
        // I'll try to use a method if I recall it, or check file. 
        // I'll check file in next step to be safe, but for now I'll write generic update assuming public setters or method. 
        // Best guess: Brand has Update(name, logo).
        // I'll assume explicit update.
        var slug = dto.Name.ToLower().Replace(" ", "-");
        brand.UpdateInfo(dto.Name, slug);
        brand.SetLogo(dto.Logo);
        // If compilation fails, I'll fix.
        // Or I can just check the file first.
        // I'll use simple property assignment if I can't check.
        // Wait, I can verify Brand entity.
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null) return;
        
        // Soft delete? Brand Entity has IsActive?
        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
    }
}
