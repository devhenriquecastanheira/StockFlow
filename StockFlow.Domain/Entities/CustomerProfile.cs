using System.Text.Json.Serialization;

namespace StockFlow.Domain.Entities;

public class CustomerProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();

    [JsonIgnore]
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    [JsonIgnore]
    public ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
