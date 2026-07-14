using StockFlow.Domain.Entities;

namespace StockFlow.Domain.Interfaces;

public interface ITagRepository
{
    Task<List<Tag>> GetAllAsync();
    Task<Tag?> GetByIdAsync(int id);
    Task AddAsync(Tag tag);
    Task UpdateAsync(Tag tag);
    Task DeleteAsync(Tag tag);
}
