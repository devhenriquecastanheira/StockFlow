namespace StockFlow.Web.Models;

public class CartViewModel
{
    public int Id { get; set; }
    public int CustomerProfileId { get; set; }
    public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
    public decimal Total => Items.Sum(item => item.SubTotal);
}
