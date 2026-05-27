using FluentValidation;
using StockFlow.Domain.Entities;

namespace StockFlow.Application.Stock;

public class StockMovementValidator : AbstractValidator<StockMovement>
{
    public StockMovementValidator()
    {
        RuleFor(movement => movement.ProductVariantId)
            .GreaterThan(0);

        RuleFor(movement => movement.WarehouseId)
            .GreaterThan(0);

        RuleFor(movement => movement.Quantity)
            .GreaterThan(0);

        RuleFor(movement => movement.Type)
            .IsInEnum();

        RuleFor(movement => movement.Reason)
            .MaximumLength(500);
    }
}