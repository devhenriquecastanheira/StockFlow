namespace StockFlow.Web.Models;

public class CheckoutViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
    public decimal Total { get; set; }
    public List<CustomerAddressViewModel> Addresses { get; set; } = new List<CustomerAddressViewModel>();
    public int? SelectedAddressId { get; set; }
}
