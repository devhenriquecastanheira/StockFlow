namespace StockFlow.Application.Orders;

public class InvoiceDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public decimal TotalAmount { get; set; }
}
