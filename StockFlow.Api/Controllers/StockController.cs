using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Stock;
using StockFlow.Domain.Entities;

namespace StockFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<ActionResult<List<StockItem>>> GetStockItems()
    {
        var stockItems = await _stockService.GetStockItemsAsync();

        return Ok(stockItems);
    }

    [HttpGet("{productVariantId:int}/{warehouseId:int}")]
    public async Task<ActionResult<StockItem>> GetStockItem(
        int productVariantId,
        int warehouseId)
    {
        var stockItem = await _stockService.GetStockItemAsync(
            productVariantId,
            warehouseId);

        if (stockItem is null)
        {
            return NotFound();
        }

        return Ok(stockItem);
    }

    [HttpGet("movements/{productVariantId:int}")]
    public async Task<ActionResult<List<StockMovement>>> GetMovements(int productVariantId)
    {
        var movements = await _stockService.GetMovementsAsync(productVariantId);

        return Ok(movements);
    }

    [HttpPost("movements")]
    public async Task<ActionResult<StockMovement>> RegisterMovement(StockMovement movement)
    {
        try
        {
            var createdMovement = await _stockService.RegisterMovementAsync(movement);

            return CreatedAtAction(
                nameof(GetMovements),
                new { productVariantId = createdMovement.ProductVariantId },
                createdMovement);
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
