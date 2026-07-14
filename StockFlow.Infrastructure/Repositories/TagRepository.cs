using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly StockFlowDbContext _context;

    public TagRepository(StockFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tag>> GetAllAsync()
    {
        return await _context.Tags
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        return await _context.Tags
            .FirstOrDefaultAsync(tag => tag.Id == id);
    }

    public async Task AddAsync(Tag tag)
    {
        await _context.Tags.AddAsync(tag);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Tag tag)
    {
        _context.Tags.Update(tag);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tag tag)
    {
        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
    }
}
