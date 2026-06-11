using FluentValidation;
using StockFlow.Application.Suppliers;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using StockFlow.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Application.Orders;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IValidator<Order> _orderValidator;

    public OrderService(
        IOrderRepository orderRepository,
        IValidator<Order> orderValidator)
    {
        _orderRepository = orderRepository;
        _orderValidator = orderValidator;
    }

    public async Task<List<Order>> GetOrdersAsync()
    {
        return await _orderRepository.GetAllAsync();
    }

    public async Task<Order?> GetOrderAsync(int id)
    {
        return await _orderRepository.GetByIdAsync(id);
    }

    public async Task<Order> AddOrderAsync(Order order)
    {
        order.Status = OrderStatus.Pending;
        order.CreatedAt = DateTime.UtcNow;

        await _orderValidator.ValidateAndThrowAsync(order);

        await _orderRepository.AddAsync(order);

        return order;
    }

    public async Task<Order?> UpdateOrderAsync(int id, Order order)
    {
        await _orderValidator.ValidateAndThrowAsync(order);
        var existingOrder = await _orderRepository.GetByIdAsync(id);

        if (existingOrder is null)
        {
            return null;
        }

        existingOrder.CustomerName = order.CustomerName;
        existingOrder.CustomerEmail = order.CustomerEmail;
        existingOrder.Status = order.Status;

        await _orderRepository.UpdateAsync(existingOrder);

        return existingOrder;
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
        {
            return false;
        }

        await _orderRepository.DeleteAsync(order);

        return true;
    }

    public async Task<Order?> ChangeOrderStatusAsync(int id, OrderStatus newStatus)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null)
        {
            return null;
        }
        order.Status = newStatus;
        await _orderRepository.UpdateAsync(order);
        return order;
    }

    public async Task<Order?> AddOrderItemAsync(int orderId, CreateOrderItemDto item)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null)
        {
            return null;
        }
        var orderItem = new OrderItem
        {
            OrderId = orderId,
            ProductVariantId = item.ProductVariantId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
        };
        order.Items.Add(orderItem);
        await _orderRepository.UpdateAsync(order);
        return order;
    }

    public async Task<Order?> RemoveOrderItemAsync(int orderId, int itemId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null)
        {
            return null;
        }
        var item = order.Items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return null;
        }
        order.Items.Remove(item);
        await _orderRepository.UpdateAsync(order);
        return order;
    }
}
