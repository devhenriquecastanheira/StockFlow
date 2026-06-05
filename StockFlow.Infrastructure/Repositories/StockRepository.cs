using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Repositories;

public class StockRepository : IStockRepository
{
    private readonly StockFlowDbContext _context;

    public StockRepository(StockFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockItem>> GetStockItemsAsync()
    {
        return await _context.StockItems
            .AsNoTracking()
            .Include(stockItem => stockItem.ProductVariant)
            .Include(stockItem => stockItem.Warehouse)
            .ToListAsync();
    }

    public async Task<StockItem?> GetStockItemAsync(int productVariantId, int warehouseId)
    {
        return await _context.StockItems
            .Include(stockItem => stockItem.ProductVariant)
            .Include(stockItem => stockItem.Warehouse)
            .FirstOrDefaultAsync(stockItem =>
                stockItem.ProductVariantId == productVariantId &&
                stockItem.WarehouseId == warehouseId);
    }

    public async Task<List<StockMovement>> GetMovementsAsync(int productVariantId)
    {
        return await _context.StockMovements
            .AsNoTracking()
            .Include(movement => movement.ProductVariant)
            .Include(movement => movement.Warehouse)
            .Where(movement => movement.ProductVariantId == productVariantId)
            .OrderByDescending(movement => movement.CreatedAt)
            .ToListAsync();
    }

    public async Task RegisterMovementAsync(StockItem stockItem, StockMovement movement)
    {
        var existingStockItem = await _context.StockItems
            .FirstOrDefaultAsync(item =>
                item.ProductVariantId == stockItem.ProductVariantId &&
                item.WarehouseId == stockItem.WarehouseId);

        if (existingStockItem is null)
        {
            await _context.StockItems.AddAsync(stockItem);
        }
        else
        {
            existingStockItem.Quantity = stockItem.Quantity;
        }

        await _context.StockMovements.AddAsync(movement);
        await _context.SaveChangesAsync();
    }
}
