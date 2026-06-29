using StockFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Application.Stock;

public interface IStockService
{
    Task<List<StockItem>> GetStockItemsAsync();
    Task<StockItem?> GetStockItemAsync(int productVariantId, int warehouseId);
    Task<List<StockMovement>> GetMovementsAsync(int productVariantId);
    Task<StockMovement> RegisterMovementAsync(StockMovement movement);
    Task<List<StockMovement>> RegisterExitAcrossWarehousesAsync(int productVariantId, int quantity, string reason);
    Task<StockTransfer> RegisterTransferAsync(StockTransfer transfer);
}
