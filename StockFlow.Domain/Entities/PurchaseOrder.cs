using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StockFlow.Domain.Entities;

public class PurchaseOrder
{
    public int Id { get; set; }
    public int SupplierId { get; set; }

    [JsonIgnore]
    public Supplier? Supplier { get; set; }

    public DateTime CreatedAt { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public ICollection<PurchaseOrderItem> Items { get; set; } = [];
}
