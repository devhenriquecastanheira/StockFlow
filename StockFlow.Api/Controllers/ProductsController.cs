using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Products;
using StockFlow.Domain.Entities;

namespace StockFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Operador")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IWebHostEnvironment _environment;

    public ProductsController(IProductService productService, IWebHostEnvironment environment)
    {
        _productService = productService;
        _environment = environment;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Product>>> GetAll()
    {
        var products = await _productService.GetAllAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
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
    [AllowAnonymous]
    public async Task<ActionResult<List<ProductVariantDto>>> GetVariants()
    {
        var variants = await _productService.GetVariantsAsync();

        return Ok(variants);
    }

    [HttpGet("variants/{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductVariantDto>> GetVariantById(int id)
    {
        var variant = await _productService.GetVariantByIdAsync(id);

        if (variant is null)
        {
            return NotFound();
        }

        return Ok(variant);
    }

    [HttpGet("{productId:int}/variants")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProductVariantDto>>> GetVariantsByProductId(int productId)
    {
        var variants = await _productService.GetVariantsByProductIdAsync(productId);

        return Ok(variants);
    }

    [HttpPost("{productId:int}/variants")]
    public async Task<ActionResult<ProductVariantDto>> CreateVariant(int productId, ProductVariantDto variant)
    {
        var createdVariant = await _productService.CreateVariantAsync(productId, variant);

        return CreatedAtAction(nameof(GetVariantById), new { id = createdVariant.Id }, createdVariant);
    }

    [HttpPut("variants/{id:int}")]
    public async Task<ActionResult<ProductVariantDto>> UpdateVariant(int id, ProductVariantDto variant)
    {
        var updatedVariant = await _productService.UpdateVariantAsync(id, variant);

        if (updatedVariant is null)
        {
            return NotFound();
        }

        return Ok(updatedVariant);
    }

    [HttpDelete("variants/{id:int}")]
    public async Task<IActionResult> DeleteVariant(int id)
    {
        var deleted = await _productService.DeleteVariantAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("images")]
    public async Task<IActionResult> UploadToCatalog(IFormFile imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;

            string uploadFolder = Path.Combine(_environment.WebRootPath, "images");
            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            Directory.CreateDirectory(uploadFolder);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            var pathString = "/images/" + uniqueFileName;
            var productImage = new ProductImage
            {
                ImageUrl = pathString,
                IsMain = false
            };
            await _productService.AddImageAsync(1, productImage);
        }
        return RedirectToAction("Index");
    }
}
