using StockFlow.Domain.Entities;

namespace StockFlow.Application.Products;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetByTagIdAsync(int tagId);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(int id, Product product);
    Task<bool> DeleteAsync(int id);
    Task<List<ProductVariantDto>> GetVariantsAsync();
    Task<ProductVariantDto?> GetVariantByIdAsync(int id);
    Task<List<ProductVariantDto>> GetVariantsByProductIdAsync(int productId);
    Task<ProductVariantDto> CreateVariantAsync(int productId, ProductVariantDto variant);
    Task<ProductVariantDto?> UpdateVariantAsync(int id, ProductVariantDto variant);
    Task<bool> DeleteVariantAsync(int id);
    Task<List<ProductImage>?> GetImagesAsync(int productId);
    Task<ProductImage?> AddImageAsync(int productId, ProductImage image);
    Task<ProductImage?> SetMainImageAsync(int productId, int imageId);
    Task<ProductImage?> DeleteImageAsync(int productId, int imageId);
    Task<List<Tag>?> GetTagsByProductIdAsync(int productId);
    Task<Tag?> AddTagAsync(int productId, int tagId);
    Task<bool> RemoveTagAsync(int productId, int tagId);
}
