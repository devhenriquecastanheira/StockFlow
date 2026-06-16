using StockFlow.Domain.Entities;

namespace StockFlow.Domain.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
    Task<List<ProductVariant>> GetVariantsAsync();
    Task<ProductVariant?> GetVariantByIdAsync(int id);
}
