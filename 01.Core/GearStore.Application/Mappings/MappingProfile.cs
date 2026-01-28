using AutoMapper;
using GearStore.Application.DTOs.Brand;
using GearStore.Application.DTOs.Category;
using GearStore.Application.DTOs.Product;
using GearStore.Domain.Entities;

namespace GearStore.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Brand
        CreateMap<Brand, BrandDto>()
            .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.LogoUrl));

        CreateMap<CreateBrandDto, Brand>()
            .ConstructUsing(src => new Brand(src.Name, src.Name.ToLower().Replace(" ", "-")))
            .AfterMap((src, dest) => 
            {
                if (!string.IsNullOrEmpty(src.Logo)) dest.SetLogo(src.Logo);
            });

        // Category
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryDto, Category>()
            .ConstructUsing(src => new Category(src.Name, src.Name.ToLower().Replace(" ", "-"), 0));

        // Product
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : string.Empty))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

        CreateMap<CreateProductDto, Product>()
            .ConstructUsing(src => new Product(
                src.Name, 
                src.Name.ToLower().Replace(" ", "-"), 
                src.CategoryId, 
                src.BrandId, 
                src.Price));
        
        CreateMap<ProductVariant, ProductVariantDto>();
    }
}
