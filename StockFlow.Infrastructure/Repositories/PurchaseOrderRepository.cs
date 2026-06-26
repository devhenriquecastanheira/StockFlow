using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Repositories;

public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly StockFlowDbContext _context;

    public PurchaseOrderRepository(StockFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<PurchaseOrder>> GetAllAsync()
    {
        return await _context.PurchaseOrders
            .Include(order => order.Supplier)
            .Include(order => order.Items)
            .ThenInclude(item => item.ProductVariant)
            .ThenInclude(variant => variant!.Product)
            .ToListAsync();
    }

    public async Task<PurchaseOrder?> GetByIdAsync(int id)
    {
        return await _context.PurchaseOrders
            .Include(order => order.Supplier)
            .Include(order => order.Items)
            .ThenInclude(item => item.ProductVariant)
            .ThenInclude(variant => variant!.Product)
            .FirstOrDefaultAsync(order => order.Id == id);
    }

    public async Task AddAsync(PurchaseOrder purchaseOrder)
    {
        await _context.PurchaseOrders.AddAsync(purchaseOrder);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PurchaseOrder purchaseOrder)
    {
        _context.PurchaseOrders.Update(purchaseOrder);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(PurchaseOrder purchaseOrder)
    {
        _context.PurchaseOrders.Remove(purchaseOrder);
        await _context.SaveChangesAsync();
    }
}
