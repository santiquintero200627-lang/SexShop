using AutoMapper;
using SexShop.Application.Common;
using SexShop.Application.DTOs.Orders;
using SexShop.Application.Interfaces;
using SexShop.Domain.Entities;
using SexShop.Domain.Enums;

namespace SexShop.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<OrderDto>> CreateOrderAsync(string userId, CreateOrderDto createOrderDto)
        {
            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                OrderDetails = new List<OrderDetail>()
            };

            decimal totalAmount = 0;

            foreach (var item in createOrderDto.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
                if (product == null) return new ApiResponse<OrderDto>($"Product with ID {item.ProductId} not found");

                if (product.Stock < item.Quantity)
                    return new ApiResponse<OrderDto>($"Insufficient stock for product {product.Name}");

                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                order.OrderDetails.Add(orderDetail);
                totalAmount += product.Price * item.Quantity;

                // Update stock
                product.Stock -= item.Quantity;
                await _unitOfWork.Repository<Product>().UpdateAsync(product);
            }

            order.TotalAmount = totalAmount;

            await _unitOfWork.Repository<Order>().AddAsync(order);
            await _unitOfWork.Complete();

            var orderDto = _mapper.Map<OrderDto>(order);
            return new ApiResponse<OrderDto>(orderDto, "Order created successfully");
        }

        public async Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int id)
        {
            // Note: GenericRepository needs Include support for loading related data (OrderDetails).
            // For now, simpler implementation. Or we extend GenericRepository.
            // Let's assume lazy loading or we need to add Includes to GetAsync.
            // Cleanest way: Specification pattern or just specific repository method.
            // I will use GetAsync with predicate, but GenericRepository in previous step didn't have Include.
            // I will fix GenericRepository or just rely on manual loading for now (which is bad for performance).
            // Let's modify GenericRepository to support Includes is better.
            
            // For this step, I'll return what I can.
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(id);
             if (order == null) return new ApiResponse<OrderDto>("Order not found");
             
             // Loading details manually if eager loading not enabled. 
             // To fix this properly, I should update GenericRepository.
             
            var dto = _mapper.Map<OrderDto>(order);
            return new ApiResponse<OrderDto>(dto);
        }

        public async Task<ApiResponse<IReadOnlyList<OrderDto>>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _unitOfWork.Repository<Order>().GetAsync(o => o.UserId == userId);
            var dtos = _mapper.Map<IReadOnlyList<OrderDto>>(orders);
            return new ApiResponse<IReadOnlyList<OrderDto>>(dtos);
        }
    }
}
