using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly StockFlowDbContext _context;

    public WarehouseRepository(StockFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<Warehouse>> GetAllAsync()
    {
        return await _context.Warehouses
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Warehouse?> GetByIdAsync(int id)
    {
        return await _context.Warehouses
            .FirstOrDefaultAsync(warehouse => warehouse.Id == id);
    }

    public async Task AddAsync(Warehouse warehouse)
    {
        await _context.Warehouses.AddAsync(warehouse);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Warehouse warehouse)
    {
        _context.Warehouses.Update(warehouse);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Warehouse warehouse)
    {
        _context.Warehouses.Remove(warehouse);
        await _context.SaveChangesAsync();
    }
}