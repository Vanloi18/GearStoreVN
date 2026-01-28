using GearStore.Application.Common;
using GearStore.Application.DTOs.Brand;

namespace GearStore.Application.Interfaces.Services;

public interface IBrandService
{
    Task<IEnumerable<BrandDto>> GetAllAsync();
    Task<BrandDto?> GetByIdAsync(int id);
    Task<BrandDto> CreateAsync(CreateBrandDto dto);
    Task UpdateAsync(int id, CreateBrandDto dto);
    Task DeleteAsync(int id);
}
