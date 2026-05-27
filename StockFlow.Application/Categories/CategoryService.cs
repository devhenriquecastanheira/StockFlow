using FluentValidation;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Application.Categories;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<Category> _categoryValidator;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IValidator<Category> categoryValidator)
    {
        _categoryRepository = categoryRepository;
        _categoryValidator = categoryValidator;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<Category> CreateAsync(Category category)
    {
        await _categoryValidator.ValidateAndThrowAsync(category);
        await _categoryRepository.AddAsync(category);

        return category;
    }

    public async Task<Category?> UpdateAsync(int id, Category category)
    {
        await _categoryValidator.ValidateAndThrowAsync(category);
        var existingCategory = await _categoryRepository.GetByIdAsync(id);

        if (existingCategory is null)
        {
            return null;
        }

        existingCategory.Name = category.Name;
        existingCategory.Description = category.Description;

        await _categoryRepository.UpdateAsync(existingCategory);

        return existingCategory;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return false;
        }

        await _categoryRepository.DeleteAsync(category);

        return true;
    }
}
