namespace StockFlow.Web.Models;

public class ProductImageViewModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; }
}
