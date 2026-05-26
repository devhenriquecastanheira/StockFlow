using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Domain.Entities;

public class PurchaseOrder
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public ICollection<PurchaseOrderItem> Items { get; set; } = [];
}
