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

    [HttpGet("by-tag/{tagId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Product>>> GetByTagId(int tagId)
    {
        var products = await _productService.GetByTagIdAsync(tagId);

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

    [HttpGet("{productId:int}/images")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProductImage>>> GetImages(int productId)
    {
        var images = await _productService.GetImagesAsync(productId);

        return images is null ? NotFound() : Ok(images);
    }

    [HttpGet("{productId:int}/images/{imageId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductImage>> GetImage(int productId, int imageId)
    {
        var images = await _productService.GetImagesAsync(productId);
        var image = images?.FirstOrDefault(image => image.Id == imageId);

        return image is null ? NotFound() : Ok(image);
    }

    [HttpPost("{productId:int}/images")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ProductImage>> UploadImage(
        int productId,
        IFormFile imageFile,
        [FromForm] bool isMain = false)
    {
        if (imageFile is null || imageFile.Length == 0)
        {
            return BadRequest("Envie uma imagem.");
        }

        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var uploadFolder = Path.Combine(webRootPath, "images", "products");
        Directory.CreateDirectory(uploadFolder);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadFolder, fileName);

        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(fileStream);
        }

        var imageUrl = $"/images/products/{fileName}";

        var createdImage = await _productService.AddImageAsync(productId, new ProductImage
        {
            ImageUrl = imageUrl,
            IsMain = isMain
        });

        if (createdImage is null)
        {
            return NotFound();
        }

        return CreatedAtAction(
            nameof(GetImage),
            new { productId, imageId = createdImage.Id },
            createdImage);
    }

    [HttpPut("{productId:int}/images/{imageId:int}/main")]
    public async Task<ActionResult<ProductImage>> SetMainImage(int productId, int imageId)
    {
        var image = await _productService.SetMainImageAsync(productId, imageId);

        return image is null ? NotFound() : Ok(image);
    }

    [HttpDelete("{productId:int}/images/{imageId:int}")]
    public async Task<IActionResult> DeleteImage(int productId, int imageId)
    {
        var image = await _productService.DeleteImageAsync(productId, imageId);

        if (image is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(image.ImageUrl))
        {
            var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var relativePath = image.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var filePath = Path.Combine(webRootPath, relativePath);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        return NoContent();
    }

    [HttpGet("{productId:int}/tags")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Tag>>> GetTagsByProductId(int productId)
    {
        var tags = await _productService.GetTagsByProductIdAsync(productId);

        return tags is null ? NotFound() : Ok(tags);
    }

    [HttpPost("{productId:int}/tags/{tagId:int}")]
    public async Task<ActionResult<Tag>> AddTag(int productId, int tagId)
    {
        var tag = await _productService.AddTagAsync(productId, tagId);

        return tag is null ? NotFound() : Ok(tag);
    }

    [HttpDelete("{productId:int}/tags/{tagId:int}")]
    public async Task<IActionResult> RemoveTag(int productId, int tagId)
    {
        var removed = await _productService.RemoveTagAsync(productId, tagId);

        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}
