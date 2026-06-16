using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StockFlow.Domain.Entities;

public class ProductVariant
{
    public int Id { get; set; }
    public int ProductId { get; set; }

    [JsonIgnore]
    public Product Product { get; set; } = null!;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int MinimumStockLevel { get; set; }
}
