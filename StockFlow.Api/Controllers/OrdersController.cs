using Microsoft.AspNetCore.Http;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Orders;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;

namespace StockFlow.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll()
    {
        var orders = await _orderService.GetOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Create(Order order)
    {
        var createdOrder = await _orderService.AddOrderAsync(order);
        return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OrderDto>> Update(int id, Order order)
    {
        var updatedOrder = await _orderService.UpdateOrderAsync(id, order);
        if (updatedOrder == null)
        {
            return NotFound();
        }
        return Ok(updatedOrder);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _orderService.DeleteOrderAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<OrderDto>> ChangeStatus(int id, [FromBody] OrderStatus newStatus)
    {
        var updatedOrder = await _orderService.ChangeOrderStatusAsync(id, newStatus);
        if (updatedOrder == null)
        {
            return NotFound();
        }
        return Ok(updatedOrder);
    }

    [HttpPost("{orderId}/items")]
    public async Task<ActionResult<OrderDto>> AddItem(int orderId, CreateOrderItemDto item)
    {
        var updatedOrder = await _orderService.AddOrderItemAsync(orderId, item);
        if (updatedOrder == null)
        {
            return NotFound();
        }
        return Ok(updatedOrder);
    }

    [HttpDelete("{orderId}/items/{itemId}")]
    public async Task<ActionResult> RemoveItem(int orderId, int itemId)
    {
        var removedItem = await _orderService.RemoveOrderItemAsync(orderId, itemId);
        if (removedItem is null)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpPost("{orderId}/confirm")]
    public async Task<ActionResult<OrderDto>> ConfirmOrder(int orderId)
    {
        try
        {
            var confirmedOrder = await _orderService.ConfirmOrderAsync(orderId);

            if (confirmedOrder == null)
            {
                return NotFound();
            }

            return Ok(confirmedOrder);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
