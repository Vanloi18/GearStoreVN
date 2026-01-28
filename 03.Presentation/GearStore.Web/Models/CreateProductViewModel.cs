using System.ComponentModel.DataAnnotations;
using GearStore.Domain.Entities;

namespace GearStore.Web.Models;

public class CreateProductViewModel
{
    [Display(Name = "Tên sản phẩm")]
    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Mô tả ngắn")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Giá tiền (VNĐ)")]
    [Required(ErrorMessage = "Vui lòng nhập giá")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
    public decimal Price { get; set; }

    [Display(Name = "Danh mục")]
    public int CategoryId { get; set; }

    [Display(Name = "Ảnh đại diện")]
    [Required(ErrorMessage = "Vui lòng chọn ảnh")]
    public IFormFile? ImageFile { get; set; }

    public List<ProductSpecViewModel> Specs { get; set; } = new List<ProductSpecViewModel>();
}
public class ProductSpecViewModel
{
    public string Name { get; set; } = string.Empty; 
    public string Value { get; set; } = string.Empty;
}