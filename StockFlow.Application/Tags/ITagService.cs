using StockFlow.Domain.Entities;

namespace StockFlow.Application.Tags;

public interface ITagService
{
    Task<List<Tag>> GetAllAsync();
    Task<Tag?> GetByIdAsync(int id);
    Task<Tag> CreateAsync(Tag tag);
    Task<Tag?> UpdateAsync(int id, Tag tag);
    Task<bool> DeleteAsync(int id);
}
