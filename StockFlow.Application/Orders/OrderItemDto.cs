using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Application.Orders;

public class OrderItemDto
{
    public int Id { get; set; }
    public int ProductVariantId { get; set; }
    public string? ProductName { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
