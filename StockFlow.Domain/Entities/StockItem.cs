using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Domain.Entities;

public class StockItem
{
    public int Id { get; set; }
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public int Quantity { get; set; }
}
