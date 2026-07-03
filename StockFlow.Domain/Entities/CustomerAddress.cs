using System.Text.Json.Serialization;

namespace StockFlow.Domain.Entities;

public class CustomerAddress
{
    public int Id { get; set; }
    public int CustomerProfileId { get; set; }
    [JsonIgnore]
    public CustomerProfile CustomerProfile { get; set; } = null!;
    public string Street { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}
