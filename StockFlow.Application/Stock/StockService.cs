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

    public async Task<List<StockMovement>> RegisterExitAcrossWarehousesAsync(int productVariantId, int quantity, string reason)
    {
        if (productVariantId <= 0)
        {
            throw new InvalidOperationException("Produto inválido.");
        }

        if (quantity <= 0)
        {
            throw new InvalidOperationException("Quantidade deve ser maior que zero.");
        }

        var stockItems = await _stockRepository.GetStockItemsByProductVariantAsync(productVariantId);

        var availableItems = stockItems
            .Where(item => item.Quantity > 0)
            .OrderByDescending(item => item.Quantity)
            .ToList();

        var totalAvailable = availableItems.Sum(item => item.Quantity);

        if (totalAvailable < quantity)
        {
            throw new InvalidOperationException("Estoque total insuficiente.");
        }

        var remainingQuantity = quantity;
        var movements = new List<StockMovement>();

        foreach (var stockItem in availableItems)
        {
            if (remainingQuantity <= 0)
            {
                break;
            }

            var quantityToRemove = Math.Min(stockItem.Quantity, remainingQuantity);

            stockItem.Quantity -= quantityToRemove;
            remainingQuantity -= quantityToRemove;

            movements.Add(new StockMovement
            {
                ProductVariantId = productVariantId,
                WarehouseId = stockItem.WarehouseId,
                Type = StockMovementType.Exit,
                Quantity = quantityToRemove,
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _stockRepository.RegisterMovementsAsync(movements);

        return movements;
    }

    public async Task<StockTransfer> RegisterTransferAsync(StockTransfer transfer)
    {
        if (transfer.FromWarehouseId == transfer.ToWarehouseId)
        {
            throw new InvalidOperationException("Origem e destino devem ser diferentes.");
        }

        if (transfer.Quantity <= 0)
        {
            throw new InvalidOperationException("Quantidade deve ser maior que zero.");
        }

        transfer.CreatedAt = DateTime.UtcNow;

        return await _stockRepository.RegisterTransferAsync(transfer);
    }
}