using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Application.Orders;

public class OrderDto
{
    public int Id { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}
