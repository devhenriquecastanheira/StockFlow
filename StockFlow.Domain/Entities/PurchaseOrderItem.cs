using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StockFlow.Domain.Entities;

public class PurchaseOrderItem
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }

    [JsonIgnore]
    public PurchaseOrder? PurchaseOrder { get; set; }

    public int ProductVariantId { get; set; }

    [JsonIgnore]
    public ProductVariant? ProductVariant { get; set; }

    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
}
