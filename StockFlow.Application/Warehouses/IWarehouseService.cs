using StockFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Application.Warehouses;

public interface IWarehouseService
{
    Task<List<Warehouse>> GetAllAsync();
    Task<Warehouse?> GetByIdAsync(int id);
    Task<Warehouse> CreateAsync(Warehouse warehouse);
    Task<Warehouse?> UpdateAsync(int id, Warehouse warehouse);
    Task<bool> DeleteAsync(int id);
}
