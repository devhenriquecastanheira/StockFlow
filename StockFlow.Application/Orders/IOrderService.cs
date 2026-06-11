using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Application.Orders;

public interface IOrderService
{
    Task<List<Order>> GetOrdersAsync();
    Task<Order?> GetOrderAsync(int id);
    Task<Order> AddOrderAsync(Order order);
    Task<Order?> UpdateOrderAsync(int id, Order order);
    Task<bool> DeleteOrderAsync(int id);
    Task<Order?> ChangeOrderStatusAsync(int id, OrderStatus newStatus);
    Task<Order?> AddOrderItemAsync(int orderId, CreateOrderItemDto item);
    Task<Order?> RemoveOrderItemAsync(int orderId, int itemId);
}
