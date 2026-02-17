using SexShop.Application.Common;
using SexShop.Application.DTOs.Orders;
using SexShop.Application.Interfaces;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace SexShop.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
             _httpClient = httpClientFactory.CreateClient("SexShopAPI");
            _httpContextAccessor = httpContextAccessor;
        }
        
        private void AddAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("Token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<ApiResponse<OrderDto>> CreateOrderAsync(string userId, CreateOrderDto createOrderDto)
        {
            AddAuthorizationHeader(); // Requires Auth
            var response = await _httpClient.PostAsJsonAsync("orders", createOrderDto);
            return await response.Content.ReadFromJsonAsync<ApiResponse<OrderDto>>()
                   ?? new ApiResponse<OrderDto>("Error creating order");
        }

        public async Task<ApiResponse<IReadOnlyList<OrderDto>>> GetOrdersByUserIdAsync(string userId)
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<ApiResponse<IReadOnlyList<OrderDto>>>("orders/my-orders")
                   ?? new ApiResponse<IReadOnlyList<OrderDto>>("Error fetching orders");
        }

        public async Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int id)
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<ApiResponse<OrderDto>>($"orders/{id}")
                   ?? new ApiResponse<OrderDto>("Error fetching order");
        }
    }
}
