using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Cart;
using StockFlow.Application.Customers;
using System.Security.Claims;

namespace StockFlow.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Cliente")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ICustomerService _customerService;

    public CartController(ICartService cartService, ICustomerService customerService)
    {
        _cartService = cartService;
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetMyCart()
    {
        var customerProfileId = await GetCustomerProfileId();
        if (customerProfileId is null)
        {
            return Unauthorized();
        }

        var cart = await _cartService.GetCartAsync(customerProfileId.Value);
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem(AddCartItemDto item)
    {
        var customerProfileId = await GetCustomerProfileId();
        if (customerProfileId is null)
        {
            return Unauthorized();
        }

        var cart = await _cartService.AddItemAsync(customerProfileId.Value, item);
        if (cart is null)
        {
            return BadRequest("Produto invalido ou quantidade invalida.");
        }

        return Ok(cart);
    }

    [HttpPut("items/{itemId}")]
    public async Task<ActionResult<CartDto>> UpdateItem(int itemId, UpdateCartItemDto item)
    {
        var customerProfileId = await GetCustomerProfileId();
        if (customerProfileId is null)
        {
            return Unauthorized();
        }

        var cart = await _cartService.UpdateItemAsync(customerProfileId.Value, itemId, item);
        if (cart is null)
        {
            return BadRequest("Item invalido ou quantidade invalida.");
        }

        return Ok(cart);
    }

    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> RemoveItem(int itemId)
    {
        var customerProfileId = await GetCustomerProfileId();
        if (customerProfileId is null)
        {
            return Unauthorized();
        }

        var removed = await _cartService.RemoveItemAsync(customerProfileId.Value, itemId);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var customerProfileId = await GetCustomerProfileId();
        if (customerProfileId is null)
        {
            return Unauthorized();
        }

        await _cartService.ClearCartAsync(customerProfileId.Value);
        return NoContent();
    }

    private async Task<int?> GetCustomerProfileId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userId, out var id))
        {
            return null;
        }

        var profile = await _customerService.GetProfileAsync(id);
        return profile?.Id;
    }
}
