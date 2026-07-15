using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly StockFlowDbContext _context;

    public CartRepository(StockFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByCustomerProfileIdAsync(int customerProfileId)
    {
        return await _context.Carts
            .Include(cart => cart.Items)
            .ThenInclude(item => item.ProductVariant)
            .ThenInclude(variant => variant.Product)
            .FirstOrDefaultAsync(cart => cart.CustomerProfileId == customerProfileId);
    }

    public async Task AddAsync(Cart cart)
    {
        await _context.Carts.AddAsync(cart);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Cart cart)
    {
        _context.Carts.Update(cart);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(CartItem item)
    {
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteItemsAsync(List<CartItem> items)
    {
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
    }
}
