namespace StockFlow.Web.Models;

public class ProductVariantViewModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int MinimumStockLevel { get; set; }
}
