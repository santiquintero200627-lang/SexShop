using SexShop.Application.Common;
using SexShop.Application.DTOs.Products;

namespace SexShop.Application.Interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<IReadOnlyList<ProductDto>>> GetAllProductsAsync();
        Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id);
        Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductDto createProductDto);
        Task<ApiResponse<ProductDto>> UpdateProductAsync(UpdateProductDto updateProductDto);
        Task<ApiResponse<bool>> DeleteProductAsync(int id);
    }
}
