using StockFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Domain.Interfaces;

public interface IOrderService
{
    Task<List<Order>> GetOrdersAsync();
    Task<Order?> GetOrderAsync(int id);
    Task<Order> AddOrderAsync(Order order);
    Task<Order?> UpdateOrderAsync(int id, Order order);
    Task<bool> DeleteOrderAsync(int id);
}
