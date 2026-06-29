using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
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

    public async Task<List<StockItem>> GetStockItemsByProductVariantAsync(int productVariantId)
    {
        return await _context.StockItems
            .Where(item => item.ProductVariantId == productVariantId)
            .OrderByDescending(item => item.Quantity)
            .ToListAsync();
    }

    public async Task RegisterMovementsAsync(List<StockMovement> movements)
    {
        await _context.StockMovements.AddRangeAsync(movements);
        await _context.SaveChangesAsync();
    }

    public async Task<StockTransfer> RegisterTransferAsync(StockTransfer transfer)
    {
        var fromStock = await _context.StockItems
            .FirstAsync(item =>
                item.ProductVariantId == transfer.ProductVariantId &&
                item.WarehouseId == transfer.FromWarehouseId);

        var toStock = await _context.StockItems
            .FirstOrDefaultAsync(item =>
                item.ProductVariantId == transfer.ProductVariantId &&
                item.WarehouseId == transfer.ToWarehouseId);

        if (fromStock.Quantity < transfer.Quantity)
        {
            throw new InvalidOperationException("Estoque insuficiente.");
        }

        if (toStock is null)
        {
            toStock = new StockItem
            {
                ProductVariantId = transfer.ProductVariantId,
                WarehouseId = transfer.ToWarehouseId,
                Quantity = 0
            };

            await _context.StockItems.AddAsync(toStock);
        }

        fromStock.Quantity -= transfer.Quantity;
        toStock.Quantity += transfer.Quantity;

        await _context.StockMovements.AddAsync(new StockMovement
        {
            ProductVariantId = transfer.ProductVariantId,
            WarehouseId = transfer.FromWarehouseId,
            Type = StockMovementType.Exit,
            Quantity = transfer.Quantity,
            CreatedAt = transfer.CreatedAt,
            Reason = "Transferência entre armazéns"
        });

        await _context.StockMovements.AddAsync(new StockMovement
        {
            ProductVariantId = transfer.ProductVariantId,
            WarehouseId = transfer.ToWarehouseId,
            Type = StockMovementType.Entry,
            Quantity = transfer.Quantity,
            CreatedAt = transfer.CreatedAt,
            Reason = "Transferência entre armazéns"
        });

        await _context.StockTransfers.AddAsync(transfer);

        await _context.SaveChangesAsync();

        return transfer;
    }
}
