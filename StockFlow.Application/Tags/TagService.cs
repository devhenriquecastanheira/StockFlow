using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;

namespace StockFlow.Application.Tags;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<List<Tag>> GetAllAsync()
    {
        return await _tagRepository.GetAllAsync();
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        return await _tagRepository.GetByIdAsync(id);
    }

    public async Task<Tag> CreateAsync(Tag tag)
    {
        await _tagRepository.AddAsync(tag);

        return tag;
    }

    public async Task<Tag?> UpdateAsync(int id, Tag tag)
    {
        var existingTag = await _tagRepository.GetByIdAsync(id);

        if (existingTag is null)
        {
            return null;
        }

        existingTag.Name = tag.Name;

        await _tagRepository.UpdateAsync(existingTag);

        return existingTag;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);

        if (tag is null)
        {
            return false;
        }

        await _tagRepository.DeleteAsync(tag);

        return true;
    }
}
