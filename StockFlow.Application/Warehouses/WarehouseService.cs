using FluentValidation;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;

namespace StockFlow.Application.Warehouses;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<List<Warehouse>> GetAllAsync()
    {
        return await _warehouseRepository.GetAllAsync();
    }

    public async Task<Warehouse?> GetByIdAsync(int id)
    {
        return await _warehouseRepository.GetByIdAsync(id);
    }

    public async Task<Warehouse> CreateAsync(Warehouse warehouse)
    {
        await _warehouseRepository.AddAsync(warehouse);

        return warehouse;
    }

    public async Task<Warehouse?> UpdateAsync(int id, Warehouse warehouse)
    {

        var existingWarehouse = await _warehouseRepository.GetByIdAsync(id);

        if (existingWarehouse is null)
        {
            return null;
        }

        existingWarehouse.Name = warehouse.Name;
        existingWarehouse.Location = warehouse.Location;

        await _warehouseRepository.UpdateAsync(existingWarehouse);

        return existingWarehouse;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id);

        if (warehouse is null)
        {
            return false;
        }

        await _warehouseRepository.DeleteAsync(warehouse);

        return true;
    }
}