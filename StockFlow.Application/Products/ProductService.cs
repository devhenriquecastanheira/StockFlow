using FluentValidation;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;

namespace StockFlow.Application.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<Product> _productValidator;

    public ProductService(
        IProductRepository productRepository,
        IValidator<Product> productValidator)
    {
        _productRepository = productRepository;
        _productValidator = productValidator;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        await _productValidator.ValidateAndThrowAsync(product);
        await _productRepository.AddAsync(product);

        return product;
    }

    public async Task<Product?> UpdateAsync(int id, Product product)
    {
        await _productValidator.ValidateAndThrowAsync(product);

        var existingProduct = await _productRepository.GetByIdAsync(id);

        if (existingProduct is null)
        {
            return null;
        }

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.CostPrice = product.CostPrice;
        existingProduct.SalePrice = product.SalePrice;
        existingProduct.CategoryId = product.CategoryId;

        await _productRepository.UpdateAsync(existingProduct);

        return existingProduct;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(product);

        return true;
    }

    public async Task<List<ProductVariantDto>> GetVariantsAsync()
    {
        var variants = await _productRepository.GetVariantsAsync();
        return variants.Select(v => new ProductVariantDto
        {
            Id = v.Id,
            ProductId = v.ProductId,
            ProductName = v.Product.Name,
            Size = v.Size,
            Color = v.Color,
            Sku = v.Sku,
            MinimumStockLevel = v.MinimumStockLevel
        }).ToList();
    }

    public async Task<ProductVariantDto?> GetVariantByIdAsync(int id)
    {
        var variant = await _productRepository.GetVariantByIdAsync(id);
        if (variant is null)
        {
            return null;
        }

        return new ProductVariantDto
        {
            Id = variant.Id,
            ProductId = variant.ProductId,
            ProductName = variant.Product.Name,
            Size = variant.Size,
            Color = variant.Color,
            Sku = variant.Sku,
            MinimumStockLevel = variant.MinimumStockLevel
        };
    }

    public async Task<List<ProductVariantDto>> GetVariantsByProductIdAsync(int productId)
    {
        var variants = await _productRepository.GetVariantsByProductIdAsync(productId);

        return variants.Select(v => new ProductVariantDto
        {
            Id = v.Id,
            ProductId = v.ProductId,
            ProductName = v.Product.Name,
            Size = v.Size,
            Color = v.Color,
            Sku = v.Sku,
            MinimumStockLevel = v.MinimumStockLevel
        }).ToList();
    }

    public async Task<ProductVariantDto> CreateVariantAsync(int productId, ProductVariantDto variant)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        variant.ProductId = productId;

        await _productRepository.AddVariantAsync(new ProductVariant
        {
            ProductId = variant.ProductId,
            Size = variant.Size,
            Color = variant.Color,
            Sku = variant.Sku,
            MinimumStockLevel = variant.MinimumStockLevel
        });

        return variant;
    }

    public async Task<ProductVariantDto> UpdateVariantAsync(int id, ProductVariantDto variant)
    {
        var existingVariant = await _productRepository.GetVariantByIdAsync(id);

        existingVariant.Size = variant.Size;
        existingVariant.Color = variant.Color;
        existingVariant.Sku = variant.Sku;
        existingVariant.MinimumStockLevel = variant.MinimumStockLevel;

        await _productRepository.UpdateVariantAsync(existingVariant);

        return variant;
    }

    public async Task<bool> DeleteVariantAsync(int id)
    {
        var variant = await _productRepository.GetVariantByIdAsync(id);

        if (variant is null)
        {
            return false;
        }

        await _productRepository.DeleteVariantAsync(variant);

        return true;
    }
}