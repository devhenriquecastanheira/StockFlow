namespace StockFlow.Web.Models;

public class OrderViewModel
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
}
