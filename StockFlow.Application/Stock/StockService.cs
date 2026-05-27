using FluentValidation;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using StockFlow.Domain.Interfaces;

namespace StockFlow.Application.Stock;

public class StockService : IStockService
{
    private readonly IStockRepository _stockRepository;
    private readonly IValidator<StockMovement> _stockMovementValidator;

    public StockService(
        IStockRepository stockRepository,
        IValidator<StockMovement> stockMovementValidator)
    {
        _stockRepository = stockRepository;
        _stockMovementValidator = stockMovementValidator;
    }

    public async Task<List<StockItem>> GetStockItemsAsync()
    {
        return await _stockRepository.GetStockItemsAsync();
    }

    public async Task<StockItem?> GetStockItemAsync(int productVariantId, int warehouseId)
    {
        return await _stockRepository.GetStockItemAsync(productVariantId, warehouseId);
    }

    public async Task<List<StockMovement>> GetMovementsAsync(int productVariantId)
    {
        return await _stockRepository.GetMovementsAsync(productVariantId);
    }

    public async Task<StockMovement> RegisterMovementAsync(StockMovement movement)
    {
        await _stockMovementValidator.ValidateAndThrowAsync(movement);

        var stockItem = await _stockRepository.GetStockItemAsync(
            movement.ProductVariantId,
            movement.WarehouseId);

        stockItem ??= new StockItem
        {
            ProductVariantId = movement.ProductVariantId,
            WarehouseId = movement.WarehouseId,
            Quantity = 0
        };

        stockItem.Quantity = movement.Type switch
        {
            StockMovementType.Entry => stockItem.Quantity + movement.Quantity,

            StockMovementType.Exit when stockItem.Quantity >= movement.Quantity
                => stockItem.Quantity - movement.Quantity,

            StockMovementType.Exit
                => throw new InvalidOperationException("Insufficient stock."),

            StockMovementType.Adjustment
                => movement.Quantity,

            _ => throw new InvalidOperationException("Invalid stock movement type.")
        };

        movement.CreatedAt = DateTime.UtcNow;

        await _stockRepository.RegisterMovementAsync(stockItem, movement);

        return movement;
    }
}