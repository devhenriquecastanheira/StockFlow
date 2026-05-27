using StockFlow.Domain.Entities;
using FluentValidation;

namespace StockFlow.Application.Categories;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(category => category.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(category => category.Description)
            .MaximumLength(500);
    }
}