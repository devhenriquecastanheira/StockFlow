using StockFlow.Domain.Enums;
using System.Text.Json.Serialization;

namespace StockFlow.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int CustomerProfileId { get; set; }

    [JsonIgnore]
    public CustomerProfile CustomerProfile { get; set; } = null!;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public int? DeliveryAddressId { get; set; }
    public string DeliveryStreet { get; set; } = string.Empty;
    public string DeliveryNumber { get; set; } = string.Empty;
    public string DeliveryCity { get; set; } = string.Empty;
    public string DeliveryState { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];
    public Invoice? Invoice { get; set; }
}
