using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<List<OrderDto>>> GetAll()
    {
        var orders = await _orderService.GetOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("me")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<List<OrderDto>>> GetMine()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var orders = await _orderService.GetCustomerOrdersAsync(userId.Value);
        if (orders is null)
        {
            return NotFound();
        }

        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Operador,Cliente")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        OrderDto? order;

        if (User.IsInRole("Cliente"))
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            order = await _orderService.GetCustomerOrderAsync(userId.Value, id);
        }
        else
        {
            order = await _orderService.GetOrderAsync(id);
        }

        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    [HttpPost("checkout")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<OrderDto>> Checkout(CheckoutRequestDto request)
    {
        if (request.SelectedAddressId <= 0)
        {
            return BadRequest("Selecione um endereço de entrega.");
        }

        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var order = await _orderService.CheckoutAsync(userId.Value, request.SelectedAddressId);
            if (order is null)
            {
                return Unauthorized();
            }

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
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

    [HttpPost]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<Order>> Create(Order order)
    {
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
        if (User.IsInRole("Cliente"))
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var order = await _orderService.GetCustomerOrderAsync(userId.Value, orderId);
            if (order is null)
            {
                return NotFound();
            }
        }

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
        if (User.IsInRole("Cliente"))
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var order = await _orderService.GetCustomerOrderAsync(userId.Value, orderId);
            if (order is null)
            {
                return NotFound();
            }
        }

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
        if (User.IsInRole("Cliente"))
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var order = await _orderService.GetCustomerOrderAsync(userId.Value, orderId);
            if (order is null)
            {
                return NotFound();
            }
        }

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

    [HttpGet("{orderId}/invoice/pdf")]
    [Authorize(Roles = "Admin,Operador,Cliente")]
    public async Task<IActionResult> DownloadInvoice(int orderId)
    {
        if (User.IsInRole("Cliente"))
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var order = await _orderService.GetCustomerOrderAsync(userId.Value, orderId);
            if (order is null)
            {
                return NotFound();
            }
        }

        var pdf = await _orderService.GetInvoicePdfAsync(orderId);

        if (pdf is null)
        {
            return NotFound();
        }

        return File(pdf, "application/pdf", $"fatura-{orderId}.pdf");
    }

    private int? GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userId, out var value) ? value : null;
    }
}
