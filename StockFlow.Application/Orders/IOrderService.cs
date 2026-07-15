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
    Task<List<OrderDto>> GetOrdersAsync();
    Task<OrderDto?> GetOrderAsync(int id);
    Task<Order> AddOrderAsync(Order order);
    Task<OrderDto?> CheckoutAsync(int userId, int selectedAddressId);
    Task<OrderDto?> UpdateOrderAsync(int id, Order order);
    Task<bool> DeleteOrderAsync(int id);
    Task<OrderDto?> ChangeOrderStatusAsync(int id, OrderStatus newStatus);
    Task<OrderDto?> AddOrderItemAsync(int orderId, CreateOrderItemDto item);
    Task<Order?> RemoveOrderItemAsync(int orderId, int itemId);
    Task<OrderDto?> ConfirmOrderAsync(int orderId);
}
