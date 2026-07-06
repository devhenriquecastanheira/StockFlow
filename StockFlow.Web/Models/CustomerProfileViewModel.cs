namespace StockFlow.Web.Models;

public class CustomerProfileViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserViewModel User { get; set; } = new();
    public ICollection<CustomerAddressViewModel> Addresses { get; set; } = [];
}
