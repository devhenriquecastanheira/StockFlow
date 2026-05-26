using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Domain.Entities;

public class PhysicalInventory
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public DateTime CountedAt { get; set; }
    public ICollection<PhysicalInventoryItem> Items { get; set; } = [];
}
