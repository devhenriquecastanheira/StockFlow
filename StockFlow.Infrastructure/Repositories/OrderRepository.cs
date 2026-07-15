using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;
using StockFlow.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly StockFlowDbContext _context;

    public OrderRepository(StockFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(order => order.CustomerProfile)
            .ThenInclude(profile => profile!.User)
            .Include(order => order.Items)
            .ThenInclude(item => item.ProductVariant)
            .ThenInclude(variant => variant.Product)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(order => order.CustomerProfile)
            .ThenInclude(profile => profile!.User)
            .Include(order => order.Items)
            .ThenInclude(item => item.ProductVariant)
            .ThenInclude(variant => variant.Product)
            .FirstOrDefaultAsync(order => order.Id == id);
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task CheckoutAsync(Order order, List<CartItem> cartItems)
    {
        await _context.Orders.AddAsync(order);
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Order order)
    {
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }
}
