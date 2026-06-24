using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Warehouses;
using StockFlow.Domain.Entities;

namespace StockFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Warehouse>>> GetAll()
    {
        var warehouses = await _warehouseService.GetAllAsync();

        return Ok(warehouses);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Warehouse>> GetById(int id)
    {
        var warehouse = await _warehouseService.GetByIdAsync(id);

        if (warehouse is null)
        {
            return NotFound();
        }

        return Ok(warehouse);
    }

    [HttpPost]
    public async Task<ActionResult<Warehouse>> Create(Warehouse warehouse)
    {
        try
        {
            var createdWarehouse = await _warehouseService.CreateAsync(warehouse);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdWarehouse.Id },
                createdWarehouse);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Warehouse>> Update(int id, Warehouse warehouse)
    {
        try
        {
            var updatedWarehouse = await _warehouseService.UpdateAsync(id, warehouse);

            if (updatedWarehouse is null)
            {
                return NotFound();
            }

            return Ok(updatedWarehouse);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _warehouseService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}