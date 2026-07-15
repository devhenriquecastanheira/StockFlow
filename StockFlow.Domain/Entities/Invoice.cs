using System.Text.Json.Serialization;

namespace StockFlow.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    public int OrderId { get; set; }

    [JsonIgnore]
    public Order Order { get; set; } = null!;
    public string Number { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public string? PdfPath { get; set; }
}
