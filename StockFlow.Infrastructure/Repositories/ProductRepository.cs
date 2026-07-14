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
            .Include(product => product.Images
                .OrderByDescending(image => image.IsMain)
                .ThenBy(image => image.Id))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Product>> GetByTagIdAsync(int tagId)
    {
        return await _context.Products
            .Include(product => product.Images
                .OrderByDescending(image => image.IsMain)
                .ThenBy(image => image.Id))
            .Where(product => product.ProductTags
                .Any(productTag => productTag.TagId == tagId))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(product => product.Images
                .OrderByDescending(image => image.IsMain)
                .ThenBy(image => image.Id))
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

    public async Task<List<ProductVariant>> GetVariantsByProductIdAsync(int productId)
    {
        return await _context.ProductVariants
            .Where(variant => variant.ProductId == productId)
            .Include(variant => variant.Product)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddVariantAsync(ProductVariant variant)
    {
        await _context.ProductVariants.AddAsync(variant);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateVariantAsync(ProductVariant variant)
    {
        _context.ProductVariants.Update(variant);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteVariantAsync(ProductVariant variant)
    {
        _context.ProductVariants.Remove(variant);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ProductImage>> GetImagesByProductIdAsync(int productId)
    {
        return await _context.ProductImages
            .Where(image => image.ProductId == productId)
            .OrderByDescending(image => image.IsMain)
            .ThenBy(image => image.Id)
            .ToListAsync();
    }

    public async Task<ProductImage?> GetImageByIdAsync(int productId, int imageId)
    {
        return await _context.ProductImages
            .FirstOrDefaultAsync(image => image.ProductId == productId && image.Id == imageId);
    }

    public async Task AddImageAsync(ProductImage image)
    {
        await _context.ProductImages.AddAsync(image);
        await _context.SaveChangesAsync();
    }

    public async Task SetMainImageAsync(int productId, int imageId)
    {
        var images = await _context.ProductImages
            .Where(image => image.ProductId == productId)
            .ToListAsync();

        foreach (var image in images)
        {
            image.IsMain = image.Id == imageId;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteImageAsync(ProductImage image)
    {
        _context.ProductImages.Remove(image);
        await _context.SaveChangesAsync();
    }

    public async Task<ProductTag?> GetProductTagAsync(int productId, int tagId)
    {
        return await _context.ProductTags
            .FirstOrDefaultAsync(productTag =>
                productTag.ProductId == productId &&
                productTag.TagId == tagId);
    }

    public async Task AddProductTagAsync(ProductTag productTag)
    {
        await _context.ProductTags.AddAsync(productTag);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductTagAsync(ProductTag productTag)
    {
        _context.ProductTags.Remove(productTag);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Tag>> GetTagsByProductIdAsync(int productId)
    {
        return await _context.ProductTags
            .Where(productTag => productTag.ProductId == productId)
            .Select(productTag => productTag.Tag)
            .AsNoTracking()
            .ToListAsync();
    }
}
