using StockFlow.Domain.Entities;

namespace StockFlow.Application.Products;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(int id, Product product);
    Task<bool> DeleteAsync(int id);
    Task<List<ProductVariantDto>> GetVariantsAsync();
    Task<ProductVariantDto?> GetVariantByIdAsync(int id);
    Task<List<ProductVariantDto>> GetVariantsByProductIdAsync(int productId);
    Task<ProductVariantDto> CreateVariantAsync(int productId, ProductVariantDto variant);
    Task<ProductVariantDto> UpdateVariantAsync(int id, ProductVariantDto variant);
    Task<bool> DeleteVariantAsync(int id);
    Task<ProductImage> AddImageAsync(int productId, ProductImage image);
}
