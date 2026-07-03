using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Customers;
using StockFlow.Domain.Entities;
using System.Security.Claims;

namespace StockFlow.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Cliente")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;

    public CustomersController(ICustomerService service)
    {
        _service = service;
    }

    [HttpGet("me")]
    public async Task<ActionResult<CustomerProfile>> GetProfile()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var profile = await _service.GetProfileAsync(userId.Value);
        if (profile is null)
        {
            return NotFound();
        }

        return Ok(profile);
    }

    [HttpPost("me/addresses")]
    public async Task<ActionResult<CustomerAddress>> AddAddress(CustomerAddress request)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var address = await _service.AddAddressAsync(userId.Value, request);
        if (address is null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(GetProfile), new { id = address.Id }, address);
    }

    [HttpPut("me/addresses/{addressId}")]
    public async Task<IActionResult> UpdateAddress(int addressId, CustomerAddress request)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var updated = await _service.UpdateAddressAsync(userId.Value, addressId, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("me/addresses/{addressId}")]
    public async Task<IActionResult> DeleteAddress(int addressId)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var deleted = await _service.DeleteAddressAsync(userId.Value, addressId);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private int? GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userId, out var value) ? value : null;
    }
}
