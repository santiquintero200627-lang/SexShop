using System.ComponentModel.DataAnnotations;

namespace SexShop.Application.DTOs.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public string? ImageUrl { get; set; }
    }

    public class UpdateProductDto : CreateProductDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
