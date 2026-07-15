using System.Text.Json.Serialization;

namespace StockFlow.Domain.Entities;

public class Cart
{
    public int Id { get; set; }
    public int CustomerProfileId { get; set; }

    [JsonIgnore]
    public CustomerProfile CustomerProfile { get; set; } = null!;
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
