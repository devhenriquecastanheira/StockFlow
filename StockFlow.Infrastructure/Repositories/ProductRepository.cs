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

public class ProductRepository : IProductRepository
{
    private readonly StockFlowDbContext _context;

    public ProductRepository(StockFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .FirstOrDefaultAsync(product => product.Id == id);
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ProductVariant>> GetVariantsAsync()
    {
        return await _context.ProductVariants
            .Include(variant => variant.Product)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ProductVariant?> GetVariantByIdAsync(int id)
    {
        return await _context.ProductVariants
            .Include(variant => variant.Product)
            .FirstOrDefaultAsync(variant => variant.Id == id);
    }
}
