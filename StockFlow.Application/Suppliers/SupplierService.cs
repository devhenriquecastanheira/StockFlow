using FluentValidation;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;

namespace StockFlow.Application.Suppliers;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IValidator<Supplier> _supplierValidator;

    public SupplierService(
        ISupplierRepository supplierRepository,
        IValidator<Supplier> supplierValidator)
    {
        _supplierRepository = supplierRepository;
        _supplierValidator = supplierValidator;
    }

    public async Task<List<Supplier>> GetAllAsync()
    {
        return await _supplierRepository.GetAllAsync();
    }

    public async Task<Supplier?> GetByIdAsync(int id)
    {
        return await _supplierRepository.GetByIdAsync(id);
    }

    public async Task<Supplier> CreateAsync(Supplier supplier)
    {
        await _supplierValidator.ValidateAndThrowAsync(supplier);
        await _supplierRepository.AddAsync(supplier);

        return supplier;
    }

    public async Task<Supplier?> UpdateAsync(int id, Supplier supplier)
    {
        await _supplierValidator.ValidateAndThrowAsync(supplier);

        var existingSupplier = await _supplierRepository.GetByIdAsync(id);

        if (existingSupplier is null)
        {
            return null;
        }

        existingSupplier.Name = supplier.Name;
        existingSupplier.Email = supplier.Email;
        existingSupplier.Phone = supplier.Phone;

        await _supplierRepository.UpdateAsync(existingSupplier);

        return existingSupplier;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);

        if (supplier is null)
        {
            return false;
        }

        await _supplierRepository.DeleteAsync(supplier);

        return true;
    }
}
