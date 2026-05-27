using FluentValidation;
using StockFlow.Domain.Entities;

namespace StockFlow.Application.Products;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(product => product.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(product => product.Description)
            .MaximumLength(500);

        RuleFor(product => product.CostPrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(product => product.SalePrice)
            .GreaterThanOrEqualTo(0)
            .GreaterThanOrEqualTo(product => product.CostPrice);

        RuleFor(product => product.CategoryId)
            .GreaterThan(0);
    }
}