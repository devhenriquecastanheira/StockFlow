using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Products;
using StockFlow.Domain.Entities;

namespace StockFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll()
    {
        var products = await _productService.GetAllAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        try
        {
            var createdProduct = await _productService.CreateAsync(product);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdProduct.Id },
                createdProduct);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> Update(int id, Product product)
    {
        try
        {
            var updatedProduct = await _productService.UpdateAsync(id, product);

            if (updatedProduct is null)
            {
                return NotFound();
            }

            return Ok(updatedProduct);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("variants")]
    public async Task<ActionResult<List<ProductVariantDto>>> GetVariants()
    {
        var variants = await _productService.GetVariantsAsync();

        return Ok(variants);
    }

    [HttpGet("variants/{id:int}")]
    public async Task<ActionResult<ProductVariantDto>> GetVariantById(int id)
    {
        var variant = await _productService.GetVariantByIdAsync(id);

        if (variant is null)
        {
            return NotFound();
        }

        return Ok(variant);
    }
}
