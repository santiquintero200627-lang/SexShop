using SexShop.Application.Common;
using SexShop.Application.DTOs.Products;
using SexShop.Application.Interfaces;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace SexShop.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("SexShopAPI");
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddAuthorization(HttpRequestMessage request)
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("Token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<ApiResponse<IReadOnlyList<ProductDto>>> GetAllProductsAsync()
        {
             var response = await _httpClient.GetAsync("products");
             if (!response.IsSuccessStatusCode)
                 return new ApiResponse<IReadOnlyList<ProductDto>>($"Error: {response.StatusCode}");

             return await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<ProductDto>>>()
                    ?? new ApiResponse<IReadOnlyList<ProductDto>>("Error parsing products");
        }

        public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id)
        {
             var response = await _httpClient.GetAsync($"products/{id}");
             if (!response.IsSuccessStatusCode)
                 return new ApiResponse<ProductDto>($"Error: {response.StatusCode}");

             return await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>()
                    ?? new ApiResponse<ProductDto>("Error parsing product");
        }

        public async Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductDto createProductDto)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "products");
            request.Content = JsonContent.Create(createProductDto);
            AddAuthorization(request);

            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return new ApiResponse<ProductDto>($"API Error {response.StatusCode}: {errorBody}");
            }

            return await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>()
                   ?? new ApiResponse<ProductDto>("Error parsing created product");
        }

        public async Task<ApiResponse<ProductDto>> UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, $"products/{updateProductDto.Id}");
            request.Content = JsonContent.Create(updateProductDto);
            AddAuthorization(request);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return new ApiResponse<ProductDto>($"API Error {response.StatusCode}: {errorBody}");
            }

            return await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>()
                   ?? new ApiResponse<ProductDto>("Error parsing updated product");
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(int id)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"products/{id}");
            AddAuthorization(request);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return new ApiResponse<bool>($"API Error {response.StatusCode}: {errorBody}");
            }

            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>()
                   ?? new ApiResponse<bool>("Error parsing delete response");
        }
    }
}
