using StockFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Domain.Interfaces;

public interface IStockRepository
{
    Task<List<StockItem>> GetStockItemsAsync();
    Task<StockItem?> GetStockItemAsync(int productVariantId, int warehouseId);
    Task<List<StockMovement>> GetMovementsAsync(int productVariantId);
    Task RegisterMovementAsync(StockItem stockItem, StockMovement movement);
    Task<List<StockItem>> GetStockItemsByProductVariantAsync(int productVariantId);
    Task RegisterMovementsAsync(List<StockMovement> movements);
}
