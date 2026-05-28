using FluentValidation;
using StockFlow.Domain.Entities;

namespace StockFlow.Application.Suppliers;

public class SupplierValidator : AbstractValidator<Supplier>
{
    public SupplierValidator()
    {
        RuleFor(supplier => supplier.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(supplier => supplier.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150);

        RuleFor(supplier => supplier.Phone)
            .NotEmpty()
            .MaximumLength(30);
    }
}
