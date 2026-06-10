using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;

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
    public async Task<ActionResult<List<Order>>> GetAll()
    {
        var orders = await _orderService.GetOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetById(int id)
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
    public async Task<ActionResult<Order>> Update(int id, Order order)
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
}