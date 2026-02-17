using SexShop.Application.Common;
using SexShop.Application.DTOs.Orders;

namespace SexShop.Application.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse<OrderDto>> CreateOrderAsync(string userId, CreateOrderDto createOrderDto);
        Task<ApiResponse<IReadOnlyList<OrderDto>>> GetOrdersByUserIdAsync(string userId);
        Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int id);
    }
}
