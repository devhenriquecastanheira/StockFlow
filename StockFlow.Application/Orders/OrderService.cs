using FluentValidation;
using StockFlow.Application.Suppliers;
using StockFlow.Domain.Entities;
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
}
