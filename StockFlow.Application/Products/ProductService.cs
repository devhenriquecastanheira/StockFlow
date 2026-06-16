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

    public async Task<List<ProductVariant>> GetVariantsAsync()
    {
        return await _productRepository.GetVariantsAsync();
    }

    public async Task<ProductVariant?> GetVariantByIdAsync(int id)
    {
        return await _productRepository.GetVariantByIdAsync(id);
    }
}