using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;

namespace StockFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Supplier>>> GetAll()
    {
        var suppliers = await _supplierService.GetAllAsync();

        return Ok(suppliers);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Supplier>> GetById(int id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);

        if (supplier is null)
        {
            return NotFound();
        }

        return Ok(supplier);
    }

    [HttpPost]
    public async Task<ActionResult<Supplier>> Create(Supplier supplier)
    {
        try
        {
            var createdSupplier = await _supplierService.CreateAsync(supplier);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdSupplier.Id },
                createdSupplier);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Supplier>> Update(int id, Supplier supplier)
    {
        try
        {
            var updatedSupplier = await _supplierService.UpdateAsync(id, supplier);

            if (updatedSupplier is null)
            {
                return NotFound();
            }

            return Ok(updatedSupplier);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _supplierService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
