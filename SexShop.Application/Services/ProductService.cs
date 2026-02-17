using AutoMapper;
using SexShop.Application.Common;
using SexShop.Application.DTOs.Products;
using SexShop.Application.Interfaces;
using SexShop.Domain.Entities;

namespace SexShop.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IReadOnlyList<ProductDto>>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Repository<Product>().GetAsync(p => p.IsActive);
            var dtos = _mapper.Map<IReadOnlyList<ProductDto>>(products);
            return new ApiResponse<IReadOnlyList<ProductDto>>(dtos);
        }

        public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null || !product.IsActive) return new ApiResponse<ProductDto>("Product not found");

            var dto = _mapper.Map<ProductDto>(product);
            return new ApiResponse<ProductDto>(dto);
        }

        public async Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            if (string.IsNullOrEmpty(product.ImageUrl))
                product.ImageUrl = "https://via.placeholder.com/150?text=No+Image";

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.Complete();

            var dto = _mapper.Map<ProductDto>(product);
            return new ApiResponse<ProductDto>(dto, "Product created successfully");
        }

        public async Task<ApiResponse<ProductDto>> UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(updateProductDto.Id);
            if (product == null) return new ApiResponse<ProductDto>("Product not found");

            _mapper.Map(updateProductDto, product);
            await _unitOfWork.Repository<Product>().UpdateAsync(product);
            await _unitOfWork.Complete();

            var dto = _mapper.Map<ProductDto>(product);
            return new ApiResponse<ProductDto>(dto, "Product updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null) return new ApiResponse<bool>("Product not found");

            await _unitOfWork.Repository<Product>().DeleteAsync(product);
            await _unitOfWork.Complete();

            return new ApiResponse<bool>(true, "Product deleted successfully");
        }
    }
}
