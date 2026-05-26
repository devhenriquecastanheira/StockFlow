using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Domain.Entities;

public class PurchaseOrderItem
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
}
