using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Customers;
using StockFlow.Application.Orders;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using System.Security.Claims;

namespace StockFlow.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ICustomerService _customerService;

    public OrdersController(IOrderService orderService, ICustomerService customerService)
    {
        _orderService = orderService;
        _customerService = customerService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<List<OrderDto>>> GetAll()
    {
        var orders = await _orderService.GetOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Operador,Cliente")]
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
    [Authorize(Roles = "Admin,Operador,Cliente")]
    public async Task<ActionResult<Order>> Create(Order order)
    {
        if (User.IsInRole("Cliente"))
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var profile = await _customerService.GetProfileAsync(userId.Value);
            if (profile is null)
            {
                return Unauthorized();
            }

            order.CustomerProfileId = profile.Id;
            order.CustomerName = profile.User.Name;
            order.CustomerEmail = profile.User.Email;

            var address = profile.Addresses.FirstOrDefault(address => address.Id == order.DeliveryAddressId)
                ?? profile.Addresses.FirstOrDefault();

            if (address is not null)
            {
                order.DeliveryAddressId = address.Id;
                order.DeliveryStreet = address.Street;
                order.DeliveryNumber = address.Number;
                order.DeliveryCity = address.City;
                order.DeliveryState = address.State;
            }
        }

        var createdOrder = await _orderService.AddOrderAsync(order);
        return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Operador")]
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
    [Authorize(Roles = "Admin,Operador")]
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
    [Authorize(Roles = "Admin,Operador")]
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
    [Authorize(Roles = "Admin,Operador,Cliente")]
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
    [Authorize(Roles = "Admin,Operador,Cliente")]
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
    [Authorize(Roles = "Admin,Operador,Cliente")]
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

    private int? GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userId, out var value) ? value : null;
    }
}
