using System.Text.Json.Serialization;

namespace StockFlow.Domain.Entities;

public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }

    [JsonIgnore]
    public Cart Cart { get; set; } = null!;
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
