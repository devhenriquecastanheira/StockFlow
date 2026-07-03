using StockFlow.Domain.Enums;
using System.Text.Json.Serialization;

namespace StockFlow.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    [JsonIgnore]
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    [JsonIgnore]
    public CustomerProfile? CustomerProfile { get; set; }
}
