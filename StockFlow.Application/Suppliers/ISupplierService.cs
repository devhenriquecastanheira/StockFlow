using StockFlow.Domain.Entities;

namespace StockFlow.Application.Suppliers;

public interface ISupplierService
{
    Task<List<Supplier>> GetAllAsync();
    Task<Supplier?> GetByIdAsync(int id);
    Task<Supplier> CreateAsync(Supplier supplier);
    Task<Supplier?> UpdateAsync(int id, Supplier supplier);
    Task<bool> DeleteAsync(int id);
}
