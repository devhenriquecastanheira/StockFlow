using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Domain.Entities;

public class PhysicalInventoryItem
{
    public int Id { get; set; }
    public int PhysicalInventoryId { get; set; }
    public PhysicalInventory PhysicalInventory { get; set; } = null!;
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
    public int CountedQuantity { get; set; }
}
