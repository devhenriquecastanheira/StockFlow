using StockFlow.Domain.Entities;

namespace StockFlow.Application.PurchaseOrders;

public interface IPurchaseOrderService
{
    Task<List<PurchaseOrder>> GetPurchaseOrdersAsync();
    Task<PurchaseOrder?> GetPurchaseOrderAsync(int id);
    Task<PurchaseOrder> AddPurchaseOrderAsync(PurchaseOrder purchaseOrder);
    Task<PurchaseOrder?> AddPurchaseOrderItemAsync(int purchaseOrderId, PurchaseOrderItem item);
    Task<PurchaseOrder?> ReceiveAsync(int id, int warehouseId);
}
