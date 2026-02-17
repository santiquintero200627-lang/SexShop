using SexShop.Domain.Enums;
using System.Collections.Generic;

namespace SexShop.Application.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }

    public class OrderDetailDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
       // public string ProductImage { get; set; } // Optional
    }

    public class CreateOrderDto
    {
        public List<CreateOrderDetailDto> Items { get; set; } = new();
    }

    public class CreateOrderDetailDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
