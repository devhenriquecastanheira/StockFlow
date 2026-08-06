using StockFlow.Application.Stock;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using StockFlow.Domain.Interfaces;

namespace StockFlow.Application.PurchaseOrders;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IStockService _stockService;

    public PurchaseOrderService(
        IPurchaseOrderRepository purchaseOrderRepository,
        IStockService stockService)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _stockService = stockService;
    }

    public async Task<List<PurchaseOrder>> GetPurchaseOrdersAsync()
    {
        return await _purchaseOrderRepository.GetAllAsync();
    }

    public async Task<PurchaseOrder?> GetPurchaseOrderAsync(int id)
    {
        return await _purchaseOrderRepository.GetByIdAsync(id);
    }

    public async Task<PurchaseOrder> AddPurchaseOrderAsync(PurchaseOrder purchaseOrder)
    {
        purchaseOrder.CreatedAt = DateTime.UtcNow;
        purchaseOrder.Status = PurchaseOrderStatus.Pending;

        await _purchaseOrderRepository.AddAsync(purchaseOrder);

        return purchaseOrder;
    }

    public async Task<PurchaseOrder?> AddPurchaseOrderItemAsync(int purchaseOrderId, PurchaseOrderItem item)
    {
        var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(purchaseOrderId);

        if (purchaseOrder is null)
        {
            return null;
        }

        if (purchaseOrder.Status != PurchaseOrderStatus.Pending)
        {
            throw new InvalidOperationException("Apenas ordens pendentes podem receber itens.");
        }

        item.PurchaseOrderId = purchaseOrderId;
        purchaseOrder.Items.Add(item);

        await _purchaseOrderRepository.UpdateAsync(purchaseOrder);

        return purchaseOrder;
    }

    public async Task<PurchaseOrder?> ReceiveAsync(int id, int warehouseId)
    {
        var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(id);

        if (purchaseOrder is null)
        {
            return null;
        }

        if (warehouseId <= 0)
        {
            throw new InvalidOperationException("Depósito inválido.");
        }

        if (purchaseOrder.Status != PurchaseOrderStatus.Pending)
        {
            throw new InvalidOperationException("Apenas ordens pendentes podem ser recebidas.");
        }

        if (purchaseOrder.Items.Count == 0)
        {
            throw new InvalidOperationException("A ordem de compra deve conter pelo menos um item.");
        }

        foreach (var item in purchaseOrder.Items)
        {
            await _stockService.RegisterMovementAsync(new StockMovement
            {
                ProductVariantId = item.ProductVariantId,
                WarehouseId = warehouseId,
                Type = StockMovementType.Entry,
                Quantity = item.Quantity,
                Reason = $"Recebimento da ordem de compra #{purchaseOrder.Id}"
            });
        }

        purchaseOrder.Status = PurchaseOrderStatus.Received;

        await _purchaseOrderRepository.UpdateAsync(purchaseOrder);

        return purchaseOrder;
    }

    public async Task<bool> DeletePurchaseOrderAsync(int id)
    {
        var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(id);

        if (purchaseOrder is null)
        {
            return false;
        }

        await _purchaseOrderRepository.DeleteAsync(purchaseOrder);

        return true;
    }
}