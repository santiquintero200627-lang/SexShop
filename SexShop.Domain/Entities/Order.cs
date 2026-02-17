using SexShop.Domain.Common;
using SexShop.Domain.Enums;
using System.Collections.Generic;

namespace SexShop.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}
