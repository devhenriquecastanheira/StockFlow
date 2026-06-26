using StockFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Domain.Interfaces;

public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetByIdAsync(int id);
    Task<List<PurchaseOrder>> GetAllAsync();
    Task AddAsync(PurchaseOrder purchaseOrder);
    Task DeleteAsync(PurchaseOrder purchaseOrder);
    Task UpdateAsync(PurchaseOrder purchaseOrder);
}
